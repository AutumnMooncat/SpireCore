using System;
using System.Collections.Generic;
using System.Linq;
using AutumnMooncat.SpireCore.ExternalAPI;
using AutumnMooncat.SpireCore.ExternalAPI.Kokoro;
using AutumnMooncat.SpireCore.Util;
using Nickel;

namespace AutumnMooncat.SpireCore.Features;

public class PowerCoreStatus : IRStatus, IKokoroApi.IV2.IStatusLogicApi.IHook, IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public static string ID => nameof(PowerCoreStatus);
    public static IStatusEntry Entry { get; set; }
    
    public static GlossaryTooltip GetTooltip => new (ID)
    {
        Icon = Entry.Configuration.Definition.icon,
        TitleColor = Colors.status,
        Title = MainModFile.Loc(["status", ID, "name"]),
        Description = MainModFile.Loc(["status", ID, "description2"]),
    };
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("powerCore4"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("59c8ff"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
        var _ = new PowerCoreStatus();
        MainModFile.Instance.KokoroApi.V2.StatusLogic.RegisterHook(_);
        MainModFile.Instance.KokoroApi.V2.StatusRendering.RegisterHook(_);
    }
    
    public IReadOnlyList<Tooltip> OverrideStatusTooltips(IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusTooltipsArgs args)
    {
        if (args.Status != Entry.Status)
        {
            return args.Tooltips;
        }
        foreach (var tooltip in args.Tooltips)
        {
            if (tooltip is TTGlossary { vals.Length: > 0 } ttg && ttg.key == "status."+Entry.Status)
            {
                ttg.vals = [..ttg.vals, ttg.vals[0]?.ToString()?.Replace("boldPink", "cardtrait")];
            }
        }
        return [..args.Tooltips, ChargeStatus.GetTooltip];
    }
}