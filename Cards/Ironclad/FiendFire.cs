using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Actions;
using AutumnMooncat.Spirecore.Patches;
using AutumnMooncat.Spirecore.Util;

namespace AutumnMooncat.Spirecore.Cards.Ironclad;

internal sealed class FiendFire : Card, IRCard
{
    public static string ID => nameof(FiendFire);
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
            cost = upgrade == Upgrade.B ? 3 : upgrade == Upgrade.A ? 1 : 2,
            exhaust = true
        };
        return data;
    }

    public int GetBaseDamage()
    {
        return upgrade == Upgrade.B ? 2 : 1;
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = [];
        switch (upgrade)
        {
            case Upgrade.None:
                actions = 
                [
                    new AVariableHint()
                    {
                        hand = true,
                        handAmount = CardsInHand(s)
                    },
                    new AAttack()
                    {
                        damage = GetDmg(s, GetBaseDamage())
                    }.WithExtraIcons([new (){spr = CommonIcons.Repeat, amount = CardsInHand(s), xHint = 1}]),
                    MainModFile.AddTooltips([ITooltipHelper.MakeTooltip("card", ID, CommonIcons.Repeat, Colors.action)]),
                    new ATorchEntireHand()
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AVariableHint()
                    {
                        hand = true,
                        handAmount = CardsInHand(s)
                    },
                    new AAttack()
                    {
                        damage = GetDmg(s, GetBaseDamage())
                    }.WithExtraIcons([new (){spr = CommonIcons.Repeat, amount = CardsInHand(s), xHint = 1}]),
                    MainModFile.AddTooltips([ITooltipHelper.MakeTooltip("card", ID, CommonIcons.Repeat, Colors.action)]),
                    new ATorchEntireHand()
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AVariableHint()
                    {
                        hand = true,
                        handAmount = CardsInHand(s)
                    },
                    new AAttack()
                    {
                        damage = GetDmg(s, GetBaseDamage())
                    }.WithExtraIcons([new (){spr = CommonIcons.Repeat, amount = CardsInHand(s), xHint = 1}]),
                    MainModFile.AddTooltips([ITooltipHelper.MakeTooltip("card", ID, CommonIcons.Repeat, Colors.action)]),
                    new ATorchEntireHand()
                ];
                break;
        }
        return actions;
    }
    
    public int CardsInHand(State s)
    {
        int total = 0;
        if (s.route is Combat route)
            total = route.hand.Count - 1;
        //if (upgrade == Upgrade.A)
            //++total;
        return total;
    }
}
