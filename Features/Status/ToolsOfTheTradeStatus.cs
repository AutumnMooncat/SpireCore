using AutumnMooncat.SpireCore.ExternalAPI;
using AutumnMooncat.SpireCore.Util;
using HarmonyLib;
using Nickel;

namespace AutumnMooncat.SpireCore.Features;

public class ToolsOfTheTradeStatus : IRStatus, IKokoroApi.IV2.IStatusLogicApi.IHook
{
    public static string ID => nameof(ToolsOfTheTradeStatus);
    public static IStatusEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("toolsOfTheTrade"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("e7be32"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
        MainModFile.Instance.KokoroApi.V2.StatusLogic.RegisterHook(new ToolsOfTheTradeStatus());
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
                status = MainModFile.Instance.KokoroApi.V2.RedrawStatus.Status,
                statusAmount = args.Amount,
                targetPlayer = true,
                timer = 0
            });
        }
        return false;
    }
}