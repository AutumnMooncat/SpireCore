using System;
using System.Collections.Generic;
using AutumnMooncat.SpireCore.ExternalAPI.Kokoro;
using AutumnMooncat.SpireCore.Util;
using Nickel;

namespace AutumnMooncat.SpireCore.Features;

public class ChargeStatus : IRStatus, IKokoroApi.IV2.IStatusLogicApi.IHook, IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public static string ID => nameof(ChargeStatus);
    public static IStatusEntry Entry { get; set; }
    private static KokoroUtils.CompoundStatusInfoRenderer Renderer { get; set; }
    private static Color ActiveColor { get; set; }
    private static Color InactiveColor { get; set; }
    public static int Max => 5;
    
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
                icon = CommonIcons.Find("charge2"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("f8ff63"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
        var _ = new ChargeStatus();
        //MainModFile.Instance.KokoroApi.V2.StatusLogic.RegisterHook(_);
        MainModFile.Instance.KokoroApi.V2.StatusRendering.RegisterHook(_);

        Renderer = new KokoroUtils.CompoundStatusInfoRenderer(
            MainModFile.Kokoro().StatusRendering.MakeBarStatusInfoRenderer().SetRows(1).SetHorizontalSpacing(1).SetSegmentWidth(2),
            MainModFile.Kokoro().StatusRendering.MakeTextStatusInfoRenderer("").SetColor(Colors.cardtrait),
            args =>
            {
                args.SetAmount(EffectiveCharge(args.State, args.Combat, args.Ship));
                return Max;
            });
        ActiveColor = new Color("f9ff7e");
        InactiveColor = new Color("343422");
    }

    public static int EffectiveCharge(State s, Combat c, Ship ship)
    {
        int charge = 0;
        foreach (var pair in c.stuff)
        {
            if (pair.Value is LightningObject or PlasmaObject)
            {
                charge++;
            }
        }
        return charge;
    }

    public IEnumerable<(Status Status, double Priority)> GetExtraStatusesToShow(IKokoroApi.IV2.IStatusRenderingApi.IHook.IGetExtraStatusesToShowArgs args)
    {
        if (EffectiveCharge(args.State, args.Combat, args.Ship) > 0)
        {
            return [(Entry.Status, 0)];
        }

        return [];
    }

    public IKokoroApi.IV2.IStatusRenderingApi.IStatusInfoRenderer OverrideStatusInfoRenderer(
        IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusInfoRendererArgs args)
    {
        if (args.Status == Entry.Status)
        {
            List<Color> clr = [];
            var amount = Math.Min(Max, EffectiveCharge(args.State, args.Combat, args.Ship));
            for (int i = 0; i < amount; i++)
            {
                clr.Add(ActiveColor);
            }

            for (int i = 0; i < Max - amount; i++)
            {
                clr.Add(InactiveColor);
            }

            Renderer.BarPart.SetSegments(clr);
            return Renderer;
        }

        return null;
    }
}