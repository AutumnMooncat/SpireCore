using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace AutumnMooncat.Spirecore.Cards.Ironclad;

internal sealed class ShrugItOff : Card, IRCard
{
    public static string ID => nameof(ShrugItOff);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Ironclad.DeckEntry.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = MainModFile.Bind(["card", ID, "name"]).Localize,
            Art = IRCard.LookUpSpr(Characters.Ironclad.CardAssetPath + ID)
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
                        targetPlayer = false
                    },
                    new AStatus()
                    {
                        status = Status.shield,
                        statusAmount = 1,
                        targetPlayer = true
                    }
                    /*MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.shield), 1), 
                        new AStatus()
                        {
                            status = Status.shield,
                            statusAmount = 2,
                            targetPlayer = true
                        }).AsCardAction,*/
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AMove()
                    {
                        dir = 2,
                        targetPlayer = false
                    },
                    new AStatus()
                    {
                        status = Status.shield,
                        statusAmount = 1,
                        targetPlayer = true
                    }
                    /*MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.shield), 1), 
                        new AStatus()
                        {
                            status = Status.shield,
                            statusAmount = 2,
                            targetPlayer = true
                        }).AsCardAction,*/
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AMove()
                    {
                        dir = 2,
                        targetPlayer = false
                    },
                    new AStatus()
                    {
                        status = Status.shield,
                        statusAmount = 2,
                        targetPlayer = true
                    }
                    /*MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.shield), 1), 
                        new AStatus()
                        {
                            status = Status.shield,
                            statusAmount = 3,
                            targetPlayer = true
                        }).AsCardAction,*/
                ];
                break;
        }
        return actions;
    }
}
