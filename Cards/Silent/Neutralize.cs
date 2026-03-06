using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace AutumnMooncat.Spirecore.Cards.Silent;

internal sealed class Neutralize : Card, IRCard
{
    public static string ID => nameof(Neutralize);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Silent.DeckEntry.Deck,
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
            cost = 0
        };
        return data;
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = [];
        switch (upgrade)
        {
            case Upgrade.None:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 0),
                        status = MainModFile.Instance.KokoroApi.V2.DriveStatus.Underdrive,
                        statusAmount = 1
                    }
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 1),
                        status = MainModFile.Instance.KokoroApi.V2.DriveStatus.Underdrive,
                        statusAmount = 1
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AAttack()
                    {
                        damage = GetDmg(s, 0)
                    },
                    new AStatus()
                    {
                        status = MainModFile.Instance.KokoroApi.V2.DriveStatus.Underdrive,
                        statusAmount = 1,
                        targetPlayer = false
                    }
                ];
                break;
        }
        return actions;
    }
}
