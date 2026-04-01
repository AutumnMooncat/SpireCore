using System.Collections.Generic;
using System.Linq;
using AutumnMooncat.Spirecore.Patches;

namespace AutumnMooncat.Spirecore;

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
    
    public static IEnumerable<Card> GetAllCards(this State state)
    {
        IEnumerable<Card> results = state.deck;
        if (state.route is Combat combat)
            results = results.Concat(combat.hand).Concat(combat.discard).Concat(combat.exhausted);
        return results;
    }
}