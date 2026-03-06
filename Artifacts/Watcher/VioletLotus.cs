using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Features;

namespace AutumnMooncat.Spirecore.Artifacts.Watcher;

internal sealed class VioletLotus : Artifact, IRArtifact
{
    public static string ID => nameof(VioletLotus);
    public static IArtifactEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Artifacts.RegisterArtifact(ID, new ArtifactConfiguration
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new ArtifactMeta
            {
                owner = Characters.Watcher.DeckEntry!.Deck,
                pools = [ArtifactPool.Boss]
            },
            Sprite = IRArtifact.LookUpSpr(Characters.Watcher.ArtifactAssetPath + ID)!.Value,
            Name = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "description"]).Localize
        });
    }

    public override List<Tooltip> GetExtraTooltips()
    {
        return [
            new GlossaryTooltip(MantraStatus.ID)
            {
                Icon = CalmStatus.Entry.Configuration.Definition.icon,
                TitleColor = Colors.status,
                Title = MainModFile.Loc(["status", CalmStatus.ID, "name"]),
                Description = MainModFile.Loc(["status", CalmStatus.ID, "description"], new {Max = CalmStatus.MaxCalm}),
            }
        ];
    }

    public int ModifyCalmEnergy(State s, Combat c, int amount)
    {
        Pulse();
        return amount + 1;
    }
}
