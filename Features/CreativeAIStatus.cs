using System;
using System.Collections.Generic;
using System.Linq;
using AutumnMooncat.Spirecore.ExternalAPI;
using AutumnMooncat.SpireCore.ExternalAPI.Kokoro;
using AutumnMooncat.Spirecore.Util;
using AutumnMooncat.SpireCore.Util;
using HarmonyLib;
using JetBrains.Annotations;
using Nickel;

namespace AutumnMooncat.Spirecore.Features;

public class CreativeAIStatus : IRStatus, IKokoroApi.IV2.IStatusLogicApi.IHook, IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public static string ID => nameof(CreativeAIStatus);
    public static IStatusEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Statuses.RegisterStatus(ID, new()
        {
            Definition = new()
            {
                /* We provide the icon as a Sprite type, you can find it in the given file location */
                icon = CommonIcons.Find("creativeAI"),
                /* We give it a color, this is the border color that surrounds the status icon & number in-game */
                color = new("44a5f1"),
                /* We define if it's isGood = true or isGood = false. This will dictate if the number will be either blue or red */
                isGood = true
            },
            Name = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["status", ID, "description"]).Localize
        });
        var _ = new CreativeAIStatus();
        MainModFile.Instance.KokoroApi.V2.StatusLogic.RegisterHook(_);
        MainModFile.Instance.KokoroApi.V2.StatusRendering.RegisterHook(_);
    }
    
    public IReadOnlyList<Tooltip> OverrideStatusTooltips(IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusTooltipsArgs args)
    {
        if (args.Status != Entry.Status)
        {
            return args.Tooltips;
        }
        
        List<Tooltip> ret = [];
        ret.AddRange(args.Tooltips);
        ret.Add(new TTGlossary("cardtrait.exhaust"));
        return ret;
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

        if (args.Amount > 0)
        {
            List<Deck?> decks = args.State.characters.Select(c => c.deckType).ToList();
            List<Card> allCards = DB.releasedCards.Where(c =>
            {
                var meta = c.GetMeta();
                if (!decks.Contains(meta.deck) || meta.dontOffer || meta.unreleased)
                {
                    return false;
                }

                if (!c.GetData(args.State).exhaust || !Wiz.GivesStatus(c, args.State, args.Combat))
                {
                    return false;
                }
                
                return true;
            }).ToList();
            for (int i = 0; i < args.Amount; i++)
            {
                Card card = (Card) Activator.CreateInstance(allCards.Random(args.State.rngCardOfferingsMidcombat).GetType());
                if (card != null)
                {
                    card.drawAnim = 1.0;
                    args.Combat.QueueImmediate(new AAddCard()
                    {
                        card = card,
                        destination = CardDestination.Hand
                    });
                }
            }
        }
        return false;
    }
}