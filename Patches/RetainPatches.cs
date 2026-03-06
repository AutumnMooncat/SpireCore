using AutumnMooncat.Spirecore.Features;
using HarmonyLib;

namespace AutumnMooncat.Spirecore.Patches;

[HarmonyPatch]
public static class RetainPatches
{
    
    [HarmonyPatch(typeof(AEndTurn), nameof(AEndTurn.Begin))]
    public static class RetainToggle
    {
        public static void Finalizer(Combat c)
        {
            var amt = MG.inst.g.state.ship.Get(EquilibriumStatus.Entry.Status);
            MainModFile.Log("Equilibrium check: {}", amt);
            if (amt > 0)
            {
                c.cardActions.RemoveAll(a => a is ADiscard);
            }
        }
    }
}