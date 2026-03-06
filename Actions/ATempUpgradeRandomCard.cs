using System.Collections.Generic;
using System.Linq;
using AutumnMooncat.Spirecore.Util;

namespace AutumnMooncat.Spirecore.Actions;

public class ATempUpgradeRandomCard : CardAction, IMultiIconAction, ITooltipHelper
{
    public static string ID => nameof(ATempUpgradeRandomCard);
    
    public int? howManyCards;
    public Upgrade? upgrade;
    
    public override void Begin(G g, State s, Combat c)
    {
        if (howManyCards == null)
        {
            foreach (var card in c.hand)
            {
                if (MainModFile.Instance.KokoroApi.V2.TemporaryUpgrades.GetTemporaryUpgrade(s, card) == Upgrade.None && card.IsUpgradable())
                {
                    MainModFile.Instance.KokoroApi.V2.TemporaryUpgrades.SetTemporaryUpgrade(s, card,
                        upgrade ?? (card.GetMeta().upgradesTo.Length == 1 || s.rngActions.NextInt() >= 0
                            ? Upgrade.A : Upgrade.B));
                }
            }
            return;
        }
        for (int i = 0; i < howManyCards; i++)
        {
            Card card = c.hand.Where(card2 => MainModFile.Instance.KokoroApi.V2.TemporaryUpgrades.GetTemporaryUpgrade(s, card2) == Upgrade.None && card2.IsUpgradable()).Shuffle(s.rngActions).FirstOrDefault();
            if (card != null)
            {
                MainModFile.Instance.KokoroApi.V2.TemporaryUpgrades.SetTemporaryUpgrade(s, card, 
                    upgrade ?? (card.GetMeta().upgradesTo.Length == 1 || s.rngActions.NextInt() >= 0 
                        ? Upgrade.A : Upgrade.B));
            }
        }
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        string suffix = upgrade == null ? "Random" : upgrade == Upgrade.None ? "Down" : upgrade == Upgrade.A ? "A" : "B";
        if (howManyCards == null)
        {
            suffix += "Hand";
        }

        return
        [
            ITooltipHelper.MakeTooltip("action", ID, CommonIcons.TempUpgrade, Colors.action, suffix, new { Cards = howManyCards }),
            upgrade == Upgrade.None 
                ? MainModFile.Instance.KokoroApi.V2.TemporaryUpgrades.DowngradeTooltip 
                : MainModFile.Instance.KokoroApi.V2.TemporaryUpgrades.UpgradeTooltip
        ];
    }

    public override Icon? GetIcon(State s)
    {
        if (upgrade == Upgrade.None)
        {
            return new Icon(CommonIcons.TempDowngrade, howManyCards, Colors.textMain);
        } 
        
        return new Icon(CommonIcons.TempUpgrade, upgrade == null ? howManyCards : null, Colors.textMain);
    }

    public List<Records.RenderPayload> GetExtraIcons(State s)
    {
        List<Records.RenderPayload> ret = [];
        if (upgrade == Upgrade.A)
        {
            ret.Add(new (){spr = CommonIcons.LetterA, amount = howManyCards, dx = -4, dy = -1});
        }
        else if (upgrade == Upgrade.B)
        {
            ret.Add(new (){spr = CommonIcons.LetterB, amount = howManyCards, dx = -4, dy = -1});
        }

        if (howManyCards == null)
        {
            ret.Add(new (){spr = CommonIcons.Hand});
        }
        return ret;
    }
}