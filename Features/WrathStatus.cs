using System;
using System.Collections.Generic;
using AutumnMooncat.Spirecore.ExternalAPI;
using AutumnMooncat.SpireCore.ExternalAPI.Kokoro;
using AutumnMooncat.Spirecore.Util;
using Nickel;

namespace AutumnMooncat.Spirecore.Features;

public class WrathStatus : IRStatus, IKokoroApi.IV2.IStatusLogicApi.IHook, IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public static string ID => nameof(WrathStatus);
    public static IStatusEntry Entry { get; set; }
    private static IKokoroApi.IV2.IStatusRenderingApi.IBarStatusInfoRenderer BarRender { get; set; }
    private static Color ActiveColor { get; set; }
    private static Color InactiveColor { get; set; }
    public static int MaxWrath => 3;
    
    public static GlossaryTooltip GetTooltip => new (ID)
    {
        Icon = Entry.Configuration.Definition.icon,
        TitleColor = Colors.status,
        Title = MainModFile.Loc(["status", ID, "name"]),
        Description = MainModFile.Loc(["status", ID, "description"], new { Max = MaxWrath }),
    };
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("wrath2"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("ff8040"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"], new {Max = MaxWrath}).Localize
        });
        
        BarRender = MainModFile.Instance.KokoroApi.V2.StatusRendering.MakeBarStatusInfoRenderer().SetRows(1).SetHorizontalSpacing(1).SetSegmentWidth(2);
        ActiveColor = new Color("ff8040");
        InactiveColor = new Color("331a0d");
        
        
        var _ = new WrathStatus();
        MainModFile.Instance.KokoroApi.V2.StatusLogic.RegisterHook(_);
        MainModFile.Instance.KokoroApi.V2.StatusRendering.RegisterHook(_);
        MainModFile.GetHelper().Events.RegisterBeforeArtifactsHook(nameof(Artifact.ModifyBaseDamage), (int baseDamage,
            Card card,
            State state,
            Combat combat,
            bool fromPlayer) => state?.ship.Get(Entry.Status) ?? 0 + combat?.otherShip.Get(Entry.Status) ?? 0
        );
    }
    
    public IKokoroApi.IV2.IStatusRenderingApi.IStatusInfoRenderer OverrideStatusInfoRenderer(
        IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusInfoRendererArgs args)
    {
        if (args.Status == Entry.Status)
        {
            List<Color> clr = [];
            for (int i = 0; i < args.Amount; i++)
            {
                clr.Add(ActiveColor);
            }

            for (int i = 0; i < MaxWrath - args.Amount; i++)
            {
                clr.Add(InactiveColor);
            }

            BarRender.SetSegments(clr);
            return BarRender;
        }

        return null;
    }
    
    public int ModifyStatusChange(IKokoroApi.IV2.IStatusLogicApi.IHook.IModifyStatusChangeArgs args)
    {
        if (args.Status != Entry.Status)
        {
            return args.NewAmount;
        }
        int realNew = Math.Min(MaxWrath, args.NewAmount);
        int delta = realNew - args.OldAmount;
        //MainModFile.Log("Got Wrath, old: {}, new {}({}), delta: {}", args.OldAmount, args.NewAmount, realNew, delta);
        if (delta > 0)
        {
            args.Combat.QueueImmediate(new AStatus(){status = CalmStatus.Entry.Status, statusAmount = 0, mode = AStatusMode.Set, targetPlayer = true});
        }
        return realNew;
    }
}