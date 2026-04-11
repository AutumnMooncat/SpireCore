using AutumnMooncat.Spirecore.ExternalAPI;
using AutumnMooncat.Spirecore.Util;
using Nickel;

namespace AutumnMooncat.Spirecore.Features;

public class JuggernautStatus : IRStatus, IKokoroApi.IV2.IStatusLogicApi.IHook
{
    public static string ID => nameof(JuggernautStatus);
    public static IStatusEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("juggernaut"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("be0000"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
        MainModFile.Instance.KokoroApi.V2.StatusLogic.RegisterHook(new JuggernautStatus());
    }

    public int ModifyStatusChange(IKokoroApi.IV2.IStatusLogicApi.IHook.IModifyStatusChangeArgs args)
    {
        //MainModFile.Log("Checking status change {} {} -> {}, player? {}", args.Status, args.OldAmount, args.NewAmount, args.Ship.isPlayerShip);
        if (args.Status == Status.shield && args.NewAmount > args.OldAmount && args.Ship.Get(Entry.Status) > 0)
        {
            args.Combat.QueueImmediate(new AAttack()
            {
                damage = Card.GetActualDamage(args.State, args.Ship.Get(Entry.Status)),
                targetPlayer = !args.Ship.isPlayerShip,
                fast = true,
                //storyFromStrafe = true // TODO story dialogue
            });
        }
        return args.NewAmount;
    }
}