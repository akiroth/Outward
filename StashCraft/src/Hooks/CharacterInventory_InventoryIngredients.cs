using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StashCraft.Hooks
{
    [HarmonyPatch(typeof(CharacterInventory), "InventoryIngredients",
    new Type[] { typeof(Tag), typeof(DictionaryExt<int, CompatibleIngredient>) },
    new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref })]
    public class CharacterInventory_InventoryIngredients
    {
        [HarmonyPostfix]
        public static void InventoryIngredients(CharacterInventory __instance, Tag _craftingStationTag, ref DictionaryExt<int, CompatibleIngredient> _sortedIngredient)
        {
            try
            { 
                // Check if using the stash is enabled
                if (!SettingsConfig.cfg_Stash_Craft.Value)
                {
                    return;
                }
                // Check if you can craft while outside town
                if (!AreaManager.Instance.GetIsCurrentAreaTownOrCity() && !SettingsConfig.cfg_Stash_Craft_Any.Value)
                {
                    return;
                }

                // Set the stash instance to work from
                ItemContainer CharStash;
                if (SettingsConfig.cfg_Stash_Craft_Host.Value)
                {
                    CharStash = CharacterManager.Instance.GetWorldHostCharacter().Stash;
                    //StashCraft.Log.LogDebug($"InventoryIngredients: Loaded host Stash for crafting");
                }
                else
                {
                    CharStash = __instance.m_characterStash;
                    //StashCraft.Log.LogDebug($"InventoryIngredients: Loaded {__instance.m_character.Name} Stash for crafting");
                }

                // Set ingerdients list
                MethodInfo mi = AccessTools.GetDeclaredMethods(typeof(CharacterInventory)).FirstOrDefault(m => m.Name == "InventoryIngredients" 
                    && m.GetParameters().Any(p => p.Name == "_items"));
                mi.Invoke(__instance, new object[] { _craftingStationTag, _sortedIngredient, CharStash.GetContainedItems() });
            }
            catch (Exception ex)
            {
                StashCraft.Log.LogError("InventoryIngredients: " + ex.Message);
            }
        }
    }
}
