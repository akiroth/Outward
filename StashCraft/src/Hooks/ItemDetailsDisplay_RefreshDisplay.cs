using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using static ItemDetailsDisplay;

namespace StashCraft.Hooks
{
    [HarmonyPatch(typeof(ItemDetailsDisplay), "RefreshDisplay")]
    public class ItemDetailsDisplay_RefreshDisplay
    {
        [HarmonyPostfix]
        public static void RefreshDisplay(ItemDetailsDisplay __instance, IItemDisplay _itemDisplay)
        {
            try
            {
                Text m_lblItemName = (Text)AccessTools.Field(typeof(ItemDetailsDisplay), "m_lblItemName").GetValue(__instance);
                if (m_lblItemName == null || _itemDisplay == null || _itemDisplay.RefItem == null)
                {
                    return;
                }

                #region Show inventory and stash quantities in the name
                if ( SettingsConfig.cfg_Dets_Quantity.Value )
                {
                    if (!(_itemDisplay.RefItem is Skill))
                    {
                        int invQty = __instance.LocalCharacter.Inventory.ItemCount(_itemDisplay.RefItem.ItemID);
                        int stashQty = 0;
                        ItemContainer CharStash;
                        if (SettingsConfig.cfg_Stash_Craft.Value && !(!AreaManager.Instance.GetIsCurrentAreaTownOrCity() && !SettingsConfig.cfg_Stash_Craft_Any.Value))
                        {
                            // Set the stash instance to work from
                            if (SettingsConfig.cfg_Stash_Craft_Host.Value)
                            {
                                CharStash = CharacterManager.Instance.GetWorldHostCharacter().Stash;
                                //StashCraft.Log.LogDebug($"RefreshDisplay: Loaded host Stash for crafting");
                            }
                            else
                            {
                                CharStash = _itemDisplay.LocalCharacter.Stash;
                                //StashCraft.Log.LogDebug($"RefreshDisplay: Loaded {_itemDisplay.LocalCharacter.Name} Stash for crafting");
                            }
                            stashQty = CharStash.ItemStackCount(_itemDisplay.RefItem.ItemID);
                        }
                        if (invQty + stashQty > 0)
                        {
                            m_lblItemName.text += $" ({invQty + stashQty})";
                        }
                    }
                }
                #endregion
                
                #region Add Value/Weight Ratio information
                if ( SettingsConfig.cfg_Dets_Value.Value )
                {
                    if (_itemDisplay.RefItem.Value > 0 && _itemDisplay.RefItem.Weight > 0 && _itemDisplay.RefItem.IsSellable)
                    {
                        List<ItemDetailRowDisplay> m_detailRows = (List<ItemDetailRowDisplay>)AccessTools.Field(typeof(ItemDetailsDisplay), "m_detailRows").GetValue(__instance);
                        ItemDetailRowDisplay row = (ItemDetailRowDisplay)AccessTools.Method(typeof(ItemDetailsDisplay), "GetRow").Invoke(__instance, new object[] { m_detailRows.Count });
                        double sellp = Math.Round(_itemDisplay.RefItem.RawCurrentValue * Item.DEFAULT_SELL_MODIFIER, 0);
                        row.SetInfo("Sell Value", $"{sellp} ({Math.Round(sellp / _itemDisplay.RefItem.RawWeight, 2).ToString()}/lb)");
                    }
                }
                #endregion

                #region Add Durability information
                if (SettingsConfig.cfg_Dets_Durability.Value)
                {
                    if (_itemDisplay.RefItem.IsPerishable && _itemDisplay.RefItem.CurrentDurability > 0 && !_itemDisplay.RefItem.DisplayedInfos.ToList().Contains(DisplayedInfos.Durability))
                    {
                        List<ItemDetailRowDisplay> m_detailRows = (List<ItemDetailRowDisplay>)AccessTools.Field(typeof(ItemDetailsDisplay), "m_detailRows").GetValue(__instance);
                        ItemDetailRowDisplay row = (ItemDetailRowDisplay)AccessTools.Method(typeof(ItemDetailsDisplay), "GetRow").Invoke(__instance, new object[] { m_detailRows.Count });
                        if (SettingsConfig.cfg_Dets_Perish_Days.Value == false || (_itemDisplay.RefItem.TypeDisplay == "Equipment" && SettingsConfig.cfg_Dets_Lantern.Value))
                        {
                            row.SetInfo(LocalizationManager.Instance.GetLoc("ItemStat_Durability"), $"{_itemDisplay.RefItem.CurrentDurability}/{_itemDisplay.RefItem.MaxDurability}");
                        }
                        else
                        {
                            if (_itemDisplay.RefItem.PerishScript.DepletionRateModifier < .0001)
                            {
                                row.SetInfo(LocalizationManager.Instance.GetLoc("ItemStat_Durability"), $"[{GameTimetoDays(_itemDisplay.RefItem.CurrentDurability / _itemDisplay.RefItem.PerishScript.m_baseDepletionRate)}]");
                            }
                            else
                            {
                                row.SetInfo(LocalizationManager.Instance.GetLoc("ItemStat_Durability"), GameTimetoDays(_itemDisplay.RefItem.CurrentDurability / _itemDisplay.RefItem.PerishScript.DepletionRate));
                            }
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                StashCraft.Log.LogError("RefreshDisplay: " + ex.Message);
            }
        }

        [HarmonyPatch(typeof(ItemDetailsDisplay), "RefreshDetail")]
        public class ItemDetailsDisplay_RefreshDetail
        {
            [HarmonyPostfix]
            public static void RefreshDetail(ItemDetailsDisplay __instance, int _rowIndex, DisplayedInfos _infoType)
            {
                if (!SettingsConfig.cfg_Dets_Perish_Days.Value)
                {
                    return;
                }
                
                try
                {
                    if (_infoType != DisplayedInfos.Durability)
                    {
                        return;
                    }
                    Item m_lastItem = (Item)AccessTools.Field(typeof(ItemDetailsDisplay), "m_lastItem").GetValue(__instance);
                    if (m_lastItem.IsPerishable && m_lastItem.CurrentDurability > 0)
                    {
                        ItemDetailRowDisplay row = (ItemDetailRowDisplay)AccessTools.Method(typeof(ItemDetailsDisplay), "GetRow").Invoke(__instance, new object[] { _rowIndex });
                        Text m_lblDataName = (Text)AccessTools.Field(typeof(ItemDetailRowDisplay), "m_lblDataName").GetValue(row);
                        if (SettingsConfig.cfg_Dets_Perish_Days.Value == false || (m_lastItem.TypeDisplay == "Equipment" && SettingsConfig.cfg_Dets_Lantern.Value))
                        {
                            row.SetInfo(LocalizationManager.Instance.GetLoc("ItemStat_Durability"), $"{m_lastItem.CurrentDurability}/{m_lastItem.MaxDurability}");
                        }
                        else
                        {
                            if (m_lastItem.PerishScript.DepletionRateModifier < .0001)
                            {
                                row.SetInfo(LocalizationManager.Instance.GetLoc("ItemStat_Durability"), $"[{GameTimetoDays(m_lastItem.CurrentDurability / m_lastItem.PerishScript.m_baseDepletionRate)}]");
                            }
                            else
                            {
                                row.SetInfo(LocalizationManager.Instance.GetLoc("ItemStat_Durability"), GameTimetoDays(m_lastItem.CurrentDurability / m_lastItem.PerishScript.DepletionRate));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    StashCraft.Log.LogError("RefreshDetail: " + ex.Message);
                }
            }
        }

        private static string GameTimetoDays(double p_gametime)
        {
            string str = "";
            int days = (int)(p_gametime / 24);
            if (days > 0) str = $"{days}d, ";
            int hours = (int)(p_gametime % 24);
            str += $"{hours}h";
            if (days == 0)
            {
                hours = (int)Math.Ceiling(p_gametime % 24);
                str = $"{hours}h";
                if (hours <= 1)
                {
                    int minutes = (int)(p_gametime * 60);
                    str = $"{minutes} min";
                    if (minutes == 0)
                    {
                        int seconds = (int)(p_gametime * 3600);
                        str = $"{seconds} sec";
                    }
                }
            }
            return str;
        }
    }
}
