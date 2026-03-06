using System;
using System.Collections.Generic;
using AutumnMooncat.Spirecore.Util;
using Nickel;

namespace AutumnMooncat.Spirecore.Actions;

public class MoveSelectedCardToPile : CardAction, ITooltipHelper
{
    public static string ID => nameof(MoveSelectedCardToPile);
    
    public CardBrowse.Source targetLocation;
    
    public override void Begin(G g, State s, Combat c)
    {
        if (selectedCard == null)
        {
            return;
        }

        bool wasInHAnd = c.hand.Contains(selectedCard);
        s.RemoveCardFromWhereverItIs(selectedCard.uuid);

        switch (targetLocation)
        {
            case CardBrowse.Source.ExhaustPile:
                selectedCard.ExhaustFX();
                Audio.Play(FSPRO.Event.CardHandling);
                c.SendCardToExhaust(s, selectedCard);
                break;
            case CardBrowse.Source.Hand:
                c.SendCardToHand(s, selectedCard);
                break;
            case CardBrowse.Source.DiscardPile:
                selectedCard.OnDiscard(s, c);
                c.SendCardToDiscard(s, selectedCard);
                break;
            case CardBrowse.Source.DrawPile:
                s.SendCardToDeck(selectedCard, true, true);
                break;
            case CardBrowse.Source.Deck:
                s.SendCardToDeck(selectedCard, true, false);
                break;
        }
        
    }
    
    public override List<Tooltip> GetTooltips(State s)
    {
        var spr = CommonIcons.ActionSpr(targetLocation);
        return [ITooltipHelper.MakeTooltip("action", ID, spr, Colors.action, targetLocation.ToString())];
    }
    
    public override string GetCardSelectText(State s)
    {
        return MainModFile.Loc(["action", ID, "text"+targetLocation]);
    }

    public override Icon? GetIcon(State s)
    {
        var spr = CommonIcons.ActionSpr(targetLocation);
        if (spr.HasValue)
        {
            return new Icon(spr.Value, null, Colors.textMain);
        }

        return null;
    }
}