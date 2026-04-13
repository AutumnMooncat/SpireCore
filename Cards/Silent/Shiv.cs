using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Cards.Silent;

internal sealed class Shiv : Card, IRCard
{
    public static string ID => nameof(Shiv);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Silent.DeckEntry.Deck,
                rarity = Rarity.common,
                dontOffer = true,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = MainModFile.Bind(["card", ID, "name"]).Localize,
            Art = IRCard.LookUpSpr(Characters.Silent.CardAssetPath + ID)
        });
    }
    
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = 0,
            temporary = true,
            exhaust = true
            //infinite = true
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
                    /*new ASpawn()
                    {
                        thing = new ShivObject()
                        {
                            yAnimation = 0.0
                        }
                    }*/
                    new ALaunchShiv()
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new ALaunchShiv()
                    {
                        bubble = true
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new ALaunchShiv(),
                    new ALaunchShiv()
                ];
                break;
        }
        return actions;
    }
}
