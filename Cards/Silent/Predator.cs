using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace AutumnMooncat.SpireCore.Cards.Silent;

internal sealed class Predator : Card, IRCard
{
    public static string ID => nameof(Predator);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Silent.DeckEntry.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = MainModFile.Bind(["card", ID, "name"]).Localize,
            Art = IRCard.LookUpSpr(Characters.Silent.CardAssetPath + ID)
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
                        damage = GetDmg(s, 3)
                    },
                    new AStatus()
                    {
                        status = Status.drawNextTurn,
                        statusAmount = 2,
                        targetPlayer = true
                    },
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 5)
                    },
                    new AStatus()
                    {
                        status = Status.drawNextTurn,
                        statusAmount = 2,
                        targetPlayer = true
                    },
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 3)
                    },
                    new AStatus()
                    {
                        status = Status.drawNextTurn,
                        statusAmount = 4,
                        targetPlayer = true
                    },
                ];
                break;
        }
        return actions;
    }
}
