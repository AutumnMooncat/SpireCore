using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Util;

namespace AutumnMooncat.SpireCore.Cards.Defect;

internal sealed class BeamCell : Card, IRCard
{
    public static string ID => nameof(BeamCell);
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
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(KokoroUtils.ChargeResource, 1), 
                        new AAttack()
                        {
                            damage = GetDmg(s, 1)
                        }).AsCardAction,
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(KokoroUtils.ChargeResource, 3), 
                        new AAttack()
                        {
                            damage = GetDmg(s, 1)
                        }).AsCardAction,
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 1)
                    },
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(KokoroUtils.ChargeResource, 2), 
                        new AAttack()
                        {
                            damage = GetDmg(s, 1)
                        }).AsCardAction,
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(KokoroUtils.ChargeResource, 1), 
                        new AAttack()
                        {
                            damage = GetDmg(s, 1),
                            piercing = true
                        }).AsCardAction,
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(KokoroUtils.ChargeResource, 3), 
                        new AAttack()
                        {
                            damage = GetDmg(s, 1),
                            piercing = true
                        }).AsCardAction,
                ];
                break;
        }
        return actions;
    }
}
