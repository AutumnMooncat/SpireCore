using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Util;

namespace AutumnMooncat.SpireCore.Cards.Defect;

internal sealed class Recursion : Card, IRCard, ITooltipHelper
{
    public static string ID => nameof(Recursion);
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
            floppable = true,
            art = flipped ? StableSpr.cards_Adaptability_Bottom : StableSpr.cards_Adaptability_Top
        };
        return data;
    }

    public List<Tooltip> GetTips(State s)
    {
        Records.TexturePayload[] pl = 
        [
            new (){spr = CommonIcons.Spawn},
            new (){spr = CommonIcons.Question, x = 10, flipY = s.ship.Get(Status.backwardsMissiles) > 0 == flipped}
        ];
        int suffix = s.ship.Get(Status.backwardsMissiles) > 0 != flipped ? 2 : 3;
        if (upgrade == Upgrade.B)
        {
            suffix += 2;
        }
        List<Tooltip> ret = 
        [
            ITooltipHelper.MakeTooltip("card", ID, CommonIcons.Catch2, Colors.action),
            ITooltipHelper.MakeMultiTooltip("card", ID, pl, 10, Colors.action, suffix.ToString())
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
                    MainModFile.Kokoro().HiddenActions.MakeAction(new DelegateAction()
                    {
                        icon = new Icon(CommonIcons.Catch2, null, Colors.textMain),
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
                            Audio.Play(FSPRO.Event.Click);
                            
                            if (!c.stuff.TryGetValue(worldX, out StuffBase stuff))
                            {
                                return null;
                            }
                            
                            StuffBase clone = Mutil.DeepCopy(stuff);
                            clone.targetPlayer = flipped;
                            c.QueueImmediate(new ASpawn()
                            {
                                thing = clone,
                                offset = 1
                            });
                            
                            return null;
                        }
                    }).SetShowTooltips(true).AsCardAction,
                    new InfoOnlyAction()
                    {
                        disabled = flipped,
                        icon = new Icon(CommonIcons.Catch2, null, Colors.textMain),
                    },
                    new InfoOnlyAction()
                    {
                        disabled = flipped,
                        extraIcons = 
                        [
                            new (){spr = CommonIcons.SpawnR, amount = 1, color = Colors.drone},
                            new (){spr = CommonIcons.Question, flipY = s.ship.Get(Status.backwardsMissiles) == 0}
                        ]
                    },
                    new ADummyAction(),
                    new InfoOnlyAction()
                    {
                        disabled = !flipped,
                        icon = new Icon(CommonIcons.Catch2, null, Colors.textMain),
                    },
                    new InfoOnlyAction()
                    {
                        disabled = !flipped,
                        extraIcons = 
                        [
                            new (){spr = CommonIcons.SpawnR, amount = 1, color = Colors.drone},
                            new (){spr = CommonIcons.Question, flipY = s.ship.Get(Status.backwardsMissiles) > 0}
                        ]
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    MainModFile.Kokoro().HiddenActions.MakeAction(new DelegateAction()
                    {
                        icon = new Icon(CommonIcons.Catch2, null, Colors.textMain),
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
                            Audio.Play(FSPRO.Event.Click);
                            
                            if (!c.stuff.TryGetValue(worldX, out StuffBase stuff))
                            {
                                return null;
                            }
                            
                            StuffBase clone = Mutil.DeepCopy(stuff);
                            clone.targetPlayer = flipped;
                            c.QueueImmediate(new ASpawn()
                            {
                                thing = clone,
                                offset = -1
                            });
                            StuffBase clone2 = Mutil.DeepCopy(stuff);
                            clone2.targetPlayer = flipped;
                            c.QueueImmediate(new ASpawn()
                            {
                                thing = clone2,
                                offset = 1
                            });
                            
                            return null;
                        }
                    }).SetShowTooltips(true).AsCardAction,
                    new InfoOnlyAction()
                    {
                        disabled = flipped,
                        icon = new Icon(CommonIcons.Catch2, null, Colors.textMain),
                    },
                    new InfoOnlyAction()
                    {
                        disabled = flipped,
                        extraIcons = 
                        [
                            new (){spr = CommonIcons.SpawnL},
                            new (){spr = CommonIcons.SpawnR, amount = 1, color = Colors.drone},
                            new (){spr = CommonIcons.Question, flipY = s.ship.Get(Status.backwardsMissiles) == 0}
                        ]
                    },
                    new ADummyAction(),
                    new InfoOnlyAction()
                    {
                        disabled = !flipped,
                        icon = new Icon(CommonIcons.Catch2, null, Colors.textMain),
                    },
                    new InfoOnlyAction()
                    {
                        disabled = !flipped,
                        extraIcons = 
                        [
                            new (){spr = CommonIcons.SpawnL},
                            new (){spr = CommonIcons.SpawnR, amount = 1, color = Colors.drone},
                            new (){spr = CommonIcons.Question, flipY = s.ship.Get(Status.backwardsMissiles) > 0}
                        ]
                    },
                    
                ];
                break;
        }
        return actions;
    }
}