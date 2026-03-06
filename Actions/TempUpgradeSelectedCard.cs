using System.Collections.Generic;
using AutumnMooncat.Spirecore.Util;
using Nickel;

namespace AutumnMooncat.Spirecore.Actions;

public class TempUpgradeSelectedCard : CardAction, IMultiIconAction, ITooltipHelper
{
    public static string ID => nameof(TempUpgradeSelectedCard);
    
    public Upgrade? upgrade;
    
    public override void Begin(G g, State s, Combat c)
    {
        if (selectedCard != null)
        {
            MainModFile.Instance.KokoroApi.V2.TemporaryUpgrades.SetTemporaryUpgrade(s, selectedCard, upgrade ?? (s.rngActions.NextInt() >= 0 ? Upgrade.A : Upgrade.B));
        }
    }

    private string GetSuffix(State s)
    {
        return upgrade == null ? "Random" : upgrade == Upgrade.None ? "Down" : upgrade == Upgrade.A ? "A" : "B";
    }

    public override string GetCardSelectText(State s)
    {
        return MainModFile.Instance.Localizations.Localize(["action", ID, "text"+GetSuffix(s)]);
    }
    
    public override List<Tooltip> GetTooltips(State s)
    {
        return 
        [
            ITooltipHelper.MakeTooltip("action", ID, CommonIcons.TempUpgrade, Colors.action, GetSuffix(s)),
            upgrade == Upgrade.None 
                ? MainModFile.Instance.KokoroApi.V2.TemporaryUpgrades.DowngradeTooltip 
                : MainModFile.Instance.KokoroApi.V2.TemporaryUpgrades.UpgradeTooltip
        ];
    }
    
    public override Icon? GetIcon(State s)
    {
        if (upgrade == Upgrade.None)
        {
            return new Icon(CommonIcons.TempDowngrade, null, Colors.textMain);
        } 
        
        return new Icon(CommonIcons.TempUpgrade, null, Colors.textMain);
    }
    
    public List<Records.RenderPayload> GetExtraIcons(State s)
    {
        if (upgrade == Upgrade.A)
        {
            return [new (){spr = CommonIcons.LetterA, dx = -4, dy = -1}];
        } 
        if (upgrade == Upgrade.B)
        {
            return [new (){spr = CommonIcons.LetterB, dx = -4, dy = -1}];
        }

        return [];
    }
}