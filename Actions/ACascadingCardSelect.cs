using System;
using System.Collections.Generic;
using AutumnMooncat.Spirecore.Features;
using AutumnMooncat.Spirecore.Util;

namespace AutumnMooncat.Spirecore.Actions;

public class ACascadingCardSelect : ACardSelect, IMultiIconAction, ITooltipHelper
{
    public static string ID => nameof(ACascadingCardSelect);

    public override Route BeginWithRoute(G g, State s, Combat c)
    {
        var ret = base.BeginWithRoute(g, s, c);
        if (ret is CardBrowse cb && cb.GetCardList(g).Count == 1 && cb.browseAction != null)
        {
            cb.OnPickCardAction(g, cb.GetCardList(g)[0], cb.browseAction);
            timer = 0;
            return null;
        }
        return ret;
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        var extra = CommonIcons.SourceSpr(browseSource);
        Records.TexturePayload[] pl = extra.HasValue
            ? [new() { spr = CommonIcons.Search }, new() { spr = extra.Value, x = 10 }]
            : [new() { spr = CommonIcons.Search }];
        List<Tooltip> ret = 
        [
            ITooltipHelper.MakeMultiTooltip("action", ID, pl, extra.HasValue ? 10 : 5, Colors.action, browseSource.ToString()),
        ];
        ret.AddRange(browseAction.GetTooltips(s));
        return ret;
    }

    public List<Records.RenderPayload> GetExtraIcons(State s)
    {
        List<Records.RenderPayload> ret = [];
        var spr = CommonIcons.SourceSpr(browseSource);
        if (spr.HasValue)
        {
            ret.Add(new (){spr = spr.Value});
        }

        var nested = browseAction.GetIcon(s);
        if (nested.HasValue)
        {
            ret.Add(new (){spr = nested.Value.path, amount = nested.Value.number, color = nested.Value.color, flipY = nested.Value.flipY});
        }

        if (browseAction is IMultiIconAction mia)
        {
            ret.AddRange(mia.GetExtraIcons(s));
        }
        return ret;
    }
}