using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Actions;
using AutumnMooncat.Spirecore.Features;

namespace AutumnMooncat.Spirecore.Cards.Watcher;

internal sealed class SpiritShield : Card, IRCard
{
    public static string ID => nameof(SpiritShield);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Watcher.DeckEntry.Deck,
                rarity = Rarity.rare,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = MainModFile.Bind(["card", ID, "name"]).Localize,
            Art = IRCard.LookUpSpr(Characters.Watcher.CardAssetPath + ID)
        });
    }
    
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = 2,
            exhaust = upgrade == Upgrade.B
        };
        return data;
    }
    
    public int CardsInHand(State s)
    {
        int cards = 0;
        if (s.route is Combat route)
            cards = route.hand.Count - 1;
        if (upgrade == Upgrade.A)
            ++cards;
        return cards;
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
                    new AStatus()
                    {
                        status = Status.tempShield,
                        statusAmount = CardsInHand(s),
                        xHint = 1,
                        targetPlayer = true
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new ADrawCard()
                    {
                        count = 1
                    },
                    new AVariableHint()
                    {
                        hand = true,
                        handAmount = CardsInHand(s)
                    },
                    new AStatus()
                    {
                        status = Status.tempShield,
                        statusAmount = CardsInHand(s),
                        xHint = 1,
                        targetPlayer = true
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
                    new AStatus()
                    {
                        status = Status.tempShield,
                        statusAmount = CardsInHand(s),
                        xHint = 1,
                        targetPlayer = true
                    },
                    new AStatus()
                    {
                        status = Status.shield,
                        statusAmount = CardsInHand(s),
                        xHint = 1,
                        targetPlayer = true
                    }
                ];
                break;
        }
        return actions;
    }
}
