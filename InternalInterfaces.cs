using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutumnMooncat.SpireCore.Features;
using AutumnMooncat.SpireCore.Features.Dialogue;
using AutumnMooncat.SpireCore.Patches;
using Nickel;

namespace AutumnMooncat.SpireCore;

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

internal interface IRDialogue : IRegisterable
{
    internal class DialogueRegistry<T>(Func<IReadOnlyList<string>, T, string> realKeyFunc, Func<IReadOnlyList<string>, T, IReadOnlyList<string>> lookupKeyFunc)
    {
        private readonly Dictionary<string, int> _keyCounter = [];
        private readonly Dictionary<IReadOnlyList<string>, T> _stuff = [];

        internal void Register(IReadOnlyList<string> key, T value)
        {
            var check = Format(lookupKeyFunc(key, value));
            _keyCounter.TryAdd(check, 0);
            //MainModFile.Log(!_keyCounter.TryAdd(check, 0) ? "Incrementing key {}" : "Starting key {}", check);
            _stuff[[..key, _keyCounter[check].ToString()]] = value;
            _keyCounter[check] += 1;
        }

        internal Dictionary<IReadOnlyList<string>, T> Get()
        {
            return _stuff;
        }

        internal string GetRealKey(IReadOnlyList<string> key, T val) => realKeyFunc(key, val);
        internal IReadOnlyList<string> GetLookupKey(IReadOnlyList<string> key, T val) => lookupKeyFunc(key, val);
    }
    
    internal static string Format(IReadOnlyList<string> key)
    {
        return key.Aggregate((a, b) => (a ?? "<null>") + ", " + (b ?? "<null>"));
    }
    
    static DialogueRegistry<StoryNode> MakeNormalRegistry() => new(
        (key,val) => MainModFile.MakeID(string.Join(".", key)),
        (key, val) => key);

    static DialogueRegistry<StoryNode> MakeHardcodedRegistry() => new(
        (key,val) => string.Join(".", key.Select(s => s.Replace("{{CharacterType}}", key[0])[1..])),
        (key, val) => key.Skip(1).ToList());

    static DialogueRegistry<Say> MakeSaySwitchRegistry() => new(
        (key,val) => key[^2],
        (key, val) => key);
    
    static DialogueRegistry<Say> MakeHashReplacerRegistry() => new(
        (key,val) => key[^2],
        (key, val) => key);
    
    static INonNullLocalizationProvider<IReadOnlyList<string>> GetLoc(Func<string, Stream> localeStreamFunction)
    {
        return new MissingPlaceholderNonBoundLocalizationProvider<IReadOnlyList<string>>(
            new EnglishFallbackLocalizationProvider<IReadOnlyList<string>>(
                new JsonLocalizationProvider(
                    tokenExtractor: new SimpleLocalizationTokenExtractor(),
                    localeStreamFunction: localeStreamFunction
                )
            ),
            list =>
            {
                var msg = Format(list);
                MainModFile.LogError("Failed to Loc [{}]", msg);
                return msg;
            }
        );    
    }

    static void InjectStory(DialogueRegistry<StoryNode> reg, NodeType newNodeType)
    {
        foreach (var (key, node) in reg.Get())
        {
            var realKey = reg.GetRealKey(key, node);
            //MainModFile.Log("Injecting story [{}]", Format(key));
            node.type = newNodeType;
            DB.story.all[realKey] = node;

            for (var i = 0; i < node.lines.Count; i++)
            {
                if (node.lines[i] is Say say)
                {
                    say.hash = i.ToString();
                }
                else if (node.lines[i] is SaySwitch saySwitch)
                {
                    if (saySwitch.HasSplitFlag())
                    {
                        foreach (var saySwitchLine in saySwitch.lines)
                        {
                            saySwitchLine.hash = i.ToString();
                        }
                    }
                    else
                    {
                        int switchIndex = 0;
                        foreach (var saySwitchLine in saySwitch.lines)
                        {
                            if (saySwitchLine.HasCopyFlag())
                            {
                                switchIndex--;
                            }
                            saySwitchLine.hash = i + ":" + switchIndex;
                            switchIndex++;
                        }
                    }
                }
            }
        }
    }

    static void InjectSwitches(DialogueRegistry<Say> reg, NodeType newNodeType)
    {
        foreach (var (key, line) in reg.Get())
        {
            var realKey = reg.GetRealKey(key, line);
            //MainModFile.Log("Injected switch [{}]", Format(key));
            if (!DB.story.all.TryGetValue(realKey, out var node))
            {
                MainModFile.Log("Failed to Register Missing SaySwitch Node {}", realKey);
                continue;
            }

            // TODO can only hit last switch
            if (node.lines.OfType<SaySwitch>().LastOrDefault() is not { } saySwitch)
            {
                MainModFile.Log("Failed to Register Non-SaySwitch Node {}", realKey);
                continue;
            }

            if (string.IsNullOrEmpty(line.hash))
                line.hash = $"{line.who}::{realKey}";
            saySwitch.lines.Add(line);
        }
    }

