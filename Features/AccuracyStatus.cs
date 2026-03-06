using System;
using System.Collections.Generic;
using AutumnMooncat.Spirecore.ExternalAPI;
using AutumnMooncat.Spirecore.Util;
using HarmonyLib;
using JetBrains.Annotations;
using Nickel;

namespace AutumnMooncat.Spirecore.Features;

public class AccuracyStatus : IRStatus, IKokoroApi.IV2.IStatusLogicApi.IHook, IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public static string ID => nameof(AccuracyStatus);
    public static IStatusEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("accuracy"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("f55050"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
        var _ = new AccuracyStatus();
        MainModFile.Instance.KokoroApi.V2.StatusLogic.RegisterHook(_);
        MainModFile.Instance.KokoroApi.V2.StatusRendering.RegisterHook(_);
    }
    
    public IReadOnlyList<Tooltip> OverrideStatusTooltips(IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusTooltipsArgs args)
    {
        if (args.Status != Entry.Status)
        {
            return args.Tooltips;
        }
        
        List<Tooltip> ret = [];
        ret.AddRange(args.Tooltips);
        ret.Add(new GlossaryTooltip(PierceChargeStatus.ID)
        {
            Icon = PierceChargeStatus.Entry.Configuration.Definition.icon,
            TitleColor = Colors.status,
            Title = MainModFile.Loc(["status", PierceChargeStatus.ID, "name"]),
            Description = MainModFile.Loc(["status", PierceChargeStatus.ID, "description2"]),
        });
        ret.Add(new TTGlossary("action.attackPiercing"));
        return ret;
    }

    public bool HandleStatusTurnAutoStep(IKokoroApi.IV2.IStatusLogicApi.IHook.IHandleStatusTurnAutoStepArgs args)
    {
        if (args.Timing != IKokoroApi.IV2.IStatusLogicApi.StatusTurnTriggerTiming.TurnStart)
        {
            return false;
        }

        if (args.Status != Entry.Status)
        {
            return false;
        }

        if (args.Amount > 0)
        {
            args.Combat.QueueImmediate(new AStatus
            {
                status = PierceChargeStatus.Entry.Status,
                statusAmount = args.Amount,
                targetPlayer = true,
                timer = 0
            });
        }
        return false;
    }
}