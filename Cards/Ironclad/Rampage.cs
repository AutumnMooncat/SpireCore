using System;
using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Actions;
using AutumnMooncat.Spirecore.Util;
using Newtonsoft.Json;

namespace AutumnMooncat.Spirecore.Cards.Ironclad;

[IRegisterable.Ignore]
internal sealed class Rampage : Card, IRCard, ITooltipHelper
{
    public static string ID => nameof(Rampage);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Ironclad.DeckEntry.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = MainModFile.Bind(["card", ID, "name"]).Localize,
            Art = IRCard.LookUpSpr(Characters.Ironclad.CardAssetPath + ID)
        });
    }

    public static Tooltip GetTooltip(int amount)
    {
        Records.TexturePayload[] pl = [
            new (){spr = CommonIcons.Attack, x = 0}, 
            new (){spr = CommonIcons.Plus_Small, x = 5}, 
            new (){spr = amount == 1 ? CommonIcons.Plus_Small : CommonIcons.EqualSign_Small, x = 9},
            /*new (){spr = CommonIcons.Plus, x = -3}, 
            new (){spr = amount == 1 ? CommonIcons.Plus : CommonIcons.EqualSign, x = 3}*/
        ];
        return ITooltipHelper.MakeMultiTooltip("card", ID, pl, 8, Colors.action, null, new {Scale = amount});
    }

    [JsonProperty]
    public int bonus;
    
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = upgrade == Upgrade.B ? 0 : 1
        };
        return data;
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
                        damage = GetDmg(s, 1 + bonus)
                    },
                    new InfoOnlyAction()
                    {
                        tips = 
                        [
                            GetTooltip(1)
                        ],
                        icon = new Icon(CommonIcons.Attack, null, Colors.textMain),
                        extraIcons = 
                        [
                            new (){spr = CommonIcons.Plus, dx = -3},
                            new (){spr = CommonIcons.Plus, dx = -4}
                        ]
                    },
                    MainModFile.Kokoro().HiddenActions.MakeAction(new DelegateAction()
                    {
                        begin = ((g, state, combat, thiz) =>
                        {
                            bonus += upgrade == Upgrade.B ? 2 : 1;
                            return null;
                        })
                    }).AsCardAction
                ];
                break;
            case Upgrade.A:
                actions =
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 2 + bonus)
                    },
                    new InfoOnlyAction()
                    {
                        tips = 
                        [
                            GetTooltip(1)
                        ],
                        icon = new Icon(CommonIcons.Attack, null, Colors.textMain),
                        extraIcons = 
                        [
                            new (){spr = CommonIcons.Plus, dx = -3},
                            new (){spr = CommonIcons.Plus, dx = -4}
                        ]
                    },
                    MainModFile.Kokoro().HiddenActions.MakeAction(new DelegateAction()
                    {
                        begin = ((g, state, combat, thiz) =>
                        {
                            bonus += upgrade == Upgrade.B ? 2 : 1;
                            return null;
                        })
                    }).AsCardAction
                ];
                break;
            case Upgrade.B:
                actions =
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 0 + bonus),
                    },
                    new InfoOnlyAction()
                    {
                        tips = 
                        [
                            GetTooltip(2)
                        ],
                        icon = new Icon(CommonIcons.Attack, null, Colors.textMain),
                        extraIcons = 
                        [
                            new (){spr = CommonIcons.Plus, dx = -3},
                            new (){spr = CommonIcons.EqualSign, amount = 2, dx = -4}
                        ]
                    },
                    MainModFile.Kokoro().HiddenActions.MakeAction(new DelegateAction()
                    {
                        begin = ((g, state, combat, thiz) =>
                        {
                            bonus += upgrade == Upgrade.B ? 2 : 1;
                            return null;
                        })
                    }).AsCardAction
                ];
                break;
        }
        return actions;
    }

    public override void OnExitCombat(State s, Combat c)
    {
        bonus = 0;
    }
}
