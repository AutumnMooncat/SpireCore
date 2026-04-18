using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Cards.Watcher;

[IRegisterable.Ignore]
internal sealed class Worship : Card, IRCard
{
    public static string ID => nameof(Worship);
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
            cost = upgrade == Upgrade.A ? 1 : 2,
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
                    new AScry()
                    {
                        count = 3
                    },
                    new AStatus()
                    {
                        status = MantraStatus.Entry.Status,
                        statusAmount = 3,
                        targetPlayer = true
                    },
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AScry()
                    {
                        count = 3
                    },
                    new AStatus()
                    {
                        status = MantraStatus.Entry.Status,
                        statusAmount = 3,
                        targetPlayer = true
                    },
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AScry()
                    {
                        count = 3
                    },
                    new AStatus()
                    {
                        status = MantraStatus.Entry.Status,
                        statusAmount = 3,
                        targetPlayer = true
                    },
                ];
                break;
        }
        return actions;
    }
}
