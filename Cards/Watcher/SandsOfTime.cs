using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Actions;
using AutumnMooncat.Spirecore.Features;
using AutumnMooncat.Spirecore.Util;
using Newtonsoft.Json;

namespace AutumnMooncat.Spirecore.Cards.Watcher;

internal sealed class SandsOfTime : Card, IRCard
{
    public static string ID => nameof(SandsOfTime);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Watcher.DeckEntry.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = MainModFile.Bind(["card", ID, "name"]).Localize,
            Art = IRCard.LookUpSpr(Characters.Watcher.CardAssetPath + ID)
        });
    }
    
    [JsonProperty]
    public int retains;
    
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = 4,
            retain = true,
            exhaust = upgrade == Upgrade.B
        };
        data.cost -= retains;
        if (data.cost < 0)
        {
            data.cost = 0;
        }
        return data;
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
                        damage = GetDmg(s, 4)
                    },
                    MainModFile.Instance.KokoroApi.V2.OnTurnEnd.MakeAction(new DelegateAction()
                    {
                        begin = (_, _, _, _) =>
                        {
                            retains++;
                            return null;
                        },
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
                    }).AsCardAction
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 5)
                    },
                    MainModFile.Instance.KokoroApi.V2.OnTurnEnd.MakeAction(new DelegateAction()
                    {
                        begin = (_, _, _, _) =>
                        {
                            retains++;
                            return null;
                        },
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
                    }).AsCardAction
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 4)
                    },
                    MainModFile.Instance.KokoroApi.V2.OnTurnEnd.MakeAction(new DelegateAction()
                    {
                        begin = (_, _, _, _) =>
                        {
                            retains += 2;
                            return null;
                        },
                        tips = 
                        [
                            GetTooltip(2)
                        ],
                        icon = new Icon(CommonIcons.Cost, null, Colors.textMain),
                        extraIcons = 
                        [
                            new (){spr = CommonIcons.Minus, dx = -3},
                            new (){spr = CommonIcons.EqualSign, amount = 2, dx = -4}
                        ]
                    }).AsCardAction
                ];
                break;
        }
        return actions;
    }
}
