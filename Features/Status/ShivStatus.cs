using System.Linq;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.ExternalAPI;
using AutumnMooncat.SpireCore.Util;
using Nickel;

namespace AutumnMooncat.SpireCore.Features;

public class ShivStatus : IRStatus, IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public static string ID => nameof(ShivStatus);
    public static IStatusEntry Entry { get; set; }
    
    public static GlossaryTooltip GetTooltip => new (ID)
    {
        Icon = Entry.Configuration.Definition.icon,
        TitleColor = Colors.status,
        Title = MainModFile.Loc(["status", ID, "name"]),
        Description = MainModFile.Loc(["status", ID, "description"]),
    };
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("shivIcon"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("dddddd"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
        //var _ = new ShivStatus();
        //MainModFile.Instance.KokoroApi.V2.StatusRendering.RegisterHook(_);
        helper.Events.RegisterBeforeArtifactsHook(nameof(Artifact.OnPlayerAttack), (State state, Combat combat) =>
        {
            var amt = state.ship.Get(Entry.Status);
            if (amt <= 0) return;
            
            //&& !combat.stuff.ContainsKey(i + state.ship.x)
            var bays = state.ship.parts.Select((p, i) => p is { type: PType.missiles, active: true } ? i : -1).Where(i => i >= 0).ToList();
            if (bays.Count <= 0) return;
            
            state.ship.Set(Entry.Status, amt - 1);
            combat.QueueImmediate(bays.ConvertAll(x => new ALaunchShiv()
            {
                fromX = x + state.ship.x
            }));
        });
    }
    
    /*public IReadOnlyList<Tooltip> OverrideStatusTooltips(IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusTooltipsArgs args)
    {
        if (args.Status != Entry.Status)
        {
            return args.Tooltips;
        }
        return [
            ..args.Tooltips, 
            ITooltipHelper.MakeTooltip("stuff", ShivObject.ID, ShivObject.icon, Colors.drone, null,
            new { ShivObject.Damage })
        ];
    }*/
}