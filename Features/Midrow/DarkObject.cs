using System;
using System.Collections.Generic;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Patches;
using AutumnMooncat.SpireCore.Util;
using Microsoft.Xna.Framework.Graphics;

namespace AutumnMooncat.SpireCore.Features;

public class DarkObject : StuffBase, ITooltipHelper, IDroneShieldOverride
{
    public static string ID => nameof(DarkObject);
    public static Spr icon = CommonIcons.Find("darkIcon");
    public static Spr obj = IRegisterable.LookUpSpr(IRegisterable.DefaultAssetPath + "stuff/dark")!.Value;

    public int damage;
    
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
            ITooltipHelper.MakeTooltip("stuff", ID, GetIcon(), Colors.drone, null, new {Damage = damage})
        ];
        if (bubbleShield)
            ret.Add(new TTGlossary("midrow.bubbleShield"));
        return ret;
    }

    public Vec GetBob(G g)
    {
        return new Vec(Math.Sin(x + g.state.time * 10.0), Math.Cos(x + g.state.time * 20.0 + Math.PI / 2.0)).round();
    } 

    public override void Render(G g, Vec v)
    {
        Vec bob = GetBob(g);
        Vec off = GetOffset(g, true);
        Vec vec = v + off + bob;
        Vec org = new Vec(8, 15);
        var spinny = damage > 0 ? damage : 0.25;
        if (ShouldDrawHilight(g))
        {
            Texture2D outlined = SpriteLoader.GetOutlined(obj);
            Draw.Sprite(outlined, vec.x + org.x, vec.y + org.y, false, false, (g.state.time * spinny) % (2 * Math.PI), null, new Vec(0.5, 0.5));
        }
        Draw.Sprite(obj, vec.x + org.x, vec.y + org.y, false, false, (g.state.time * spinny) % (2 * Math.PI), null, new Vec(0.5, 0.5));
    }

    public void RenderShieldOverride(G g)
    {
        if (!bubbleShield)
        {
            return;
        }
        
        Vec vec = GetOffset(g) + GetBob(g);
        Box box = g.Push(null, GetGetRect());
        Draw.Sprite((StableSpr.drones_shield), box.rect.x - 4.5 + vec.x, box.rect.y + 2.5 + vec.y);
        g.Pop();
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return [
            new DelegateAction()
            {
                begin = ((g, state, combat, thiz) => { 
                    damage++;
                    return null;
                })
            }
        ];
    }

    public override List<CardAction> GetActionsOnDestroyed(State s, Combat c, bool wasPlayer, int worldX)
    {
        var atk = new AAttack()
        {
            fromDroneX = worldX,
            targetPlayer = false,
            damage = damage
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