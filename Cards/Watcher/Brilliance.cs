using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Actions;
using AutumnMooncat.Spirecore.Features;

namespace AutumnMooncat.Spirecore.Cards.Watcher;

internal sealed class Brilliance : Card, IRCard
{
    public static string ID => nameof(Brilliance);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Watcher.DeckEntry.Deck,
                rarity = Rarity.rare,
                upgradesTo = [Upgrade.A, Upgrade.B],
                extraGlossary = ["status."+MantraStatus.Entry.Status.Key()]
            },
            Name = MainModFile.Bind(["card", ID, "name"]).Localize,
            Art = IRCard.LookUpSpr(Characters.Watcher.CardAssetPath + ID)
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
                    new AAttack()
                    {
                        damage = GetDmg(s, 1)
                    },
                    new AVariableHint()
                    {
                        status = MantraStatus.Entry.Status
                    },
                    new AAttack()
                    {
                        damage = GetDmg(s, s.ship.Get(MantraStatus.Entry.Status)),
                        xHint = 1
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 1),
                        stunEnemy = true
                    },
                    new AVariableHint()
                    {
                        status = MantraStatus.Entry.Status
                    },
                    new AAttack()
                    {
                        damage = GetDmg(s, s.ship.Get(MantraStatus.Entry.Status)),
                        xHint = 1,
                        stunEnemy = true
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 1),
                        piercing = true
                    },
                    new AVariableHint()
                    {
                        status = MantraStatus.Entry.Status
                    },
                    new AAttack()
                    {
                        damage = GetDmg(s, s.ship.Get(MantraStatus.Entry.Status)),
                        xHint = 1,
                        piercing = true
                    }
                ];
                break;
        }
        return actions;
    }
}