    static void InjectHashReplacements(DialogueRegistry<Say> reg, NodeType newNodeType)
    {
        foreach (var (key, line) in reg.Get())
        {
            var realKey = reg.GetRealKey(key, line);
            if (!DB.story.all.TryGetValue(realKey, out var node))
            {
                MainModFile.Log("Failed to Register Missing Hash Replacement {}", realKey);
                continue;
            }

            if (node.lines.OfType<Say>().FirstOrDefault(s => s.hash == line.hash) is not { } say)
            {
                MainModFile.Log("Failed to Find Node {}", realKey);
                continue;
            }
            
            var data = say.GetOrMakeData(DialoguePatches.SayPatch.ReplacementKey, new Dictionary<string, Say>());
            data[line.who] = line;
        }
    }

    static void LocalizeStory(INonNullLocalizationProvider<IReadOnlyList<string>> loc, DialogueRegistry<StoryNode> reg,
        LoadStringsForLocaleEventArgs e)
    {
        foreach (var (key, node) in reg.Get())
        {
            var realKey = reg.GetRealKey(key, node);
            var lookupKey = reg.GetLookupKey(key, node);
            //MainModFile.Log("Attempting to loc story [{}]", Format(key));

            var index = 0;
            foreach (var line in node.lines)
            {
                if (line is Say say)
                {
                    e.Localizations[$"{realKey}:{index}"] = loc.Localize(e.Locale, [..lookupKey, index.ToString()]);
                }
                else if (line is Wait or Jump)
                {
                    index--;
                }
                else if (line is SaySwitch saySwitch)
                {
                    if (saySwitch.HasSplitFlag())
                    {
                        e.Localizations[$"{realKey}:{index}"] = loc.Localize(e.Locale, [..lookupKey, index.ToString()]);
                    }
                    else
                    {
                        int switchIndex = 0;
                        foreach (var saySwitchLine in saySwitch.lines)
                        {
                            if (saySwitchLine.HasCopyFlag())
                            {
                                continue;
                            }
                            e.Localizations[$"{realKey}:{index}:{switchIndex}"] = loc.Localize(e.Locale, [..lookupKey, index.ToString(), switchIndex.ToString()]);
                            switchIndex++;
                        }
                    }
                }
                else
                {
                    throw new ArgumentException($"Unhandled story node type {line.GetType().Name} for key {realKey}");
                }
                index++;
            }
        }
    }

    static void LocalizeSwitches(INonNullLocalizationProvider<IReadOnlyList<string>> loc,
        DialogueRegistry<Say> reg,
        LoadStringsForLocaleEventArgs e)
    {
        foreach (var (key, line) in reg.Get())
        {
            var realKey = reg.GetRealKey(key, line);
            var lookupKey = reg.GetLookupKey(key, line);
            //MainModFile.Log("Attempting to loc switch [{}]", Format(key));
            if (string.IsNullOrEmpty(line.hash))
                line.hash = $"{line.who}::{realKey}";

            e.Localizations[$"{realKey}:{line.hash}"] = loc.Localize(e.Locale, lookupKey);
        }
    }

    static void LocalizeHashReplacements(INonNullLocalizationProvider<IReadOnlyList<string>> loc,
        DialogueRegistry<Say> reg,
        LoadStringsForLocaleEventArgs e)
    {
        foreach (var (key, line) in reg.Get())
        {
            var realKey = reg.GetRealKey(key, line);
            var lookupKey = reg.GetLookupKey(key, line);
            e.Localizations[$"{realKey}:{line.who}::{line.hash}"] = loc.Localize(e.Locale, lookupKey);
        }
    }
    
