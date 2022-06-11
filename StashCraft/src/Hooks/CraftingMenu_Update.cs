using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using HarmonyLib;
using UnityEngine;

namespace StashCraft.Hooks
{
    [HarmonyPatch(typeof(CraftingMenu), "Update")]
    public class CraftingMenu_Update
    {
        [HarmonyPostfix]
        static void Update(CraftingMenu __instance)
        {
            try
            {
                StashCraft.CraftMenuItemDisplay(__instance);
            }
            catch (Exception ex)
            {
                StashCraft.Log.LogError("CraftingMenu_OnRecipeSelected: " + ex.Message);
            }
        }
    }
}
