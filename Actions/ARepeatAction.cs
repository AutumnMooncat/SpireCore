using System.Collections.Generic;
using AutumnMooncat.Spirecore.Util;

namespace AutumnMooncat.Spirecore.Actions;

public class ARepeatAction : CardAction, IMultiIconAction, ITooltipHelper
{
    public static string ID => nameof(ARepeatAction);
    
    public CardAction action;
    public int amount;

    public override void Begin(G g, State s, Combat c)
    {
        for (int i = 0; i < amount; i++)
        {
            c.QueueImmediate(action);
        }
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        List<Tooltip> ret = [];
        if (xHint.HasValue)
        {
            ret.Add(ITooltipHelper.MakeTooltip("action", ID, CommonIcons.Repeat, Colors.action, "X"));
        }
        else
        {
            ret.Add(ITooltipHelper.MakeTooltip("action", ID, CommonIcons.Repeat, Colors.action, null, new {Times = amount}));
        }
        ret.AddRange(action.GetTooltips(s));
        return ret;
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon(CommonIcons.Repeat, amount, Colors.textMain);
    }

    public List<Records.RenderPayload> GetExtraIcons(State s)
    {
        List<Records.RenderPayload> ret = [/*new (){spr = CommonIcons.OpenBracket, dx = -4}*/];
        var nested = action.GetIcon(s);
        if (nested.HasValue)
        {
            ret.Add(new (){spr = nested.Value.path, amount = nested.Value.number, color = nested.Value.color, flipY = nested.Value.flipY});
        }

        if (action is IMultiIconAction mia)
        {
            ret.AddRange(mia.GetExtraIcons(s));
        }
        //ret.Add(new (){spr = CommonIcons.CloseBracket, dx = -5});
        return ret;
    }
}