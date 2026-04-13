using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Patches;
using AutumnMooncat.SpireCore.Util;

namespace AutumnMooncat.SpireCore.Cards.Ironclad;

internal sealed class FiendFire : Card, IRCard
{
    public static string ID => nameof(FiendFire);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Ironclad.DeckEntry!.Deck,
                rarity = Rarity.rare,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = MainModFile.Bind(["card", ID, "name"]).Localize,
            Art = IRCard.LookUpSpr(Characters.Ironclad.CardAssetPath + ID)
        });
    }
    
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = upgrade == Upgrade.B ? 3 : upgrade == Upgrade.A ? 1 : 2,
            exhaust = true
        };
        return data;
    }

    public int GetBaseDamage()
    {
        return upgrade == Upgrade.B ? 2 : 1;
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = [];
        switch (upgrade)
        {
            case Upgrade.None:
                actions = 
                [
                    new AVariableHint()
                    {
                        hand = true,
                        handAmount = CardsInHand(s)
                    },
                    MainModFile.Kokoro().SpoofedActions.MakeAction(
                        new AAttack() 
                        { 
                            damage = GetDmg(s, GetBaseDamage()) 
                        }.WithExtraIcons([new (){spr = CommonIcons.Repeat, amount = CardsInHand(s), xHint = 1}]),
                        new ARepeatAction()
                        {
                            action = new AAttack()
                            {
                                damage = GetDmg(s, GetBaseDamage())
                            },
                            amount = CardsInHand(s)
                        }
                        ).AsCardAction,
                    MainModFile.AddTooltips([ITooltipHelper.MakeTooltip("card", ID, CommonIcons.Repeat, Colors.action)]),
                    new ATorchEntireHand()
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AVariableHint()
                    {
                        hand = true,
                        handAmount = CardsInHand(s)
                    },
                    MainModFile.Kokoro().SpoofedActions.MakeAction(
                        new AAttack() 
                        { 
                            damage = GetDmg(s, GetBaseDamage()) 
                        }.WithExtraIcons([new (){spr = CommonIcons.Repeat, amount = CardsInHand(s), xHint = 1}]),
                        new ARepeatAction()
                        {
                            action = new AAttack()
                            {
                                damage = GetDmg(s, GetBaseDamage())
                            },
                            amount = CardsInHand(s)
                        }
                    ).AsCardAction,
                    MainModFile.AddTooltips([ITooltipHelper.MakeTooltip("card", ID, CommonIcons.Repeat, Colors.action)]),
                    new ATorchEntireHand()
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AVariableHint()
                    {
                        hand = true,
                        handAmount = CardsInHand(s)
                    },
                    MainModFile.Kokoro().SpoofedActions.MakeAction(
                        new AAttack() 
                        { 
                            damage = GetDmg(s, GetBaseDamage()) 
                        }.WithExtraIcons([new (){spr = CommonIcons.Repeat, amount = CardsInHand(s), xHint = 1}]),
                        new ARepeatAction()
                        {
                            action = new AAttack()
                            {
                                damage = GetDmg(s, GetBaseDamage())
                            },
                            amount = CardsInHand(s)
                        }
                    ).AsCardAction,
                    MainModFile.AddTooltips([ITooltipHelper.MakeTooltip("card", ID, CommonIcons.Repeat, Colors.action)]),
                    new ATorchEntireHand()
                ];
                break;
        }
        return actions;
    }
    
    public int CardsInHand(State s)
    {
        int total = 0;
        if (s.route is Combat route)
            total = route.hand.Count - 1;
        //if (upgrade == Upgrade.A)
            //++total;
        return total;
    }
}
