using System;
using System.Collections.Generic;
using AutumnMooncat.SpireCore.Features;
using AutumnMooncat.SpireCore.Util;

namespace AutumnMooncat.SpireCore.Actions;

public class ATopDiscardContext : CardAction, IMultiIconAction, ITooltipHelper
{
    public static string ID => nameof(ATopDiscardContext);

    public CardAction followup;
    public Card thatIsnt;

    public override void Begin(G g, State s, Combat c)
    {
        if (s.route is Combat && c.discard.Count > 0)
        {
            var top = c.discard[^1];
            if (top == thatIsnt)
            {
                if (c.discard.Count <= 1)
                {
                    return;
                }
                top = c.discard[^2];
            }
            followup.selectedCard = top;
            c.QueueImmediate(followup);
        }
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon(CommonIcons.TopDiscard, null, Colors.textMain);
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        List<Tooltip> ret = 
        [
            ITooltipHelper.MakeTooltip("action", ID, CommonIcons.TopDiscard, Colors.action),
        ];
        if (s.route is Combat c && c.discard.Count > 0)
        {
            var top = c.discard[^1];
            ret.Add(new TTCard()
            {
                card = Mutil.DeepCopy(top),
                showCardTraitTooltips = false
            });
        }
        ret.AddRange(followup.GetTooltips(s));
        return ret;
    }

    public List<Records.RenderPayload> GetExtraIcons(State s)
    {
        List<Records.RenderPayload> ret = [];

        var nested = followup.GetIcon(s);
        if (nested.HasValue)
        {
            ret.Add(new (){spr = nested.Value.path, amount = nested.Value.number, color = nested.Value.color, flipY = nested.Value.flipY});
        }

        if (followup is IMultiIconAction mia)
        {
            ret.AddRange(mia.GetExtraIcons(s));
        }
        return ret;
    }
}