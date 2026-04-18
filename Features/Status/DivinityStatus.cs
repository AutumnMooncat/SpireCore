using AutumnMooncat.SpireCore.ExternalAPI;
using AutumnMooncat.SpireCore.Util;
using Nickel;

namespace AutumnMooncat.SpireCore.Features;

public class DivinityStatus : IRStatus, IKokoroApi.IV2.IStatusLogicApi.IHook, IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public static string ID => nameof(DivinityStatus);
    public static IStatusEntry Entry { get; set; }
    public static string RemoveMeKey => "DivinityTempCannon";
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("omega2"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("f88dfb"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
        var _ = new DivinityStatus();
        MainModFile.Instance.KokoroApi.V2.StatusLogic.RegisterHook(_, 10);
        MainModFile.Instance.KokoroApi.V2.StatusRendering.RegisterHook(_);
        MainModFile.GetHelper().Events.RegisterBeforeArtifactsHook(nameof(Artifact.OnCombatEnd), (State state) =>
        {
            CleanUp(state);
        });
    }

    public static void CleanUp(State state)
    {
        state.ship.parts.RemoveAll(p =>
        {
            if (MainModFile.GetHelper().ModData.TryGetModData(p, RemoveMeKey, out bool data))
            {
                return data;
            } 
                
            return false;
        });
    }
    
    /*public IKokoroApi.IV2.IStatusRenderingApi.IStatusInfoRenderer OverrideStatusInfoRenderer(
        IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusInfoRendererArgs args)
    {
        if (args.Status == Entry.Status)
        {
            return MainModFile.Instance.KokoroApi.V2.StatusRendering.EmptyStatusInfoRenderer;
        }
        return null;
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
        
        CleanUp(args.State);
        args.Amount = 0;
        args.SetStrategy = IKokoroApi.IV2.IStatusLogicApi.StatusTurnAutoStepSetStrategy.Direct;
        return false;
    }
    
    /*public bool CanHandleImmediateStatusTrigger(IKokoroApi.IV2.IStatusLogicApi.IHook.ICanHandleImmediateStatusTriggerArgs args)
    {
        return true;
    }

    public void HandleImmediateStatusTrigger(IKokoroApi.IV2.IStatusLogicApi.IHook.IHandleImmediateStatusTriggerArgs args)
    {
        MainModFile.Log("Got instant trigger {}", args.Status);
        var cannon = new Part()
        {
            type = PType.cannon,
            skin = "cannon_artemis",
            key = "cannon",
            damageModifier = PDamMod.armor
        };
        MainModFile.GetHelper().ModData.SetModData(cannon, "transient", true);
        args.Ship.parts.Insert(args.Ship.parts.Count/2, cannon);
    }*/
}