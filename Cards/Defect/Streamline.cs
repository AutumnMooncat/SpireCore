using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Util;
using Newtonsoft.Json;

namespace AutumnMooncat.SpireCore.Cards.Defect;

internal sealed class Streamline : Card, IRCard, ITooltipHelper
{
    public static string ID => nameof(Streamline);
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
    
    public static Tooltip GetTooltip(int amount)
    {
        Records.TexturePayload[] pl = [
            new (){spr = CommonIcons.Cost, x = 0}, 
            new (){spr = CommonIcons.Minus_Small, x = 6}, 
            new (){spr = amount == 1 ? CommonIcons.Minus_Small : CommonIcons.EqualSign_Small, x = 10}
        ];
        return ITooltipHelper.MakeMultiTooltip("card", ID, pl, 9, Colors.action, null, new {Scale = amount});
    }

    [JsonProperty]
    public int plays;
    
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = upgrade == Upgrade.B ? 3 : 2
        };
        data.cost -= plays;
        if (data.cost < 0)
        {
            data.cost = 0;
        }
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
                        damage = GetDmg(s, 3)
                    },
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(KokoroUtils.ChargeResource, 1), 
                        MainModFile.Kokoro().SpoofedActions.MakeAction(new InfoOnlyAction()
                            {
                                tips = 
                                [
                                    GetTooltip(1)
                                ],
                                icon = new Icon(CommonIcons.Cost, null, Colors.textMain),
                                extraIcons = 
                                [
                                    new (){spr = CommonIcons.Minus, dx = -3},
                                    new (){spr = CommonIcons.Minus, dx = -4}
                                ]
                            },
                            new DelegateAction()
                            {
                                begin = ((g, state, combat, thiz) =>
                                {
                                    plays++;
                                    return null;
                                })
                            }).AsCardAction).AsCardAction,
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 3)
                    },
                    new InfoOnlyAction()
                    {
                        tips = 
                        [
                            GetTooltip(1)
                        ],
                        icon = new Icon(CommonIcons.Cost, null, Colors.textMain),
                        extraIcons = 
                        [
                            new (){spr = CommonIcons.Minus, dx = -3},
                            new (){spr = CommonIcons.Minus, dx = -4}
                        ]
                    },
                    MainModFile.Kokoro().HiddenActions.MakeAction(new DelegateAction()
                    {
                        begin = ((g, state, combat, thiz) =>
                        {
                            plays++;
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
                        damage = GetDmg(s, 5)
                    },
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(KokoroUtils.ChargeResource, 1), 
                        MainModFile.Kokoro().SpoofedActions.MakeAction(new InfoOnlyAction()
                            {
                                tips = 
                                [
                                    GetTooltip(1)
                                ],
                                icon = new Icon(CommonIcons.Cost, null, Colors.textMain),
                                extraIcons = 
                                [
                                    new (){spr = CommonIcons.Minus, dx = -3},
                                    new (){spr = CommonIcons.Minus, dx = -4}
                                ]
                            },
                            new DelegateAction()
                            {
                                begin = ((g, state, combat, thiz) =>
                                {
                                    plays++;
                                    return null;
                                })
                            }).AsCardAction).AsCardAction,
                ];
                break;
        }
        return actions;
    }
    
    public override void OnExitCombat(State s, Combat c)
    {
        plays = 0;
    }
}
