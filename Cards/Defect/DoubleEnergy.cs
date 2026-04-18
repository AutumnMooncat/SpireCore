using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace AutumnMooncat.SpireCore.Cards.Defect;

internal sealed class DoubleEnergy : Card, IRCard
{
    public static string ID => nameof(DoubleEnergy);
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
            exhaust = upgrade != Upgrade.B
        };
        return data;
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = [];
        var x = MainModFile.Instance.KokoroApi.V2.EnergyAsStatus.MakeVariableHint().AsCardAction;
        if (upgrade != Upgrade.A)
        {
            MainModFile.Instance.KokoroHelper.ModData.SetOptionalModData<int>(x, "energyTooltipOverride", c.energy-1);
        }
        switch (upgrade)
        {
            case Upgrade.None:
                actions = 
                [
                    x,
                    new AEnergy()
                    {
                        changeAmount = c.energy - 1,
                        xHint = 1
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    x,
                    new AEnergy()
                    {
                        changeAmount = c.energy,
                        xHint = 1
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    x,
                    new AEnergy()
                    {
                        changeAmount = c.energy - 1,
                        xHint = 1
                    }
                ];
                break;
        }
        return actions;
    }
}
