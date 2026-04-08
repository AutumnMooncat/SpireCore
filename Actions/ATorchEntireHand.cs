using System;
using System.Collections.Generic;
using AutumnMooncat.Spirecore.Util;
using Nickel;

namespace AutumnMooncat.Spirecore.Actions;

public class ATorchEntireHand : CardAction, ITooltipHelper, IMultiIconAction
{
    public static string ID => nameof(ATorchEntireHand);
    
    public override void Begin(G g, State s, Combat c)
    {
        if (c.hand.Count == 0)
        {
            return;
        }
        
        foreach (Card card in c.hand)
        {
            c.Queue(new AExhaustOtherCard()
            {
                uuid = card.uuid
            });
        }
        
        c.Queue(new AStatus()
        {
            status = Status.heat,
            statusAmount = c.hand.Count,
            targetPlayer = true
        });
    }
    
    public override List<Tooltip> GetTooltips(State s)
    {
        Records.TexturePayload[] pl = [new() { spr = CommonIcons.Torch }, new() { spr = CommonIcons.Hand, x = 10 }];
        return [
            ITooltipHelper.MakeMultiTooltip("action", ID, pl, 10, Colors.action),
            new TTGlossary("status.heat", $"<c=boldPink>{s.ship.heatTrigger}</c>")
        ];
    }

    public override Icon? GetIcon(State s)
    {
        return null;
    }

    public List<Records.RenderPayload> GetExtraIcons(State s)
    {
        return [new Records.RenderPayload(){spr = CommonIcons.Torch}, new Records.RenderPayload(){spr = CommonIcons.Hand}];
    }
}