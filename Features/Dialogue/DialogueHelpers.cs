using System.Collections.Generic;
using System.Linq;
using AutumnMooncat.SpireCore.Characters;

namespace AutumnMooncat.SpireCore.Features.Dialogue;

public static class CType
{
    public static string Ironclad => Characters.Ironclad.Entry.CharacterType;
    public static string Silent => Characters.Silent.Entry.CharacterType;
    public static string Defect => Characters.Defect.Entry.CharacterType;
    public static string Watcher => Characters.Watcher.Entry.CharacterType;

    public static string Dizzy => Deck.dizzy.Key();
    public static string Riggs => Deck.riggs.Key();
    public static string Peri => Deck.peri.Key();
    public static string Isaac => Deck.goat.Key();
    public static string Drake => Deck.eunice.Key();
    public static string Max => Deck.hacker.Key();
    public static string Books => Deck.shard.Key();
    public static string CAT => "comp";

    public static string RandomCrew => "crew";
    public static List<string> AllVanilla => [Dizzy, Riggs, Peri, Isaac, Drake, Max, Books, CAT];
    public static List<string> AllModded => MainModFile.GetHelper().Content.Characters.V2.RegisteredPlayableCharacters.Values.Select(e => e.CharacterType).ToList();
    public static List<string> AllPlayable => [..AllVanilla, ..AllModded];

    public static class Events
    {
        public static string Brimford => "walrus";
        public static string Cleo => "nerd";
        public static string Dracula => "dracula";
        public static string DrBjorn => "scientist";
        public static string GarboGirl => "garbogirl";
        public static string Grandma => "grandma";
        public static string Sasha => "sasha";
        public static string Selene => "selene";
        public static string Slug => "slug";
        public static string Soggens => "soggens";
        public static string Tooth => "tooth";
        public static string Wren => "wren";
    }
    
    public static class Enemies
    {
        public static string Brac => "crab";
        public static string Crystal => "crystal";
        public static string Dahlia => "bandit";
        public static string Dillian => "dillian";
        public static string DrakePirate => "pirate";
        public static string Duncan => "skunk";
        public static string Gemma => "gemma";
        public static string George => "spike";
        public static string Jumbo => "miner";
        public static string Lira => "lizard";
        public static string Ratzo => "knight";
        public static string RiggsPirate => "pirateBoss";
        public static string Smiff => "batboy";
        public static string Snute => "snoot";
        public static string Spike => "oxygenguy";
        public static string Stardog => "wolf";
        public static string Wizbo => "wizard";

        public static class Robots
        {
            public static string Biped => "biped";
            public static string Chunk => "chunk";
            public static string DrakeBot => "drakebot";
            public static string Janitor => "dual";
            public static string Rail => "rail";
            public static string Scrap => "scrap";
            public static string Wasp => "wasp";

        }
    }
}

public static class DType
{
    public static Deck Ironclad => Characters.Ironclad.DeckEntry.Deck;
    public static Deck Silent => Characters.Silent.DeckEntry.Deck;
    public static Deck Defect => Characters.Defect.DeckEntry.Deck;
    public static Deck Watcher => Characters.Watcher.DeckEntry.Deck;

    public static Deck Dizzy => Deck.dizzy;
    public static Deck Riggs => Deck.riggs;
    public static Deck Peri => Deck.peri;
    public static Deck Isaac => Deck.goat;
    public static Deck Drake => Deck.eunice;
    public static Deck Max => Deck.hacker;
    public static Deck Books => Deck.shard;
    public static Deck CAT => Deck.colorless;
}

public static class LookupKeys
{
    public static string ShopBefore => "shopBefore";
    
    public static string IroncladReturnedFromMissing => MainModFile.MakeID(Ironclad.ID+"ReturningFromMissing");
    public static string SilentReturnedFromMissing => MainModFile.MakeID(Silent.ID+"ReturningFromMissing");
    public static string DefectReturnedFromMissing => MainModFile.MakeID(Defect.ID+"ReturningFromMissing");
    public static string WatcherReturnedFromMissing => MainModFile.MakeID(Watcher.ID+"ReturningFromMissing");
}

public static class StoryTags
{
    // Vanilla
    public static string AboutToDie => "aboutToDie";
    public static string NoOverlap => "NoOverlapBetweenShips";
    public static string NoOverlapSeeker => "NoOverlapBetweenShipsSeeker";
    
    // New
    public static string ICHasCupcakes => "CupcakesGet";
    public static string ICLossCounter => "IroncladLosses";
    public static string ICDrakeTheBetIsOn => "IroncladDrakeBetIsOn";
    public static string ICBeatDrakeCounter => "IroncladWinsVsDrake";
    public static string DrakeBeatICCounter => "DrakesWinsVsIronclad";

    public static class WentMissing
    {
        public static string Ironclad => "ironcladWentMissing";
        public static string Silent => "silentWentMissing";
        public static string Defect => "defectWentMissing";
        public static string Watcher => "watcherWentMissing";
        
