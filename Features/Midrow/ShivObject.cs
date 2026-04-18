using System;
using System.Collections.Generic;
using AutumnMooncat.SpireCore.Util;

namespace AutumnMooncat.SpireCore.Features;

public class ShivObject : Missile, ITooltipHelper
{
    public static string ID => nameof(ShivObject);
    public static Spr icon = CommonIcons.Find("shivIcon");
    public static Spr obj = IRegisterable.LookUpSpr(IRegisterable.DefaultAssetPath + "stuff/shiv")!.Value;
    public static int Damage => 1;
    
    public override bool IsHostile()
    {
        return targetPlayer;
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
            ITooltipHelper.MakeTooltip("stuff", ID, GetIcon(), Colors.drone, null, new {Damage})
        ];
        if (bubbleShield)
            ret.Add(new TTGlossary("midrow.bubbleShield"));
        return ret;
    }

    public override void Render(G g, Vec v)
    {
        Vec vec = v + GetOffset(g, true) + new Vec(Math.Sin(x + g.state.time * 10.0), Math.Cos(x + g.state.time * 20.0 + Math.PI / 2.0)).round();
        DrawWithHilight(g, obj, vec, false, targetPlayer);
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return 
        [
            new AMissileHit()
            {
                worldX = x,
                outgoingDamage = 1
            }
        ];
    }
}