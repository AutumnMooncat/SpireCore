using HarmonyLib;

namespace AutumnMooncat.SpireCore.Patches;

[HarmonyPatch]
public static class DiscardPatches
{
    public static Card lastPlayedCard;
    public static int cardsDiscardedThisTurn;
    
    [HarmonyPatch(typeof(Combat), nameof(Combat.SendCardToDiscard))]
    public static class SendToDiscardPatch
    {
        public static void Prefix(Combat __instance, State s, Card card)
        {
            if (card != lastPlayedCard && __instance.isPlayerTurn)
            {
                cardsDiscardedThisTurn++;
                foreach (var artifact in s.EnumerateAllArtifacts())
                {
                    if (artifact is IArtifactOnDiscard aod)
                    {
                        aod.OnDiscard(s, __instance, card);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(AStartPlayerTurn), nameof(AStartPlayerTurn.Begin))]
    public static class ResetPatch
    {
        public static void Prefix()
        {
            cardsDiscardedThisTurn = 0;
        }
    }

    [HarmonyPatch(typeof(Combat), nameof(Combat.TryPlayCard))]
    public static class IgnorePlayedCardPatch
    {
        public static void Prefix(Card card)
        {
            lastPlayedCard = card;
        }

        public static void Finalizer(Card card)
        {
            lastPlayedCard = null;
        }
    }
}