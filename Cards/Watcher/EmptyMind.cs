using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Cards.Watcher;

internal sealed class EmptyMind : Card, IRCard
{
    public static string ID => nameof(EmptyMind);
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
                    new AStatus()
                    {
                        status = Status.evade,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    new AStatus()
                    {
                        status = WrathStatus.Entry.Status,
                        statusAmount = 0,
                        mode = AStatusMode.Set,
                        targetPlayer = true
                    },
                    new AStatus()
                    {
                        status = CalmStatus.Entry.Status,
                        statusAmount = 0,
                        mode = AStatusMode.Set,
                        targetPlayer = true
                    }
                ];
                break;
            case Upgrade.A:
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
                        status = WrathStatus.Entry.Status,
                        statusAmount = 0,
                        mode = AStatusMode.Set,
                        targetPlayer = true
                    },
                    new AStatus()
                    {
                        status = CalmStatus.Entry.Status,
                        statusAmount = 0,
                        mode = AStatusMode.Set,
                        targetPlayer = true
                    }
                ];
                break;
            case Upgrade.B:
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
                        status = WrathStatus.Entry.Status,
                        statusAmount = 0,
                        mode = AStatusMode.Set,
                        targetPlayer = true
                    },
                    new AStatus()
                    {
                        status = CalmStatus.Entry.Status,
                        statusAmount = 0,
                        mode = AStatusMode.Set,
                        targetPlayer = true
                    }
                ];
                break;
        }
        return actions;
    }
}
