using AutumnMooncat.SpireCore.ExternalAPI;
using AutumnMooncat.SpireCore.Util;
using Nickel;

namespace AutumnMooncat.SpireCore.Features;

public class DevaFormStatus : IRStatus, IKokoroApi.IV2.IStatusLogicApi.IHook/*, IKokoroApi.IV2.IStatusRenderingApi.IHook*/
{
    public static string ID => nameof(DevaFormStatus);
    public static IStatusEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("devaForm"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("eab2e7"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
        var _ = new DevaFormStatus();
        MainModFile.Instance.KokoroApi.V2.StatusLogic.RegisterHook(_);
        //MainModFile.Instance.KokoroApi.V2.StatusRendering.RegisterHook(_);
    }
    
    /*public IReadOnlyList<Tooltip> OverrideStatusTooltips(IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusTooltipsArgs args)
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
    }*/

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
            args.Combat.QueueImmediate(new AEnergy()
            {
                changeAmount = args.Amount
            });
        }
        return false;
    }
}