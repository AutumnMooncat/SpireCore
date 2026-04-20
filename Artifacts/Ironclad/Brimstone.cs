using Nickel;
using System.Reflection;

namespace AutumnMooncat.SpireCore.Artifacts.Ironclad;

internal sealed class Brimstone : Artifact, IRArtifact
{
    public static string ID => nameof(Brimstone);
    public static IArtifactEntry Entry { get; set; }
    public static int Amount => 2;
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Artifacts.RegisterArtifact(ID, new ArtifactConfiguration
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new ArtifactMeta
            {
                owner = Characters.Ironclad.DeckEntry!.Deck,
                pools = [ArtifactPool.Boss],
                extraGlossary = ["status.overdriveAlt"]
            },
            Sprite = IRArtifact.LookUpSpr(Characters.Ironclad.ArtifactAssetPath + ID)!.Value,
            Name = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "description"], new {Amount}).Localize
        });
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if (combat.turn != 1)
        {
            return;
        }
        
        combat.QueueImmediate([
            new AStatus()
            {
                status = Status.overdrive,
                statusAmount = Amount,
                targetPlayer = true
            },
            new AStatus()
            {
                status = Status.overdrive,
                statusAmount = Amount,
                targetPlayer = false
            }
        ]);
    }
}