        public static string Riggs => "riggsWentMissing";
        public static string Dizzy => "dizzyWentMissing";
        public static string Peri => "periWentMissing";
        public static string Isaac => "isaacWentMissing";
        public static string Drake => "drakeWentMissing";
        public static string Max => "maxWentMissing";
        public static string Books => "booksWentMissing";
        public static string CAT => "CatWentMissing";
    }
}

public static class Anim
{
    public static string Neutral => "neutral";
    public static string Squint => "squint";
    public static string GameOver => "gameover";
    public static string Mini => "mini";
    
    public static class Ironclad
    {
        public static string Blush => "blush";
        public static string Happy => "happy";
        public static string Intense => "intense";
        public static string Mad => "mad";
        public static string Sword => "sword";
    }

    public static class Silent
    {
        
    }

    public static class Defect
    {
        public static string Frown => "frown";
        public static string Happy => "happy";
        public static string Intense => "intense";
        public static string Mad => "mad";
        public static string MadRed => "madRed";
        public static string ReallyMad => "reallyMad";
        public static string ReallyMadRed => "reallyMadRed";
        public static string Sad => "sad";
    }

    public static class Watcher
    {
        public static string Blush => "blush";
        public static string Calm => "calm";
        public static string Doubt => "doubt";
        public static string Frown => "frown";
        public static string Happy => "happy";
        public static string Mad => "mad";
        public static string MadSmile => "madSmile";
        public static string Shock => "shock";
        public static string Smile => "smile";
        public static string SmileBlush => "smileBlush";
        public static string Surprise => "surprise";
        public static string SurpriseBlush => "surpriseBlush";
    }

    public static class Dizzy
    {
        public static string ButtonOff => "buttonOff";
        public static string ButtonOn => "buttonOn";
        public static string Crystal => "crystal";
        public static string Explains => "explains";
        public static string Frown => "frown";
        public static string Geiger => "geiger";
        public static string Intense => "intense";
        public static string Mad => "mad";
        public static string Serious => "serious";
        public static string Shrug => "shrug";
    }
    
    public static class Riggs
    {
        public static string Banana => "banana";
        public static string Boba => "bobaSlurp";
        public static string Catch => "catch";
        public static string CatchAir => "catchAir";
        public static string CatchEyes => "catchEyes";
        public static string Egg => "egg";
        public static string EggCrack => "eggCrack";
        public static string Gun => "gun";
        public static string Huh => "huh";
        public static string Nervous => "nervous";
        public static string Sad => "sad";
        public static string Serious => "serious";
        public static string Suit => "suit";
        public static string SuitCrystal => "suitCrystal";
        public static string SuitIntense => "suitIntense";
        public static string SuitScared => "suitScared";
    }

    public static class Peri
    {
        public static string Anime => "anime";
        public static string Blink => "blink";
        public static string Mad => "mad";
        public static string Nap => "nap";
        public static string Panic => "panic";
        public static string Shy => "shy";
        public static string Vengeful => "vengeful";
    }

    public static class Isaac
    {
        public static string Explains => "explains";
        public static string EyesClosed => "eyesClosed";
        public static string Panic => "panic";
        public static string Shy => "shy";
        public static string Sly => "sly";
        public static string Writing => "writing";
    }

    public static class Drake
    {
        public static string Blush => "blush";
        public static string Mad => "mad";
        public static string Panic => "panic";
        public static string ReallyMad => "reallymad";
        public static string Sad => "sad";
        public static string SadEyes => "sadEyes";
        public static string Sly => "sly";
        public static string SlyBlush => "slyBlush";
    }
    
    public static class Max
    {
        public static string Blush => "blush";
        public static string Gloves => "gloves";
        public static string Intense => "intense";
        public static string Mad => "mad";
        public static string Smile => "smile";
    }

    public static class Books
    {
        public static string Blush => "blush";
        public static string Book => "books";
        public static string Crystal => "crystal";
        public static string Intense => "intense";
        public static string Paws => "paws";
        public static string Plan => "plan";
        public static string Relaxed => "relaxed";
        public static string Stoked => "stoked";
    }

    public static class CAT
    {
        public static string GlitchA => "glitchA";
        public static string Grumpy => "grumpy";
        public static string Intense => "intense";
        public static string Lean => "lean";
        public static string Mad => "mad";
        public static string Peace => "peace";
        public static string Smug => "smug";
        public static string Worried => "worried";

        public static class Extras
        {
            public static string GlitchA => "glitchA";
            public static string GlitchB => "glitchB";
            public static string Transition => "transition";
            public static string Transition2 => "transition2";
            public static string TransitionVoid => "transitionVoid";
            public static string Being => "being";
            public static string BeingAngry => "beingAngery";
            public static string BeingDissolve => "dissolve";
            public static string BeingFeral => "feral";
            public static string BeingStill => "eb";
        }
    }

    public static class Cleo
    {
        public static string Explains => "explains";
        public static string Nervous => "nervous";
    }
}