using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Actions;
using AutumnMooncat.Spirecore.Patches;
using AutumnMooncat.Spirecore.Util;

namespace AutumnMooncat.Spirecore.Cards.Silent;

internal sealed class CalculatedGamble : Card, IRCard
{
    public static string ID => nameof(CalculatedGamble);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Silent.DeckEntry.Deck,
                rarity = Rarity.uncommon,
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
            cost = 0,
            exhaust = upgrade != Upgrade.A,
            retain = upgrade == Upgrade.B
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
                    new ADiscard()
                    {
                        
                    },
                    new ADrawCard()
                    {
                        xHint = 1,
                        count = CardsInHand(s)
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
                    new ADiscard()
                    {
                        
                    },
                    new ADrawCard()
                    {
                        xHint = 1,
                        count = CardsInHand(s)
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
                    new ADiscard()
                    {
                        
                    },
                    new ADrawCard()
                    {
                        xHint = 1,
                        count = CardsInHand(s)
                    }
                ];
                break;
        }
        return actions;
    }
}
