using System.Collections.Generic;
using AutumnMooncat.SpireCore.Util;

namespace AutumnMooncat.SpireCore.Actions;

public class APlayAndTorchTopCard : CardAction, IMultiIconAction, ITooltipHelper
{
    public static string ID => nameof(APlayAndTorchTopCard);

    public override void Begin(G g, State s, Combat c)
    {
        if (s.deck.Count == 0 && c.discard.Count > 0)
        {
            foreach (Card card in c.discard)
            {
                s.SendCardToDeck(card, true);
            }
            c.discard.Clear();
            s.ShuffleDeck(true);
        }
        if (s.deck.Count > 0)
        {
            Card card = s.deck[^1];
            List<CardAction> backup = [..c.cardActions];
            c.cardActions.Clear();
            s.RemoveCardFromWhereverItIs(card.uuid);
            c.SendCardToHand(s, card);
            c.TryPlayCard(s, card, true, true);
            c.Queue(new AStatus()
            {
                status = Status.heat,
                statusAmount = 1,
                targetPlayer = true
            });
            c.Queue(backup);
        }
        else
        {
            Audio.Play(FSPRO.Event.CardHandling);
        }
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        return [
            ITooltipHelper.MakeTooltip("action", ID, CommonIcons.TopCard, Colors.action),
            ITooltipHelper.MakeTooltip("action", ID, CommonIcons.Bypass, Colors.action, "2"),
            ITooltipHelper.MakeTooltip("action", ID, CommonIcons.Torch, Colors.action, "3")
        ];
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon(CommonIcons.TopCard, null, Colors.textMain);
    }

    public List<Records.RenderPayload> GetExtraIcons(State s)
    {
        return [
            new (){spr = CommonIcons.Bypass, dx = -1, dy = -1},
            new (){spr = CommonIcons.Torch, dx = -1}
        ];
    }
}