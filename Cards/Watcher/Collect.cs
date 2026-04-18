using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;

namespace AutumnMooncat.SpireCore.Cards.Watcher;

internal sealed class Collect : Card, IRCard
{
    public static string ID => nameof(Collect);
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
                    new AMove()
                    {
                        dir = 2,
                        targetPlayer = true
                    },
                    new ACascadingAddCard()
                    {
                        amount = 1,
                        card = new Miracle(),
                        destination = CardDestination.Hand
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AMove()
                    {
                        dir = 2,
                        targetPlayer = true
                    },
                    new ACascadingAddCard()
                    {
                        amount = 1,
                        card = new Miracle(),
                        destination = CardDestination.Hand
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AMove()
                    {
                        dir = 2,
                        targetPlayer = true
                    },
                    new ACascadingAddCard()
                    {
                        amount = 1,
                        card = new Miracle(),
                        destination = CardDestination.Hand
                    },
                    new ADrawCard()
                    {
                        count = 1
                    }
                ];
                break;
        }
        return actions;
    }
}
