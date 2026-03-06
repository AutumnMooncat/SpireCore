using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Cards.Defect;

namespace AutumnMooncat.Spirecore.Artifacts.Defect;

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
        return [
            new TTCard
            {
                card = new Fusion()
            }
        ];
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        combat.QueueImmediate(new AAddCard
        {
            card = new Fusion(),
            destination = CardDestination.Hand,
            amount = 2
        });
    }
}
