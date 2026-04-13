using System.Collections.Generic;

namespace AutumnMooncat.SpireCore.Actions;

public class DoIfAction : CardAction, IMultiIconAction
{
    public delegate bool Check();
    
    public CardAction action;
    public List<Tooltip> tips;
    public Icon? icon;
    public List<Records.RenderPayload> extraIcons;
    public Check check;
    
    public override void Begin(G g, State s, Combat c)
    {
        if (check())
        {
            c.QueueImmediate(action);
        }
    }
    
    public override List<Tooltip> GetTooltips(State s)
    {
        var ret = tips ?? [];
        if (action != null)
        {
            ret.AddRange(action.GetTooltips(s));
        }
        return ret;
    }

    public override Icon? GetIcon(State s)
    {
        return icon;
    }

    public List<Records.RenderPayload> GetExtraIcons(State s)
    {
        return extraIcons ?? [];
    }
}