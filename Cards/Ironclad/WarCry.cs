using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;

namespace AutumnMooncat.SpireCore.Cards.Ironclad;

[IRegisterable.Ignore]
internal sealed class WarCry : Card, IRCard
{
    public static string ID => nameof(WarCry);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Ironclad.DeckEntry!.Deck,
                rarity = Rarity.common,
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
            cost = 0,
            flippable = upgrade == Upgrade.B
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
                    new AMove()
                    {
                        dir = 2,
                        isRandom = true,
                        targetPlayer = false
                    },
                    new ACascadingCardSelect()
                    {
                        browseSource = CardBrowse.Source.Hand,
                        browseAction = new MoveSelectedCardToPile
                        {
                            targetLocation = CardBrowse.Source.Deck
                        }
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AMove()
                    {
                        dir = 3,
                        isRandom = true,
                        targetPlayer = false
                    },
                    new ACascadingCardSelect()
                    {
                        browseSource = CardBrowse.Source.Hand,
                        browseAction = new MoveSelectedCardToPile
                        {
                            targetLocation = CardBrowse.Source.Deck
                        }
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AMove()
                    {
                        dir = 2,
                        targetPlayer = false
                    },
                    new ACascadingCardSelect()
                    {
                        browseSource = CardBrowse.Source.Hand,
                        browseAction = new MoveSelectedCardToPile
                        {
                            targetLocation = CardBrowse.Source.Deck
                        }
                    }
                ];
                break;
        }
        return actions;
    }
}
