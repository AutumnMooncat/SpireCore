using System;
using System.Linq;

namespace AutumnMooncat.SpireCore.Actions;

public class AAddRetainToRandomCard : AAddRetainToCard
{
    public override void Begin(G g, State s, Combat c)
    {
        for (int i = 0; i < howManyCards; i++)
        {
            Card card = c.hand.Where(card2 => !chooseNextIfItAlreadyRetains || !card2.GetDataWithOverrides(s).retain).Shuffle(s.rngActions).FirstOrDefault();
            if (card != null)
            {
                MainModFile.Instance.Helper.Content.Cards.SetCardTraitOverride(s, card, MainModFile.Instance.Helper.Content.Cards.RetainCardTrait, true, permanent);
                /*card.retainOverride = true;
                if (permanent)
                {
                    card.recycleOverrideIsPermanent = true;
                }*/
            }
        }
    }

    public override Icon? GetIcon(State s)
    {
        if (Enum.TryParse<Spr>("icons_applyRetain", out var spr))
        {
            return new Icon(spr, howManyCards, Colors.textMain);
        }
        return base.GetIcon(s);
    }
}