using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;

namespace AutumnMooncat.SpireCore.Cards.Defect;

[IRegisterable.Ignore]
internal sealed class Hologram : Card, IRCard
{
    public static string ID => nameof(Hologram);
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
            cost = upgrade == Upgrade.B ? 0 : 1,
            exhaust = upgrade != Upgrade.A,
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
                        status = Status.shield,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    new ACascadingCardSelect()
                    {
                        browseSource = CardBrowse.Source.DiscardPile,
                        browseAction = new MoveSelectedCardToPile
                        {
                            targetLocation = CardBrowse.Source.Hand
                        }
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AStatus()
                    {
                        status = Status.shield,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    new ACascadingCardSelect()
                    {
                        browseSource = CardBrowse.Source.DiscardPile,
                        browseAction = new MoveSelectedCardToPile
                        {
                            targetLocation = CardBrowse.Source.Hand
                        }
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AStatus()
                    {
                        status = Status.shield,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    new ACascadingCardSelect()
                    {
                        browseSource = CardBrowse.Source.DiscardPile,
                        browseAction = new MoveSelectedCardToPile
                        {
                            targetLocation = CardBrowse.Source.Hand
                        }
                    }
                ];
                break;
        }
        return actions;
    }
}
