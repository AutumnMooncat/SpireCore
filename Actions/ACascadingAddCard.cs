using System.Collections.Generic;
using AutumnMooncat.Spirecore.Features;
using AutumnMooncat.Spirecore.Util;

namespace AutumnMooncat.Spirecore.Actions;

public class ACascadingAddCard : AAddCard, IMultiIconAction, ITooltipHelper
{
    public static string ID => nameof(ACascadingAddCard);

    public override void Begin(G g, State s, Combat c)
    {
        if (amount == 0)
        {
            return;
        }
        base.Begin(g, s, c);
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        var extra = CommonIcons.DestinationSpr(destination, insertRandomly);
        Records.TexturePayload[] pl = extra.HasValue
            ? [new() { spr = CommonIcons.AddCard }, new() { spr = extra.Value, x = 10 }]
            : [new() { spr = CommonIcons.AddCard }];
        var suffix = destination.ToString();
        if (destination == CardDestination.Deck && insertRandomly)
        {
            suffix += "Random";
        }
        List<Tooltip> ret = 
        [
            ITooltipHelper.MakeMultiTooltip("action", ID, pl, extra.HasValue ? 10 : 5, Colors.action, suffix, new {Cards = amount}),
            new TTCard()
            {
                card = card,
                showCardTraitTooltips = showCardTraitTooltips
            }
        ];
        return ret;
    }

    public override Icon? GetIcon(State s)
    {
        return null;
        //return new Icon(CommonIcons.AddCard, null, Colors.textMain);
    }

    public List<Records.RenderPayload> GetExtraIcons(State s)
    {
        List<Records.RenderPayload> ret = [new (){spr = CommonIcons.AddCard}];
        //ret.Add(new (){spr = CommonIcons.ApplyTo});
        var spr = CommonIcons.DestinationSpr(destination, insertRandomly);
        if (spr.HasValue)
        {
            ret.Add(new (){spr = spr.Value, amount = amount, xHint = xHint});
        }
        return ret;
    }
}