using AutumnMooncat.SpireCore.ExternalAPI;
using AutumnMooncat.SpireCore.ExternalAPI.Kokoro;
using AutumnMooncat.SpireCore.Util;
using Nickel;

namespace AutumnMooncat.SpireCore.Features;

public class FeebledriveStatus : IRStatus, IKokoroApi.IV2.IStatusLogicApi.IHook
{
    public static string ID => nameof(FeebledriveStatus);
    public static IStatusEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("feebledrive2"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("477992"), // 2186b8
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = false
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
        var _ = new FeebledriveStatus();
        MainModFile.Instance.KokoroApi.V2.StatusLogic.RegisterHook(_);
        MainModFile.GetHelper().Events.RegisterAfterArtifactsHook(nameof(Artifact.ModifyBaseDamage), (int baseDamage,
            Card card,
            State state,
            Combat combat,
            bool fromPlayer) =>
        {
            if (fromPlayer)
            {
                return -state.ship.Get(Entry.Status);
            }
            
            return -combat?.otherShip.Get(Entry.Status) ?? 0;
        });
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
        
        args.Amount = 0;
        return true;
    }
}