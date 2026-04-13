using System.Collections.Generic;
using System.Linq;
using AutumnMooncat.SpireCore.Patches;

namespace AutumnMooncat.SpireCore;

public static class SpirecoreExtensions
{
    public static void SetData<T>(this object o, string key, T data)
    {
        MainModFile.SetData(o, key, data);
    }
    
    public static bool GetData<T>(this object o, string key, out T data)
    {
        return MainModFile.GetData(o, key, out data);
    }

    public static void SetExtraIcons(this CardAction a, List<Records.RenderPayload> pl)
    {
        a.SetData(RenderActionPatches.ExtraPayloadKey, pl);
    }

    public static bool GetExtraIcons(this CardAction a, out List<Records.RenderPayload> pl)
    {
        return a.GetData(RenderActionPatches.ExtraPayloadKey, out pl);
    }
    
    public static CardAction WithExtraIcons(this CardAction a, List<Records.RenderPayload> pl)
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

    public static CardAction WithTooltipFix(this CardAction a, string key, params object[] vals)
    {
        a.AddTooltipFix(key, vals);
        return a;
    }
    
    public static CardAction WithDisabled(this CardAction a, bool disabled)
    {
        a.disabled = disabled;
        return a;
    }

    public static AAttack WithHeatTipFix(this AAttack a)
    {
        a.AddTooltipFix("status.heat", $"<c=boldPink>{(a.targetPlayer ? MG.inst.g.state.ship.heatTrigger : 3)}</c>");
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