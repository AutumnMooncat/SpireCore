using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Features;

namespace AutumnMooncat.Spirecore.Cards.Watcher;

internal sealed class Insight : Card, IRCard
{
    public static string ID => nameof(Insight);
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
                dontOffer = true,
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
            cost = 0,
            retain = true,
            temporary = true,
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
                    new ADrawCard()
                    {
                        count = 2
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new ADrawCard()
                    {
                        count = 3
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new ADrawCard()
                    {
                        count = 2
                    },
                    new AStatus()
                    {
                        status = Status.drawNextTurn,
                        statusAmount = 2,
                        targetPlayer = true
                    }
                ];
                break;
        }
        return actions;
    }
}
