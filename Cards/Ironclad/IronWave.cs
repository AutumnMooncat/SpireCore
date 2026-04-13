using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace AutumnMooncat.SpireCore.Cards.Ironclad;

internal sealed class IronWave : Card, IRCard
{
    public static string ID => nameof(IronWave);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Ironclad.DeckEntry.Deck,
                rarity = Rarity.common,
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
            cost = 1,
            floppable = true,
            art = flipped ? StableSpr.cards_MiningDrill_Bottom : StableSpr.cards_MiningDrill_Top
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
                    /*MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.shield), 1), 
                        new AAttack()
                        {
                            damage = GetDmg(s, 2)
                        }).AsCardAction,*/
                    new AStatus() { 
                        status = Status.shield, 
                        statusAmount = 1, 
                        targetPlayer = true,
                        disabled = flipped
                    },
                    new AStatus() { 
                        status = Status.heat, 
                        statusAmount = 1, 
                        targetPlayer = true,
                        disabled = flipped
                    },
                    new ADummyAction(),
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.heat), 1), 
                        new AAttack()
                        {
                            damage = GetDmg(s, 1)
                        }).AsCardAction.WithDisabled(!flipped),
                    /*new ADummyAction(),
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.shield), 1), 
                        new AAttack()
                        {
                            damage = GetDmg(s, 2)
                        }).AsCardAction.WithDisabled(!flipped)*/
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AStatus() { 
                        status = Status.shield, 
                        statusAmount = 1, 
                        targetPlayer = true,
                        disabled = flipped
                    },
                    new AStatus() { 
                        status = Status.heat, 
                        statusAmount = 1, 
                        targetPlayer = true,
                        disabled = flipped
                    },
                    new ADummyAction(),
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.heat), 1), 
                        new AAttack()
                        {
                            damage = GetDmg(s, 2)
                        }).AsCardAction.WithDisabled(!flipped),
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 1),
                        disabled = flipped
                    },
                    new AStatus() { 
                        status = Status.heat, 
                        statusAmount = 1, 
                        targetPlayer = true,
                        disabled = flipped
                    },
                    new ADummyAction(),
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.heat), 1), 
                        new AStatus() { 
                            status = Status.shield, 
                            statusAmount = 2, 
                            targetPlayer = true
                        }).AsCardAction.WithDisabled(!flipped),
                ];
                break;
        }
        return actions;
    }
}
