using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Actions;
using AutumnMooncat.Spirecore.Features;

namespace AutumnMooncat.Spirecore.Cards.Defect;

internal sealed class Seek : Card, IRCard
{
    public static string ID => nameof(Seek);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Defect.DeckEntry.Deck,
                rarity = Rarity.rare,
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
            cost = 0,
            exhaust = upgrade != Upgrade.A,
            buoyant = upgrade == Upgrade.B,
            retain = true
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
                        browseSource = CardBrowse.Source.DrawPile,
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
                    new ACascadingCardSelect()
                    {
                        browseSource = CardBrowse.Source.DrawPile,
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
                    new ACascadingCardSelect()
                    {
                        browseSource = CardBrowse.Source.DrawPile,
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
