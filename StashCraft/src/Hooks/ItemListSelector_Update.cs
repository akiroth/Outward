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
    // This is an example of a Harmony patch.
    //[HarmonyPatch(typeof(ResourcesPrefabManager), nameof(ResourcesPrefabManager.Load))]
    //public class ResourcesPrefabManager_Load
    //{
    //    static void Postfix()
    //    {
    //      This is a "Postfix" (runs after original) on ResourcesPrefabManager.Load
    //      For more documentation on Harmony, see the Harmony Wiki.
    //      https://harmony.pardeike.net/
    //    }
    //}

    [HarmonyPatch(typeof(global::ItemListSelector), "Update")]
    public class ItemListSelector_Update
    {
        [HarmonyPostfix]
        public static void Update(global::ItemListSelector __instance)
        {
            try
            {
                foreach (ItemDisplay m_Item in __instance.m_assignedDisplays)
                {
                    // Assign quantities
                    StashCraft.ItemDisplayQty(m_Item);

                    // Assign Values
                    //StashCraft.ItemDisplayVal(m_Item);
                }
            }
            catch (Exception ex)
            {
                StashCraft.Log.LogError("ItemListSelector_ItemDisplay: " + ex.Message);
            }
        }
    }
}
