using System;
using System.Collections.Generic;
using System.Linq;
using AutumnMooncat.Spirecore.Features;
using AutumnMooncat.Spirecore.Patches;
using AutumnMooncat.Spirecore.Util;
using FSPRO;

namespace AutumnMooncat.Spirecore.Actions;

public class ALaunchShiv : CardAction, ITooltipHelper, IMultiIconAction
{
    public static string ID => nameof(ALaunchShiv);
    private const double ShipDistanceFromMidrow = 40;
    private const double TopOuterSpaceDistanceFromMidrow = 100;
    private const double BottomOuterSpaceDistanceFromMidrow = 180;
    private const double DistancePerSecond = (ShipDistanceFromMidrow + TopOuterSpaceDistanceFromMidrow) / 0.75;

    public int? fromX;
    public bool targetPlayer;
    public ShivObject shiv;
    public double initialPosition;
    public double finalPosition;
    public bool bubble;
    public bool punchThrough;
    public bool passThrough;
    
    public override List<Tooltip> GetTooltips(State s)
    {
        for (var partIndex = 0; partIndex < s.ship.parts.Count; partIndex++)
        {
            var part = s.ship.parts[partIndex];
            if (part.type != PType.missiles || !part.active)
                continue;

            part.hilight = true;
            if (s.route is Combat combat && combat.stuff.TryGetValue(s.ship.x + partIndex, out var @object))
                @object.hilight = 2;
        }

        List<Tooltip> ret =
        [
            ITooltipHelper.MakeTooltip("action", ID, CommonIcons.Launch, Colors.action),
            ITooltipHelper.MakeTooltip("stuff", ShivObject.ID, ShivObject.icon, Colors.drone, null,
                new { ShivObject.Damage })
        ];
        if (bubble)
        {
            ret.Add(new TTGlossary("midrow.bubbleShield"));
        }
        return ret;
    }

    public override void Begin(G g, State s, Combat c)
    {
        var ownerShip = targetPlayer ? c.otherShip : s.ship;
        var targetShip = targetPlayer ? s.ship : c.otherShip;
        
        int worldX = fromX ?? 0;
        
        if (fromX == null)
        {
            List<int> bays = ownerShip.parts.Select((p, i) => p.type == PType.missiles && p.active ? i : -1).Where(i => i >= 0).ToList();
            
            if (bays.Count == 0)
            {
                timer = 0.4;
                return;
            }

            if (bays.Count > 1)
            {
                timer = 0;
                List<CardAction> copies = [];
                foreach (var bay in bays)
                {
                    var copy = Mutil.DeepCopy(this);
                    copy.fromX = ownerShip.x + bay;
                    copies.Add(copy);
                }
                c.QueueImmediate(copies);
                return;
            }
            
            worldX = ownerShip.x + bays[0];
            fromX = worldX;
            //MainModFile.Log("Found missile bay at index {}, setting worldX {}", bays[0], worldX);
        }

        shiv = new ShivObject
        {
            x = worldX,
            xLerped = worldX,
            bubbleShield = bubble
        };
        
        initialPosition = targetPlayer ? -ShipDistanceFromMidrow : ShipDistanceFromMidrow;
        var hasMidrow = c.stuff.ContainsKey(worldX);

        if (hasMidrow && bubble)
        {
            punchThrough = true;
        }
        
        if (hasMidrow && !bubble)
            finalPosition = 0;
        else if (targetShip.GetPartAtWorldX(worldX) is { } targetPart && targetPart.type != PType.empty)
            finalPosition = targetPlayer ? ShipDistanceFromMidrow : -ShipDistanceFromMidrow;
        else
            finalPosition = targetPlayer ? BottomOuterSpaceDistanceFromMidrow : -TopOuterSpaceDistanceFromMidrow;

        timer = Math.Abs(finalPosition - initialPosition) / DistancePerSecond;
        Audio.Play(hasMidrow ? Event.Hits_DroneCollision : Event.Drones_MissileLaunch);
        if (ownerShip.GetPartAtWorldX(worldX) is { } firingPart)
            firingPart.pulse = 1;
    }

