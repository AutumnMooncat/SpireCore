using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Actions;

namespace AutumnMooncat.Spirecore.Cards.Ironclad;

internal sealed class LimitBreak : Card, IRCard
{
    public static string ID => nameof(LimitBreak);
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
            cost = 1,
            exhaust = upgrade != Upgrade.A
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
                    new AVariableHint()
                    {
                        status = Status.overdrive
                    },
                    new AStatus()
                    {
                        status = Status.overdrive,
                        statusAmount = s.ship.Get(Status.overdrive),
                        xHint = 1,
                        targetPlayer = true
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AVariableHint()
                    {
                        status = Status.overdrive
                    },
                    new AStatus()
                    {
                        status = Status.overdrive,
                        statusAmount = s.ship.Get(Status.overdrive),
                        xHint = 1,
                        targetPlayer = true
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AStatus()
                    {
                        status = Status.overdrive,
                        statusAmount = 1,
                        targetPlayer = true,
                        omitFromTooltips = true
                    },
                    new AVariableHint()
                    {
                        status = Status.overdrive
                    },
                    new AStatus()
                    {
                        status = Status.overdrive,
                        statusAmount = s.ship.Get(Status.overdrive) + 1,
                        xHint = 1,
                        targetPlayer = true
                    }
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
