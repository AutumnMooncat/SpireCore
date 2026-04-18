using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Cards.Defect;

internal sealed class ThunderStrike : Card, IRCard
{
    public static string ID => nameof(ThunderStrike);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Defect.DeckEntry.Deck,
                rarity = Rarity.rare,
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
            cost = upgrade == Upgrade.B ? 2 : 3,
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
                    new AVariableHint()
                    {
                        status = ChargeStatus.Entry.Status
                    }.WithChargeTipFix(s, c),
                    new AAttack()
                    {
                        damage = GetDmg(s, ChargeStatus.EffectiveCharge(s, c, s.ship)),
                        xHint = 1
                    },
                    new AAttack()
                    {
                        damage = GetDmg(s, ChargeStatus.EffectiveCharge(s, c, s.ship)),
                        xHint = 1
                    },
                    MainModFile.AddTooltips([ChargeStatus.GetTooltip])
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AVariableHint()
                    {
                        status = ChargeStatus.Entry.Status
                    }.WithChargeTipFix(s, c),
                    new AAttack()
                    {
                        damage = GetDmg(s, ChargeStatus.EffectiveCharge(s, c, s.ship)),
                        xHint = 1
                    },
                    new AAttack()
                    {
                        damage = GetDmg(s, ChargeStatus.EffectiveCharge(s, c, s.ship)),
                        xHint = 1
                    },
                    MainModFile.AddTooltips([ChargeStatus.GetTooltip])
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AVariableHint()
                    {
                        status = ChargeStatus.Entry.Status
                    }.WithChargeTipFix(s, c),
                    new AAttack()
                    {
                        damage = GetDmg(s, ChargeStatus.EffectiveCharge(s, c, s.ship)),
                        xHint = 1
                    },
                    new AAttack()
                    {
                        damage = GetDmg(s, ChargeStatus.EffectiveCharge(s, c, s.ship)),
                        xHint = 1
                    },
                    MainModFile.AddTooltips([ChargeStatus.GetTooltip])
                ];
                break;
        }
        return actions;
    }
}
