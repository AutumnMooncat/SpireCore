using HarmonyLib;

namespace AutumnMooncat.SpireCore.Patches;

[HarmonyPatch]
public static class UpgradePatches
{
    [HarmonyPatch(typeof(Card), nameof(Card.GetFullDisplayName))]
    public static class CardGetFullDisplayNamePatch
    {
        [HarmonyPostfix]
        public static void Plz(Card __instance, ref string __result)
        {
            if (__instance is ICustomUpgrades icu)
            {
                __result = icu.GetFullDisplayNameOverride(__result);
            }
        }
    }
    
    [HarmonyPatch(typeof(Card), nameof(Card.IsUpgradable))]
    public static class CardIsUpgradablePatch
    {
        [HarmonyPostfix]
        public static void Plz(Card __instance, ref bool __result)
        {
            if (__instance is ICustomUpgrades icu)
            {
                __result = icu.IsUpgradableOverride(__result);
            }
        }
    }
    
    [HarmonyPatch(typeof(Card), nameof(Card.GetMeta))]
    public static class CardGetMetaPatch
    {
        [HarmonyPostfix]
        public static void Plz(Card __instance, ref CardMeta __result)
        {
            if (__instance is ICustomUpgrades icu)
            {
                __result = icu.GetMetaOverride(__result);
            }
        }
    }
    
    [HarmonyPatch(typeof(CardUpgrade), nameof(CardUpgrade.Render))]
    public static class CardUpgradeRenderPatch
    {
        [HarmonyPostfix]
        public static void Plz(CardUpgrade __instance)
        {
            // TODO This actually renders wrong for 1 frame, transpiler can fix this
            if (__instance.topCard is ICustomUpgrades icu)
            {
                __instance.topCard.upgrade = __instance.cardCopy.upgrade;
            }
        }
    }
}