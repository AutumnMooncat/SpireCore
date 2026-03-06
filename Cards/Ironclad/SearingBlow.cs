using System;
using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Actions;
using AutumnMooncat.Spirecore.Features;
using AutumnMooncat.Spirecore.Util;

namespace AutumnMooncat.Spirecore.Cards.Ironclad;

internal sealed class SearingBlow : Card, IRCard, ITooltipHelper, ICustomUpgrades
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
                upgradesTo = [Upgrade.A]
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
        MainModFile.Instance.Helper.Content.Cards.SetCardTraitOverride(state, this, Limitless.Entry, true, true);
        return data;
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = 
        [
            new AAttack()
            {
                damage = GetDmg(s, 4 + 2*(int)upgrade)
            }
        ];
        
        return actions;
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
}
