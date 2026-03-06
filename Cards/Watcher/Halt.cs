using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Actions;
using AutumnMooncat.Spirecore.Features;

namespace AutumnMooncat.Spirecore.Cards.Watcher;

[IRegisterable.Ignore]
internal sealed class Halt : Card, IRCard
{
    public static string ID => nameof(Halt);
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
            cost = 0
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
                    MainModFile.Instance.KokoroApi.V2.Conditional.MakeAction(
                        MainModFile.Instance.KokoroApi.V2.Conditional.HasStatus(WrathStatus.Entry.Status),
                        new AStatus()
                        {
                            status = Status.tempShield,
                            statusAmount = 1,
                            targetPlayer = true
                        }).AsCardAction,
                    /*new AVariableHint()
                    {
                        status = WrathStatus.Entry.Status
                    },
                    new AStatus()
                    {
                        status = Status.tempShield,
                        statusAmount = s.ship.Get(WrathStatus.Entry.Status),
                        xHint = 1,
                        targetPlayer = true
                    },
                    MainModFile.AddTooltips([WrathStatus.GetTooltip])*/
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AStatus()
                    {
                        status = Status.tempShield,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    MainModFile.Instance.KokoroApi.V2.Conditional.MakeAction(
                        MainModFile.Instance.KokoroApi.V2.Conditional.HasStatus(WrathStatus.Entry.Status),
                        new AStatus()
                        {
                            status = Status.tempShield,
                            statusAmount = 2,
                            targetPlayer = true
                        }).AsCardAction,
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
                    MainModFile.Instance.KokoroApi.V2.Conditional.MakeAction(
                        MainModFile.Instance.KokoroApi.V2.Conditional.HasStatus(WrathStatus.Entry.Status),
                        new AStatus()
                        {
                            status = Status.tempShield,
                            statusAmount = 1,
                            targetPlayer = true
                        }).AsCardAction,
                    MainModFile.Instance.KokoroApi.V2.Conditional.MakeAction(
                        MainModFile.Instance.KokoroApi.V2.Conditional.HasStatus(CalmStatus.Entry.Status),
                        new AStatus()
                        {
                            status = Status.tempShield,
                            statusAmount = 1,
                            targetPlayer = true
                        }).AsCardAction,
                    /*new AVariableHint()
                    {
                        status = WrathStatus.Entry.Status,
                        secondStatus = CalmStatus.Entry.Status
                    },
                    new AStatus()
                    {
                        status = Status.shield,
                        statusAmount = s.ship.Get(WrathStatus.Entry.Status) + s.ship.Get(CalmStatus.Entry.Status),
                        xHint = 1,
                        targetPlayer = true
                    },
                    MainModFile.AddTooltips([WrathStatus.GetTooltip, CalmStatus.GetTooltip])*/
                ];
                break;
        }
        return actions;
    }
}
