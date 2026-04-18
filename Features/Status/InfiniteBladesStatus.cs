using System.Collections.Generic;
using AutumnMooncat.SpireCore.ExternalAPI;
using AutumnMooncat.SpireCore.Util;
using Nickel;

namespace AutumnMooncat.SpireCore.Features;

public class InfiniteBladesStatus : IRStatus, IKokoroApi.IV2.IStatusLogicApi.IHook, IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public static string ID => nameof(InfiniteBladesStatus);
    public static IStatusEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("infiniteBlades"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("af97a5"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
        var _ = new InfiniteBladesStatus();
        MainModFile.Instance.KokoroApi.V2.StatusLogic.RegisterHook(_);
        MainModFile.Instance.KokoroApi.V2.StatusRendering.RegisterHook(_);
    }

    public IReadOnlyList<Tooltip> OverrideStatusTooltips(IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusTooltipsArgs args)
    {
        if (args.Status != Entry.Status)
        {
            return args.Tooltips;
        }
        
        /*List<Tooltip> ret = [];
        ret.AddRange(args.Tooltips);
        ret.Add(new TTCard()
        {
            card = new Shiv(),
            showCardTraitTooltips = true
        });
        return ret;*/
        return [..args.Tooltips, ShivStatus.GetTooltip];
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
            args.Combat.QueueImmediate(new AStatus()
            {
                status = ShivStatus.Entry.Status,
                statusAmount = args.Amount,
                targetPlayer = true
            });
            /*args.Combat.QueueImmediate(new ACascadingAddCard()
            {
                amount = args.Amount,
                card = new Shiv(),
                destination = CardDestination.Hand
            });*/
        }
        return false;
    }
}