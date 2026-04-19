using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Patches;
using AutumnMooncat.SpireCore.Util;

namespace AutumnMooncat.SpireCore.Cards.Silent;

[IRegisterable.Ignore]
internal sealed class SneakyStrike : Card, IRCard
{
    public static string ID => nameof(SneakyStrike);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Silent.DeckEntry.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = MainModFile.Bind(["card", ID, "name"]).Localize,
            Art = IRCard.LookUpSpr(Characters.Silent.CardAssetPath + ID)
        });
    }
    
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = 2
        };
        return data;
    }
    
    public static Tooltip GetTooltip()
    {
        Records.TexturePayload[] pl = [
            new (){spr = CommonIcons.Discard, x = 0}, 
            new (){spr = CommonIcons.ExtraQuestion, x = -1}
        ];
        return ITooltipHelper.MakeMultiTooltip("card", ID, pl, 5, Colors.action);
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = [];
        switch (upgrade)
        {
            case Upgrade.None:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 1)
                    },
                    new AStatus()
                    {
                        status = Status.evade,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    new DoIfAction()
                    {
                        action = new AEnergy{changeAmount = 2},
                        check = () => DiscardPatches.cardsDiscardedThisTurn > 0,
                        tips = 
                        [
                            GetTooltip(),
                        ],
                        icon = new Icon(CommonIcons.Discard, null, Colors.textMain),
                        extraIcons = 
                        [
                            new (){spr = CommonIcons.ExtraQuestion, dx = -11},
                            new (){spr = CommonIcons.Energy, amount = 2, dx = 1}
                        ]
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 2)
                    },
                    new AStatus()
                    {
                        status = Status.evade,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    new DoIfAction()
                    {
                        action = new AEnergy{changeAmount = 2},
                        check = () => DiscardPatches.cardsDiscardedThisTurn > 0,
                        tips = 
                        [
                            GetTooltip(),
                        ],
                        icon = new Icon(CommonIcons.Discard, null, Colors.textMain),
                        extraIcons = 
                        [
                            new (){spr = CommonIcons.ExtraQuestion, dx = -11}, // -10?
                            new (){spr = CommonIcons.Energy, amount = 2, dx = 1}, // 0?
                        ]
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 1)
                    },
                    new AStatus()
                    {
                        status = Status.evade,
                        statusAmount = 2,
                        targetPlayer = true
                    },
                    new DoIfAction()
                    {
                        action = new AEnergy{changeAmount = 2},
                        check = () => DiscardPatches.cardsDiscardedThisTurn > 0,
                        tips = 
                        [
                            GetTooltip(),
                        ],
                        icon = new Icon(CommonIcons.Discard, null, Colors.textMain),
                        extraIcons = 
                        [
                            new (){spr = CommonIcons.ExtraQuestion, dx = -11}, // -10?
                            new (){spr = CommonIcons.Energy, amount = 2, dx = 1}, // 0?
                        ]
                    }
                ];
                break;
        }
        return actions;
    }
}
