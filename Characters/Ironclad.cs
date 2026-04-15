using AutumnMooncat.SpireCore.Cards.Ironclad;
using AutumnMooncat.SpireCore.ExternalAPI;
using AutumnMooncat.SpireCore.Features.Dialogue;
using Nickel;
using Nickel.Common;

namespace AutumnMooncat.SpireCore.Characters;

public class Ironclad : IRCharacter
{
    public static string ID => nameof(Ironclad);
    public static string AssetPath => IRCharacter.DefaultAssetPath + ID.ToLower() + "/";
    public static string CardAssetPath => IRCard.DefaultAssetPath + ID.ToLower() + "/";
    public static string ArtifactAssetPath => IRArtifact.DefaultAssetPath + ID.ToLower() + "/";
    public static ISpriteEntry CardBackground { get; set; }
    public static ISpriteEntry CardFrame { get; set; }
    public static ISpriteEntry CharacterPanel { get; set; }
    public static IDeckEntry DeckEntry { get; set; }
    public static IPlayableCharacterEntryV2 Entry { get; set; }

    public static void Register(IModHelper helper)
    {
        CardBackground = MainModFile.MakeSprite(AssetPath + "cardbackground.png");
        CardFrame = MainModFile.MakeSprite(AssetPath + "cardframe.png");
        CharacterPanel = MainModFile.MakeSprite(AssetPath + "characterpanel.png");
        DeckEntry = helper.Content.Decks.RegisterDeck(ID+"Deck", new DeckConfiguration()
        {
            Definition = new DeckDef()
            {
                /* This color is used in various situations.
                 * It is used as the deck's rarity 'shine'
                 * If a playable character uses this deck, the character ID will use this color
                 * If a playable character uses this deck, the character mini panel will use this color */
                color = new Color("bf3f43"), // 943134

                /* This color is for the card name in-game
                 * Make sure it has a good contrast against the CardFrame, and take rarity 'shine' into account as well */
                titleColor = new Color("000000")
            },
            /* We give it a default art and border some Sprite types by adding '.Sprite' at the end of the ISpriteEntry definitions we made above. */
            DefaultCardArt = CardBackground.Sprite,
            BorderSprite = CardFrame.Sprite,

            /* Since this deck will be used by our Demo Character, we'll use their name. */
            Name = MainModFile.Bind(["character", ID, "name"]).Localize,
        });
        Entry = helper.Content.Characters.V2.RegisterPlayableCharacter(ID, new PlayableCharacterConfigurationV2()
        {
            Deck = DeckEntry.Deck,
            Starters = new StarterDeck()
            {
                cards = [new Bash(), new TrueGrit()]
            },
            BorderSprite = CharacterPanel.Sprite,
            Description = MainModFile.Bind(["character", ID, "description"]).Localize,
            ExeCardType = typeof(IroncladExe)
        });
        IRCharacter.RegisterAnim(DeckEntry, AssetPath, Anim.Neutral, 5);
        IRCharacter.RegisterAnim(DeckEntry, AssetPath, Anim.Mini, 1);
        IRCharacter.RegisterAnim(DeckEntry, AssetPath, Anim.Squint, 5);
        IRCharacter.RegisterAnim(DeckEntry, AssetPath, Anim.GameOver, 1);
        IRCharacter.RegisterAnim(DeckEntry, AssetPath, Anim.Ironclad.Mad, 5);
        
        helper.ModRegistry.AwaitApi<IMoreDifficultiesApi>(
            "TheJazMaster.MoreDifficulties",
            new SemanticVersion(1, 3, 0),
            api => api.RegisterAltStarters(
                deck: DeckEntry.Deck,
                starterDeck: new StarterDeck
                {
                    cards = [
                        new IronWave(),
                        new ShrugItOff()
                    ]
                }
            )
        );
    }
}