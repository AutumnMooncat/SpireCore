using System.Collections.Generic;
using HarmonyLib;

namespace AutumnMooncat.SpireCore.Patches;

[HarmonyPatch]
public static class RenderCombatPatches
{
    public delegate void OnRender(G g);
    
    [HarmonyPatch(typeof(Combat), nameof(Combat.RenderDrones))]
    public static class Drones
    {
        public static List<OnRender> PreRender = [];
        public static List<OnRender> PostRender = [];

        public static void Prefix(Combat __instance, G g)
        {
            if (PreRender.Count == 0)
            {
                return;
            }
            g.Push(null, new Rect() + Combat.arenaPos + __instance.GetCamOffset());
            foreach (var onRender in PreRender)
            {
                onRender(g);
            }
            PreRender.Clear();
            g.Pop();
        }

        public static void Postfix(Combat __instance, G g)
        {
            if (PostRender.Count == 0)
            {
                return;
            }
            g.Push(null, new Rect() + Combat.arenaPos + __instance.GetCamOffset());
            foreach (var onRender in PostRender)
            {
                onRender(g);
            }
            PostRender.Clear();
            g.Pop();
        }
    }
    
    [HarmonyPatch(typeof(Combat), nameof(Combat.RenderDiscard))]
    public static class Discard
    {
        public static List<OnRender> PreRender = [];
        public static List<OnRender> PostRender = [];

        public static void Prefix(Combat __instance, G g)
        {
            if (PreRender.Count == 0)
            {
                return;
            }
            g.Push(null, new Rect() + Combat.arenaPos + __instance.GetCamOffset());
            foreach (var onRender in PreRender)
            {
                onRender(g);
            }
            PreRender.Clear();
            g.Pop();
        }

        public static void Postfix(Combat __instance, G g)
        {
            if (PostRender.Count == 0)
            {
                return;
            }
            g.Push(null, new Rect() + Combat.arenaPos + __instance.GetCamOffset());
            foreach (var onRender in PostRender)
            {
                onRender(g);
            }
            PostRender.Clear();
            g.Pop();
        }
    }
}