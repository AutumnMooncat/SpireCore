using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;

namespace AutumnMooncat.SpireCore.Cards.Defect;

internal sealed class Turbo : Card, IRCard
{
    public static string ID => nameof(Turbo);
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
            cost = 0
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
                    new AEnergy()
                    {
                        changeAmount = 1
                    },
                    new AStatus()
                    {
                        status = Status.evade,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    new ACascadingAddCard()
                    {
                        amount = 1,
                        card = new OxygenLeak(),
                        destination = CardDestination.Discard
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AEnergy()
                    {
                        changeAmount = 1
                    },
                    new AStatus()
                    {
                        status = Status.evade,
                        statusAmount = 2,
                        targetPlayer = true
                    },
                    new ACascadingAddCard()
                    {
                        amount = 1,
                        card = new OxygenLeak(),
                        destination = CardDestination.Discard
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AEnergy()
                    {
                        changeAmount = 2
                    },
                    new AStatus()
                    {
                        status = Status.evade,
                        statusAmount = 2,
                        targetPlayer = true
                    },
                    new ACascadingAddCard()
                    {
                        amount = 1,
                        card = new OxygenLeak(),
                        destination = CardDestination.Deck,
                        insertRandomly = false
                    }
                ];
                break;
        }
        return actions;
    }
}
