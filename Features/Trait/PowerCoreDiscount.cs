using System;
using System.Collections.Generic;
using AutumnMooncat.SpireCore.Util;
using Nickel;

namespace AutumnMooncat.SpireCore.Features;

public class PowerCoreDiscount : IRTrait, ITooltipHelper
{
    public static string ID => nameof(PowerCoreDiscount);
    public static ICardTraitEntry Entry { get; set; }
    public static Dictionary<int, Spr> Icons { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Icons = [];
        Icons[0] = CommonIcons.Find("powerCore4");
        Entry = helper.Content.Cards.RegisterTrait(ID, new CardTraitConfiguration
        {
            Icon = (state, card) =>
            {
                if (card?.GetData(ID, out int amt) ?? false)
                {
                    return Icons[Math.Clamp(amt, 0, 10)];
                }
                return Icons[0];
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["trait", ID, "title"]).Localize,
            Tooltips = (state, card) =>
            {
                if (card?.GetData(ID, out int amt) ?? false)
                {
                    return
                    [
                        ITooltipHelper.MakeTooltip("trait", ID, Icons[0], Colors.cardtrait, null, new { Amount = amt })
                    ];
                }
                return
                [
                    ITooltipHelper.MakeTooltip("trait", ID, Icons[0], Colors.cardtrait, "Alt")
                ];
            }
        });

        TextureProcessor.Jobs += () =>
        {
            var tex = Icons[0].GetTex();
            for (int i = 1; i <= 10; i++)
            {
                var amount = i;
                Icons[i] = TextureProcessor.MakeSprite(tex.Width, tex.Height, vec =>
                {
                    Draw.Sprite(tex, 0, 0);
                    var text = amount > 9 ? "+" : amount.ToString();
                    var textRect = Draw.Text(text, 0, 0, outline: Colors.black, dontDraw: true, dontSubstituteLocFont: true);
                    Draw.Text(text, vec.x - textRect.w, vec.y - textRect.h - 1, color: Colors.white, outline: Colors.black, dontSubstituteLocFont: true);
                }).Sprite;
            }
        };
    }
}