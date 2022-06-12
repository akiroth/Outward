using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SideLoader;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine.UI;


namespace StashCraft
{

    [BepInPlugin(GUID, NAME, VERSION)]
    public class StashCraft : BaseUnityPlugin
    {
        public static StashCraft Instance;
        public const string GUID = "Akiroth.StashCraft";
        public const string NAME = "StashCraft";
        public const string VERSION = "1.0.1";

        //public static Settings settings;

        // settings
        public ItemContainer CharStash;

        public static ManualLogSource Log => Instance.Logger;

        // Awake is called when your plugin is created. Use this to set up your mod.
        internal void Awake()
        {
            Instance = this;

            // Harmony is for patching methods. If you're not patching anything, you can comment-out or delete this line.
            var harmony = new Harmony(GUID);
            harmony.PatchAll();

            SettingsConfig.Init(Config);

            // settings

            Log.LogMessage($"Awake");
        }

        public static void CraftMenuItemDisplay(CraftingMenu __instance)
        {
            Text r_lblQuantity;
            Text lblQuantity;
            ItemDisplay m_Item;
            
            try
            {
                if (SettingsConfig.cfg_Craft_Disp_Quantity.Value)
                {
                    r_lblQuantity = __instance.m_recipeResultDisplay.m_lblQuantity;

                    foreach (IngredientSelector itemSelector in __instance.m_ingredientSelectors)
                    {
                        m_Item = itemSelector.m_itemDisplay;

                        // Create missing lblQunatities
                        if (m_Item.m_lblQuantity != null)
                        {
                            lblQuantity = itemSelector.m_itemDisplay.m_lblQuantity;
                            //Log.LogDebug($"CraftMenuItemDisplay_lbl: Got lblQuantity for slot {itemSelector.transform.GetSiblingIndex()}");
                        }
                        else
                        {
                            lblQuantity = Text.Instantiate(r_lblQuantity, itemSelector.m_itemDisplay.transform, false) as Text;
                            m_Item.m_lblQuantity = lblQuantity;
                            Log.LogDebug($"CraftMenuItemDisplay_lbl: Created lblQuantity for slot {itemSelector.transform.GetSiblingIndex()}");
                        }

                        // Assign quantities
                        ItemDisplayQty(m_Item);

                        // Assign Values
                        //ItemDisplayVal(m_Item);
                    }
                }
            }
            catch (Exception ex)
            {
                StashCraft.Log.LogError("CraftMenuItemDisplay: " + ex.Message);
            }
        }

