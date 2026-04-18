using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Cards.Watcher;

internal sealed class ThirdEye : Card, IRCard
{
    public static string ID => nameof(ThirdEye);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Watcher.DeckEntry.Deck,
                rarity = Rarity.common,
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
            cost = 1
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
                        status = Status.tempShield,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    new AScry()
                    {
                        count = 3
                    },
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AStatus()
                    {
                        status = Status.tempShield,
                        statusAmount = 2,
                        targetPlayer = true
                    },
                    new AScry()
                    {
                        count = 3
                    },
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AStatus()
                    {
                        status = Status.tempShield,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    new AScry()
                    {
                        count = 5
                    },
                ];
                break;
        }
        return actions;
    }
}
