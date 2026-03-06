using System;
using System.Collections.Generic;
using System.Linq;
using AutumnMooncat.Spirecore.ExternalAPI;
using AutumnMooncat.Spirecore.Util;
using AutumnMooncat.SpireCore.Util;
using HarmonyLib;
using JetBrains.Annotations;
using Nickel;

namespace AutumnMooncat.Spirecore.Features;

public class HeatsinksStatus : IRStatus, IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public static string ID => nameof(HeatsinksStatus);
    public static IStatusEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("heatsinks"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("acbadc"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
        var _ = new HeatsinksStatus();
        MainModFile.Instance.KokoroApi.V2.StatusRendering.RegisterHook(_);
        helper.Events.RegisterBeforeArtifactsHook("OnPlayerPlayCard",
            (int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount) =>
            {
                if (state.ship.Get(Entry.Status) == 0 || !Wiz.GivesStatus(card, state, combat) || !card.GetDataWithOverrides(state).exhaust)
                {
                    return;
                }

                combat.QueueImmediate(new ADrawCard()
                {
                    count = state.ship.Get(Entry.Status)
                });
            });
    }
    
    public IReadOnlyList<Tooltip> OverrideStatusTooltips(IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusTooltipsArgs args)
    {
        if (args.Status != Entry.Status)
        {
            return args.Tooltips;
        }
        
        List<Tooltip> ret = [];
        ret.AddRange(args.Tooltips);
        ret.Add(new TTGlossary("cardtrait.exhaust"));
        return ret;
    }
}