        public static void ItemDisplayQty(ItemDisplay m_Item)
        {
            try
            {
                if (SettingsConfig.cfg_Craft_Disp_Quantity.Value)
                {
                    #region Set Quantities
                    if (m_Item != null && m_Item.RefItem != null)
                    {
                        StashCraft.Log.LogDebug("ItemDisplayQty: #" + m_Item.RefItem.ItemID);
                        int invQty = m_Item.LocalCharacter.Inventory.ItemCount(m_Item.RefItem.ItemID);
                        int stashQty = 0;
                        ItemContainer CharStash;
                        if (SettingsConfig.cfg_Stash_Craft.Value && !(!AreaManager.Instance.GetIsCurrentAreaTownOrCity() && !SettingsConfig.cfg_Stash_Craft_Any.Value))
                        {
                            // Set the stash instance to work from
                            if (SettingsConfig.cfg_Stash_Craft_Host.Value)
                            {
                                CharStash = CharacterManager.Instance.GetWorldHostCharacter().Stash;
                                //StashCraft.Log.LogDebug($"ItemDisplayQty: Loaded host Stash for crafting");
                            }
                            else
                            {
                                CharStash = m_Item.LocalCharacter.Stash;
                                //StashCraft.Log.LogDebug($"ItemDisplayQty: Loaded {m_Item.LocalCharacter.Name} Stash for crafting");
                            }
                            stashQty = CharStash.ItemStackCount(m_Item.RefItem.ItemID);
                        }
                        if (invQty + stashQty > 1)
                        {
                            m_Item.m_lblQuantity.text = $"{invQty + stashQty}";
                        }
                        else
                        {
                            m_Item.m_lblQuantity.text = "";
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                StashCraft.Log.LogError("ItemDisplayQty: " + ex.Message);
            }
        }


        public static void SetStashPreservation()
        {
            try
            {
                foreach (string CharID in CharacterManager.Instance.PlayerCharacters.Values)
                {
                    // Get character stash
                    Character Char = CharacterManager.Instance.GetCharacter(CharID);
                    ItemContainer CharStash = Char.Stash;
                    Preserver pStash;
                    
                    // Create & Set the Preserver if it doesn't exist
                    if (CharStash.m_preservationExt == null)
                    {
                        // Create & Set Preserver Object
                        pStash = CharStash.gameObject.AddComponent<Preserver>();
                        pStash.enabled = true;
                        CharStash.m_preservationExt = pStash;

                        // Create & Add Preserver Element
                        Preserver.PreservedElement pStashEl = new Preserver.PreservedElement();
                        pStash.m_preservedElements.Add(pStashEl);

                        // Get the preserver element and set the values
                        pStash = CharStash.gameObject.GetComponent<Preserver>();
                        pStash.m_preservedElements.FirstOrDefault().Tag = new TagSourceSelector(TagSourceManager.Food);
                    }
                    else
                    {
                        pStash = CharStash.gameObject.GetComponent<Preserver>();
                    }
                    

                    // Set the Preservation
                    float stashPres = GetStashPreservation(CharStash);
                    if (pStash.m_preservedElements.FirstOrDefault().Preservation != SettingsConfig.cfg_Stash_Preserv.Value)
                    {
                        pStash.m_preservedElements.FirstOrDefault().Preservation = SettingsConfig.cfg_Stash_Preserv.Value;

                        foreach(Item stashItem in CharStash.GetContainedItems().FindItemsbyTag(TagSourceManager.Food))
                        {
                            stashItem.ForceUpdateParentChange();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StashCraft.Log.LogError("SetStashPreservation: " + ex.Message);
            }
        }

        public static float GetStashPreservation(ItemContainer CharStash)
        {
            float stashPres = SettingsConfig.cfg_Stash_Preserv.Value;
            if (SettingsConfig.cfg_Stash_Preserv_Pack.Value)
            {
                foreach (Bag pack in CharStash.GetContainedItems())//TagSourceManager.Backpack 
                {
                    if (pack.Container != null)
                    {
                        float packPres = pack.Container.m_preservationExt.m_preservedElements.FirstOrDefault().Preservation;
                        if (packPres > stashPres) { stashPres = packPres; }
                    }
                }
            }
            return stashPres;
        }

        public static void MakeStashFilter(Character Char)
        {
            try
            {
                // Copy new Filter Buttons
                if (Char.CharacterUI.StashPanel.m_sectionPanel == null)
                {
                    InventorySectionDisplay invFilters = Char.CharacterUI.InventoryPanel.m_sectionPanel;
                    foreach (InventorySectionButton fButt in invFilters.GetComponentsInChildren<InventorySectionButton>()) // load tab titles
                    { 
                        fButt.StartInit(); 
                    }
                    InventorySectionDisplay stashFilters = InventorySectionDisplay.Instantiate(invFilters, Char.CharacterUI.StashPanel.m_inventoryDisplay.transform, false) as InventorySectionDisplay;
                    Char.CharacterUI.StashPanel.m_sectionPanel = stashFilters;
                    Char.CharacterUI.StashPanel.m_stashInventory.SetFilter(Char.CharacterUI.StashPanel.m_inventoryDisplay.Filter);
                    Char.CharacterUI.StashPanel.m_stashInventory.SetExceptionFilter(Char.CharacterUI.StashPanel.m_inventoryDisplay.ExceptionFilter);
                }

                // Refresh & Update the stash panel from filtering
                Char.CharacterUI.StashPanel.m_stashInventory.Refresh();
            }
            catch (Exception ex)
            {
                Log.LogError("MakeStashFilter: " + ex.Message);
            }
        }
        public static void RemoveStashFilter(Character Char)
        {
            try
            {
                // Remove new Filter Buttons
                if (Char.CharacterUI.StashPanel.m_sectionPanel != null)
                {
                    Char.CharacterUI.StashPanel.m_inventoryDisplay.Filter.Clear(true);
                    Char.CharacterUI.StashPanel.m_inventoryDisplay.ExceptionFilter.Clear(true);
                    GameObject.Destroy(Char.CharacterUI.StashPanel.m_sectionPanel.gameObject);
                    // Refresh & Update the stash panel from filtering
                    Char.CharacterUI.StashPanel.m_stashInventory.Refresh();
                }
            }
            catch (Exception ex)
            {
                Log.LogError("RemoveStashFilter: " + ex.Message);
            }
        }

        // Update is called once per frame. Use this only if needed.
        // You also have all other MonoBehaviour methods available (OnGUI, etc)
        //internal void Update() { }
    }
}
