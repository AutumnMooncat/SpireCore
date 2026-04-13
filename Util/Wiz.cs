using System;
using AutumnMooncat.SpireCore;
using HarmonyLib;

namespace AutumnMooncat.SpireCore.Util;

public static class Wiz
{
    public static bool RecursiveActionChecker(Card card, State state, Combat combat, Predicate<CardAction> check)
    {
        var actions = card.GetActions(state, combat);
        foreach (var action in actions)
        {
            if (RecursiveActionChecker(action, check))
            {
                return true;
            }
        }
        return false;
    }
    
    public static bool RecursiveActionChecker(CardAction action, Predicate<CardAction> check)
    {
        if (check(action))
        {
            return true;
        }
        
        if (action is INestingAction ina)
        {
            if (RecursiveActionChecker(ina.Nested, check))
            {
                return true;
            }
        }

        foreach (var wrapped in MainModFile.Instance.KokoroApi.V2.WrappedActions.GetWrappedCardActions(action) ?? [])
        {
            if (RecursiveActionChecker(wrapped, check))
            {
                return true;
            }
        }

        return false;
    }

    public static bool FieldChecker(object o, Predicate<object> check)
    {
        var fields = AccessTools.GetDeclaredFields(o.GetType());
        foreach (var field in fields)
        {
            if (check(field.GetValue(o)))
            {
                return true;
            }
        }
        return false;
    }
    
    public static bool PropertyChecker(object o, Predicate<object> check)
    {
        var props = AccessTools.GetDeclaredProperties(o.GetType());
        foreach (var prop in props)
        {
            if (check(prop.GetValue(o)))
            {
                return true;
            }
        }
        return false;
    }
    
    public static bool GivesStatus(Card card, State state, Combat combat, bool? targetsPlayer = true)
    {
        return RecursiveActionChecker(card, state, combat, a =>
            a is AStatus aStatus && (targetsPlayer == null || aStatus.targetPlayer == targetsPlayer) &&
            aStatus.status != Status.shield &&
            aStatus.status != Status.tempShield);
    }
    
    public static bool IsAttack(Card card, State state, Combat combat)
    {
        return RecursiveActionChecker(card, state, combat, a => a is AAttack);
    }
}