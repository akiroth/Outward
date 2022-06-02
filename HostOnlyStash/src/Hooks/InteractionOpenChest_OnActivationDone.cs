using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostOnlyStash.Hooks
{
    [HarmonyPatch(typeof(InteractionOpenChest), "OnActivationDone")]  //InteractionOpenContainer
    public class InteractionOpenChest_OnActivationDone
    {
        [HarmonyPostfix]

        public static bool OnActivationDone(InteractionOpenChest __instance)
        {
            // Disable generation content for house stash
            if (__instance.Item is TreasureChest && (__instance.Item as TreasureChest).SpecialType == ItemContainer.SpecialContainerTypes.Stash)
            {
                Character charHost = CharacterManager.Instance.GetWorldHostCharacter();
                ItemContainer CharStash = charHost.Stash;
                HostOnlyStash.Log.LogMessage($"HOSTONLY: {CharStash.UID}");
                
                Character Char0 = __instance.LastCharacter;
                HostOnlyStash.Log.LogMessage($"HOSTONLY: {Char0.Name}");
                
                Char0.CharacterUI.StashPanel.SetStash(CharStash);
                CharStash.ShowContent(Char0);
                __instance.enabled = true;
                
                return false;

                //Character Char0 = CharacterManager.Instance.GetCharacter(CharacterManager.Instance.PlayerCharacters.Values[0]);
                //Char0.CharacterUI.StashPanel.SetStash(CharStash);
                //Char0.CharacterUI.StashPanel.
                //if (CharacterManager.Instance.PlayerCharacters.Count > 1)
                //{
                //    Character Char1 = CharacterManager.Instance.GetCharacter(CharacterManager.Instance.PlayerCharacters.Values[1]);
                //    Char1.CharacterUI.StashPanel.SetStash(CharStash);
                //}
            }
            return true;
        }

    }
}
