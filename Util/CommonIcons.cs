using System;
using System.IO;
using Nickel;

namespace AutumnMooncat.Spirecore.Util;

public class CommonIcons : IRegisterable
{
    public static Spr Hand { get; set; }
    public static Spr DrawPile { get; set; }
    public static Spr DiscardPile { get; set; }
    public static Spr DrawOrDiscardPile { get; set; }
    public static Spr ExhaustPile { get; set; }
    public static Spr TopCard { get; set; }
    public static Spr TopDiscard { get; set; }
    public static Spr TopExhaust { get; set; }
    public static Spr ApplyTo { get; set; }
    public static Spr Bypass { get; set; }
    public static Spr Exhaust { get; set; }
    public static Spr Discard { get; set; }
    public static Spr Draw { get; set; }
    public static Spr Plus { get; set; }
    public static Spr Attack { get; set; }
    public static Spr TempUpgrade { get; set; }
    public static Spr TempSidegrade { get; set; }
    public static Spr TempDowngrade { get; set; }
    public static Spr LetterA { get; set; }
    public static Spr LetterB { get; set; }
    public static Spr AddCard { get; set; }
    public static Spr Limitless { get; set; }
    public static Spr EqualSign { get; set; }
    public static Spr Repeat { get; set; }
    public static Spr OpenBracket { get; set; }
    public static Spr CloseBracket { get; set; }
    public static Spr Search { get; set; }
    public static Spr Question { get; set; }
    public static Spr Exclaim { get; set; }
    public static Spr Colon { get; set; }
    public static Spr Energy { get; set; }
    public static Spr ExtraQuestion { get; set; }
    public static Spr ExtraExclaim { get; set; }
    public static Spr Discount { get; set; }
    public static Spr Reshuffle { get; set; }
    public static Spr Catch { get; set; }
    public static Spr Catch2 { get; set; }
    public static Spr Spawn { get; set; }
    public static Spr SpawnL { get; set; }
    public static Spr SpawnR { get; set; }
    public static Spr Cost { get; set; }
    public static Spr Minus { get; set; }
    public static Spr Minus_Small { get; set; }
    public static Spr Plus_Small { get; set; }
    public static Spr EqualSign_Small { get; set; }
    public static Spr Launch { get; set; }
    public static Spr RightCard { get; set; }
    public static Spr LeftCard { get; set; }
    public static Spr Torch { get; set; }

    public static void Register(IModHelper helper)
    {
        //Hand = StableSpr.icons_dest_hand;
        Hand = Enum.Parse<Spr>("icons_dest_hand");
        DrawPile = Find("drawPile");
        DiscardPile = Find("discardPile");
        DrawOrDiscardPile = Find("drawOrDiscardPile");
        //ExhaustPile = Find("exhaustPile");
        ExhaustPile = Enum.Parse<Spr>("icons_exhaust");
        TopCard = Find("topCard");
        TopDiscard = Find("topDiscard");
        TopExhaust = Find("topExhaust");
        ApplyTo = Find("applyTo");
        Bypass = Enum.Parse<Spr>("icons_bypass");
        Exhaust = Enum.Parse<Spr>("icons_exhaust");
        Discard = Enum.Parse<Spr>("icons_discardCard");
        Draw = Enum.Parse<Spr>("icons_drawCard");
        Plus = Enum.Parse<Spr>("icons_plus");
        Attack = Enum.Parse<Spr>("icons_attack");
        LetterA = Find("letterA");
        LetterB = Find("letterB");
        AddCard = Enum.Parse<Spr>("icons_addCard");
        Limitless = Find("limitless");
        EqualSign = Find("equals");
        Repeat = Find("repeat");
        OpenBracket = Find("bracketOpen");
        CloseBracket = Find("bracketClose");
        Search = Enum.Parse<Spr>("icons_searchCard");
        //Question = Find("questionMark");
        Question = Enum.Parse<Spr>("icons_questionMark");
        Exclaim = Find("exclamationMark");
        Colon = Find("colon");
        Energy = Enum.Parse<Spr>("icons_energy");
        ExtraExclaim = Find("extraExclamation");
        ExtraQuestion = Find("extraQuestion");
        Discount = Enum.Parse<Spr>("icons_discount");
        Reshuffle = Find("reshuffle");
        Catch = Enum.Parse<Spr>("icons_catch");
        Catch2 = Find("catch2");
        Spawn = Enum.Parse<Spr>("icons_spawn");
        SpawnL = Enum.Parse<Spr>("icons_spawnOffsetLeft");
        SpawnR = Enum.Parse<Spr>("icons_spawnOffsetRight");
        Cost = Find("cost");
        Minus = Find("minus");
        Minus_Small = Find("minus2");
        Plus_Small = Find("plus2");
        EqualSign_Small = Find("equals2");
        Launch = Find("launch");
        RightCard = Find("rightCard");
        LeftCard = Find("leftCard");
        Torch = StableSpr.icons_removeCard;
        
        if (MainModFile.Instance.KokoroApi.V2.TemporaryUpgrades.UpgradeTooltip is GlossaryTooltip utt)
        {
            TempUpgrade = utt.Icon.GetValueOrDefault();
        }
        
        if (MainModFile.Instance.KokoroApi.V2.TemporaryUpgrades.SidegradeTooltip is GlossaryTooltip stt)
        {
            TempSidegrade = stt.Icon.GetValueOrDefault();
        }

        if (MainModFile.Instance.KokoroApi.V2.TemporaryUpgrades.DowngradeTooltip is GlossaryTooltip dtt)
        {
            TempDowngrade = dtt.Icon.GetValueOrDefault();
        }
    }

    public static Spr? SourceSpr(CardBrowse.Source src)
    {
        return src switch
        {
            CardBrowse.Source.DrawPile => DrawPile,
            CardBrowse.Source.DiscardPile => DiscardPile,
            CardBrowse.Source.ExhaustPile => ExhaustPile,
            CardBrowse.Source.Hand => Hand,
            CardBrowse.Source.Deck => TopCard,
            CardBrowse.Source.DrawOrDiscardPile => DrawOrDiscardPile,
            _ => null
        };
    }
    
    public static Spr? ActionSpr(CardBrowse.Source src)
    {
        return src switch
        {
            CardBrowse.Source.DrawPile => Reshuffle,
            CardBrowse.Source.DiscardPile => Discard,
            CardBrowse.Source.ExhaustPile => Exhaust,
            CardBrowse.Source.Hand => Draw,
            CardBrowse.Source.Deck => TopCard,
            _ => null
        };
    }
    
    public static Spr? ActionSpr(CardDestination dst, bool random)
    {
        return dst switch
        {
            CardDestination.Deck => random ? Reshuffle : TopCard,
            CardDestination.Hand => Draw,
            CardDestination.Discard => Discard,
            CardDestination.Exhaust => Exhaust,
            _ => null
        };
    }

    public static Spr? DestinationSpr(CardDestination dst, bool random)
    {
        return dst switch
        {
            CardDestination.Deck => random ? Reshuffle : TopCard,
            CardDestination.Hand => Hand,
            CardDestination.Discard => DiscardPile,
            CardDestination.Exhaust => ExhaustPile,
            _ => null
        };
    }

    public static Spr Find(string name)
    {
        var spr = IRStatus.LookUpSpr(IRStatus.DefaultAssetPath + name);
        if (spr.HasValue)
        {
            return spr.Value;
        }

        throw new FileNotFoundException($"Failed to load Spr for \"{name}\"");
    }
}