using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Actions;

namespace AutumnMooncat.Spirecore.Cards.Ironclad;

internal sealed class BurningPact : Card, IRCard
{
    public static string ID => nameof(BurningPact);
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
                    new AStatus()
                    {
                        status = Status.heat,
                        statusAmount = 1,
                        targetPlayer = false
                    },
                    new ACascadingCardSelect()
                    {
                        browseSource = CardBrowse.Source.Hand,
                        browseAction = new TorchCard()
                    },
                    new ADrawCard()
                    {
                        count = 2
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AStatus()
                    {
                        status = Status.heat,
                        statusAmount = 1,
                        targetPlayer = false
                    },
                    new ACascadingCardSelect()
                    {
                        browseSource = CardBrowse.Source.Hand,
                        browseAction = new TorchCard()
                    },
                    new ADrawCard()
                    {
                        count = 3
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AStatus()
                    {
                        status = Status.heat,
                        statusAmount = 1,
                        targetPlayer = false
                    },
                    new ACascadingCardSelect()
                    {
                        browseSource = CardBrowse.Source.Hand,
                        browseAction = new TorchCard()
                    },
                    new AStatus()
                    {
                        status = Status.drawNextTurn,
                        statusAmount = 2,
                        targetPlayer = true
                    }
                ];
                break;
        }
        return actions;
    }
}
