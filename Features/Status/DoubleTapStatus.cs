using System;
using AutumnMooncat.SpireCore.ExternalAPI;
using AutumnMooncat.SpireCore.Patches;
using AutumnMooncat.SpireCore.Util;
using Nickel;

namespace AutumnMooncat.SpireCore.Features;

public class DoubleTapStatus : IRStatus, IKokoroApi.IV2.IStatusLogicApi.IHook
{
    public static string ID => nameof(DoubleTapStatus);
    public static IStatusEntry Entry { get; set; }

    private static int? lastID;
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("doubleTap"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("be2000"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
        MainModFile.Instance.KokoroApi.V2.StatusLogic.RegisterHook(new DoubleTapStatus());
        helper.Events.RegisterBeforeArtifactsHook("OnPlayerPlayCard",
            (int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount) =>
            {
                if (state.ship.Get(Entry.Status) == 0 || !Wiz.IsAttack(card, state, combat) || card.uuid == lastID)
                {
                    return;
                }

                combat.Queue(new AZeroDoublerAction()
                {
                    uuid = card.uuid,
                    backupCard = card.CopyWithNewId()
                });
                combat.cardActions.RemoveAll((Predicate<CardAction>) (action => action is AEndTurn));
                
                //var copy = card.CopyWithNewId();
                //MainModFile.Instance.Helper.Content.Cards.SetCardTraitOverride(state, copy, MainModFile.Instance.Helper.Content.Cards.SingleUseCardTrait, true, true);
                lastID = card.uuid;
                state.ship.Set(Entry.Status, state.ship.Get(Entry.Status) - 1);
                // combat.cardActions.RemoveAll((Predicate<CardAction>) (action => action is AEndTurn)); // ??
                //combat.QueueImmediate(new DoTheThing{card = copy});
            });
    }

    private class DoTheThing : CardAction
    {
        public Card card;
        
        public override void Begin(G g, State s, Combat c)
        {
            c.hand.Add(card);
            PlayCardPatches.ignoreThis = card;
            c.TryPlayCard(s, card, true);
        }
    }

    public bool HandleStatusTurnAutoStep(IKokoroApi.IV2.IStatusLogicApi.IHook.IHandleStatusTurnAutoStepArgs args)
    {
        if (args.Timing != IKokoroApi.IV2.IStatusLogicApi.StatusTurnTriggerTiming.TurnStart)
        {
            return false;
        }

        if (args.Status != Entry.Status)
        {
            return false;
        }
        
        args.Amount = 0;
        return false;
    }
}