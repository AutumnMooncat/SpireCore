using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Cards.Defect;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Artifacts.Defect;

internal sealed class NuclearBattery : Artifact, IRArtifact
{
    public static string ID => nameof(NuclearBattery);
    public static IArtifactEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Artifacts.RegisterArtifact(ID, new ArtifactConfiguration
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new ArtifactMeta
            {
                owner = Characters.Defect.DeckEntry!.Deck,
                pools = [ArtifactPool.Boss]
            },
            Sprite = IRArtifact.LookUpSpr(Characters.Defect.ArtifactAssetPath + ID)!.Value,
            Name = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "description"]).Localize
        });
    }
    
    public override List<Tooltip> GetExtraTooltips()
    {
        return [PowerCoreStatus.GetTooltip];
    }
    
    public override void OnTurnStart(State state, Combat combat)
    {
        if (combat.turn != 1)
        {
            return;
        }
        combat.QueueImmediate(new AStatus()
        {
            status = PowerCoreStatus.Entry.Status,
            statusAmount = 2,
            targetPlayer = true
        });
    }
}
