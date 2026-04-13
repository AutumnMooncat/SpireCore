using System.Linq;
using AutumnMooncat.SpireCore.Characters;
using AutumnMooncat.SpireCore.Features.Dialogue;
using HarmonyLib;
using Nickel;

namespace AutumnMooncat.SpireCore.Patches;

[HarmonyPatch]
public class DialoguePatches : IRDialogue
{
    public static void Register(IModHelper helper)
    {
        helper.Events.RegisterAfterArtifactsHook(nameof(Artifact.OnPlayerPlayCard), (Card card, State state) =>
        {
            if (!card.GetDataWithOverrides(state).recycle)
                return;
            state.storyVars.SetJustPlayedRecycleCard(true);
        }, double.NegativeInfinity);
    }

    [HarmonyPatch(typeof(StoryVars), nameof(StoryVars.ResetAfterCombatLine))]
    public static class StoryVarResetAfterCombat
    {
        public static void Postfix(StoryVars __instance)
        {
            MainModFile.GetHelper().ModData.RemoveModData(__instance, "JustPlayedRecycleCard");
            MainModFile.GetHelper().ModData.RemoveModData(__instance, "Strengthened");
            MainModFile.GetHelper().ModData.RemoveModData(__instance, "Discounted");
        }
    }

    [HarmonyPatch(typeof(StoryVars), nameof(StoryVars.ResetAfterEndTurn))]
    public static class StoryVarResetAfterTurn
    {
        public static void Postfix(StoryVars __instance)
        {
            MainModFile.GetHelper().ModData.RemoveModData(__instance, "ShieldLostThisTurn");
        }
    }

    [HarmonyPatch(typeof(StoryNode), nameof(StoryNode.Filter))]
    public static class StoryNodeFilter
    {
        public static void Postfix(StoryNode n, State s, ref bool __result)
        {
            if (!__result)
                return;

            if (s.storyVars.GetShieldLostThisTurn() < n.GetMinShieldLostThisTurn())
            {
                __result = false;
                return;
            }
            if (n.GetJustPlayedRecycleCard() is { } justPlayedRecycleCard && s.storyVars.GetJustPlayedRecycleCard() != justPlayedRecycleCard)
            {
                __result = false;
                return;
            }
            if (n.GetStrengthened() is { } strengthened && s.storyVars.GetStrengthened() != strengthened)
            {
                __result = false;
                return;
            }
            if (n.GetDiscounted() is { } discounted && s.storyVars.GetDiscounted() != discounted)
            {
                __result = false;
                return;
            }
        }
    }

    [HarmonyPatch(typeof(AStatus), nameof(AStatus.Begin))]
    public static class AStatusBegin
    {
        public static void Prefix(AStatus __instance, State s, out int __state)
        {
            __state = __instance.targetPlayer ? s.ship.Get(__instance.status) : 0;
        }

        public static void Postfix(AStatus __instance, State s, Combat c, in int __state)
        {
            if (!__instance.targetPlayer)
                return;

            if (__instance.status == Ironclad.Entry.MissingStatus.Status && __state > 0 && s.ship.Get(Ironclad.Entry.MissingStatus.Status) <= 0)
                c.QueueImmediate(new ADummyAction { dialogueSelector = MainModFile.MakeID(Ironclad.ID+"ReturningFromMissing") });
            
            if (__instance.status == Silent.Entry.MissingStatus.Status && __state > 0 && s.ship.Get(Silent.Entry.MissingStatus.Status) <= 0)
                c.QueueImmediate(new ADummyAction { dialogueSelector = MainModFile.MakeID(Silent.ID+"ReturningFromMissing") });
            
            if (__instance.status == Defect.Entry.MissingStatus.Status && __state > 0 && s.ship.Get(Defect.Entry.MissingStatus.Status) <= 0)
                c.QueueImmediate(new ADummyAction { dialogueSelector = MainModFile.MakeID(Defect.ID+"ReturningFromMissing") });
            
            if (__instance.status == Watcher.Entry.MissingStatus.Status && __state > 0 && s.ship.Get(Watcher.Entry.MissingStatus.Status) <= 0)
                c.QueueImmediate(new ADummyAction { dialogueSelector = MainModFile.MakeID(Watcher.ID+"ReturningFromMissing") });
        }
    }

    [HarmonyPatch(typeof(Ship), nameof(Ship.NormalDamage))]
    public static class ShipNormalDamage
    {
        [HarmonyPriority(Priority.First)]
        public static void Prefix(Ship __instance, out int __state)
        {
            __state = __instance.Get(Status.shield) + __instance.Get(Status.tempShield);
        }
        
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(Ship __instance, State s, in int __state)
        {
            var newShields = __instance.Get(Status.shield) + __instance.Get(Status.tempShield);
            if (newShields >= __state)
                return;

            s.storyVars.SetShieldLostThisTurn(s.storyVars.GetShieldLostThisTurn() + (__state - newShields));
        }
    }

    [HarmonyPatch(typeof(Combat), nameof(Combat.Update))]
    public static class CombatUpdate
    {
        public static void Postfix(Combat __instance, G g)
        {
            var currentCardsInDeck = g.state.GetAllCards().Select(card => card.uuid).ToHashSet();
            var lastCardsInDeck = __instance.GetLastCardIdsInDeck();

            foreach (var cardId in currentCardsInDeck)
            {
                if (lastCardsInDeck.Contains(cardId))
                    continue;
                if (g.state.FindCard(cardId) is not { } card)
                    continue;

                var meta = card.GetMeta();
                if (meta.deck != Ironclad.DeckEntry.Deck && NewRunOptions.allChars.Contains(meta.deck) && card.GetDataWithOverrides(g.state).temporary)
                    __instance.QueueImmediate(new ADummyAction { dialogueSelector = MainModFile.MakeID("NewNon"+Ironclad.ID+"NonTrashTempCard") });
                
                if (meta.deck != Silent.DeckEntry.Deck && NewRunOptions.allChars.Contains(meta.deck) && card.GetDataWithOverrides(g.state).temporary)
                    __instance.QueueImmediate(new ADummyAction { dialogueSelector = MainModFile.MakeID("NewNon"+Silent.ID+"NonTrashTempCard") });
                
                if (meta.deck != Defect.DeckEntry.Deck && NewRunOptions.allChars.Contains(meta.deck) && card.GetDataWithOverrides(g.state).temporary)
                    __instance.QueueImmediate(new ADummyAction { dialogueSelector = MainModFile.MakeID("NewNon"+Defect.ID+"NonTrashTempCard") });
                
                if (meta.deck != Watcher.DeckEntry.Deck && NewRunOptions.allChars.Contains(meta.deck) && card.GetDataWithOverrides(g.state).temporary)
                    __instance.QueueImmediate(new ADummyAction { dialogueSelector = MainModFile.MakeID("NewNon"+Watcher.ID+"NonTrashTempCard") });
            }

            MainModFile.GetHelper().ModData.SetModData(__instance, "LastCardIdsInDeck", currentCardsInDeck);
        }
    }
}