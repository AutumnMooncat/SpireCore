using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Cards.Watcher;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Artifacts.Watcher;

internal sealed class PureWater : Artifact, IRArtifact
{
    public static string ID => nameof(PureWater);
    public static IArtifactEntry Entry { get; set; }
    
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
            Description = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "description"]).Localize
        });
    }

    public override List<Tooltip> GetExtraTooltips()
    {
        return [
            new TTCard
            {
                card = new Miracle()
            },
            new TTGlossary("cardtrait.retain"),
        ];
    }
    
    public override void OnCombatStart(State state, Combat combat)
    {
        combat.QueueImmediate(new AAddCard
        {
            card = new Miracle(),
            destination = CardDestination.Hand,
            amount = 1
        });
    }
}
