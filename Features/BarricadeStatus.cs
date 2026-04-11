using AutumnMooncat.Spirecore.ExternalAPI;
using AutumnMooncat.SpireCore.ExternalAPI.Kokoro;
using AutumnMooncat.Spirecore.Util;
using Nickel;

namespace AutumnMooncat.Spirecore.Features;

public class BarricadeStatus : IRStatus, IKokoroApi.IV2.IStatusLogicApi.IHook, IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public static string ID => nameof(BarricadeStatus);
    public static IStatusEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("barricade"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("009ebe"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
        var _ = new BarricadeStatus();
        MainModFile.Instance.KokoroApi.V2.StatusLogic.RegisterHook(_);
        MainModFile.Instance.KokoroApi.V2.StatusRendering.RegisterHook(_);
    }
    
    public IKokoroApi.IV2.IStatusRenderingApi.IStatusInfoRenderer OverrideStatusInfoRenderer(
        IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusInfoRendererArgs args)
    {
        if (args.Status == Entry.Status)
        {
            return MainModFile.Instance.KokoroApi.V2.StatusRendering.EmptyStatusInfoRenderer;
        }

        return null;
    }

    public bool HandleStatusTurnAutoStep(IKokoroApi.IV2.IStatusLogicApi.IHook.IHandleStatusTurnAutoStepArgs args)
    {
        if (args.Timing != IKokoroApi.IV2.IStatusLogicApi.StatusTurnTriggerTiming.TurnStart)
        {
            return false;
        }

        if (args.Status != Status.tempShield)
        {
            return false;
        }

        if (args.Ship.Get(Entry.Status) == 0)
        {
            return false;
        }
        
        args.Amount = args.Ship.Get(Status.tempShield);
        return false;
    }
}