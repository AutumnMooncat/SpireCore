using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Cards.Defect;

internal sealed class Rainbow : Card, IRCard
{
    public static string ID => nameof(Rainbow);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Defect.DeckEntry.Deck,
                rarity = Rarity.rare,
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
            cost = upgrade == Upgrade.B ? 1 : 2,
            exhaust = upgrade != Upgrade.A
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
                        },
                        offset = -1
                    },
                    new ASpawn()
                    {
                        thing = new FrostObject()
                        {
                            yAnimation = 0.0
                        }
                    },
                    new ASpawn()
                    {
                        thing = new DarkObject()
                        {
                            yAnimation = 0.0,
                        },
                        offset = 1
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
                        },
                        offset = -1
                    },
                    new ASpawn()
                    {
                        thing = new FrostObject()
                        {
                            yAnimation = 0.0
                        }
                    },
                    new ASpawn()
                    {
                        thing = new DarkObject()
                        {
                            yAnimation = 0.0,
                        },
                        offset = 1
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
                        },
                        offset = -1
                    },
                    new ASpawn()
                    {
                        thing = new FrostObject()
                        {
                            yAnimation = 0.0
                        }
                    },
                    new ASpawn()
                    {
                        thing = new DarkObject()
                        {
                            yAnimation = 0.0,
                        },
                        offset = 1
                    }
                ];
                break;
        }
        return actions;
    }
}
