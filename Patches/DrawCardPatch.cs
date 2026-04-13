using AutumnMooncat.SpireCore.Features;
using HarmonyLib;

namespace AutumnMooncat.SpireCore.Patches;

[HarmonyPatch(typeof(Combat), nameof(Combat.DrawCards))]
public static class DrawCardsPatch
{
    public static bool Prefix(Combat __instance, State s, int count)
    {
        if (s.ship.Get(NoDrawStatus.Entry.Status) > 0)
        {
            s.ship.PulseStatus(NoDrawStatus.Entry.Status);
            return false;
        }
        return true;
    }
}