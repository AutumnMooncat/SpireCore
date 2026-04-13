using System;
using System.Collections.Generic;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Patches;
using AutumnMooncat.SpireCore.Util;
using FMOD;

namespace AutumnMooncat.SpireCore.Features;

public class LightningObject : StuffBase, ITooltipHelper
{
    public static string ID => nameof(LightningObject);
    public static Spr icon = CommonIcons.Find("lightningIcon");
    public static Spr obj = IRegisterable.LookUpSpr(IRegisterable.DefaultAssetPath + "stuff/lightning")!.Value;
    public static int Damage => 2;

    public StuffBase backup;
    
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
            ITooltipHelper.MakeTooltip("stuff", ID, GetIcon(), Colors.drone, null, new {Damage}),
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
        var atk = new AAttack()
        {
            fromDroneX = worldX,
            targetPlayer = false,
            damage = Damage
        };
        MainModFile.GetHelper().ModData.SetModData(atk, AttackPatches.DroneOverrideKey, true);
        return [atk];
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