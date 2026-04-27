using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using AutumnMooncat.SpireCore.Characters;
using AutumnMooncat.SpireCore.Features.Dialogue;
using FMOD;
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

    public static void OnPlayCard(Combat c, Card card)
    {
        if (CardDialogue.CardLookups.TryGetValue(card.GetType(), out var key))
        {
            c.QueueImmediate(new ADummyAction { dialogueSelector = "."+key });
        }
    }

    [HarmonyPatch(typeof(Shout), nameof(Shout.GetCharBabble))]
    public static class BabblePatch
    {
        public static void Postfix(ref GUID __result, string name)
        {
            if (name == CType.Ironclad)
            {
                __result = FSPRO.Event.Babble_chunk;
            }
            else if (name == CType.Silent)
            {
                __result = FSPRO.Event.Babble_peri;
            }
            else if (name == CType.Defect)
            {
                __result = FSPRO.Event.Babble_scrap;
            }
            else if (name == CType.Watcher)
            {
                __result = FSPRO.Event.Babble_riggs;
            }
        }
    }

    [HarmonyPatch(typeof(Say), nameof(Say.Execute))]
    public static class SayPatch
    {
        public static string ReplacementKey => "Replacements";
        
        public static void Postfix(Say __instance, bool __result, G g)
        {
            //MainModFile.Log("Ran Say [{},{}], result {}", __instance.who, __instance._keyCache, __result);
            if (!__result && __instance.GetOnExecutes(out var data))
            {
                foreach (var payload in data)
                {
                    payload.Apply(g.state.storyVars);
                }
            }
        }

        public static Shout ManipulateShout(Shout shout, Say say, ScriptCtx ctx)
        {
            //MainModFile.Log("Shout assembled for {}, key {}", shout.who, shout.key);
            if (say.GetData(ReplacementKey, out Dictionary<string, Say> data))
            {
                //MainModFile.Log("Found replacements: [{}], has {}", data.Keys, shout.who);
                if (data.TryGetValue(shout.who, out var line))
                {
                    //line._keyCache ??= Say.GetLocKey(ctx.script, $"{line.who}::{line.hash}");
                    //shout.flipped = line.flipped;
                    shout.loopTag = line.loopTag ?? shout.loopTag;
                    shout.key = Say.GetLocKey(ctx.script, $"{line.who}::{line.hash}");
                    //shout.delay = line.delay;
                    //__instance.choiceFunc = line.choiceFunc;
                    MainModFile.Log("Applied replacement [{}]", shout.key);

                    if (line.HasSilentFlag() || (shout.who == CType.Silent && !line.HasNotSilentFlag()))
                    {
                        return shout.WithSilentFlag();
                    }
                }
            }

            if (say.HasSilentFlag() || (shout.who == CType.Silent && !say.HasNotSilentFlag()))
            {
                return shout.WithSilentFlag();
            }
            return shout;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> input, ILGenerator generator)
        {
            var codes = input.ToList();
            var sayChoice = AccessTools.Field(typeof(Say), nameof(Say.choiceFunc));
            var shoutChoice = AccessTools.Field(typeof(Shout), nameof(Shout.choiceFunc));
            var inserted = false;
            for (var i = 0; i < codes.Count; i++)
            {
                if (!inserted && i >= 2 && codes[i - 1].opcode == OpCodes.Stfld &&
                    codes[i - 1].operand is FieldInfo f1 && f1 == shoutChoice && codes[i - 2].opcode == OpCodes.Ldfld &&
                    codes[i - 2].operand is FieldInfo f2 && f2 == sayChoice)
                {
                    inserted = true;
                    yield return CodeInstruction.LoadArgument(0);
                    yield return CodeInstruction.LoadArgument(3);
                    yield return CodeInstruction.Call(typeof(SayPatch), nameof(ManipulateShout));
                }
                
                yield return codes[i];
            }
        }
    }

    [HarmonyPatch(typeof(Shout), nameof(Shout.IsSilentLine))]
    public static class ShoutPatch
    {
        public static void Postfix(Shout __instance, ref bool __result) 
        {
            if (__instance.HasSilentFlag())
            {
                __result = true;
            }
        }
    }
    
    [HarmonyPatch(typeof(StoryVars), nameof(StoryVars.ResetAfterRun))]
    public static class StoryVarResetAfterRun
    {
        public static void Postfix(StoryVars __instance)
        {
            __instance.ClearRunData();
        }
    }

    [HarmonyPatch(typeof(StoryVars), nameof(StoryVars.ResetAfterCombatLine))]
    public static class StoryVarResetAfterCombat
    {
        public static void Postfix(StoryVars __instance)
        {
            __instance.ClearCombatData();
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
            __instance.ClearTurnData();
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

            if (n.GetRequirements(out var data))
            {
                foreach (var req in data)
                {
                    if (!req.Test(s.storyVars))
                    {
                        __result = false;
                        return;
                    }
                }
            }
            
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