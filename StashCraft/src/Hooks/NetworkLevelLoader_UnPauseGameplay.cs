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
    [HarmonyPatch(typeof(NetworkLevelLoader), "UnPauseGameplay")]
    public class NetworkLevelLoader_UnPauseGameplay
    {
        [HarmonyPostfix]
        public static void UnPauseGameplay(string _identifier)
        {
            try
            {
                if (_identifier == "Loading")
                {
                    // Create/set preserver
                    StashCraft.SetStashPreservation();
                }
            }
            catch (Exception ex)
            {
                StashCraft.Log.LogError("UnPauseGameplay: " + ex.Message);
            }
        }
    }
}
