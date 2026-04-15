using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Features;
using AutumnMooncat.SpireCore.Util;

namespace AutumnMooncat.SpireCore.Cards.Defect;

internal sealed class Bullseye : Card, IRCard
{
    public static string ID => nameof(Bullseye);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Defect.DeckEntry.Deck,
                rarity = Rarity.uncommon,
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
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(KokoroUtils.ChargeResource, 1), 
                        new AStatus()
                        {
                            status = LockOnStatus.Entry.Status,
                            statusAmount = 1,
                            targetPlayer = true
                        }).AsCardAction,
                    new AAttack()
                    {
                        damage = 1
                    },
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(KokoroUtils.ChargeResource, 1), 
                        new AStatus()
                        {
                            status = LockOnStatus.Entry.Status,
                            statusAmount = 1,
                            targetPlayer = true
                        }).AsCardAction,
                    new AAttack()
                    {
                        damage = 1
                    },
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(KokoroUtils.ChargeResource, 1), 
                        new AStatus()
                        {
                            status = LockOnStatus.Entry.Status,
                            statusAmount = 2,
                            targetPlayer = true
                        }).AsCardAction,
                    new AAttack()
                    {
                        damage = 1
                    },
                    new AAttack()
                    {
                        damage = 1
                    },
                ];
                break;
        }
        return actions;
    }
}
