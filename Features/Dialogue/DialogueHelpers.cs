namespace AutumnMooncat.Spirecore.Features.Dialogue;

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

    public static string Cleo => "nerd";
}

public static class Anim
{
    public static string Neutral => "neutral";
    public static string Squint => "squint";
    public static string GameOver => "gameover";
    
    public static class Ironclad
    {
        public static string Mad => "mad";
    }
    
    // Silent, Defect, Watcher

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