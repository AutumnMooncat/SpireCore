using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;

namespace AutumnMooncat.SpireCore.Cards.Ironclad;

internal sealed class Armaments : Card, IRCard
{
    public static string ID => nameof(Armaments);
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
                    new AStatus()
                    {
                        status = Status.shield,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.heat), 1), 
                        new ACardContext()
                        {
                            context = ACardContext.Context.LeftHand,
                            thatIsnt = this,
                            flipped = flipped,
                            followup = new TempUpgradeSelectedCard()
                            {
                                upgrade = Upgrade.A
                            }
                        }).AsCardAction,
                    /*new ACascadingCardSelect()
                    {
                        browseSource = CardBrowse.Source.Hand,
                        browseAction = new TempUpgradeSelectedCard()
                        {
                            upgrade = Upgrade.A
                        },
                        filterAvailableUpgrade = Upgrade.A,
                        allowCancel = true,
                    }*/
                ];
                break;
            case Upgrade.A:
                actions =
                [
                    new AStatus()
                    {
                        status = Status.shield,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.heat), 1), 
                        new ATempUpgradeRandomCard()
                        {
                            upgrade = Upgrade.A
                        }).AsCardAction,
                ];
                break;
            case Upgrade.B:
                actions =
                [
                    new AStatus()
                    {
                        status = Status.shield,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(MainModFile.Kokoro().ActionCosts.MakeStatusResource(Status.heat), 1), 
                        new ATempUpgradeRandomCard()
                        {
                            upgrade = Upgrade.B
                        }).AsCardAction,
                ];
                break;
        }
        return actions;
    }
}
