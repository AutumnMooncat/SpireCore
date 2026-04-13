using HarmonyLib;

namespace AutumnMooncat.SpireCore.Patches;

[HarmonyPatch(typeof(Card), nameof(Card.GetDataWithOverrides))]
public class DataOverridePatch
{
    public static void Postfix(Card __instance, CardData __result, State state)
    {
        if (state.route is Combat c)
        {
            if (!__result.flippable && state.ship.Get(Status.tableFlip) > 0)
            {
                foreach (var action in __instance.GetActions(state, c))
                {
                    if (action is IFlippableAction ifa && ifa.CanFlip(state))
                    {
                        __result.flippable = true;
                    }
                }
            }
        }
    }
}