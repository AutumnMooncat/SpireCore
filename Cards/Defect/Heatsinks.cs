using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Cards.Defect;

[IRegisterable.Ignore]
internal sealed class Heatsinks : Card, IRCard
{
    public static string ID => nameof(Heatsinks);
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
            cost = 1,
            exhaust = true,
            buoyant = upgrade == Upgrade.B
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
                        status = HeatsinksStatus.Entry.Status,
                        statusAmount = 1,
                        targetPlayer = true
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AStatus()
                    {
                        status = HeatsinksStatus.Entry.Status,
                        statusAmount = 2,
                        targetPlayer = true
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AStatus()
                    {
                        status = HeatsinksStatus.Entry.Status,
                        statusAmount = 1,
                        targetPlayer = true
                    }
                ];
                break;
        }
        return actions;
    }
}
