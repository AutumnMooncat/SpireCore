using System;
using System.Collections.Generic;
using System.Linq;
using AutumnMooncat.Spirecore.Util;

namespace AutumnMooncat.Spirecore.Actions;

public class AMultiCardSelect : CardAction, IMultiIconAction, ITooltipHelper
{
    public static string ID => nameof(AMultiCardSelect);

    public CardBrowse.Source browseSource;
    public int count;
    public bool allowCancel;
    public bool anyNumber;
    public string selectText;
    public Predicate<Card> filter = _ => true;
    public Action<List<Card>> callback = _ => { };
    public List<Tooltip> tips;
    public List<Records.RenderPayload> extraIcons;
    public List<Card> customSelection;

    public List<Card> GetCards(State s, Combat c)
    {
        if (customSelection != null)
        {
            return customSelection;
        }
        return browseSource switch
        {
            CardBrowse.Source.DrawPile => s.deck,
            CardBrowse.Source.DiscardPile => c.discard,
            CardBrowse.Source.ExhaustPile => c.exhausted,
            CardBrowse.Source.Hand => c.hand,
            CardBrowse.Source.DrawOrDiscardPile => s.deck.ToArray().Concat(c.discard).ToList(),
            _ => []
        };
    }

    public override Route BeginWithRoute(G g, State s, Combat c)
    {
        var validCards = GetCards(s, c).Where(card => filter(card)).ToList();
        if (!allowCancel && !anyNumber && count >= validCards.Count)
        {
            callback(validCards);
            return null;
        }

        if (count >= validCards.Count)
        {
            count = validCards.Count;
        }
        
        var cb = new CardBrowse()
        {
            mode = CardBrowse.Mode.Browse,
            browseSource = browseSource,
            browseAction = new DelegateAction()
            {
                begin = (_, _, _, thiz) =>
                {
                    var test = MainModFile.Instance.KokoroApi.V2.MultiCardBrowse.GetSelectedCards(thiz) ?? [];
                    callback(test.ToList());
                    return null;
                },
                selectText = selectText,
            }
        };
        var multi = MainModFile.Instance.KokoroApi.V2.MultiCardBrowse.MakeRoute(cb).SetMaxSelected(count)
            .SetMinSelected(anyNumber ? 0 : count).SetCardsOverride(validCards);
        return multi.AsRoute;
    }
    
    public override List<Tooltip> GetTooltips(State s)
    {
        var extra = CommonIcons.SourceSpr(browseSource);
        Records.TexturePayload[] pl = extra.HasValue
            ? [new() { spr = CommonIcons.Search }, new() { spr = extra.Value, x = 10 }]
            : [new() { spr = CommonIcons.Search }];
        List<Tooltip> ret = 
        [
            ITooltipHelper.MakeMultiTooltip("action", ID, pl, extra.HasValue ? 10 : 5, Colors.action, browseSource.ToString(), new {Cards = count}),
        ];
        ret.AddRange(tips);
        return ret;
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon(CommonIcons.Search, null, Colors.textMain);
    }

    public List<Records.RenderPayload> GetExtraIcons(State s)
    {
        List<Records.RenderPayload> ret = [];
        var spr = CommonIcons.SourceSpr(browseSource);
        if (spr.HasValue)
        {
            ret.Add(new Records.RenderPayload(){spr = spr.Value, amount = count});
        }
        ret.AddRange(extraIcons);
        return ret;
    }
}