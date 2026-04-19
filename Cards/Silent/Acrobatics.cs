using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;

namespace AutumnMooncat.SpireCore.Cards.Silent;

internal sealed class Acrobatics : Card, IRCard
{
    public static string ID => nameof(Acrobatics);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Silent.DeckEntry.Deck,
                rarity = Rarity.common,
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
            cost = 1,
            flippable = upgrade == Upgrade.A
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
                    new ACardContext()
                    {
                        context = ACardContext.Context.RightHand,
                        thatIsnt = this,
                        flipped = flipped,
                        followup = new MoveSelectedCardToPile()
                        {
                            targetLocation = CardBrowse.Source.DiscardPile
                        }
                    },
                    new AMove()
                    {
                        dir = 1,
                        targetPlayer = true
                    },
                    new ADrawCard()
                    {
                        count = 3
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new ACardContext()
                    {
                        context = ACardContext.Context.RightHand,
                        thatIsnt = this,
                        flipped = flipped,
                        followup = new MoveSelectedCardToPile()
                        {
                            targetLocation = CardBrowse.Source.DiscardPile
                        }
                    },
                    new AMove()
                    {
                        dir = 1,
                        targetPlayer = true
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
                    new ACardContext()
                    {
                        context = ACardContext.Context.RightHand,
                        thatIsnt = this,
                        flipped = flipped,
                        followup = new MoveSelectedCardToPile()
                        {
                            targetLocation = CardBrowse.Source.DiscardPile
                        }
                    },
                    new AMove()
                    {
                        dir = 2,
                        targetPlayer = true
                    },
                    new ADrawCard()
                    {
                        count = 4
                    }
                ];
                break;
        }
        return actions;
    }
}
