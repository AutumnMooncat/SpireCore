using System.Collections.Generic;
using System.Linq;
using AutumnMooncat.SpireCore.Util;

namespace AutumnMooncat.SpireCore.Actions;

public class AExhaustRandomCard : CardAction, ITooltipHelper
{
    public static string ID => nameof(AExhaustRandomCard);
    
    public int howManyCards;
    
    public override void Begin(G g, State s, Combat c)
    {
        for (int i = 0; i < howManyCards; i++)
        {
            Card card = c.hand.Shuffle(s.rngActions).FirstOrDefault();
            if (card != null)
            {
                card.ExhaustFX();
                Audio.Play(FSPRO.Event.CardHandling);
                s.RemoveCardFromWhereverItIs(card.uuid);
                c.SendCardToExhaust(s, card);
            }
        }
    }
    
    public override List<Tooltip> GetTooltips(State s)
    {
        return [ITooltipHelper.MakeTooltip("action", ID, CommonIcons.Exhaust, Colors.action, null, new {Cards = howManyCards})];
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon(CommonIcons.Exhaust, howManyCards, Colors.textMain);
    }
}