using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace AutumnMooncat.Spirecore.Patches;

[HarmonyPatch]
public class HilightOtherCardsPatch
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        var orig = AccessTools.Method(typeof(Card), nameof(Card.HilightOtherCards));
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsAssignableTo(typeof(Card)))
            .SelectMany(type => type.GetMethods())
            .Where(method => method.GetBaseDefinition() == orig && method.DeclaringType != orig.DeclaringType)
            .Append(orig);
    }

    public static void Postfix(Card __instance, State s, Combat c) // object[] __args, MethodBase __originalMethod
    {
        foreach (var action in __instance.GetActions(s, c))
        {
            if (action is IHilightingAction iha)
            {
                iha.HilightOtherCards(__instance, s, c);
            }
        }
    }
}