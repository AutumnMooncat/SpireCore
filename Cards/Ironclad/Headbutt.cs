using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Actions;
using AutumnMooncat.Spirecore.Patches;

namespace AutumnMooncat.Spirecore.Cards.Ironclad;

[IRegisterable.Ignore]
internal sealed class Headbutt : Card, IRCard
{
    public static string ID => nameof(Headbutt);
    public static ICardEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Cards.RegisterCard(ID, new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = Characters.Ironclad.DeckEntry.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = MainModFile.Bind(["card", ID, "name"]).Localize,
            Art = IRCard.LookUpSpr(Characters.Ironclad.CardAssetPath + ID)
        });
    }
    
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = 1
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
                        damage = GetDmg(s, 1)
                    },
                    new ATopDiscardContext()
                    {
                        thatIsnt = this,
                        followup = new MoveSelectedCardToPile
                        {
                            targetLocation = CardBrowse.Source.Deck
                        }
                    }
                    /*new ACascadingCardSelect()
                    {
                        browseSource = CardBrowse.Source.DiscardPile,
                        browseAction = new MoveSelectedCardToPile
                        {
                            targetLocation = CardBrowse.Source.Deck
                        }
                    },
                    MainModFile.AddTooltipsDel((_, _) =>
                    {
                        List<Tooltip> ret = [];
                        if (s.route is Combat && c.discard.Count > 0)
                        {
                            var top = c.discard[^1];
                            /*RenderCombatPatches.Discard.PreRender.Add(g =>
                            {
                                //var rect = new Rect(Combat.discardPos.x, Combat.discardPos.y, 99, 99);
                                top.Render(g, Combat.deckPos, hilight:true);
                            });#1#
                            ret.Add(new TTCard()
                            {
                                card = Mutil.DeepCopy(top),
                                showCardTraitTooltips = true
                            });
                        }
                        return ret;
                    })*/
                ];
                break;
            case Upgrade.A:
                actions = 
                [
                    new AAttack() 
                    {
                        damage = GetDmg(s, 2)
                    },
                    new ATopDiscardContext()
                    {
                        thatIsnt = this,
                        followup = new MoveSelectedCardToPile
                        {
                            targetLocation = CardBrowse.Source.Deck
                        }
                    }
                ];
                break;
            case Upgrade.B:
                actions = 
                [
                    new AAttack() 
                    {
                        damage = GetDmg(s, 1)
                    },
                    new ATopDiscardContext()
                    {
                        thatIsnt = this,
                        followup = new MoveSelectedCardToPile
                        {
                            targetLocation = CardBrowse.Source.Hand
                        }
                    }
                ];
                break;
        }
        return actions;
    }
}
