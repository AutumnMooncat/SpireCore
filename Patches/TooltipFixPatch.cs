using System.Collections.Generic;
using HarmonyLib;

namespace AutumnMooncat.SpireCore.Patches;

[HarmonyPatch]
public class TooltipFixPatch
{
    public static string TooltipFixKey => "TooltipFix";

    [HarmonyPatch(typeof(AAttack), nameof(AAttack.GetTooltips))]
    public static class AttackFix
    {
        public static void Postfix(AAttack __instance, List<Tooltip> __result)
        {
            if (__instance.GetTooltipFixes(out var fixes))
            {
                foreach (var pair in fixes)
                {
                    foreach (var tooltip in __result)
                    {
                        if (tooltip is TTGlossary ttg)
                        {
                            if (ttg.key == pair.Key)
                            {
                                ttg.vals = pair.Value;
                            }
                        }
                    }
                }
            }
        }
    }
}