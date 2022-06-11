using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace StashCraft.Hooks
{
    [HarmonyPatch(typeof(RecipeDisplay), "SetReferencedRecipe", new Type[] { typeof(Recipe), typeof(bool), typeof(IList<int>[]), typeof(IList<int>) })]
    public class RecipeDisplay_SetReferencedRecipe
    {
        [HarmonyPostfix]
        public static void SetReferencedRecipe(RecipeDisplay __instance, Recipe _recipe, bool _canBeCompleted, IList<int>[] _compatibleIngredients, IList<int> _ingredients)
        {
            try
            {
                if (!SettingsConfig.cfg_Craft_Disp_Count.Value)
                {
                    return;
                }
                
                if (_recipe.Results.Length == 0)
                {
                    return;
                }
                
                // Count for recipies
                Text m_lblRecipeName = (Text)AccessTools.Field(typeof(RecipeDisplay), "m_lblRecipeName").GetValue(__instance);
                int invQty = __instance.LocalCharacter.Inventory.ItemCount(_recipe.Results[0].ItemID);
                int stashQty = 0;
                ItemContainer CharStash;
                if (SettingsConfig.cfg_Stash_Craft.Value && !(!AreaManager.Instance.GetIsCurrentAreaTownOrCity() && !SettingsConfig.cfg_Stash_Craft_Any.Value))
                {
                    // Set the stash instance to work from
                    if (SettingsConfig.cfg_Stash_Craft_Host.Value)
                    {
                        CharStash = CharacterManager.Instance.GetWorldHostCharacter().Stash;
                        //StashCraft.Log.LogDebug($"SetReferencedRecipe: Loaded host Stash for crafting");
                    }
                    else
                    {
                        CharStash = __instance.LocalCharacter.Stash;
                        //StashCraft.Log.LogDebug($"SetReferencedRecipe: Loaded {__instance.LocalCharacter.Name} Stash for crafting");
                    }
                    stashQty = CharStash.ItemStackCount(_recipe.Results[0].ItemID);
                }
                if (invQty + stashQty > 0)
                {
                    __instance.SetName(m_lblRecipeName.text += $" ({invQty + stashQty})");
                }
            }
            catch (Exception ex)
            {
                StashCraft.Log.LogError("SetReferencedRecipe: " + ex.Message);
            }
        }
    }
}