    static void InjectStory(string lookupKey, string characterType, Dictionary<IReadOnlyList<string>, StoryNode> newNodes, Dictionary<IReadOnlyList<string>, StoryNode> newHardcodedNodes, Dictionary<IReadOnlyList<string>, Say> saySwitchNodes, NodeType newNodeType)
    {
        foreach (var (key, node) in newNodes)
        {
            var realKey = MainModFile.MakeID(string.Join(".", [lookupKey, .. key]));

            node.type = newNodeType;
            DB.story.all[realKey] = node;

            for (var i = 0; i < node.lines.Count; i++)
            {
                if (node.lines[i] is Say say)
                {
                    say.hash = i.ToString();
                }
                else if (node.lines[i] is SaySwitch saySwitch)
                {
                    if (saySwitch.HasSplitFlag())
                    {
                        foreach (var saySwitchLine in saySwitch.lines)
                        {
                            saySwitchLine.hash = i.ToString();
                        }
                    }
                    else
                    {
                        int switchIndex = 0;
                        foreach (var saySwitchLine in saySwitch.lines)
                        {
                            if (saySwitchLine.HasCopyFlag())
                            {
                                switchIndex--;
                            }
                            saySwitchLine.hash = i + ":" + switchIndex;
                            switchIndex++;
                        }
                    }
                }
            }
            
            //MainModFile.Log("Register Normal Node {}", realKey);
        }

        foreach (var (key, node) in newHardcodedNodes)
        {
            var realKey = string.Join(".", key.Select(s => s.Replace("{{CharacterType}}", characterType)));

            node.type = newNodeType;
            DB.story.all[realKey] = node;

            for (var i = 0; i < node.lines.Count; i++)
                if (node.lines[i] is Say say)
                    say.hash = i.ToString();
            
            //MainModFile.Log("Register Hardcoded Node {}", realKey);
        }

        foreach (var (key, line) in saySwitchNodes)
        {
            var realKey = string.Join(".", key);
            if (!DB.story.all.TryGetValue(realKey, out var node))
            {
                MainModFile.Log("Failed to Register Missing SaySwitch Node {}", realKey);
                continue;
            }

            if (node.lines.OfType<SaySwitch>().LastOrDefault() is not { } saySwitch)
            {
                MainModFile.Log("Failed to Register Non-SaySwitch Node {}", realKey);
                continue;
            }

            if (string.IsNullOrEmpty(line.hash))
                line.hash = $"{characterType}::{realKey}";
            saySwitch.lines.Add(line);
            
            //MainModFile.Log("Register SaySwitch Node {}", realKey);
        }
    }
    
    static void InjectLocalizations(string lookupKey, string characterType, INonNullLocalizationProvider<IReadOnlyList<string>> loc, Dictionary<IReadOnlyList<string>, StoryNode> newNodes, Dictionary<IReadOnlyList<string>, StoryNode> newHardcodedNodes, Dictionary<IReadOnlyList<string>, Say> saySwitchNodes, LoadStringsForLocaleEventArgs e)
    {
        foreach (var (key, node) in newNodes)
        {
            var realKey = MainModFile.MakeID(string.Join(".", [lookupKey, .. key]));

            var index = 0;
            foreach (var line in node.lines)
            {
                if (line is Say say)
                {
                    e.Localizations[$"{realKey}:{index}"] = loc.Localize(e.Locale, [lookupKey, .. key, index.ToString()]);
                    //MainModFile.Log("Loc Normal Node {} -> {}", $"{realKey}:{index}", e.Localizations[$"{realKey}:{index}"]);
                }
                else if (line is Wait or Jump)
                {
                    //MainModFile.Log("Skipping non say line in {}", realKey);
                    index--;
                }
                else if (line is SaySwitch saySwitch)
                {
                    if (saySwitch.HasSplitFlag())
                    {
                        e.Localizations[$"{realKey}:{index}"] = loc.Localize(e.Locale, [lookupKey, .. key, index.ToString()]);
                    }
                    else
                    {
                        int switchIndex = 0;
                        foreach (var saySwitchLine in saySwitch.lines)
                        {
                            if (saySwitchLine.HasCopyFlag())
                            {
                                continue;
                            }
                            e.Localizations[$"{realKey}:{index}:{switchIndex}"] = loc.Localize(e.Locale, [lookupKey, .. key, index.ToString(), switchIndex.ToString()]);
                            switchIndex++;
                        }
                    }
                }
                else
                {
                    throw new ArgumentException($"Unhandled story node type {line.GetType().Name} for key {realKey}");
                }
                index++;
            }
        }

        foreach (var (key, node) in newHardcodedNodes)
        {
            var realKey = string.Join(".", key.Select(s => s.Replace("{{CharacterType}}", characterType)));

            var index = 0;
            foreach (var line in node.lines)
            {
                if (line is Say say)
                {
                    e.Localizations[$"{realKey}:{index}"] = loc.Localize(e.Locale, [lookupKey, .. key, index.ToString()]);
                    //MainModFile.Log("Loc Hardcoded Node {} -> {}", $"{realKey}:{index}", e.Localizations[$"{realKey}:{index}"]);
                }
                else if (line is Wait or Jump)
                {
                    //MainModFile.Log("Skipping non say line in {}", realKey);
                    index--;
                }
                else
                {
                    throw new ArgumentException($"Unhandled story node type {line.GetType().Name} for key {realKey}");
                }
                index++;
            }
        }

        foreach (var (key, line) in saySwitchNodes)
        {
            var realKey = string.Join(".", key);
            if (string.IsNullOrEmpty(line.hash))
                line.hash = $"{characterType}::{realKey}";

            e.Localizations[$"{realKey}:{line.hash}"] = loc.Localize(e.Locale, [lookupKey, .. key]);
            
            //MainModFile.Log("Loc SaySwitch Node {} -> {}", $"{realKey}:{line.hash}", e.Localizations[$"{realKey}:{line.hash}"]);
        }
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

internal interface IFlippableAction
{
    bool CanFlip(State s);
}

internal interface IHilightingAction
{
    void HilightOtherCards(Card owner, State s, Combat c);
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