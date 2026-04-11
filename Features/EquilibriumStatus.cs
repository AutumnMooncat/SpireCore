using AutumnMooncat.Spirecore.ExternalAPI;
using AutumnMooncat.SpireCore.ExternalAPI.Kokoro;
using AutumnMooncat.Spirecore.Util;
using Nickel;

namespace AutumnMooncat.Spirecore.Features;

public class EquilibriumStatus : IRStatus, IKokoroApi.IV2.IStatusLogicApi.IHook
{
    public static string ID => nameof(EquilibriumStatus);
    public static IStatusEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("equilibrium"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("89ecec"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
        var _ = new EquilibriumStatus();
        MainModFile.Instance.KokoroApi.V2.StatusLogic.RegisterHook(_);
    }

    public bool HandleStatusTurnAutoStep(IKokoroApi.IV2.IStatusLogicApi.IHook.IHandleStatusTurnAutoStepArgs args)
    {
        if (args.Timing != IKokoroApi.IV2.IStatusLogicApi.StatusTurnTriggerTiming.TurnEnd)
        {
            return false;
        }

        if (args.Status != Entry.Status)
        {
            return false;
        }
        
        args.Amount--;
        return false;
    }
}