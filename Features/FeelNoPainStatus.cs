using AutumnMooncat.SpireCore.ExternalAPI;
using AutumnMooncat.SpireCore.Util;
using Nickel;

namespace AutumnMooncat.SpireCore.Features;

public class FeelNoPainStatus : IRStatus
{
    public static string ID => nameof(FeelNoPainStatus);
    public static IStatusEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("feelNoPain"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("bf6060"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
    }

    public static void OnExhaust(State s, Combat c, Card card)
    {
        if (c.otherShip.Get(Entry.Status) > 0)
        {
            c.QueueImmediate(new AStatus
            {
                status = Status.shield,
                statusAmount = c.otherShip.Get(Entry.Status),
                targetPlayer = false
            });
        }
        
        if (s.ship.Get(Entry.Status) > 0)
        {
            c.QueueImmediate(new AStatus
            {
                status = Status.shield,
                statusAmount = s.ship.Get(Entry.Status),
                targetPlayer = true
            });
        }
    }
}