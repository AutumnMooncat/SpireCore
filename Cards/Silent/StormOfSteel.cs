using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Cards.Silent;

internal sealed class StormOfSteel : Card, IRCard
{
    public static string ID => nameof(StormOfSteel);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Silent.DeckEntry.Deck,
                rarity = Rarity.rare,
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
            cost = upgrade == Upgrade.A ? 0 : 1,
            exhaust = upgrade == Upgrade.B
        };
        return data;
    }
    
    public int CardsInHand(State s)
    {
        int total = 0;
        if (s.route is Combat route)
            total = route.hand.Count - 1;
        return total;
    }
    
    public Card GetShiv()
    {
        var shiv = new Shiv();
        return shiv;
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
                    new ADiscard(),
                    new ACascadingAddCard()
                    {
                        xHint = 1,
                        amount = CardsInHand(s),
                        card = GetShiv(),
                        destination = CardDestination.Hand
                    }
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
                    new ADiscard(),
                    new ACascadingAddCard()
                    {
                        xHint = 1,
                        amount = CardsInHand(s),
                        card = GetShiv(),
                        destination = CardDestination.Hand
                    }
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
                    new ADiscard(),
                    new AStatus()
                    {
                        xHint = 1,
                        status = Status.evade,
                        statusAmount = CardsInHand(s),
                        targetPlayer = true
                    },
                    new ACascadingAddCard()
                    {
                        xHint = 1,
                        amount = CardsInHand(s),
                        card = GetShiv(),
                        destination = CardDestination.Hand
                    }
                ];
                break;
        }
        return actions;
    }
}
