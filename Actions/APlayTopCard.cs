using System.Collections.Generic;
using AutumnMooncat.SpireCore.Util;

namespace AutumnMooncat.SpireCore.Actions;

public class APlayTopCard : CardAction, IMultiIconAction, ITooltipHelper
{
    public static string ID => nameof(APlayTopCard);
    
    public bool exhaustThisCardAfterwards;

    public override void Begin(G g, State s, Combat c)
    {
        if (s.deck.Count == 0 && c.discard.Count > 0)
        {
            foreach (Card card in c.discard)
            {
                s.SendCardToDeck(card, true);
            }
            c.discard.Clear();
            s.ShuffleDeck(true);
        }
        if (s.deck.Count > 0)
        {
            Card card = s.deck[^1];
            c.TryPlayCard(s, card, true, exhaustThisCardAfterwards);
        }
        else
        {
            Audio.Play(FSPRO.Event.CardHandling);
        }
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        List<Tooltip> tooltips = 
        [
            ITooltipHelper.MakeTooltip("action", ID, CommonIcons.TopCard, Colors.action),
            ITooltipHelper.MakeTooltip("action", ID, CommonIcons.Bypass, Colors.action, "2"),
        ];
        if (exhaustThisCardAfterwards)
        {
            tooltips.Add(ITooltipHelper.MakeTooltip("action", ID, CommonIcons.Exhaust, Colors.action, "3"));
        }

        return tooltips;
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon(CommonIcons.TopCard, null, Colors.textMain);
    }

    public List<Records.RenderPayload> GetExtraIcons(State s)
    {
        List<Records.RenderPayload> ret = [new (){spr = CommonIcons.Bypass, dx = -1, dy = -1}];
        if (exhaustThisCardAfterwards)
        {
            //ret.Add(new (){spr = CommonIcons.Plus, dx = -3});
            ret.Add(new (){spr = CommonIcons.Exhaust, dx = -1});
        }
        return ret;
    }
}