using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Features;

namespace AutumnMooncat.Spirecore.Cards.Defect;

[IRegisterable.Ignore]
internal sealed class BallLightning : Card, IRCard
{
    public static string ID => nameof(BallLightning);
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
            cost = upgrade == Upgrade.A ? 0 : 1
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
                    new ASpawn()
                    {
                        thing = new LightningObject()
                        {
                            yAnimation = 0.0
                        }
                    },
                    new AAttack()
                    {
                        damage = GetDmg(s, 1)
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new ASpawn()
                    {
                        thing = new LightningObject()
                        {
                            yAnimation = 0.0
                        }
                    },
                    new AAttack()
                    {
                        damage = GetDmg(s, 1)
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new ASpawn()
                    {
                        thing = new LightningObject()
                        {
                            yAnimation = 0.0
                        }
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
