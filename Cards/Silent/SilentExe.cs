using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace AutumnMooncat.SpireCore.Cards.Silent;

internal sealed class SilentExe : Card, IRCard
{
    public static string ID => nameof(SilentExe);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Deck.colorless,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = MainModFile.Bind(["card", ID, "name"]).Localize,
            Art = IRCard.LookUpSpr(Characters.Silent.CardAssetPath + ID)
        });
    }
    
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = upgrade == Upgrade.A ? 0 : 1,
            exhaust = true,
            description = MainModFile.Loc(["card", ID, "description"], new {Choices = GetChoices()})
        };
        return data;
    }

    public int GetChoices()
    {
        return upgrade == Upgrade.B ? 3 : 2;
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        return [
            new ACardOffering()
            {
                amount = GetChoices(),
                limitDeck = Characters.Silent.DeckEntry.Deck,
                makeAllCardsTemporary = true,
                overrideUpgradeChances = false,
                canSkip = false,
                inCombat = true,
                discount = -1,
                dialogueSelector = $".summon{Characters.Silent.DeckEntry.UniqueName}"
            }
        ];
    }
}
