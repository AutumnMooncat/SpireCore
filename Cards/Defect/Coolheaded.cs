using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Cards.Defect;

[IRegisterable.Ignore]
internal sealed class Coolheaded : Card, IRCard
{
    public static string ID => nameof(Coolheaded);
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
            cost = 1,
            exhaust = upgrade == Upgrade.B
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
                        status = Status.bubbleJuice,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    new ADrawCard()
                    {
                        count = 1
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AStatus()
                    {
                        status = Status.bubbleJuice,
                        statusAmount = 1,
                        targetPlayer = true
                    },
                    new ADrawCard()
                    {
                        count = 2
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AStatus()
                    {
                        status = Status.bubbleJuice,
                        statusAmount = 2,
                        targetPlayer = true
                    },
                    new ADrawCard()
                    {
                        count = 2
                    }
                ];
                break;
        }
        return actions;
    }
}
