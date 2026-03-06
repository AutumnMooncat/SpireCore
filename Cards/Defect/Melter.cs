using System;
using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Features;

namespace AutumnMooncat.Spirecore.Cards.Defect;

internal sealed class Melter : Card, IRCard
{
    public static string ID => nameof(Melter);
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
            cost = upgrade == Upgrade.A ? 1 : 2,
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
                    new AStatus()
                    {
                        targetPlayer = false,
                        status = Status.shield,
                        statusAmount = 0,
                        mode = AStatusMode.Set
                    },
                    new AAttack()
                    {
                        damage = GetDmg(s, 2)
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AStatus()
                    {
                        targetPlayer = false,
                        status = Status.shield,
                        statusAmount = 0,
                        mode = AStatusMode.Set
                    },
                    new AAttack()
                    {
                        damage = GetDmg(s, 2)
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AStatus()
                    {
                        targetPlayer = false,
                        status = Status.shield,
                        statusAmount = 0,
                        mode = AStatusMode.Set
                    },
                    new AAttack()
                    {
                        damage = GetDmg(s, 2)
                    },
                    new AAttack()
                    {
                        damage = GetDmg(s, 2)
                    }
                ];
                break;
        }
        return actions;
    }
}
