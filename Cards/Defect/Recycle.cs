using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;

namespace AutumnMooncat.SpireCore.Cards.Defect;

internal sealed class Recycle : Card, IRCard
{
    public static string ID => nameof(Recycle);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Defect.DeckEntry.Deck,
                rarity = Rarity.uncommon,
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
            cost = upgrade == Upgrade.A ? 0 : 1,
            retain = upgrade == Upgrade.B
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
                    new ACascadingCardSelect()
                    {
                        browseSource = CardBrowse.Source.Hand,
                        browseAction = new MoveSelectedCardToPile
                        {
                            targetLocation = CardBrowse.Source.ExhaustPile
                        }
                    },
                    new AEnergy()
                    {
                        changeAmount = 2
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new ACascadingCardSelect()
                    {
                        browseSource = CardBrowse.Source.Hand,
                        browseAction = new MoveSelectedCardToPile
                        {
                            targetLocation = CardBrowse.Source.ExhaustPile
                        }
                    },
                    new AEnergy()
                    {
                        changeAmount = 2
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new ACascadingCardSelect()
                    {
                        browseSource = CardBrowse.Source.Hand,
                        browseAction = new MoveSelectedCardToPile
                        {
                            targetLocation = CardBrowse.Source.ExhaustPile
                        }
                    },
                    new AEnergy()
                    {
                        changeAmount = 2
                    }
                ];
                break;
        }
        return actions;
    }
}
