using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Cards.Silent;

internal sealed class BladeDance : Card, IRCard
{
    public static string ID => nameof(BladeDance);
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
            cost = 1
        };
        return data;
    }

    public Card GetShiv()
    {
        var shiv = new Shiv();
        /*if (upgrade == Upgrade.B)
        {
            shiv.upgrade = Upgrade.A;
        }*/
        return shiv;
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
                        status = Status.evade,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    new AStatus()
                    {
                        status = ShivStatus.Entry.Status,
                        statusAmount = 2,
                        targetPlayer = true
                    },
                    /*new ACascadingAddCard()
                    {
                        amount = 2,
                        card = GetShiv(),
                        destination = CardDestination.Hand
                    }*/
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AStatus()
                    {
                        status = Status.evade,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    new AStatus()
                    {
                        status = ShivStatus.Entry.Status,
                        statusAmount = 3,
                        targetPlayer = true
                    },
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AStatus()
                    {
                        status = Status.evade,
                        statusAmount = 2,
                        targetPlayer = true
                    },
                    new AStatus()
                    {
                        status = ShivStatus.Entry.Status,
                        statusAmount = 2,
                        targetPlayer = true
                    },
                ];
                break;
        }
        return actions;
    }
}
