using Nickel;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using System;
using System.Collections.Generic;
using System.Linq;
using AutumnMooncat.Spirecore.Actions;
using AutumnMooncat.Spirecore.ExternalAPI;
using AutumnMooncat.Spirecore.Features;
using AutumnMooncat.Spirecore.Util;

/* In the Cobalt Core modding community it is common for namespaces to be <Author>.<ModName>
 * This is helpful to know at a glance what mod we're looking at, and who made it */
namespace AutumnMooncat.Spirecore;

/* MainModFile is the base for our mod. Others like to name it Manifest, and some like to name it <ModName>
 * Notice the ': SimpleMod'. This means MainModFile is a subclass (child) of the superclass SimpleMod (parent) from Nickel. This will help us use Nickel's functions more easily! */
public sealed class MainModFile : SimpleMod
{
    internal static bool DevBuild => true;
    internal static MainModFile Instance { get; private set; } = null!;
    internal IKokoroApi KokoroApi { get; }
    internal IModHelper KokoroHelper { get; }
    internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
    internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }
    
    internal Harmony Harmony { get; }

    public MainModFile(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
    {
        Instance = this;

        /* We use Kokoro to handle our statuses. This means Kokoro is a Dependency, and our mod will fail to work without it.
         * We take from Kokoro what we need and put in our own project. Head to ExternalAPI/StatusLogicHook.cs if you're interested in what, exactly, we use.
         * If you're interested in more fancy stuff, make sure to peek at the Kokoro repository found online. */
        KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!;
        KokoroHelper = helper.ModRegistry.GetModHelper(helper.ModRegistry.LoadedMods["Shockah.Kokoro"]);

        /* These localizations lists help us organize our mod's text and messages by language.
         * For general use, prefer AnyLocalizations, as that will provide an easier time to potential localization submods that are made for your mod
         * IMPORTANT: These localizations are found in the i18n folder (short for internationalization). The Demo Mod comes with a barebones en.json localization file that you might want to check out before continuing
         * Whenever you add a card, artifact, character, ship, pretty much whatever, you will want to update your locale file in i18n with the necessary information
         * Example: You added your own character, you will want to create an appropiate entry in the i18n file.
         * If you would rather use simple strings whenever possible, that's also an option -you do you. */
        AnyLocalizations = new JsonLocalizationProvider(
            tokenExtractor: new SimpleLocalizationTokenExtractor(),
            localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"i18n/{locale}.json").OpenRead()
        );
        Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
            new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(AnyLocalizations)
        );

        AutoAdd(typeof(IRStatus), nameof(IRStatus.Register), helper);
        AutoAdd(typeof(IRCharacter), nameof(IRCharacter.Register), helper);
        AutoAdd(typeof(IRCard), nameof(IRCard.Register), helper);
        AutoAdd(typeof(IRArtifact), nameof(IRArtifact.Register), helper);
        AutoAdd(typeof(IRShip), nameof(IRShip.Register), helper);
        AutoAdd(typeof(IRTrait), nameof(IRTrait.Register), helper);
        AutoAdd(typeof(IRDialogue), nameof(IRDialogue.Register), helper);
        CommonIcons.Register(helper);
        
        Harmony = new Harmony(Instance.Package.Manifest.UniqueName);
        Harmony.PatchAll();
        helper.Events.OnModLoadPhaseFinished += OnModLoadPhaseFinished;
    }

    private static void OnModLoadPhaseFinished(object sender, ModLoadPhase e)
    {
        if (e == ModLoadPhase.AfterDbInit && DevBuild)
        {
            //Log("Attempting to open imgui editor");
            MG.inst.g.e = new Editor();
            MG.inst.g.e.IMGUI_Setup(MG.inst);
            MG.inst.g.e.isActive = true;
        }
    }

    internal static void AutoAdd(Type type, string method, params object[] paramz)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(t => t.IsAssignableTo(type) && !Attribute.IsDefined(t, typeof(IRegisterable.Ignore)));
        foreach (var t in types)
        {
            AccessTools.DeclaredMethod(t, method)?.Invoke(null, paramz);
        }
    }

    internal static IModHelper GetHelper()
    {
        return Instance.Helper;
    }

    internal static IPluginPackage<IModManifest> GetPackage()
    {
        return Instance.Package;
    }

    internal static ISpriteEntry MakeSprite(string path)
    {
        return MakeSprite(GetFile(path));
    }

    internal static ISpriteEntry MakeSprite(IFileInfo file)
    {
        return GetHelper().Content.Sprites.RegisterSprite(file);
    }

    internal static IFileInfo GetFile(string path)
    {
        return GetPackage().PackageRoot.GetRelativeFile(path);
    }

    internal static string MakeID(string s)
    {
        return Instance.Package.Manifest.UniqueName + "::" + s;
    }

    internal static string Loc(IReadOnlyList<string> key, object tokens = null)
    {
        return Instance.Localizations.Localize(key, tokens);
    }

    internal static IKeyAndTokensBoundLocalizationProvider Bind(IReadOnlyList<string> key, object tokens = null)
    {
        return Instance.AnyLocalizations.Bind(key, tokens);
    }

    internal static void Log(string s, params object[] args)
    {
        Instance.Logger.LogInformation(s, args);
    }

    internal static IKokoroApi.IV2 Kokoro()
    {
        return Instance.KokoroApi.V2;
    }

    internal static void SetData<T>(object o, string key, T data)
    {
        Instance.Helper.ModData.SetModData(o, key, data);
    }

    internal static bool GetData<T>(object o, string key, out T data)
    {
        return Instance.Helper.ModData.TryGetModData(o, key, out data);
    }

    internal static CardAction AddTooltips(List<Tooltip> tips)
    {
        return Kokoro().HiddenActions.MakeAction(new InfoOnlyAction()
        {
            tips = tips
        }).SetShowTooltips(true).AsCardAction;
    }
    
    internal static CardAction AddTooltipsDel(DelegateAction.TipDel tips)
    {
        return Kokoro().HiddenActions.MakeAction(new DelegateAction()
        {
            getTips = tips
        }).SetShowTooltips(true).AsCardAction;
    }
}
