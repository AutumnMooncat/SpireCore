using System;
using System.Collections.Generic;
using System.Linq;
using AutumnMooncat.SpireCore.Util;
using FSPRO;

namespace AutumnMooncat.SpireCore.Actions;

public class AScry : CardAction, ITooltipHelper
{
    public static string ID => nameof(AScry);
    public int count;

    public override Route BeginWithRoute(G g, State s, Combat c)
    {
        var validCards = s.deck.Where(card => s.deck.Count - s.deck.IndexOf(card) <= count).ToList();
        validCards.Reverse();

        if (count >= validCards.Count)
        {
            count = validCards.Count;
        }
        
        var cb = new CardBrowse()
        {
            mode = CardBrowse.Mode.Browse,
            browseSource = CardBrowse.Source.DrawPile,
            browseAction = new ScryFollowup()
        };
        var multi = MainModFile.Instance.KokoroApi.V2.MultiCardBrowse.MakeRoute(cb).SetMaxSelected(count).SetMinSelected(0).SetCardsOverride(validCards);
        return multi.AsRoute;
    }
    
    public override List<Tooltip> GetTooltips(State s)
    {
        return [ITooltipHelper.MakeTooltip("action", ID, CommonIcons.Scry, Colors.action, null, new {Cards = count})];
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon(CommonIcons.Scry, count, Colors.textMain);
    }

    public class ScryFollowup : CardAction
    {
        public override void Begin(G g, State s, Combat c)
        {
            var cards = MainModFile.Instance.KokoroApi.V2.MultiCardBrowse.GetSelectedCards(this) ?? [];
            for (var i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                s.RemoveCardFromWhereverItIs(card.uuid);
                card.waitBeforeMoving = i * 0.05;
                card.OnDiscard(s, c);
                c.SendCardToDiscard(s, card);
            }

            if (cards.Count != 0)
            {
                Audio.Play(Event.CardHandling);
            }
        }
        
        public override string GetCardSelectText(State s)
        {
            return MainModFile.Loc(["action", ID, "text"]);
        }
    }
}