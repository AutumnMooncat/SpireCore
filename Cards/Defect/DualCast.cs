using System;
using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Actions;
using AutumnMooncat.Spirecore.Features;
using AutumnMooncat.Spirecore.Util;

namespace AutumnMooncat.Spirecore.Cards.Defect;

[IRegisterable.Ignore]
internal sealed class DualCast : Card, IRCard, ITooltipHelper
{
    public static string ID => nameof(DualCast);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Defect.DeckEntry.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = MainModFile.Bind(["card", ID, "name"]).Localize,
            Art = IRCard.LookUpSpr(Characters.Defect.CardAssetPath + ID)
        });
    }
    
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = upgrade == Upgrade.A ? 0 : 1,
            floppable = upgrade == Upgrade.B
        };
        return data;
    }

    public List<Tooltip> GetTips(State s)
    {
        Records.TexturePayload[] pl = 
        [
            new (){spr = CommonIcons.Spawn},
            new (){spr = CommonIcons.Question, x = 10, flipY = s.ship.Get(Status.backwardsMissiles) > 0 != flipped}
        ];
        string suffix = s.ship.Get(Status.backwardsMissiles) > 0 != flipped ? "3" : "2";
        List<Tooltip> ret = 
        [
            ITooltipHelper.MakeTooltip("card", ID, CommonIcons.Catch, Colors.action),
            ITooltipHelper.MakeMultiTooltip("card", ID, pl, 10, Colors.action, suffix)
        ];
        int index = s.ship.parts.FindIndex(p => p.type == PType.missiles && p.active);
        if (index != -1)
        {
            s.ship.parts[index].hilight = true;
            if (s.route is Combat route)
            {
                int key = s.ship.x + index;
                StuffBase stuffBase;
                if (route.stuff.TryGetValue(key, out stuffBase))
                    stuffBase.hilight = 2;
            }
        }
        return ret;
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = [];
        switch (upgrade)
        {
            case Upgrade.None:
            case Upgrade.A:
                actions = 
                [
                    new DelegateAction()
                    {
                        icon = new Icon(CommonIcons.Catch, null, Colors.textMain),
                        getTips = (_, _) => GetTips(s),
                        begin = (_, _, _, _) =>
                        {
                            if (s.ship.parts.FindIndex(p => p.type == PType.missiles && p.active) == -1)
                            {
                                return null;
                            }
                            int worldX = s.ship.parts.FindIndex(p => p.type == PType.missiles && p.active) + s.ship.x;
                            
                            Part partAtWorldX = s.ship.GetPartAtWorldX(worldX);
                            if (partAtWorldX != null)
                            {
                                partAtWorldX.pulse = 1.0;
                            }
                            ParticleBursts.DroneSpawn(worldX, true);
                            //StuffBase stuff;
                            if (!c.stuff.TryGetValue(worldX, out StuffBase stuff))
                            {
                                return null;
                            }
                            StuffBase clone = Mutil.DeepCopy(stuff);
                            clone.targetPlayer = true;
                            Audio.Play(FSPRO.Event.Drones_MissileLaunch);
                            c.stuff.Remove(worldX);
                            c.QueueImmediate(new ASpawn()
                            {
                                thing = clone
                            });
                            StuffBase clone2 = Mutil.DeepCopy(stuff);
                            clone2.targetPlayer = true;
                            Audio.Play(FSPRO.Event.Drones_MissileLaunch);
                            c.stuff.Remove(worldX);
                            c.QueueImmediate(new ASpawn()
                            {
                                thing = clone2,
                                offset = 1
                            });
                            
                            return null;
                        }
                    },
                    new InfoOnlyAction()
                    {
                        extraIcons = 
                        [
                            new (){spr = CommonIcons.Spawn},
                            new (){spr = CommonIcons.SpawnR, amount = 1, color = Colors.drone},
                            new (){spr = CommonIcons.Question, flipY = s.ship.Get(Status.backwardsMissiles) > 0 != flipped}
                        ]
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new DelegateAction()
                    {
                        icon = new Icon(CommonIcons.Catch, null, Colors.textMain),
                        getTips = (_, _) => GetTips(s),
                        begin = (_, _, _, _) =>
                        {
                            if (s.ship.parts.FindIndex(p => p.type == PType.missiles && p.active) == -1)
                            {
                                return null;
                            }
                            int worldX = s.ship.parts.FindIndex(p => p.type == PType.missiles && p.active) + s.ship.x;
                            
                            Part partAtWorldX = s.ship.GetPartAtWorldX(worldX);
                            if (partAtWorldX != null)
                            {
                                partAtWorldX.pulse = 1.0;
                            }
                            ParticleBursts.DroneSpawn(worldX, true);
                            //StuffBase stuff;
                            if (!c.stuff.TryGetValue(worldX, out StuffBase stuff))
                            {
                                return null;
                            }
                            StuffBase clone = Mutil.DeepCopy(stuff);
                            clone.targetPlayer = !flipped;
                            Audio.Play(FSPRO.Event.Drones_MissileLaunch);
                            c.stuff.Remove(worldX);
                            c.QueueImmediate(new ASpawn()
                            {
                                thing = clone
                            });
                            StuffBase clone2 = Mutil.DeepCopy(stuff);
                            clone2.targetPlayer = !flipped;
                            Audio.Play(FSPRO.Event.Drones_MissileLaunch);
                            c.stuff.Remove(worldX);
                            c.QueueImmediate(new ASpawn()
                            {
                                thing = clone2,
                                offset = 1
                            });
                            
                            return null;
                        }
                    },
                    new InfoOnlyAction()
                    {
                        disabled = flipped,
                        extraIcons = 
                        [
                            new (){spr = CommonIcons.Spawn},
                            new (){spr = CommonIcons.SpawnR, amount = 1, color = Colors.drone},
                            new (){spr = CommonIcons.Question, flipY = s.ship.Get(Status.backwardsMissiles) > 0 != flipped}
                        ]
                    },
                    new InfoOnlyAction()
                    {
                        disabled = !flipped,
                        extraIcons = 
                        [
                            new (){spr = CommonIcons.Spawn},
                            new (){spr = CommonIcons.SpawnR, amount = 1, color = Colors.drone},
                            new (){spr = CommonIcons.Question, flipY = s.ship.Get(Status.backwardsMissiles) > 0 == flipped}
                        ]
                    }
                ];
                break;
        }
        return actions;
    }
}