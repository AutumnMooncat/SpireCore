using System.Collections.Generic;
using AutumnMooncat.Spirecore.Util;
using Nickel;

namespace AutumnMooncat.Spirecore.Actions;

public class ADiscountTopCard : CardAction, IMultiIconAction, ITooltipHelper
{
    public static string ID => nameof(ADiscountTopCard);
    
    public int discount;
    public bool hideTopCardTip;

    public override void Begin(G g, State s, Combat c)
    {
        if (s.deck.Count > 0)
        {
            Card card = s.deck[^1];
            card.discount -= discount;
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
            ITooltipHelper.MakeTooltip("action", ID, CommonIcons.Discount, Colors.action, "2", new {Discount = discount}),
        ];
        if (!hideTopCardTip)
        {
            tooltips.Insert(0, ITooltipHelper.MakeTooltip("action", ID, CommonIcons.TopCard, Colors.action));
        }
        return tooltips;
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon(CommonIcons.TopCard, null, Colors.textMain);
    }

    public List<Records.RenderPayload> GetExtraIcons(State s)
    {
        List<Records.RenderPayload> ret = [new (){spr = CommonIcons.Discount, amount = discount, dx = -1}];
        return ret;
    }
}