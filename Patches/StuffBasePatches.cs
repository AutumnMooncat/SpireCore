using HarmonyLib;

namespace AutumnMooncat.Spirecore.Patches;

[HarmonyPatch]
public static class StuffBasePatches
{
    [HarmonyPatch(typeof(StuffBase), nameof(StuffBase.RenderShield))]
    public static class RenderShield
    {
        public static bool Prefix(StuffBase __instance, G g)
        {
            if (__instance is IDroneShieldOverride dso)
            {
                dso.RenderShieldOverride(g);
                return false;
            }
            return true;
        }
    }
}