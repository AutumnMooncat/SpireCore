using System;
using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Features;
using AutumnMooncat.SpireCore.Util;

namespace AutumnMooncat.SpireCore.Cards.Ironclad;

internal sealed class SearingBlow : Card, IRCard/*, ITooltipHelper, ICustomUpgrades, IHasCustomCardTraits*/
{
    public static string ID => nameof(SearingBlow);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Ironclad.DeckEntry.Deck,
                rarity = Rarity.uncommon,
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
            cost = 2
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
                        damage = GetDmg(s, 5),
                        status = Status.heat,
                        statusAmount = 3,
                    }.WithHeatTipFix(),
                    new AStatus()
                    {
                        status = Status.heat,
                        statusAmount = 3,
                        targetPlayer = true
                    },
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 5),
                        status = Status.heat,
                        statusAmount = 3,
                        piercing = true
                    }.WithHeatTipFix(),
                    new AStatus()
                    {
                        status = Status.heat,
                        statusAmount = 3,
                        targetPlayer = true
                    },
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 3),
                        status = Status.heat,
                        statusAmount = 3
                    }.WithHeatTipFix(),
                    new AAttack()
                    {
                        damage = GetDmg(s, 3),
                        status = Status.heat,
                        statusAmount = 3
                    }.WithHeatTipFix(),
                    new AStatus()
                    {
                        status = Status.heat,
                        statusAmount = 3,
                        targetPlayer = true
                    },
                ];
                break;
        }
        return actions;
    }
    
    /*public override List<CardAction> GetActions(State s, Combat c)
    {
        var att = new AAttack()
        {
            damage = GetDmg(s, 4 + 2 * (int)upgrade),
            status = Status.heat,
            statusAmount = 1 + (int)upgrade
        }.WithHeatTipFix();
        
        return [att];
    }

    public bool IsUpgradableOverride(bool baseResult)
    {
        return true;
    }

    public string GetFullDisplayNameOverride(string result)
    {
        return DB.Join(GetLocName(), upgrade == Upgrade.None ? "" : " +" + ((int)upgrade));
    }

    public CardMeta GetMetaOverride(CardMeta baseResult)
    {
        var ret = Mutil.DeepCopy(baseResult);
        ret.upgradesTo = [(Upgrade)((int)upgrade + 1)];
        return ret;
    }

    public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state)
    {
        HashSet<ICardTraitEntry> ret = [Limitless.Entry];
        return ret;
    }*/
}
