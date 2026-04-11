using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Actions;

namespace AutumnMooncat.Spirecore.Cards.Ironclad;

[IRegisterable.Ignore]
internal sealed class WildStrike : Card, IRCard
{
    public static string ID => nameof(WildStrike);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Ironclad.DeckEntry!.Deck,
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
            cost = 1,
            flippable = upgrade == Upgrade.A
        };
        return data;
    }

    public int GetBaseDamage()
    {
        return upgrade == Upgrade.B ? 3 : 2;
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
                        damage = GetDmg(s, GetBaseDamage()),
                        moveEnemy = -2
                    },
                    new ACascadingAddCard()
                    {
                        amount = 1,
                        card = new TrashUnplayable(),
                        destination = CardDestination.Deck,
                        insertRandomly = true
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AAttack() 
                    {
                        damage = GetDmg(s, GetBaseDamage()),
                        moveEnemy = -2
                    },
                    new ACascadingAddCard()
                    {
                        amount = 1,
                        card = new TrashUnplayable(),
                        destination = CardDestination.Deck,
                        insertRandomly = true
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AAttack() 
                    {
                        damage = GetDmg(s, GetBaseDamage()),
                        moveEnemy = -3
                    },
                    new ACascadingAddCard()
                    {
                        amount = 1,
                        card = new TrashUnplayable(),
                        destination = CardDestination.Deck,
                        insertRandomly = true
                    }
                ];
                break;
        }
        return actions;
    }
}
