using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Features;
using AutumnMooncat.SpireCore.Util;

namespace AutumnMooncat.SpireCore.Cards.Defect;

internal sealed class Turbo : Card, IRCard
{
    public static string ID => nameof(Turbo);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Defect.DeckEntry.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = MainModFile.Bind(["card", ID, "name"]).Localize,
            Art = IRCard.LookUpSpr(Characters.Defect.CardAssetPath + ID)
        });
    }
    
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = 0,
            exhaust = upgrade == Upgrade.B
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
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(KokoroUtils.ChargeResource, 1), 
                        new AEnergy()
                        {
                            changeAmount = 1
                        }).AsCardAction,
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(KokoroUtils.ChargeResource, 1), 
                        new AStatus()
                        {
                            status = Status.droneShift,
                            statusAmount = 1,
                            targetPlayer = true
                        }).AsCardAction,
                    new AStatus()
                    {
                        status = NoChargeStatus.Entry.Status,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(KokoroUtils.ChargeResource, 1), 
                        new AEnergy()
                        {
                            changeAmount = 1
                        }).AsCardAction,
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(KokoroUtils.ChargeResource, 1), 
                        new AStatus()
                        {
                            status = Status.droneShift,
                            statusAmount = 1,
                            targetPlayer = true
                        }).AsCardAction,
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(KokoroUtils.ChargeResource, 1), 
                        new AEnergy()
                        {
                            changeAmount = 2
                        }).AsCardAction,
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(KokoroUtils.ChargeResource, 1), 
                        new AStatus()
                        {
                            status = Status.droneShift,
                            statusAmount = 2,
                            targetPlayer = true
                        }).AsCardAction,
                    new AStatus()
                    {
                        status = NoChargeStatus.Entry.Status,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                ];
                break;
        }
        return actions;
    }
}
