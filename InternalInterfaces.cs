using System;
using System.Collections.Generic;
using System.IO;
using AutumnMooncat.Spirecore.Features;
using Nickel;

namespace AutumnMooncat.Spirecore;

internal interface IRegisterable
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class Ignore : Attribute { }
    
    static abstract void Register(IModHelper helper);
    
    static string DefaultAssetPath => "assets/";

    static Spr? LookUpSpr(string path)
    {
        var file = MainModFile.GetFile(path + ".png");
        if (file.Exists)
        {
            return MainModFile.MakeSprite(file).Sprite;
        }
        return null;
    }
}

internal interface IRCard : IRegisterable
{
    static abstract string ID { get; }
    static abstract ICardEntry Entry { get; set; }

    new static string DefaultAssetPath => IRegisterable.DefaultAssetPath + "cards/";
}

internal interface IRArtifact : IRegisterable
{
    static abstract string ID { get; }
    static abstract IArtifactEntry Entry { get; set; }
    
    new static string DefaultAssetPath => IRegisterable.DefaultAssetPath + "artifacts/";
}

internal interface IRStatus : IRegisterable
{
    static abstract string ID { get; }
    static abstract IStatusEntry Entry { get; set; }

    new static string DefaultAssetPath => IRegisterable.DefaultAssetPath + "icons/";
}

internal interface IRTrait : IRegisterable
{
    static abstract string ID { get; }
    static abstract ICardTraitEntry Entry { get; set; }

    new static string DefaultAssetPath => IRegisterable.DefaultAssetPath + "icons/";
}

internal interface IRShip : IRegisterable
{
    static abstract string ID { get; }
    static abstract IShipEntry Entry { get; set; }
    
    new static string DefaultAssetPath => IRegisterable.DefaultAssetPath + "ships/";
}

internal interface IRCharacter : IRegisterable
{
    static abstract string ID { get; }
    static abstract string AssetPath { get; }
    static abstract string CardAssetPath { get; }
    static abstract string ArtifactAssetPath { get; }
    static abstract ISpriteEntry CardBackground { get; set; }
    static abstract ISpriteEntry CardFrame { get; set; }
    static abstract ISpriteEntry CharacterPanel { get; set; }
    static abstract IDeckEntry DeckEntry { get; set; }
    static abstract IPlayableCharacterEntryV2 Entry { get; set; }

    new static string DefaultAssetPath => IRegisterable.DefaultAssetPath + "characters/";

    static void RegisterAnim(IDeckEntry entry, string path, string name, int frames)
    {
        MainModFile.Instance.Helper.Content.Characters.V2.RegisterCharacterAnimation(new CharacterAnimationConfigurationV2()
        {
            CharacterType = entry.Deck.Key(),
            LoopTag = name,
            Frames = GetFrames(path, name, frames)
        });
    }
    
    static IReadOnlyList<Spr> GetFrames(string path, string name, int frames)
    {
        List<Spr> sprites = new List<Spr>();
        var def = LookUpSpr(path+name);
        for (int i = 0; i < frames; i++)
        {
            var spr = LookUpSpr(path + name + "_" + i);
            if (spr.HasValue)
            {
                sprites.Add(spr.Value);
            }
            else
            {
                if (def.HasValue)
                {
                    sprites.Add(def.Value);
                }
                else
                {
                    throw new FileNotFoundException("Could not load frame data for " + path + name + "_" + i);
                }
            }
        }

        return sprites;
    }
}

internal interface IMultiIconAction
{
    List<Records.RenderPayload> GetExtraIcons(State s);
}

internal interface INestingAction
{
    CardAction Nested { get; set; }
}

internal interface ITooltipHelper
{
    static abstract string ID { get; }

    static Tooltip MakeTooltip(string type, string identifier, Spr? icon = null, Color? color = null,
        string suffix = null, object tokens = null, bool wide = false)
    {
        return new GlossaryTooltip($"{type}.{MainModFile.MakeID(identifier)}{suffix??""}")
        {
            Icon = icon,
            TitleColor = color ?? Colors.textMain,
            IsWideIcon = wide,
            Title = MainModFile.Loc([type, identifier, "title"+(suffix??"")]),
            Description = MainModFile.Loc([type, identifier, "description"+(suffix??"")], tokens)
        };
    }
    
    static Tooltip MakeMultiTooltip(string type, string identifier, Records.TexturePayload[] payload, int? offset, Color? color = null,
        string suffix = null, object tokens = null)
    {
        return new CustomTooltip($"{type}.{MainModFile.MakeID(identifier)}{suffix??""}")
        {
            IconData = payload,
            TitleColor = color ?? Colors.textMain,
            TitleOffset = offset ?? 0,
            Title = MainModFile.Loc([type, identifier, "title"+(suffix??"")]),
            Description = MainModFile.Loc([type, identifier, "description"+(suffix??"")], tokens)
        };
    }
}

internal interface ICustomUpgrades
{
    bool IsUpgradableOverride(bool baseResult);
    string GetFullDisplayNameOverride(string baseResult);
    CardMeta GetMetaOverride(CardMeta baseResult);
}

internal interface IArtifactOnDiscard
{
    void OnDiscard(State s, Combat c, Card card);
}

internal interface IArtifactOnExhaust
{
    void OnExhaust(State s, Combat c, Card card);
}

internal interface IDroneShieldOverride
{
    void RenderShieldOverride(G g);
}