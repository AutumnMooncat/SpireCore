using AutumnMooncat.Spirecore.Features;
using HarmonyLib;

namespace AutumnMooncat.Spirecore.Patches;

[HarmonyPatch(typeof(Combat), nameof(Combat.SendCardToExhaust))]
public static class ExhaustPatch
{
    [HarmonyPostfix]
    public static void OnExhaust(Combat __instance, State s, Card card)
    {
        FeelNoPainStatus.OnExhaust(s, __instance, card);
        foreach (var artifact in s.EnumerateAllArtifacts())
        {
            if (artifact is IArtifactOnExhaust aod)
            {
                aod.OnExhaust(s, __instance, card);
            }
        }
    }
}