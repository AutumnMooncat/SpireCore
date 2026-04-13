using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Cards.Watcher;

internal sealed class Miracle : Card, IRCard
{
    public static string ID => nameof(Miracle);
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
                    new AEnergy()
                    {
                        changeAmount = 1
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AEnergy()
                    {
                        changeAmount = 2
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AEnergy()
                    {
                        changeAmount = 2
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
