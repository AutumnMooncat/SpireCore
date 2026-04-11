using AutumnMooncat.Spirecore.ExternalAPI;
using AutumnMooncat.SpireCore.ExternalAPI.Kokoro;
using AutumnMooncat.Spirecore.Util;
using Nickel;

namespace AutumnMooncat.Spirecore.Features;

public class HeatCapStatus : IRStatus, IKokoroApi.IV2.IStatusLogicApi.IHook
{
    public static string ID => nameof(HeatCapStatus);
    public static IStatusEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("heatCap"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("ff842e"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
        MainModFile.Instance.KokoroApi.V2.StatusLogic.RegisterHook(new HeatCapStatus());
        MainModFile.GetHelper().Events.RegisterBeforeArtifactsHook(nameof(Artifact.OnCombatEnd), (State state) =>
        {
            state.ship.heatTrigger -= state.ship.Get(Entry.Status);
        });
    }

    public int ModifyStatusChange(IKokoroApi.IV2.IStatusLogicApi.IHook.IModifyStatusChangeArgs args)
    {
        if (args.Status == Entry.Status)
        {
            int delta = args.NewAmount - args.OldAmount;
            args.Ship.heatTrigger += delta;
        }
        return args.NewAmount;
    }
}