    public override void Update(G g, State s, Combat c)
    {
        if (timer <= 0)
        {
            //MainModFile.Log("Action already complete, terminating");
            return;
        }
        
        base.Update(g, s, c);

        if (fromX == null)
        {
            //MainModFile.Log("No worldX on upgrade, terminating");
            return;
        }
        
        var duration = Math.Abs(finalPosition - initialPosition) / DistancePerSecond;
        var progress = 1.0 - Math.Clamp(timer / duration, 0, 1);
        var yOffset = initialPosition + (finalPosition - initialPosition) * progress;
        
        shiv.Update(g);
        RenderCombatPatches.Drones.PreRender.Add(g1 =>
        {
            Box box = g1.Push(null, shiv.GetGetRect());
            shiv.Render(g1, box.rect.xy + new Vec(0, yOffset));
            if (shiv.bubbleShield)
            {
                Vec offset = shiv.GetOffset(g) + new Vec(0, yOffset);
                Draw.Sprite(StableSpr.drones_shield, box.rect.x - 5.0 + offset.x, box.rect.y + 3.0 + offset.y);
            }
            g1.Pop();
        });

        if (punchThrough && Math.Sign(initialPosition) != Math.Sign(yOffset))
        {
            punchThrough = false;
            var stuff2 = c.stuff.GetValueOrDefault(fromX.Value);
            CollideDrone(s, c, stuff2);
            passThrough = true;
        }

        if (timer > 0)
        {
            //MainModFile.Log("Updating... {}", timer);
            return;
        }
        
        var worldX = fromX.Value;
        var stuff = c.stuff.GetValueOrDefault(worldX);
        var targetShip = targetPlayer ? s.ship : c.otherShip;
        var resultPos = new Vec((shiv.x * 16), yOffset) + new Vec(7.5, 4.0);

        //MainModFile.Log("Resolving launch at worldX {}", worldX);
        if (!passThrough && CollideDrone(s, c, stuff))
        {
            return;
        }
        if (targetShip.GetPartAtWorldX(worldX) is { } part && part.type != PType.empty)
        {
            //MainModFile.Log("Hitting enemy {}", part.type);
            var dmg = targetShip.NormalDamage(s, c, ShivObject.Damage, worldX);
            EffectSpawner.NonCannonHit(g, targetPlayer, new RaycastResult{fromDrone = true, hitShip = true, worldX = shiv.x}, dmg);
        }
    }

    public bool CollideDrone(State s, Combat c, StuffBase stuff)
    {
        if (stuff != null)
        {
            shiv.bubbleShield = false;
            //MainModFile.Log("Found collision with stuff {}", stuff);
            var outcome = ASpawn.GetCollisionOutcome(shiv, stuff);
            stuff.bubbleShield = false;

            if (outcome is ASpawn.Outcome.BothDie or ASpawn.Outcome.LaunchedWins)
                c.DestroyDroneAt(s, fromX.Value, !targetPlayer);
            else if (stuff.Invincible())
                c.QueueImmediate(stuff.GetActionsOnBonkedWhileInvincible(s, c, targetPlayer, shiv));
            
            return !bubble;
        }

        return false;
    }

    public static void OnHitFX(State s, Combat c, Vec pos)
    {
        s.AddShake(0.5);
        c.fx.Add(new AsteroidExplosion()
        {
            pos = pos
        });
    }

    public override bool CanSkipTimerIfLastEvent()
    {
        return false;
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon(CommonIcons.Launch, null, Colors.textMain);
    }

    public List<Records.RenderPayload> GetExtraIcons(State s)
    {
        List<Records.RenderPayload> ret = [new (){ spr = ShivObject.icon }];
        if (bubble)
        {
            ret.Add(new Records.RenderPayload{spr = StableSpr.icons_bubbleShield});
        }
        return ret;
    }
}