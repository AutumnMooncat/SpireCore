using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace AutumnMooncat.SpireCore.Patches;

[HarmonyPatch]
public class TooltipFixPatch
{
    public static string TooltipFixKey => "TooltipFix";
    public static string TooltipAdditionKey => "TooltipAddition";

    public record Defer { }
    
    public static IEnumerable<MethodBase> TargetMethods()
    {
        var orig = AccessTools.Method(typeof(CardAction), nameof(CardAction.GetTooltips));
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsAssignableTo(typeof(CardAction)))
            .SelectMany(type => type.GetMethods())
            .Where(method => method.GetBaseDefinition() == orig && method.DeclaringType != orig.DeclaringType);
    }
    
    public static void Postfix(CardAction __instance, List<Tooltip> __result)
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
                            if (pair.Value is { } newArgs && ttg.vals is { } args)
                            {
                                for (var i = 0; i < newArgs.Length; i++)
                                {
                                    if (newArgs[i] is Defer && args.Length >= i)
                                    {
                                        newArgs[i] = args[i];
                                    }
                                } 
                            }
                            
                            ttg.vals = pair.Value;
                        }
                    }
                }
            }
        }

        if (__instance.GetExtraTooltips(out var extra))
        {
            __result.AddRange(extra);
        }
    }
}