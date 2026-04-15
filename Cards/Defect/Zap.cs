using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Cards.Defect;

internal sealed class Zap : Card, IRCard
{
    public static string ID => nameof(Zap);
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
            cost = upgrade switch {
                Upgrade.A => 0,
                Upgrade.B => 2,
                _ => 1
            }
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
                    new AStatus()
                    {
                        status = Status.droneShift,
                        statusAmount = 1,
                        targetPlayer = true
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
                    new AStatus()
                    {
                        status = Status.droneShift,
                        statusAmount = 1,
                        targetPlayer = true
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
                        thing = new LightningObject()
                        {
                            yAnimation = 0.0
                        }
                    },
                    new AStatus()
                    {
                        status = Status.droneShift,
                        statusAmount = 1,
                        targetPlayer = true
                    }
                ];
                break;
        }
        return actions;
    }
}
