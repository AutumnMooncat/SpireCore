using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Cards.Watcher;
using AutumnMooncat.SpireCore.Features;
using AutumnMooncat.SpireCore.Util;

namespace AutumnMooncat.SpireCore.Artifacts.Watcher;

internal sealed class Melange : Artifact, IRArtifact
{
    public static string ID => nameof(Melange);
    public static IArtifactEntry Entry { get; set; }
    public static int Count => 3;
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Artifacts.RegisterArtifact(ID, new ArtifactConfiguration
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new ArtifactMeta
            {
                owner = Characters.Watcher.DeckEntry!.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = IRArtifact.LookUpSpr(Characters.Watcher.ArtifactAssetPath + ID)!.Value,
            Name = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "description"], new {Count}).Localize
        });
    }

    public override List<Tooltip> GetExtraTooltips()
    {
        return [
            new GlossaryTooltip(AScry.ID)
            {
                Icon = CommonIcons.Scry,
                TitleColor = Colors.action,
                Title = MainModFile.Loc(["action", AScry.ID, "title"]),
                Description = MainModFile.Loc(["action", AScry.ID, "description"], new {Cards = Count}),
            }
        ];
    }
    
    public override void OnCombatStart(State state, Combat combat)
    {
        combat.QueueImmediate(new AScry()
        {
            count = Count
        });
    }
}
