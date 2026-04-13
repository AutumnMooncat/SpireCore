using System.Collections.Generic;

namespace AutumnMooncat.SpireCore.Actions;

public class InfoOnlyAction : CardAction, IMultiIconAction
{
    public delegate List<Tooltip> TipDel(State s, InfoOnlyAction thiz);
    public TipDel getTips;
    public List<Tooltip> tips;
    public Icon? icon;
    public List<Records.RenderPayload> extraIcons;
    
    public override List<Tooltip> GetTooltips(State s)
    {
        if (getTips != null)
        {
            return getTips(s, this);
        }
        return tips ?? [];
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