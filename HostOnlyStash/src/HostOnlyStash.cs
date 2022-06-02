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

namespace HostOnlyStash
{

    [BepInPlugin(GUID, NAME, VERSION)]
    public class HostOnlyStash : BaseUnityPlugin
    {
        public static HostOnlyStash Instance;
        public const string GUID = "Akiroth.HostOnlyStash";
        public const string NAME = "HostOnlyStash";
        public const string VERSION = "1.0.0";

        // settings
        
        public static ManualLogSource Log => Instance.Logger;

        // Awake is called when your plugin is created. Use this to set up your mod.
        internal void Awake()
        {
            try
            {
                Instance = this;
                // Harmony is for patching methods. If you're not patching anything, you can comment-out or delete this line.
                var harmony = new Harmony(GUID);
                harmony.PatchAll();

                Log.LogMessage("Awake");
            }
            catch (Exception ex)
            {
                Log.LogError("Awake:" + ex.Message);
            }
        }

        // Update is called once per frame. Use this only if needed.
        // You also have all other MonoBehaviour methods available (OnGUI, etc)
        //internal void Update()
        //{
        //}


        // This is an example of a Harmony patch.
        // If you're not using this, you should delete it.
        //[HarmonyPatch(typeof(ResourcesPrefabManager), nameof(ResourcesPrefabManager.Load))]
        //public class ResourcesPrefabManager_Load
        //{
        //    static void Postfix()
        //    {
        // This is a "Postfix" (runs after original) on ResourcesPrefabManager.Load
        // For more documentation on Harmony, see the Harmony Wiki.
        // https://harmony.pardeike.net/
        //    }
        //}
    }
}
