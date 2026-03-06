using AutumnMooncat.Spirecore.Util;
using Nickel;

namespace AutumnMooncat.Spirecore.Features;

public class Limitless : IRTrait, ITooltipHelper
{
    public static string ID => nameof(Limitless);
    public static ICardTraitEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterTrait(ID, new CardTraitConfiguration
        {
            Icon = (state, card) => CommonIcons.Limitless,
            Name = MainModFile.Instance.AnyLocalizations.Bind(["trait", ID, "title"]).Localize,
            Tooltips = (state, card) => [ITooltipHelper.MakeTooltip("trait", ID, CommonIcons.Limitless, Colors.cardtrait)]
        });
    }
}