using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Features;
using AutumnMooncat.SpireCore.Util;

namespace AutumnMooncat.SpireCore.Cards.Defect;

internal sealed class StaticDischarge : Card, IRCard
{
    public static string ID => nameof(StaticDischarge);
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
            cost = upgrade == Upgrade.A ? 0 : 1,
            exhaust = true
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
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(KokoroUtils.ChargeResource, 3), 
                        new AStunShip()).AsCardAction,*/
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(KokoroUtils.ChargeResource, 3), 
                        MainModFile.Kokoro().SpoofedActions.MakeAction(
                            new InfoOnlyAction()
                            {
                                extraIcons = [new Records.RenderPayload(){spr = StableSpr.icons_stunShip, dx = 0, width = 20}],
                                tips = [new TTGlossary("action.stunShip")]
                            }, new AStunShip()).AsCardAction).AsCardAction,
                    new AStatus()
                    {
                        status = NoChargeStatus.Entry.Status,
                        statusAmount = 1,
                        targetPlayer = true
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(KokoroUtils.ChargeResource, 3), 
                        MainModFile.Kokoro().SpoofedActions.MakeAction(
                            new InfoOnlyAction()
                            {
                                extraIcons = [new Records.RenderPayload(){spr = StableSpr.icons_stunShip, dx = 0, width = 20}],
                                tips = [new TTGlossary("action.stunShip")]
                            }, new AStunShip()).AsCardAction).AsCardAction,
                    new AStatus()
                    {
                        status = NoChargeStatus.Entry.Status,
                        statusAmount = 1,
                        targetPlayer = true
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    MainModFile.Kokoro().ActionCosts.MakeCostAction(
                        MainModFile.Kokoro().ActionCosts.MakeResourceCost(KokoroUtils.ChargeResource, 2), 
                        MainModFile.Kokoro().SpoofedActions.MakeAction(
                            new InfoOnlyAction()
                            {
                                extraIcons = [new Records.RenderPayload(){spr = StableSpr.icons_stunShip, dx = 0, width = 20}],
                                tips = [new TTGlossary("action.stunShip")]
                            }, new AStunShip()).AsCardAction).AsCardAction,
                ];
                break;
        }
        return actions;
    }
}
