using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Actions;

namespace AutumnMooncat.Spirecore.Cards.Ironclad;

internal sealed class Anger : Card, IRCard
{
    public static string ID => nameof(Anger);
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
            cost = 0
        };
        return data;
    }

    public int GetBaseDamage()
    {
        return upgrade == Upgrade.A ? 2 : 1;
    }

    public Card GetCopy()
    {
        Card ret = new Anger();
        ret.temporaryOverride = true;
        ret.upgrade = upgrade;
        return ret;
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
                        damage = GetDmg(s, GetBaseDamage())
                    },
                    new ACascadingAddCard()
                    {
                        amount = 1,
                        card = GetCopy(),
                        destination = CardDestination.Discard,
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AAttack() 
                    {
                        damage = GetDmg(s, GetBaseDamage())
                    },
                    new ACascadingAddCard()
                    {
                        amount = 1,
                        card = GetCopy(),
                        destination = CardDestination.Discard,
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AAttack() 
                    {
                        damage = GetDmg(s, GetBaseDamage())
                    },
                    new ACascadingAddCard()
                    {
                        amount = 1,
                        card = GetCopy(),
                        destination = CardDestination.Deck,
                        insertRandomly = true
                    }
                ];
                break;
        }
        return actions;
    }
}
