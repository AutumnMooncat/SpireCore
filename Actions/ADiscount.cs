using System.Collections.Generic;
using System.Linq;
using AutumnMooncat.SpireCore.Util;

namespace AutumnMooncat.SpireCore.Actions;

public class ADiscount : CardAction, ITooltipHelper, IMultiIconAction
{
    public static string ID => nameof(ADiscount);
    
    public int discount;
    public bool hand;

    public override void Begin(G g, State s, Combat c)
    {
        if (hand)
        {
            foreach (var card in c.hand)
            {
                card.discount -= discount;
            }

            return;
        }
        
        var rand = c.hand.Shuffle(s.rngActions).FirstOrDefault();
        if (rand != null)
        {
            rand.discount -= discount;
        }
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon(CommonIcons.Discount, hand ? null : discount, Colors.textMain);
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        if (!hand)
        {
            return [ITooltipHelper.MakeTooltip("action", ID, CommonIcons.Discount, Colors.action, null, new { Discount = discount }),];
        }

        Records.TexturePayload[] pl = [
            new (){spr = CommonIcons.Discount},
            new (){spr = CommonIcons.Hand, x = 10}
        ];
        return [ITooltipHelper.MakeMultiTooltip("action", ID, pl, 10, Colors.action, "Hand", new { Discount = discount })];
    }

    public List<Records.RenderPayload> GetExtraIcons(State s)
    {
        List<Records.RenderPayload> ret = [];
        if (hand)
        {
            ret.Add(new Records.RenderPayload
            {
                spr = CommonIcons.Hand,
                amount = discount,
                color = Colors.textMain
            });
        }
        return ret;
    }
}