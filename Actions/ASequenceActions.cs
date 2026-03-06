using System.Collections.Generic;
using AutumnMooncat.Spirecore.Util;

namespace AutumnMooncat.Spirecore.Actions;

public class ASequenceActions: CardAction, IMultiIconAction, ITooltipHelper
{
    public static string ID => nameof(ASequenceActions);
    
    public List<CardAction> actions;
    public int amount;

    public override void Begin(G g, State s, Combat c)
    {
        for (int i = actions.Count - 1; i >= 0; i--)
        {
            c.QueueImmediate(actions[i]);
        }
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        List<Tooltip> ret = [];
        foreach (var action in actions)
        {
            ret.AddRange(action.GetTooltips(s));
        }
        return ret;
    }

    public override Icon? GetIcon(State s)
    {
        return null;
    }

    public List<Records.RenderPayload> GetExtraIcons(State s)
    {
        List<Records.RenderPayload> ret = [];
        foreach (var action in actions)
        {
            var nested = action.GetIcon(s);
            if (nested.HasValue)
            {
                ret.Add(new (){spr = nested.Value.path, amount = nested.Value.number, color = nested.Value.color, flipY = nested.Value.flipY});
            }

            if (action is IMultiIconAction mia)
            {
                ret.AddRange(mia.GetExtraIcons(s));
            }
        }
        return ret;
    }
}