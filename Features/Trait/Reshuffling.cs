using AutumnMooncat.SpireCore.Util;
using Nickel;

namespace AutumnMooncat.SpireCore.Features;

public class Reshuffling : IRTrait, ITooltipHelper
{
    public static string ID => nameof(Reshuffling);
    public static ICardTraitEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterTrait(ID, new CardTraitConfiguration
        {
            Icon = (state, card) => CommonIcons.Reshuffle,
            Name = MainModFile.Instance.AnyLocalizations.Bind(["trait", ID, "title"]).Localize,
            Tooltips = (state, card) => [ITooltipHelper.MakeTooltip("trait", ID, CommonIcons.Reshuffle, Colors.cardtrait)]
        });
    }
}