using AutumnMooncat.Spirecore.ExternalAPI;
using AutumnMooncat.Spirecore.Util;
using HarmonyLib;
using Nickel;

namespace AutumnMooncat.Spirecore.Features;

public class DrawDownStatus : IRStatus, IKokoroApi.IV2.IStatusLogicApi.IHook
{
    public static string ID => nameof(DrawDownStatus);
    public static IStatusEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("drawDown"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("86333a"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = false
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
    }
}