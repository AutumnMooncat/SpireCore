using System.Collections.Generic;
using AutumnMooncat.SpireCore.ExternalAPI;
using AutumnMooncat.SpireCore.Patches;
using AutumnMooncat.SpireCore.Util;
using Nickel;

namespace AutumnMooncat.SpireCore.Features;

public class PierceChargeStatus : IRStatus, IKokoroApi.IV2.IStatusLogicApi.IHook, IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public static string ID => nameof(PierceChargeStatus);
    public static IStatusEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("pierceCharge"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("ff3838"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
        var _ = new PierceChargeStatus();
        MainModFile.Instance.KokoroApi.V2.StatusLogic.RegisterHook(_, 10);
        MainModFile.Instance.KokoroApi.V2.StatusRendering.RegisterHook(_);
        helper.Events.RegisterAfterArtifactsHook(nameof(Artifact.OnPlayerAttackMakeItPierce),
            (State state, Combat combat) =>
            {
                bool? ret = false;
                var amt = state.ship.Get(Entry.Status);
                if (amt > 0)
                {
                    if (AttackPatches.currAttack != null && !AttackPatches.currAttack.piercing)
                    {
                        state.ship.Set(Entry.Status, amt - 1);
                    }
                    ret = true;
                }

                return ret;
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
        
        args.Amount = 0;
        args.SetStrategy = IKokoroApi.IV2.IStatusLogicApi.StatusTurnAutoStepSetStrategy.Direct;
        return false;
    }
}