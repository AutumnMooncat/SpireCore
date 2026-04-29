using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using AutumnMooncat.SpireCore.Characters;
using AutumnMooncat.SpireCore.Features.Dialogue;
using daisyowl.text;
using FMOD;
using FMOD.Studio;
using HarmonyLib;
using Microsoft.Xna.Framework;
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

    [HarmonyPatch(typeof(Shout), nameof(Shout.Update))]
    public static class ShoutVolumeCheck
    {
        public static float? shoutVol;

        public static void Prefix(Shout __instance)
        {
            shoutVol = __instance.GetVolume();
        }

        public static void Finalizer()
        {
            shoutVol = null;
        }
    }

    [HarmonyPatch(typeof(Audio), nameof(Audio.GetEventInst))]
    public static class ShoutVolumePatch
    {
        public static void Postfix(EventInstance? __result)
        {
            if (ShoutVolumeCheck.shoutVol is {} mult && __result is {} sound)
            {
                if (sound.getVolume(out var vol) == RESULT.OK)
                {
                    sound.setVolume(vol * mult);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Character), nameof(Character.Render))]
    public static class CharacterThinkingPatch
    {
        public static bool isThinking;
        
        public static void Prefix(Character __instance)
        {
            if (__instance.shout is { } shout && shout.HasThoughtFlag())
            {
                isThinking = true;
            }
        }

        public static void Finalizer()
        {
            isThinking = false;
        }
    }
    
    [HarmonyPatch(typeof(Blurbs), nameof(Blurbs.Render))]
    public static class CharacterBlurbCheckPatch
    {
        public static bool isAboutToRenderThought;
        
        public static void Prefix()
        {
            if (CharacterThinkingPatch.isThinking)
            {
                isAboutToRenderThought = true;
            }
        }

        public static void Finalizer()
        {
            isAboutToRenderThought = false;
        }
    }

    [HarmonyPatch(typeof(Dialogue), nameof(Dialogue.Render))]
    public static class DialogueThinkingPatch
    {
        public static bool isAboutToRenderThought;

        public static void ThinkCheck(Dialogue __instance)
        {
            if (__instance.shout?.HasThoughtFlag() ?? false)
            {
                isAboutToRenderThought = true;
            }
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> input, ILGenerator generator)
        {
            var codes = input.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                yield return codes[i];
                if (codes[i].opcode == OpCodes.Call && codes[i].operand is MethodInfo mi &&
                    mi == AccessTools.Method(typeof(Character), nameof(Character.GetTextColor)))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(DialogueThinkingPatch), nameof(ThinkCheck)));
                }
            }
        }
    }

    [HarmonyPatch(typeof(Draw), nameof(Draw.Text))]
    public static class ToggleAfterRendering
    {
        public static void Finalizer(string str)
        {
            DialogueThinkingPatch.isAboutToRenderThought = false;
        }
    }
    
    [HarmonyPatch(typeof(PixelFontRenderer), nameof(PixelFontRenderer.ActuallyRenderText))]
    public static class SheerTogglePatch
    {
        public static bool isThinkingAndDrawing;
        
        public static void Prefix()
        {
            if (CharacterBlurbCheckPatch.isAboutToRenderThought || DialogueThinkingPatch.isAboutToRenderThought)
            {
                isThinkingAndDrawing = true;
            }
        }

        public static void Finalizer()
        {
            isThinkingAndDrawing = false;
        }
    }

    [HarmonyPatch(typeof(Draw), nameof(Draw.RenderCharacter))]
    public static class SheerCharacter
    {
        public static void Prefix(Rect dst)
        {
            if (SheerTogglePatch.isThinkingAndDrawing)
            {
                CameraControl.Apply((float)dst.x, (float)dst.y);
            }
        }

        public static void Finalizer()
        {
            CameraControl.Cleanup();
        }
    }
    
    [HarmonyPatch(typeof(Draw), nameof(Draw.RenderCharacterOutline))]
    public static class SheerCharacterOutline
    {
        public static void Prefix(Rect dst)
        {
            if (SheerTogglePatch.isThinkingAndDrawing)
            {
                CameraControl.Apply((float)dst.x, (float)dst.y);
            }
        }

        public static void Finalizer()
        {
            CameraControl.Cleanup();
        }
    }

    public static class CameraControl
    {
        private static Matrix? backup;
        private static readonly Matrix sheer = new Matrix(
            1f, 0f, 0f, 0f, 
            -0.5f, 1f, 0f, 0f, 
            0f, 0f, 1f, 0f, 
            0f, 0f, 0f, 1f);

        public static void Apply(float x, float y)
        {
            backup = MG.inst.cameraMatrix;
            var vec = new Vector3(x, y, 0f) * MG.inst.PIX_SCALE;
            MG.inst.cameraMatrix *= Matrix.CreateTranslation(-vec);
            MG.inst.cameraMatrix *= sheer;
            MG.inst.cameraMatrix *= Matrix.CreateTranslation(vec);
            try
            {
                Draw.EndAutoBatchFrame();
                Draw.StartAutoBatchFrame();
            }
            catch
            {
                // ignored
            }
        }

        public static void Cleanup()
        {
            if (backup is {} mat)
            {
                MG.inst.cameraMatrix = mat;
                backup = null;
                try
                {
                    Draw.EndAutoBatchFrame();
                    Draw.StartAutoBatchFrame();
                }
                catch
                {
                    // ignored
                }
            }
        }
    }

    [HarmonyPatch(typeof(Say), nameof(Say.Execute))]
    public static class SayPatch
    {
        public static string ReplacementKey => "Replacements";
        
        public static void Postfix(Say __instance, bool __result, G g)
        {
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
            var check = say;
            //MainModFile.Log("Shout assembled for {}, key {}", shout.who, shout.key);
            if (say.GetData(ReplacementKey, out Dictionary<string, Say> data))
            {
                //MainModFile.Log("Found replacements: [{}], has {}", data.Keys, shout.who);
                if (data.TryGetValue(shout.who, out var line))
                {
                    check = line;
                    shout.flipped = line.flipped;
                    shout.loopTag = line.loopTag ?? shout.loopTag;
                    shout.key = Say.GetLocKey(ctx.script, $"{line.who}::{line.hash}");
                    shout.delay = line.delay;
                    //MainModFile.Log("Applied replacement [{}]", shout.key);
                }
            }
            
            if (check.HasSilentFlag())
            {
                shout.WithSilentFlag();
            }

            if (check.HasThoughtFlag() || (shout.who == CType.Silent && !check.HasNotThoughtFlag()))
            {
                shout.WithThoughtFlag();
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
    
    [HarmonyPatch(typeof(Shout), nameof(Shout.GetText))]
    public static class ShoutGetTextPatch
    {
        public static void Postfix(Shout __instance, ref string __result)
        {
            if (__instance.HasThoughtFlag())
            {
                __result = "<c=disabledText>" + __result + "</c>";
            }
        }
    }

    [HarmonyPatch(typeof(Shout), nameof(Shout.IsSilentLine))]
    public static class ShoutIsSilentPatch
    {
        public static void Postfix(Shout __instance, ref bool __result) 
        {
            if (__instance.HasSilentFlag())
            {
                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(Combat), nameof(Combat.PlayerLost))]
    public static class PlayerLostPatch
    {
        public static void Prefix(Combat __instance, G g)
        {
            if (g.state.characters.Any(c => c.deckType?.Key() == CType.Ironclad))
            {
                g.state.storyVars.AddPersistentData(StoryTags.ICLossCounter, g.state.storyVars.GetOrMakeData(StoryTags.ICLossCounter, 0) + 1);
                if (__instance.GetData(StoryTags.ICDrakeTheBetIsOn, out bool bet) && bet)
                {
                    if (__instance.otherShip.ai?.Key() == nameof(DrakePirate) && g.state.characters.All(c => c.deckType?.Key() != CType.Drake))
                    {
                        g.state.storyVars.AddPersistentData(StoryTags.DrakeBeatICCounter, g.state.storyVars.GetOrMakeData(StoryTags.DrakeBeatICCounter, 0) + 1);
                    }
                }
            }
        }
    }
    
    [HarmonyPatch(typeof(Combat), nameof(Combat.PlayerWon))]
    public static class PlayerWonPatch
    {
        public static void Prefix(Combat __instance, G g)
        {
            if (g.state.characters.Any(c => c.deckType?.Key() == CType.Ironclad))
            {
                if (__instance.GetData(StoryTags.ICDrakeTheBetIsOn, out bool bet) && bet)
                {
                    if (__instance.otherShip.ai?.Key() == nameof(DrakePirate) && g.state.characters.All(c => c.deckType?.Key() != CType.Drake))
                    {
                        g.state.storyVars.AddPersistentData(StoryTags.ICBeatDrakeCounter, g.state.storyVars.GetOrMakeData(StoryTags.ICBeatDrakeCounter, 0) + 1);
                    }
                }
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
        public static void Postfix(StoryNode n, State s, StorySearch ctx, ref bool __result)
        {
            if (!__result)
                return;

            if (n.GetRequirements(out var data))
            {
                foreach (var req in data)
                {
                    if (!req.Test(s, s.storyVars, ctx))
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
                c.QueueImmediate(new ADummyAction { dialogueSelector = LookupKeys.IroncladReturnedFromMissing });
            
            if (__instance.status == Silent.Entry.MissingStatus.Status && __state > 0 && s.ship.Get(Silent.Entry.MissingStatus.Status) <= 0)
                c.QueueImmediate(new ADummyAction { dialogueSelector = LookupKeys.SilentReturnedFromMissing });
            
            if (__instance.status == Defect.Entry.MissingStatus.Status && __state > 0 && s.ship.Get(Defect.Entry.MissingStatus.Status) <= 0)
                c.QueueImmediate(new ADummyAction { dialogueSelector = LookupKeys.DefectReturnedFromMissing });
            
            if (__instance.status == Watcher.Entry.MissingStatus.Status && __state > 0 && s.ship.Get(Watcher.Entry.MissingStatus.Status) <= 0)
                c.QueueImmediate(new ADummyAction { dialogueSelector = LookupKeys.WatcherReturnedFromMissing });
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