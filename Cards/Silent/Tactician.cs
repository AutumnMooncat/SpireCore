using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Patches;
using AutumnMooncat.SpireCore.Util;

namespace AutumnMooncat.SpireCore.Cards.Silent;

internal sealed class Tactician : Card, IRCard
{
    public static string ID => nameof(Tactician);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Silent.DeckEntry.Deck,
                rarity = Rarity.uncommon,
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
            cost = 0,
            exhaust = upgrade != Upgrade.A
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
                        status = Status.energyNextTurn,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    MainModFile.Instance.KokoroApi.V2.OnDiscard.MakeAction(new AEnergy()
                    {
                        changeAmount = 1
                    }).AsCardAction
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AStatus()
                    {
                        status = Status.energyNextTurn,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    MainModFile.Instance.KokoroApi.V2.OnDiscard.MakeAction(new AEnergy()
                    {
                        changeAmount = 1
                    }).AsCardAction
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AStatus()
                    {
                        status = Status.energyNextTurn,
                        statusAmount = 2,
                        targetPlayer = true
                    },
                    MainModFile.Instance.KokoroApi.V2.OnDiscard.MakeAction(new AEnergy()
                    {
                        changeAmount = 2
                    }).AsCardAction
                ];
                break;
        }
        return actions;
    }
}
