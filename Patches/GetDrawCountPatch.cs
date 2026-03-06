using AutumnMooncat.Spirecore.Features;
using HarmonyLib;

namespace AutumnMooncat.Spirecore.Patches;

[HarmonyPatch(typeof(Combat), nameof(Combat.GetDrawCount))]
public static class GetDrawCountPatch
{
    private static int _modified;
    
    public static void Prefix(Combat __instance, State s)
    {
        int up = s.ship.Get(DrawUpStatus.Entry.Status);
        int down = s.ship.Get(DrawDownStatus.Entry.Status);
        s.ship.baseDraw += up - down;
        _modified = up - down;
    }
    
    public static void Finalizer(Combat __instance, State s)
    {
        s.ship.baseDraw -= _modified;
        _modified = 0;
    }
}