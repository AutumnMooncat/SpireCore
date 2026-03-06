using System.Linq;
using AutumnMooncat.Spirecore.ExternalAPI;
using AutumnMooncat.Spirecore.Util;
using Nickel;

namespace AutumnMooncat.Spirecore.Features;

public class MasterRealityStatus : IRStatus, IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public static string ID => nameof(MasterRealityStatus);
    public static IStatusEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("masterReality"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("59cbff"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
        var _ = new MasterRealityStatus();
        MainModFile.Instance.KokoroApi.V2.StatusRendering.RegisterHook(_);
        MainModFile.GetHelper().Events.RegisterBeforeArtifactsHook(nameof(Artifact.OnPlayerRecieveCardMidCombat), (State state, Combat combat, Card card) =>
        {
            if (state.ship.Get(Entry.Status) > 0 && card.upgrade == Upgrade.None && card.GetMeta().upgradesTo.Contains(Upgrade.A))
            {
                card.upgrade = Upgrade.A;
            }
        });
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
}