using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace AutumnMooncat.SpireCore.Cards.Silent;

internal sealed class QuickSlash : Card, IRCard
{
    public static string ID => nameof(QuickSlash);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Silent.DeckEntry.Deck,
                rarity = Rarity.common,
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
                    new AMove()
                    {
                        dir = 1,
                        targetPlayer = true
                    },
                    new AAttack()
                    {
                        damage = GetDmg(s, 1)
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
                    new AMove()
                    {
                        dir = 1,
                        targetPlayer = true
                    },
                    new AAttack()
                    {
                        damage = GetDmg(s, 2)
                    },
                    new ADrawCard()
                    {
                        count = 1
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AMove()
                    {
                        dir = 1,
                        targetPlayer = true
                    },
                    new AAttack()
                    {
                        damage = GetDmg(s, 1)
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
