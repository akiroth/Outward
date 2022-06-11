using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StashCraft.Hooks
{
    [HarmonyPatch(typeof(StashPanel), "Show")]
    public class StashPanel_Show
    {
        [HarmonyPrefix]
        public static bool Show(StashPanel __instance)
        {
            StashCraft.SetStashPreservation();


            if (!SettingsConfig.cfg_Stash_Filter.Value)
            {
                StashCraft.RemoveStashFilter(__instance.LocalCharacter);
            }
            else
            {
                StashCraft.MakeStashFilter(__instance.LocalCharacter);
            }
            return true;
        }

    }
}