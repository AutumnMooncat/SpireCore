using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Actions;
using AutumnMooncat.Spirecore.Features;

namespace AutumnMooncat.Spirecore.Cards.Watcher;

internal sealed class Vault : Card, IRCard
{
    public static string ID => nameof(Vault);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Watcher.DeckEntry.Deck,
                rarity = Rarity.rare,
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
            cost = upgrade == Upgrade.A ? 2 : 3,
            exhaust = true
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
                        status  = Status.timeStop,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    new AStunShip(),
                    new AEndTurn()
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AStatus()
                    {
                        status  = Status.timeStop,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    new AStunShip(),
                    new AEndTurn()
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AStatus()
                    {
                        status  = Status.timeStop,
                        statusAmount = 2,
                        targetPlayer = true
                    },
                    new AStunShip(),
                    new AEndTurn()
                ];
                break;
        }
        return actions;
    }
}
