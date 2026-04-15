using System.Collections.Generic;
using AutumnMooncat.SpireCore.Util;

namespace AutumnMooncat.SpireCore.Actions;

public class AFission : CardAction, ITooltipHelper
{
    public static string ID => nameof(AFission);
    public static Spr Icon => CommonIcons.Find("fission3");
    
    public override void Begin(G g, State s, Combat c)
    {
        foreach (var pair in c.stuff)
        {
            if (pair.Value.Invincible())
            {
                c.QueueImmediate(pair.Value.GetActionsOnShotWhileInvincible(s, c, true, 0));
                continue;
            }
            c.DestroyDroneAt(s, pair.Key, true);
        }

        Audio.Play(FSPRO.Event.Hits_DroneCollision);
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        return [ITooltipHelper.MakeTooltip("action", ID, Icon, Colors.action)];
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon(Icon, null, Colors.textMain);
    }
}