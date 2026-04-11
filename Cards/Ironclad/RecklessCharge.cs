using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace AutumnMooncat.Spirecore.Cards.Ironclad;

internal sealed class RecklessCharge : Card, IRCard
{
    public static string ID => nameof(RecklessCharge);
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
                    new AAttack()
                    {
                        damage = GetDmg(s, 1)
                    },
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.shield), 1), 
                        new AAttack()
                        {
                            damage = GetDmg(s, 1)
                        }).AsCardAction
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
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.shield), 1), 
                        new AAttack()
                        {
                            damage = GetDmg(s, 2)
                        }).AsCardAction
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 1)
                    },
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.shield), 1), 
                        new AAttack()
                        {
                            damage = GetDmg(s, 1)
                        }).AsCardAction,
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.shield), 1), 
                        new AAttack()
                        {
                            damage = GetDmg(s, 1)
                        }).AsCardAction
                ];
                break;
        }
        return actions;
    }
}
