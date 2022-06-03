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

namespace MustStashes
{

    [BepInPlugin(GUID, NAME, VERSION)]
    public class MustStashes : BaseUnityPlugin
    {
        public static MustStashes Instance;
        public const string GUID = "Akiroth.MustStashes";
        public const string NAME = "Must-Stashes";
        public const string VERSION = "1.1.0";

        //public static Settings settings;

        // settings
        const string STASH_KEY_0 = "Open Stash 0 (Yours)";
        const string STASH_KEY_1 = "Open Stash 1 (Other's)";

        public static ManualLogSource Log => Instance.Logger;

        // Awake is called when your plugin is created. Use this to set up your mod.
        internal void Awake()
        {
            Instance = this;

            // Harmony is for patching methods. If you're not patching anything, you can comment-out or delete this line.
            //var harmony = new Harmony(GUID);
            //harmony.PatchAll();

            SettingsConfig.Init(Config);

            // settings
            CustomKeybindings.AddAction(STASH_KEY_0, KeybindingsCategory.CustomKeybindings, ControlType.Both);
            CustomKeybindings.AddAction(STASH_KEY_1, KeybindingsCategory.CustomKeybindings, ControlType.Both);

            Log.LogMessage($"Awake");
        }

        // Update is called once per frame. Use this only if needed.
        // You also have all other MonoBehaviour methods available (OnGUI, etc)
        internal void Update()
        {
            try  // Do nothing if the game is paused
            {
                if (MenuManager.Instance.IsInMainMenuScene || NetworkLevelLoader.Instance.IsGameplayPaused)
                    return;
            }
            catch
            {
                return;
            }

            // Check if your in a city/town
            if ( !AreaManager.Instance.GetIsCurrentAreaTownOrCity() && !SettingsConfig.Stash_Enable_Everywhere.Value )
            {
                return;
            }

            // Handle KeyPress
            if (CustomKeybindings.GetKeyDown(STASH_KEY_0, out int playerID))
            {
                OpenStashManager(playerID, playerID);
            }

            // Handle KeyPress
            if (CustomKeybindings.GetKeyDown(STASH_KEY_1, out playerID))
            {
                if ( !SettingsConfig.Stash_Enable_Others.Value || SettingsConfig.Stash_OPEN_HOST.Value ) //stop if opening other stashes is disallowed
                {
                    return;
                }

                if (playerID == 0)
                {
                    OpenStashManager(playerID, 1);
                }
                else
                {
                    OpenStashManager(playerID, 0);
                }
            }
        }

        public static void OpenStashManager(int playerID, int targetID)
        {
            try
            {
                Character Char0;
                Character Char1;
                if (playerID == 0)
                {
                    Char0 = CharacterManager.Instance.GetFirstLocalCharacter();

                    if ( SettingsConfig.Stash_OPEN_HOST.Value )
                    {
                        Char1 = CharacterManager.Instance.GetWorldHostCharacter();
                        Log.LogDebug($"OpenStashManager: Opened World Host's Stash");
                    }
                    else
                    {
                        if ( targetID == 0 )
                        {
                            Char1 = Char0;
                            Log.LogDebug($"OpenStashManager: Opened {Char0.Name}'s Stash");
                        }
                        else
                        {
                            Character charHost = CharacterManager.Instance.GetWorldHostCharacter();
                            if ( Char0.UID == charHost.UID )
                            {
                                Char1 = CharacterManager.Instance.GetCharacter(CharacterManager.Instance.PlayerCharacters.Values[1]);
                                Log.LogDebug($"OpenStashManager: Opened {Char1.Name}'s Guest Stash for {Char0.Name}");
                            }
                            else
                            {
                                Char1 = charHost;
                                Log.LogDebug($"OpenStashManager: Opened {Char1.Name}'s Host Stash for {Char0.Name}");
                            }
                        }
                    }
                }
                else
                {
                    Char0 = CharacterManager.Instance.GetSecondLocalCharacter();
                    
                    if (SettingsConfig.Stash_OPEN_HOST.Value)
                    {
                        Char1 = CharacterManager.Instance.GetWorldHostCharacter();
                        Log.LogDebug($"OpenStashManager: Opened World Host's Stash");
                    }
                    else
                    {
                        if (targetID == 0)
                        {
                            Char1 = Char0;
                            Log.LogDebug($"OpenStashManager: Opened {Char0.Name}'s Stash");
                        }
                        else
                        {
                            Char1 = CharacterManager.Instance.GetWorldHostCharacter();
                            Log.LogDebug($"OpenStashManager: Opened {Char1.Name}'s Local Stash for {Char0.Name}");
                        }
                    }
                }

                // Set&load the stash contents then open the stash window
                ItemContainer CharStash = Char1.Stash;
                Char0.CharacterUI.StashPanel.SetStash(CharStash);
                CharStash.ShowContent(Char0);

                Log.LogDebug($"OpenStashManager: Opened {Char1.Name} Stash for {Char0.Name}");
                return;
            }
            catch (Exception ex)
            {
                Log.LogError("OpenStashManager:" + ex.Message);
                return;
            }
        }

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
