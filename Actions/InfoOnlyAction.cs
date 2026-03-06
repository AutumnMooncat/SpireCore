using System.Collections.Generic;

namespace AutumnMooncat.Spirecore.Actions;

public class InfoOnlyAction : CardAction, IMultiIconAction
{
    public List<Tooltip> tips;
    public Icon? icon;
    public List<Records.RenderPayload> extraIcons;
    
    public override List<Tooltip> GetTooltips(State s)
    {
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