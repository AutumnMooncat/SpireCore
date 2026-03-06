using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Actions;
using AutumnMooncat.Spirecore.Features;

namespace AutumnMooncat.Spirecore.Cards.Watcher;

[IRegisterable.Ignore]
internal sealed class Meditate : Card, IRCard
{
    public static string ID => nameof(Meditate);
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
                    new ACascadingCardSelect()
                    {
                        browseSource = CardBrowse.Source.DiscardPile,
                        browseAction = new MoveSelectedCardToPile
                        {
                            targetLocation = CardBrowse.Source.Deck
                        }
                    },
                    new AStatus()
                    {
                        status = Status.drawNextTurn,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    new AStatus()
                    {
                        status = CalmStatus.Entry.Status,
                        statusAmount = 2,
                        targetPlayer = true
                    },
                    new AEndTurn()
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new ACascadingCardSelect()
                    {
                        browseSource = CardBrowse.Source.DiscardPile,
                        browseAction = new MoveSelectedCardToPile
                        {
                            targetLocation = CardBrowse.Source.Deck
                        }
                    },
                    new AStatus()
                    {
                        status = Status.drawNextTurn,
                        statusAmount = 2,
                        targetPlayer = true
                    },
                    new AStatus()
                    {
                        status = CalmStatus.Entry.Status,
                        statusAmount = 2,
                        targetPlayer = true
                    },
                    new AEndTurn()
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new ACascadingCardSelect()
                    {
                        browseSource = CardBrowse.Source.DiscardPile,
                        browseAction = new MoveSelectedCardToPile
                        {
                            targetLocation = CardBrowse.Source.Deck
                        }
                    },
                    new AStatus()
                    {
                        status = Status.drawNextTurn,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    new AStatus()
                    {
                        status = CalmStatus.Entry.Status,
                        statusAmount = 3,
                        targetPlayer = true
                    },
                    new AEndTurn()
                ];
                break;
        }
        return actions;
    }
}
