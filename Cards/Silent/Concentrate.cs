using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Util;
using FSPRO;

namespace AutumnMooncat.SpireCore.Cards.Silent;

internal sealed class Concentrate : Card, IRCard
{
    public static string ID => nameof(Concentrate);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Silent.DeckEntry.Deck,
                rarity = Rarity.uncommon,
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
            cost = 0
        };
        return data;
    }

    public CardAction MakeAction(int cards)
    {
        var act = new DelegateAction()
        {
            begin = (g, s, c, thiz) =>
            {
                var cardsToDiscard = c.hand
                    .Where(handCard =>
                        (MainModFile.Instance.KokoroApi.V2.MultiCardBrowse.GetSelectedCards(thiz) ?? []).Any(card =>
                            card.uuid == handCard.uuid))
                    .ToList();
                for (var i = 0; i < cardsToDiscard.Count; i++)
                {
                    var card = cardsToDiscard[i];
                    c.hand.Remove(card);
                    card.waitBeforeMoving = i * 0.05;
                    card.OnDiscard(s, c);
                    c.SendCardToDiscard(s, card);
                }

                if (cardsToDiscard.Count == 0)
                    return null;

                Audio.Play(Event.CardHandling);
                return null;
            },
            icon = new() { path = CommonIcons.Discard },
            selectText = MainModFile.Loc(["card", ID, "text"], new {Cards = cards}),
            tips = [ITooltipHelper.MakeTooltip("card", ID, CommonIcons.Discard, Colors.action)]
                
        };
        return act;
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = [];
        switch (upgrade)
        {
            case Upgrade.None:
                actions = 
                [
                    new AMultiCardSelect()
                    {
                        count = 3,
                        browseSource = CardBrowse.Source.Hand,
                        extraIcons = [new (){spr = CommonIcons.Discard}],
                        selectText = MainModFile.Loc(["card", ID, "text"], new {Cards = 3}),
                        tips = [ITooltipHelper.MakeTooltip("card", ID, CommonIcons.Discard, Colors.action)],
                        callback = cards =>
                        {
                            for (var i = 0; i < cards.Count; i++)
                            {
                                var card = cards[i];
                                c.hand.Remove(card);
                                card.waitBeforeMoving = i * 0.05;
                                card.OnDiscard(s, c);
                                c.SendCardToDiscard(s, card);
                            }

                            if (cards.Count != 0)
                            {
                                Audio.Play(Event.CardHandling);
                            }
                        }
                    },
                    new AEnergy()
                    {
                        changeAmount = 2
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AMultiCardSelect()
                    {
                        count = 2,
                        browseSource = CardBrowse.Source.Hand,
                        extraIcons = [new (){spr = CommonIcons.Discard}],
                        selectText = MainModFile.Loc(["card", ID, "text"], new {Cards = 2}),
                        tips = [ITooltipHelper.MakeTooltip("card", ID, CommonIcons.Discard, Colors.action)]
                    },
                    new AEnergy()
                    {
                        changeAmount = 2
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AMultiCardSelect()
                    {
                        count = 3,
                        browseSource = CardBrowse.Source.Hand,
                        extraIcons = [new (){spr = CommonIcons.Discard}],
                        selectText = MainModFile.Loc(["card", ID, "text"], new {Cards = 3}),
                        tips = [ITooltipHelper.MakeTooltip("card", ID, CommonIcons.Discard, Colors.action)]
                    },
                    new AEnergy()
                    {
                        changeAmount = 3
                    }
                ];
                break;
        }
        return actions;
    }
}
