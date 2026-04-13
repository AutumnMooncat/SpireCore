using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;

namespace AutumnMooncat.SpireCore.Cards.Ironclad;

internal sealed class Havoc : Card, IRCard
{
    public static string ID => nameof(Havoc);
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
            cost = upgrade == Upgrade.A ? 0 : 1
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
                    new APlayAndTorchTopCard(),
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.shield), 1), 
                        new AMove()
                        {
                            dir = 2,
                            targetPlayer = false,
                            isRandom = true
                        }).AsCardAction,
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new APlayAndTorchTopCard(),
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.shield), 1), 
                        new AMove()
                        {
                            dir = 2,
                            targetPlayer = false,
                            isRandom = true
                        }).AsCardAction,
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new APlayTopCard()
                    {
                        exhaustThisCardAfterwards = false
                    },
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.shield), 1), 
                        new AMove()
                        {
                            dir = 3,
                            targetPlayer = false,
                            isRandom = true
                        }).AsCardAction,
                ];
                break;
        }
        return actions;
    }
}
