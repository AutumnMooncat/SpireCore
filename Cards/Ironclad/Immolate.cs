using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Cards.Ironclad;

internal sealed class Immolate : Card, IRCard
{
    public static string ID => nameof(Immolate);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Ironclad.DeckEntry!.Deck,
                rarity = Rarity.rare,
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

    public int GetBaseDamage()
    {
        return upgrade switch
        {
            Upgrade.B => 6,
            Upgrade.A => 2,
            _ => 4
        };
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
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.heat), 4), 
                        new AAttack()
                        {
                            damage = GetDmg(s, GetBaseDamage()),
                            piercing = true
                        }).AsCardAction,
                    new AStatus()
                    {
                        status = HeatCapStatus.Entry.Status,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    new AStatus()
                    {
                        status = Status.heat,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.heat), 2), 
                        new AAttack()
                        {
                            damage = GetDmg(s, GetBaseDamage()),
                            piercing = true
                        }).AsCardAction,
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.heat), 2), 
                        new AAttack()
                        {
                            damage = GetDmg(s, GetBaseDamage()),
                            piercing = true
                        }).AsCardAction,
                    new AStatus()
                    {
                        status = HeatCapStatus.Entry.Status,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    new AStatus()
                    {
                        status = Status.heat,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.heat), 5), 
                        new AAttack()
                        {
                            damage = GetDmg(s, GetBaseDamage()),
                            piercing = true
                        }).AsCardAction,
                    new AStatus()
                    {
                        status = HeatCapStatus.Entry.Status,
                        statusAmount = 2,
                        targetPlayer = true
                    },
                    new AStatus()
                    {
                        status = Status.heat,
                        statusAmount = 2,
                        targetPlayer = true
                    },
                ];
                break;
        }
        return actions;
    }
}
