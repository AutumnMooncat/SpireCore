using System;
using System.Linq;
using AutumnMooncat.SpireCore.Features;
using HarmonyLib;

namespace AutumnMooncat.SpireCore.Patches;

[HarmonyPatch(typeof(AAttack), nameof(AAttack.ApplyAutododge))]
public class AutododgePatch
{
    public static void Postfix(AAttack __instance, ref bool __result, Combat c, Ship target, RaycastResult ray)
    {
        if (!ray.hitShip && !__instance.isBeam)
        {
            Ship source = __instance.targetPlayer ? c.otherShip : MG.inst.g.state.ship;
            if (source.Get(LockOnStatus.Entry.Status) > 0)
            {
                bool prefRight = true;
                int? best = null;
                for (int index = 0; index < target.parts.Count; ++index)
                {
                    if (target.parts[index].type == PType.cockpit)
                    {
                        prefRight = false;
                    }
                    if (target.parts[index].type != PType.empty)
                    {
                        int delta = target.x + index - ray.worldX;
                        if (best == null || Math.Abs(delta) < Math.Abs(best.Value) || (Math.Abs(delta) == Math.Abs(best.Value) && prefRight))
                        {
                            best = delta;
                        }
                    }
                }

                if (best != null)
                {
                    source.Add(LockOnStatus.Entry.Status, -1);
                    c.QueueImmediate([
                        new AMove()
                        {
                            targetPlayer = !__instance.targetPlayer,
                            dir = best.Value
                        },
                        __instance
                    ]);
                    __instance.timer = 0.0;
                    __result = true;
                }
            }
        }
    }
}