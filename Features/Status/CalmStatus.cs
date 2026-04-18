using System;
using System.Collections.Generic;
using AutumnMooncat.SpireCore.Artifacts.Watcher;
using AutumnMooncat.SpireCore.ExternalAPI;
using AutumnMooncat.SpireCore.Util;
using Nickel;

namespace AutumnMooncat.SpireCore.Features;

public class CalmStatus : IRStatus, IKokoroApi.IV2.IStatusLogicApi.IHook, IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public static string ID => nameof(CalmStatus);
    public static IStatusEntry Entry { get; set; }
    private static IKokoroApi.IV2.IStatusRenderingApi.IBarStatusInfoRenderer BarRender { get; set; }
    private static Color ActiveColor { get; set; }
    private static Color InactiveColor { get; set; }
    public static int MaxCalm => 3;

    public static GlossaryTooltip GetTooltip => new (ID)
    {
        Icon = Entry.Configuration.Definition.icon,
        TitleColor = Colors.status,
        Title = MainModFile.Loc(["status", ID, "name"]),
        Description = MainModFile.Loc(["status", ID, "description"], new { Max = MaxCalm }),
    };
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("calm2"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("40bfff"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"], new {Max = MaxCalm}).Localize
        });
        
        BarRender = MainModFile.Instance.KokoroApi.V2.StatusRendering.MakeBarStatusInfoRenderer().SetRows(1).SetHorizontalSpacing(1).SetSegmentWidth(2);
        ActiveColor = new Color("40bfff");
        InactiveColor = new Color("0d2633");
        
        
        var _ = new CalmStatus();
        MainModFile.Instance.KokoroApi.V2.StatusLogic.RegisterHook(_);
        MainModFile.Instance.KokoroApi.V2.StatusRendering.RegisterHook(_);
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

            for (int i = 0; i < MaxCalm - args.Amount; i++)
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

        int realNew = Math.Min(MaxCalm, args.NewAmount);
        int delta = realNew - args.OldAmount;
        //MainModFile.Log("Got Calm, old: {}, new {}({}), delta: {}", args.OldAmount, args.NewAmount, realNew, delta);
        if (delta < 0)
        {
            int nrg = -delta;
            foreach (var artifact in args.State.EnumerateAllArtifacts())
            {
                if (artifact is VioletLotus vl)
                {
                    nrg = vl.ModifyCalmEnergy(args.State, args.Combat, nrg);
                }
            }
            args.Combat.QueueImmediate(new AEnergy(){changeAmount = nrg});
        } 
        else if (delta > 0)
        {
            args.Combat.QueueImmediate(new AStatus(){status = WrathStatus.Entry.Status, statusAmount = 0, mode = AStatusMode.Set, targetPlayer = true});
        }
        
        return realNew;
    }
}