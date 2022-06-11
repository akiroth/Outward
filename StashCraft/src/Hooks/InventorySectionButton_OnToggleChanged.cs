using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StashCraft.Hooks
{
    [HarmonyPatch(typeof(InventorySectionButton), "OnToggleChanged")]
    public class InventorySectionButton_OnToggleChanged
    {
        [HarmonyPostfix]
        public static void OnToggleChanged(InventorySectionButton __instance, bool _checked)
        {
            try
            {
                if (SettingsConfig.cfg_Stash_Filter.Value)
                {
                    __instance.CharacterUI.StashPanel.m_stashInventory.Refresh();
                }
            }
            catch (Exception ex)
            {
                StashCraft.Log.LogError("InventorySectionButton_OnToggleChanged: " + ex.Message);
            }
        }
    }
}