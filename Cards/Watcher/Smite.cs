using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Util;

namespace AutumnMooncat.SpireCore.Cards.Watcher;

internal sealed class Smite : Card, IRCard
{
    public static string ID => nameof(Smite);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Watcher.DeckEntry.Deck,
                rarity = Rarity.common,
                dontOffer = true,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = MainModFile.Bind(["card", ID, "name"]).Localize,
            Art = IRCard.LookUpSpr(Characters.Watcher.CardAssetPath + ID)
        });
    }

    public int? realDamage;
    
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = 1,
            retain = true,
            temporary = true,
            exhaust = true
        };
        return data;
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        if (realDamage is null)
        {
            return upgrade switch
            {
                Upgrade.A => [new AAttack() { xHint = 1 }.WithExtraIcons([new () { spr = CommonIcons.Plus, amount = 1, color = Colors.textMain}])],
                Upgrade.B => [new AAttack() { xHint = 1 }, new AAttack() { xHint = 1 }],
                _ => [new AAttack() { xHint = 1 }]
            };
        }
        List<CardAction> actions = [];
        switch (upgrade)
        {
            case Upgrade.None:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, realDamage.Value)
                    },
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, realDamage.Value + 1)
                    },
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, realDamage.Value)
                    },
                    new AAttack()
                    {
                        damage = GetDmg(s, realDamage.Value)
                    },
                ];
                break;
        }
        return actions;
    }
}
