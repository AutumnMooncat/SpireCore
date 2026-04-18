using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Cards.Defect;

internal sealed class CompileDriver : Card, IRCard
{
    public static string ID => nameof(CompileDriver);
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
            cost = 1,
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
                    new AAttack()
                    {
                        damage = GetDmg(s, 1)
                    },
                    new AVariableHint()
                    {
                        status = ChargeStatus.Entry.Status
                    }.WithChargeTipFix(s, c),
                    new ADrawCard()
                    {
                        count = ChargeStatus.EffectiveCharge(s, c, s.ship),
                        xHint = 1
                    },
                    MainModFile.AddTooltips([ChargeStatus.GetTooltip])
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 2)
                    },
                    new AVariableHint()
                    {
                        status = ChargeStatus.Entry.Status
                    }.WithChargeTipFix(s, c),
                    new ADrawCard()
                    {
                        count = ChargeStatus.EffectiveCharge(s, c, s.ship),
                        xHint = 1
                    },
                    MainModFile.AddTooltips([ChargeStatus.GetTooltip])
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 1)
                    },
                    new AVariableHint()
                    {
                        status = ChargeStatus.Entry.Status
                    }.WithChargeTipFix(s, c),
                    new ADrawCard()
                    {
                        count = ChargeStatus.EffectiveCharge(s, c, s.ship),
                        xHint = 1
                    },
                    MainModFile.AddTooltips([ChargeStatus.GetTooltip])
                ];
                break;
        }
        return actions;
    }
}
