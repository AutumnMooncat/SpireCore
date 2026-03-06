using HarmonyLib;

namespace AutumnMooncat.Spirecore.Patches;

[HarmonyPatch]
public static class AttackPatches
{
    public static string DroneOverrideKey => "DroneOverride";
    public static AAttack currAttack;
    public static StuffBase dummy;
    
    [HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin))]
    public static class CurrAttackPatch
    {
        public static void Prefix(AAttack __instance, State s, Combat c)
        {
            currAttack = __instance;
            if (MainModFile.GetHelper().ModData.TryGetModData(__instance, DroneOverrideKey, out bool res))
            {
                if (res && __instance.fromDroneX.HasValue)
                {
                    if (!c.stuff.TryGetValue(__instance.fromDroneX.Value, out var _))
                    {
                        dummy = new FakeDrone();
                        dummy.x = __instance.fromDroneX.Value;
                        c.stuff[dummy.x] = dummy;
                    }
                }
            }
        }

        public static void Finalizer(AAttack __instance, State s, Combat c)
        {
            currAttack = null;
            if (dummy != null)
            {
                int? i = null;
                foreach (var pair in c.stuff)
                {
                    if (pair.Value == dummy)
                    {
                        i = pair.Key;
                    }
                }

                if (i.HasValue)
                {
                    c.stuff.Remove(i.Value);
                }

                dummy = null;
            }
        }
    }
}