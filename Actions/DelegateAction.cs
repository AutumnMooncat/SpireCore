using System.Collections.Generic;

namespace AutumnMooncat.SpireCore.Actions;

public class DelegateAction : CardAction, IMultiIconAction
{
    public delegate Route BeginDel(G g, State s, Combat c, DelegateAction thiz);

    public delegate List<Tooltip> TipDel(State s, DelegateAction thiz);

    public BeginDel begin;
    public List<Tooltip> tips;
    public TipDel getTips;
    public Icon? icon;
    public List<Records.RenderPayload> extraIcons;
    public string selectText;

    public override Route BeginWithRoute(G g, State s, Combat c)
    {
        return begin(g, s, c, this);
    }

    public override string GetCardSelectText(State s)
    {
        return selectText;
    }

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