using System.Collections.Generic;
using System.Linq;
using AutumnMooncat.SpireCore.Util;
using Newtonsoft.Json;

namespace AutumnMooncat.SpireCore.Actions;

public class ACardContext : CardAction, IMultiIconAction, ITooltipHelper, IFlippableAction, IHilightingAction
{
    public static string ID => nameof(ACardContext);
    
    public enum Context
    {
        TopDraw,
        TopDiscard,
        TopExhaust,
        LeftHand,
        RightHand,
        RandomHand,
        RandomDraw,
        RandomDiscard,
        RandomExhaust
    }

    public Context context = Context.RandomHand;
    public CardAction followup;
    public Card thatIsnt;
    public bool flipped;
    
    [JsonIgnore]
    public Spr? ContextSpr => context switch
    {
        Context.TopDraw => CommonIcons.TopCard,
        Context.TopDiscard => CommonIcons.TopDiscard,
        Context.TopExhaust => CommonIcons.TopExhaust,
        Context.LeftHand when !flipped => CommonIcons.LeftCard,
        Context.LeftHand when flipped => CommonIcons.RightCard,
        Context.RightHand when !flipped => CommonIcons.RightCard,
        Context.RightHand when flipped => CommonIcons.LeftCard,
        Context.RandomHand => CommonIcons.Hand,
        _ => CommonIcons.Question
    };

    [JsonIgnore]
    public bool IsRandom => context is Context.RandomHand or Context.RandomDraw or Context.RandomDiscard or Context.RandomExhaust;
    
    public Card GetCard(State s, Combat c) => context switch
    {
        Context.TopDraw when s.deck.Count == 0 && c.discard.Count > 0 => ShuffleRecheck(s, c),
        Context.TopDraw when s.deck.Count > 1 && s.deck[^1] == thatIsnt => s.deck[^2],
        Context.TopDraw when s.deck.Count > 0 && s.deck[^1] != thatIsnt => s.deck[^1],
        Context.TopDiscard when c.discard.Count > 1 && c.discard[^1] == thatIsnt => c.discard[^2],
        Context.TopDiscard when c.discard.Count > 0 && c.discard[^1] != thatIsnt => c.discard[^1],
        Context.LeftHand when flipped => FlipRecheck(s, c),
        Context.LeftHand when c.hand.Count > 1 && c.hand[0] == thatIsnt => c.hand[1],
        Context.LeftHand when c.hand.Count > 0 && c.hand[0] != thatIsnt => c.hand[0],
        Context.RightHand when flipped => FlipRecheck(s, c),
        Context.RightHand when c.hand.Count > 1 && c.hand[^1] == thatIsnt => c.hand[^2],
        Context.RightHand when c.hand.Count > 0 && c.hand[^1] != thatIsnt => c.hand[^1],
        Context.RandomHand => c.hand.Where(card => card != thatIsnt).Shuffle(s.rngActions).FirstOrDefault(),
        Context.RandomDraw => s.deck.Where(card => card != thatIsnt).Shuffle(s.rngActions).FirstOrDefault(),
        Context.RandomDiscard => c.discard.Where(card => card != thatIsnt).Shuffle(s.rngActions).FirstOrDefault(),
        Context.RandomExhaust => c.exhausted.Where(card => card != thatIsnt).Shuffle(s.rngActions).FirstOrDefault(),
        _ => null
    };
    
    public Card ShuffleRecheck(State s, Combat c)
    {
        foreach (Card card in c.discard)
        {
            s.SendCardToDeck(card, true);
        }
        c.discard.Clear();
        s.ShuffleDeck(true);
        
        return GetCard(s, c);
    }

    public Card FlipRecheck(State s, Combat c)
    {
        if (flipped && context is Context.LeftHand or Context.RightHand)
        {
            context = context is Context.LeftHand ? Context.RightHand : Context.LeftHand;
            flipped = false;
            var ret = GetCard(s, c);
            flipped = true;
            context = context is Context.LeftHand ? Context.RightHand : Context.LeftHand;
            return ret;
        }

        return GetCard(s, c);
    }

    public override void Begin(G g, State s, Combat c)
    {
        var card = GetCard(s, c);
        if (card != null)
        {
            followup.selectedCard = card;
            c.QueueImmediate(followup);
        }
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        Context ctx = context;
        if (flipped && ctx is Context.LeftHand or Context.RightHand)
        {
            ctx = ctx is Context.LeftHand ? Context.RightHand : Context.LeftHand;
        } 
        
        List<Tooltip> ret = 
        [
            ITooltipHelper.MakeTooltip("action", ID, ContextSpr, Colors.action, ctx.ToString()),
        ];
        
        if (s.route is Combat c)
        {
            if (ctx is Context.TopDiscard && c.discard.Count > 0)
            {
                var top = c.discard[^1];
                ret.Add(new TTCard()
                {
                    card = Mutil.DeepCopy(top),
                    showCardTraitTooltips = false
                });
            }
            if (ctx is Context.TopExhaust && c.exhausted.Count > 0)
            {
                var top = c.exhausted[^1];
                ret.Add(new TTCard()
                {
                    card = Mutil.DeepCopy(top),
                    showCardTraitTooltips = false
                });
            }
        }
        
        ret.AddRange(followup.GetTooltips(s));
        return ret;
    }

    public override Icon? GetIcon(State s)
    {
        var spr = ContextSpr;
        if (spr.HasValue)
        {
            return new Icon(spr.Value, null, Colors.textMain); 
        }

        return null;
    }

    public List<Records.RenderPayload> GetExtraIcons(State s)
    {
        List<Records.RenderPayload> ret = [];

        var nested = followup.GetIcon(s);
        if (nested.HasValue)
        {
            ret.Add(new (){spr = nested.Value.path, amount = nested.Value.number, color = nested.Value.color, flipY = nested.Value.flipY});
        }

        if (followup is IMultiIconAction mia)
        {
            ret.AddRange(mia.GetExtraIcons(s));
        }
        return ret;
    }

    public bool CanFlip(State s)
    {
        return context is Context.LeftHand or Context.RightHand;
    }

    public void HilightOtherCards(Card owner, State s, Combat c)
    {
        if (context is Context.LeftHand or Context.RightHand)
        {
            Card card = GetCard(s, c);
            if (card != null)
            {
                c.hilightedCards.Add(card.uuid);
            }
        }
    }
}