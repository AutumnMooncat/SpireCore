using Nickel;
using System.Reflection;

namespace AutumnMooncat.SpireCore.Artifacts.Ship;

[IRegisterable.Ignore]
internal sealed class DemoArtifactCounting : Artifact, IRArtifact
{
    public static string ID => nameof(DemoArtifactCounting);
    public static IArtifactEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Artifacts.RegisterArtifact("Counting", new ArtifactConfiguration
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new ArtifactMeta
            {
                owner = Deck.colorless,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites.RegisterSprite(MainModFile.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/counting.png")).Sprite,
            Name = MainModFile.Instance.AnyLocalizations.Bind(["artifact", "Counting", "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["artifact", "Counting", "description"]).Localize
        });
    }

    public int counter;

    public override void OnTurnStart(State s, Combat c)
    {
        if (!c.isPlayerTurn)
            return;
        counter += 1;
        if (counter == 7)
        {
            c.QueueImmediate(new AEnergy()
            {
                changeAmount = 1
            });
            counter = 0;
            Pulse();
        }
    }

    public override void OnReceiveArtifact(State state)
    {
        this.counter = 0;
    }

    public override int? GetDisplayNumber(State s)
    {
        if (this.counter != 0)
            return this.counter;
        return null;
    }
}
