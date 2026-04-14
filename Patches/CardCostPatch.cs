using AutumnMooncat.SpireCore.Features;
using HarmonyLib;

namespace AutumnMooncat.SpireCore.Patches;

[HarmonyPatch(typeof(Card), nameof(Card.GetCurrentCost))]
public class CardCostPatch
{
    public static void Postfix(Card __instance, ref int __result, State s)
    {
        if (s.route is Combat c)
        {
            var power = s.ship.Get(PowerCoreStatus.Entry.Status);
            if (__result > c.energy && __result <= c.energy + power)
            {
                __instance.SetData(PowerCoreDiscount.ID, __result - c.energy);
                __result = c.energy;
            }
            else
            {
                __instance.RemoveData(PowerCoreDiscount.ID);
            }
        }
    }
}