using System;
using System.Collections.Generic;
using System.Linq;
using AutumnMooncat.SpireCore.Features;
using AutumnMooncat.SpireCore.Patches;
using Microsoft.Xna.Framework.Graphics;
using Nickel;

namespace AutumnMooncat.SpireCore;

public static class SpirecoreExtensions
{
    public static T EditThis<T>(this T thing, Action<T> edit)
    {
        edit(thing);
        return thing;
    }

    public static void SetData<T>(this object o, string key, T data)
    {
        MainModFile.SetData(o, key, data);
    }
    
    public static bool GetData<T>(this object o, string key, out T data)
    {
        return MainModFile.GetData(o, key, out data);
    }

    public static T GetOrMakeData<T>(this object o, string key, T def = default)
    {
        if (o.GetData(key, out T res))
        {
            return res;
        }
        o.SetData(key, def);
        return def;
    }

    public static T WithData<T, D>(this T o, string key, D data)
    {
        o.SetData(key, data);
        return o;
    }
    
    public static T WithoutData<T>(this T o, string key)
    {
        o.RemoveData(key);
        return o;
    }

    public static void RemoveData(this object o, string key)
    {
        MainModFile.RemoveData(o, key);
    }

    public static bool TryAdd<T>(this T thiz, T that, out T result)
    {
        dynamic l = thiz;
        dynamic r = that;
        
        try
        {
            result = l + r;
            return true;
        }
        catch
        {
            // ignored
        }

        result = default;
        return false;
    }

    public static V GetOrAddValue<K, V>(this Dictionary<K, V> dict, K key, V value)
    {
        if (dict.TryGetValue(key, out var found))
        {
            return found;
        }

        dict[key] = value;
        return value;
    }

    public static string PrefixID(this string s)
    {
        return MainModFile.MakeID(s);
    } 

    public static Texture2D GetTex(this Spr spr)
    {
        return MainModFile.GetHelper().Content.Sprites.LookupBySpr(spr)?.ObtainTexture();
    }

    public static void SetExtraIcons(this CardAction a, List<Records.RenderPayload> pl)
    {
        a.SetData(RenderActionPatches.ExtraPayloadKey, pl);
    }

    public static bool GetExtraIcons(this CardAction a, out List<Records.RenderPayload> pl)
    {
        return a.GetData(RenderActionPatches.ExtraPayloadKey, out pl);
    }
    
    public static T WithExtraIcons<T>(this T a, List<Records.RenderPayload> pl) where T : CardAction
    {
        a.SetExtraIcons(pl);
        return a;
    }

    public static void AddTooltipFix(this CardAction a, string key, params object[] vals)
    {
        if (a.GetTooltipFixes(out var dict))
        {
            dict[key] = vals;
            return;
        }

        Dictionary<string, object[]> data = [];
        data[key] = vals;
        a.SetData(TooltipFixPatch.TooltipFixKey, data);
    }

    public static bool GetTooltipFixes(this CardAction a, out Dictionary<string, object[]> dict)
    {
        return a.GetData(TooltipFixPatch.TooltipFixKey, out dict);
    }

    public static T WithTooltipFix<T>(this T a, string key, params object[] vals) where T : CardAction
    {
        a.AddTooltipFix(key, vals);
        return a;
    }

    public static void AddExtraTooltips(this CardAction a, params Tooltip[] vals)
    {
        if (a.GetExtraTooltips(out var tips))
        {
            tips.AddRange(vals);
            return;
        }

        List<Tooltip> data = [];
        data.AddRange(vals);
        a.SetData(TooltipFixPatch.TooltipAdditionKey, data);
    }

    public static bool GetExtraTooltips(this CardAction a, out List<Tooltip> tips)
    {
        return a.GetData(TooltipFixPatch.TooltipAdditionKey, out tips);
    }

    public static T WithExtraTooltips<T>(this T a, params Tooltip[] vals) where T : CardAction
    {
        a.AddExtraTooltips(vals);
        return a;
    }
    
    public static T WithDisabled<T>(this T a, bool disabled) where T : CardAction
    {
        a.disabled = disabled;
        return a;
    }

    public static AAttack WithHeatTipFix(this AAttack a)
    {
        a.AddTooltipFix("status.heat", $"<c=boldPink>{(a.targetPlayer ? MG.inst.g.state.ship.heatTrigger : 3)}</c>");
        return a;
    }

    public static AVariableHint WithChargeTipFix(this AVariableHint a, State s, Combat c)
    {
        a.AddTooltipFix("action.xHint.desc", new TooltipFixPatch.Defer(), $" </c>(<c=keyword>{ChargeStatus.EffectiveCharge(s, c, s.ship)}</c>)", "", "");
        return a;
    }

    public static AVariableHint WithEnergyTipFix(this AVariableHint a, Combat c, int cardCost)
    {
        MainModFile.Instance.KokoroHelper.ModData.SetOptionalModData<int>(a, "energyTooltipOverride", c.energy - cardCost);
        return a;
    }
    
    public static IEnumerable<Card> GetAllCards(this State state)
    {
        IEnumerable<Card> results = state.deck;
        if (state.route is Combat combat)
            results = results.Concat(combat.hand).Concat(combat.discard).Concat(combat.exhausted);
        return results;
    }
}