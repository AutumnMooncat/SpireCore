using System;
using System.Collections.Generic;
using AutumnMooncat.Spirecore.Util;

namespace AutumnMooncat.Spirecore.Features;

public class PlasmaObject : StuffBase, ITooltipHelper
{
    public static string ID => nameof(PlasmaObject);
    public static Spr icon = CommonIcons.Find("plasmaIcon");
    public static Spr obj = IRegisterable.LookUpSpr(IRegisterable.DefaultAssetPath + "stuff/plasma")!.Value;
    public static int Energy => 2;
    
    public override bool IsHostile()
    {
        return false;
    }

    public override bool GetIsDoneAnimatingAsExtraStuff()
    {
        return yAnimation > 14.0;
    }

    public override Spr? GetIcon()
    {
        return icon;
    }

    public override List<Tooltip> GetTooltips()
    {
        List<Tooltip> ret =
        [
            ITooltipHelper.MakeTooltip("stuff", ID, GetIcon(), Colors.drone, null, new {Energy}),
            ChargeStatus.GetTooltip
        ];
        if (bubbleShield)
            ret.Add(new TTGlossary("midrow.bubbleShield"));
        return ret;
    }

    public override void Render(G g, Vec v)
    {
        Vec vec = v + GetOffset(g, true) + new Vec(Math.Sin(x + g.state.time * 10.0), Math.Cos(x + g.state.time * 20.0 + Math.PI / 2.0)).round();
        DrawWithHilight(g, obj, vec, false, false);
    }

    public override List<CardAction> GetActionsOnDestroyed(State s, Combat c, bool wasPlayer, int worldX)
    {
        return [
            new AEnergy()
            {
                changeAmount = Energy
            }
            /*new AAttack()
            {
                isBeam = true,
                fromDroneX = x,
                targetPlayer = targetPlayer,
                damage = 0,
                status = Status.shield,
                statusAmount = 1
            }*/
        ];
    }

    public override void DoDestroyedEffect(State s, Combat c)
    {
        s.AddShake(1.0);
        c.fx.Add(new DroneExplosion()
        {
            pos = new Vec(x * 16, 60.0) + new Vec(7.5, 4.0)
        });
    }
}