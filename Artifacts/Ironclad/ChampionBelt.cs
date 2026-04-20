using System.Collections.Generic;
using Nickel;
using System.Reflection;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Artifacts.Ironclad;

internal sealed class ChampionBelt : Artifact, IRArtifact
{
    public static string ID => nameof(ChampionBelt);
    public static IArtifactEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Artifacts.RegisterArtifact(ID, new ArtifactConfiguration
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new ArtifactMeta
            {
                owner = Characters.Ironclad.DeckEntry!.Deck,
                pools = [ArtifactPool.Common],
                extraGlossary = ["status.overdriveAlt"]
            },
            Sprite = IRArtifact.LookUpSpr(Characters.Ironclad.ArtifactAssetPath + ID)!.Value,
            Name = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "description"]).Localize
        });
    }
    
    public override List<Tooltip> GetExtraTooltips()
    {
        return [
            new GlossaryTooltip(FeebledriveStatus.ID)
            {
                Icon = FeebledriveStatus.Entry.Configuration.Definition.icon,
                TitleColor = Colors.status,
                Title = MainModFile.Loc(["status", FeebledriveStatus.ID, "name"]),
                Description = MainModFile.Loc(["status", FeebledriveStatus.ID, "descriptionAlt"]),
            }
        ];
    }

    public override void AfterPlayerStatusAction(State state, Combat combat, Status status, AStatusMode mode, int statusAmount)
    {
        if (status != Status.overdrive || mode != AStatusMode.Add || statusAmount <= 0)
        {
            return;
        }
        combat.QueueImmediate(new AStatus()
        {
            status = FeebledriveStatus.Entry.Status,
            statusAmount = 1,
            targetPlayer = false,
            artifactPulse = Key()
        });
    }
}
