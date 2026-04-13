using System;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Util;
using HarmonyLib;

namespace AutumnMooncat.SpireCore.Patches;

[HarmonyPatch]
public static class RenderActionPatches
{
    public static string ExtraPayloadKey => "ExtraPayload";
    public static Card currCard;
    
    [HarmonyPatch(typeof(Card), nameof(Card.MakeAllActionIcons))]
    public static class CurrCardPatch
    {
        public static void Prefix(Card __instance)
        {
            currCard = __instance;
        }

        public static void Finalizer(Card __instance)
        {
            currCard = null;
        }
    }
    
    [HarmonyPatch(typeof(Card), nameof(Card.RenderAction))]
    public static class MultiIconPatch
    {
        private const int IconPadding = 2;
        private const int IconNumberPadding = 2;
        private const int NumberWidth = 6;
        private const int XPadding = 8;

        private static bool isFirst;
    
        [HarmonyPostfix]
        public static void Plz(ref int __result, G g, State state, CardAction action, bool dontDraw, int shardAvailable, int stunChargeAvailable, int bubbleJuiceAvailable)
        {
            if (action is IMultiIconAction mia)
            {
                if (__result == 0)
                {
                    isFirst = true;
                }
                foreach (var rpl in mia.GetExtraIcons(state))
                {
                    IconAndOrNumber(g, ref __result, rpl, dontDraw, action.disabled);
                }
            }

            if (action.GetExtraIcons(out var extraIcons))
            {
                foreach (var erpl in extraIcons)
                {
                    IconAndOrNumber(g, ref __result, erpl, dontDraw, action.disabled);
                }
            }
        }
    
        private static void IconAndOrNumber(G g, ref int width, Records.RenderPayload rpl, bool dontDraw, bool disabled)
        {
            Color iconColor = disabled ? Colors.disabledIconTint : new Color("ffffff");
            Color textColor = disabled ? Colors.disabledText : rpl.color ?? Colors.textMain;
        
            width += rpl.dx;
            if (rpl.spr.HasValue)
            {
                if (!isFirst)
                {
                    width += IconPadding;
                }
                if (!dontDraw)
                {
                    Vec xy = g.Push(rect: new Rect(width)).rect.xy;
                    Draw.Sprite(rpl.spr, xy.x, xy.y + rpl.dy, flipX: rpl.flipX, flipY: rpl.flipY, color: iconColor);
                    g.Pop();
                }
            }
            width += rpl.width;
            if (rpl.amount.HasValue && !rpl.xHint.HasValue)
            {
                width += IconNumberPadding;
                string str = DB.IntStringCache(rpl.amount.Value);
                if (!dontDraw)
                {
                    Vec xy = g.Push(rect: new Rect(width)).rect.xy;
                    BigNumbers.Render(rpl.amount.Value, xy.x, xy.y, textColor);
                    g.Pop();
                }
                width += str.Length * NumberWidth;
            }
            if (rpl.xHint.HasValue)
            {
                int xVal = rpl.xHint.Value;
                if (xVal < 0)
                {
                    width += IconNumberPadding;
                    if (!dontDraw)
                    {
                        Vec xy = g.Push(rect: new Rect(width - 2)).rect.xy;
                        Spr id = Enum.Parse<Spr>("icons_minus");
                        //Color color = disabled ? Colors.disabledIconTint : rpl.color ?? Colors.textMain;
                        Draw.Sprite(id, xy.x, xy.y - 1.0, color: iconColor);
                        g.Pop();
                    }
                    width += 3;
                }
                if (Math.Abs(xVal) > 1)
                {
                    width += IconNumberPadding + 1;
                    if (!dontDraw)
                    {
                        Vec xy = g.Push(rect : new Rect(width)).rect.xy;
                        //Color color = disabled ? Colors.disabledText : rpl.color ?? Colors.textMain; // maybe
                        BigNumbers.Render(xVal, xy.x, xy.y, textColor);
                        g.Pop();
                    }
                    width += 4;
                }
                width += IconNumberPadding;
                if (!dontDraw)
                {
                    Vec xy = g.Push(rect : new Rect(width)).rect.xy;
                    Spr? id = Enum.Parse<Spr>("icons_x_white");
                    Draw.Sprite(id, xy.x, xy.y - 1.0, color: textColor);
                    g.Pop();
                }
                width += XPadding;
            }

            isFirst = false;
        }
    }
}