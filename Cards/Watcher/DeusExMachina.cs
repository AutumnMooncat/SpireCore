using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Cards.Watcher;

internal sealed class DeusExMachina : Card, IRCard, IHasCustomCardTraits
{
    public static string ID => nameof(DeusExMachina);
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
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = MainModFile.Bind(["card", ID, "name"]).Localize,
            Art = IRCard.LookUpSpr(Characters.Watcher.CardAssetPath + ID)
        });
    }
    
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = 0,
            unplayable = upgrade != Upgrade.B,
            retain = upgrade == Upgrade.B,
            exhaust = upgrade == Upgrade.B
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
                    MainModFile.Kokoro().Impulsive.MakeAction(new ACascadingAddCard()
                    {
                        amount = 2,
                        card = new Miracle(),
                        destination = CardDestination.Hand
                    }).AsCardAction
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    MainModFile.Kokoro().Impulsive.MakeAction(new ACascadingAddCard()
                    {
                        amount = 3,
                        card = new Miracle(),
                        destination = CardDestination.Hand
                    }).AsCardAction
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new ACascadingAddCard()
                    {
                        amount = 2,
                        card = new Miracle(),
                        destination = CardDestination.Hand
                    }
                ];
                break;
        }
        return actions;
    }
    
    public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state)
    {
        HashSet<ICardTraitEntry> ret = [];
        if (upgrade != Upgrade.B)
        {
            ret.Add(MainModFile.Kokoro().Fleeting.Trait);
        }

        return ret;
    }
}
