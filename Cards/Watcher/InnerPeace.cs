using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Actions;
using AutumnMooncat.Spirecore.ExternalAPI;
using AutumnMooncat.Spirecore.Features;

namespace AutumnMooncat.Spirecore.Cards.Watcher;

internal sealed class InnerPeace : Card, IRCard
{
    public static string ID => nameof(InnerPeace);
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
            cost = 1
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
                    MainModFile.Instance.KokoroApi.V2.Conditional.MakeAction(
                        MainModFile.Instance.KokoroApi.V2.Conditional.HasStatus(CalmStatus.Entry.Status),
                        new ADrawCard()
                        {
                            count = 3
                        }).AsCardAction,
                    new AStatus()
                    {
                        status = CalmStatus.Entry.Status,
                        statusAmount = 1,
                        targetPlayer = true
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    MainModFile.Instance.KokoroApi.V2.Conditional.MakeAction(
                        MainModFile.Instance.KokoroApi.V2.Conditional.HasStatus(CalmStatus.Entry.Status),
                        new ADrawCard()
                        {
                            count = 4
                        }).AsCardAction,
                    new AStatus()
                    {
                        status = CalmStatus.Entry.Status,
                        statusAmount = 1,
                        targetPlayer = true
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    MainModFile.Instance.KokoroApi.V2.Conditional.MakeAction(
                        MainModFile.Instance.KokoroApi.V2.Conditional.HasStatus(CalmStatus.Entry.Status),
                        new ADrawCard()
                        {
                            count = 3
                        }).AsCardAction,
                    new AStatus()
                    {
                        status = CalmStatus.Entry.Status,
                        statusAmount = 2,
                        targetPlayer = true
                    }
                ];
                break;
        }
        return actions;
    }
}
