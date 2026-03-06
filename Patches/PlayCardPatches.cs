using System;
using AutumnMooncat.Spirecore.Features;
using HarmonyLib;

namespace AutumnMooncat.Spirecore.Patches;

[HarmonyPatch]
public static class PlayCardPatches
{
    public static Card cardInPlay;
    public static Card ignoreThis;
    
    [HarmonyPatch(typeof(Combat), nameof(Combat.TryPlayCard))]
    public static class CardContextPatch
    {
        public static void Prefix(Combat __instance, Card card)
        {
            cardInPlay = card;
        }

        public static void Finalizer(Combat __instance, Card card)
        {
            cardInPlay = null;
            ignoreThis = null;
        }
    }

    [HarmonyPatch(typeof(Combat), nameof(Combat.SendCardToDiscard))]
    public static class DoReboundPatch
    {
        public static bool Prefix(Combat __instance, State s, Card card)
        {
            if (cardInPlay != null && !card.GetDataWithOverrides(s).recycle && card != ignoreThis)
            {
                //MainModFile.Log("Rebound checking for {}", cardInPlay);
                int rebound = s.ship.Get(ReboundStatus.Entry.Status);
                if (rebound > 0)
                {
                    MainModFile.Log("Rebound applied to {}", cardInPlay);
                    s.ship.Set(ReboundStatus.Entry.Status, rebound - 1);
                    s.SendCardToDeck(card);
                    return false;
                }
            }
            return true;
        }
    }
}