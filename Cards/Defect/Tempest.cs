using System;
using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Features;
using AutumnMooncat.SpireCore.Util;

namespace AutumnMooncat.SpireCore.Cards.Defect;

internal sealed class Tempest : Card, IRCard
{
    public static string ID => nameof(Tempest);
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
            cost = 3,
            exhaust = true,
            flippable = upgrade == Upgrade.B
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
                        thing = new LightningObject()
                        {
                            yAnimation = 0.0
                        },
                    },
                    new ASpawn()
                    {
                        thing = new LightningObject()
                        {
                            yAnimation = 0.0
                        },
                        offset = 1
                    },
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new ASpawn()
                    {
                        thing = new LightningObject()
                        {
                            yAnimation = 0.0,
                            bubbleShield = true
                        },
                        offset = -1
                    },
                    new ASpawn()
                    {
                        thing = new LightningObject()
                        {
                            yAnimation = 0.0,
                            bubbleShield = true
                        },
                    },
                    new ASpawn()
                    {
                        thing = new LightningObject()
                        {
                            yAnimation = 0.0,
                            bubbleShield = true
                        },
                        offset = 1
                    },
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
                    },
                    new AMove()
                    {
                        targetPlayer  = true,
                        dir = 1
                    },
                    new ASpawn()
                    {
                        thing = new LightningObject()
                        {
                            yAnimation = 0.0
                        },
                    },
                    new AMove()
                    {
                        targetPlayer  = true,
                        dir = 1
                    },
                    new ASpawn()
                    {
                        thing = new LightningObject()
                        {
                            yAnimation = 0.0
                        },
                    },
                ];
                break;
        }
        return actions;
    }
}
