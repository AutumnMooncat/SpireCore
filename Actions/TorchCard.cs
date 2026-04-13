using System;
using System.Collections.Generic;
using AutumnMooncat.SpireCore.Util;
using Nickel;

namespace AutumnMooncat.SpireCore.Actions;

public class TorchCard : CardAction, ITooltipHelper
{
    public static string ID => nameof(TorchCard);
    
    public override void Begin(G g, State s, Combat c)
    {
        if (selectedCard == null)
        {
            return;
        }
        
        s.RemoveCardFromWhereverItIs(selectedCard.uuid);
        selectedCard.ExhaustFX();
        Audio.Play(FSPRO.Event.CardHandling);
        c.SendCardToExhaust(s, selectedCard);
        c.QueueImmediate(new AStatus()
        {
            status = Status.heat,
            statusAmount = 1,
            targetPlayer = true
        });
    }
    
    public override List<Tooltip> GetTooltips(State s)
    {
        return [
            ITooltipHelper.MakeTooltip("action", ID, CommonIcons.Torch, Colors.action),
            new TTGlossary("status.heat", $"<c=boldPink>{s.ship.heatTrigger}</c>")
        ];
    }
    
    public override string GetCardSelectText(State s)
    {
        return MainModFile.Loc(["action", ID, "text"]);
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon(CommonIcons.Torch, null, Colors.textMain);
    }
}