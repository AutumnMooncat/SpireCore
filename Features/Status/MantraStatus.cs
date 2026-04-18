using System.Collections.Generic;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.ExternalAPI;
using AutumnMooncat.SpireCore.Util;
using Nickel;

namespace AutumnMooncat.SpireCore.Features;

public class MantraStatus : IRStatus, IKokoroApi.IV2.IStatusLogicApi.IHook, IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public static string ID => nameof(MantraStatus);
    public static IStatusEntry Entry { get; set; }
    public static IPartEntry CannonEntry { get; set; }
    private static IKokoroApi.IV2.IStatusRenderingApi.IBarStatusInfoRenderer BarRender { get; set; }
    private static Color ActiveColor { get; set; }
    private static Color InactiveColor { get; set; }
    public static int MaxMantra => 5;
    
    public static GlossaryTooltip GetTooltip => new (ID)
    {
        Icon = Entry.Configuration.Definition.icon,
        TitleColor = Colors.status,
        Title = MainModFile.Loc(["status", ID, "name"]),
        Description = MainModFile.Loc(["status", ID, "description"], new { Max = MaxMantra }),
    };
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("mantra"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("e660f1"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
        CannonEntry = helper.Content.Ships.RegisterPart(ID+".Cannon", new PartConfiguration()
        {
            Sprite = MainModFile.MakeSprite("assets/ships/cannon_divinity.png").Sprite
        });
        var _ = new MantraStatus();
        MainModFile.Instance.KokoroApi.V2.StatusLogic.RegisterHook(_);
        MainModFile.Instance.KokoroApi.V2.StatusRendering.RegisterHook(_);

        BarRender = MainModFile.Instance.KokoroApi.V2.StatusRendering.MakeBarStatusInfoRenderer().SetRows(1).SetHorizontalSpacing(1).SetSegmentWidth(2);
        ActiveColor = new Color("f88dfb");
        InactiveColor = new Color("331d34");
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

            for (int i = 0; i < MaxMantra - args.Amount; i++)
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

        int triggers = args.NewAmount / MaxMantra;
        if (triggers > 0)
        {
            args.Combat.QueueImmediate([
                new AStatus()
                {
                    status = DivinityStatus.Entry.Status,
                    statusAmount = 1,
                    targetPlayer = true
                },
                new DelegateAction()
                {
                    begin = (g, state, combat, thiz) =>
                    {
                        for (int i = 0; i < triggers; i++)
                        {
                            var cannon = new Part()
                            {
                                type = PType.cannon,
                                skin = CannonEntry.UniqueName,
                                key = "cannon",
                                damageModifier = PDamMod.armor
                            };
                            MainModFile.GetHelper().ModData.SetModData(cannon, DivinityStatus.RemoveMeKey, true);
                            args.Ship.parts.Insert(args.Ship.parts.Count/2, cannon);
                        }
                        return null;
                    }
                }
            ]);
        }
        return args.NewAmount - MaxMantra * triggers;
    }
}