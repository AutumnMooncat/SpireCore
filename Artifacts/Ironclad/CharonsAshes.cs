using Nickel;
using System.Reflection;

namespace AutumnMooncat.SpireCore.Artifacts.Ironclad;

internal sealed class CharonsAshes : Artifact, IRArtifact, IArtifactOnExhaust
{
    public static string ID => nameof(CharonsAshes);
    public static IArtifactEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Artifacts.RegisterArtifact(ID, new ArtifactConfiguration
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new ArtifactMeta
            {
                owner = Characters.Ironclad.DeckEntry!.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = IRArtifact.LookUpSpr(Characters.Ironclad.ArtifactAssetPath + ID)!.Value,
            Name = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "description"]).Localize
        });
    }


    public void OnExhaust(State s, Combat c, Card card)
    {
        c.QueueImmediate(new AAttack()
        {
            damage = Card.GetActualDamage(s, 1),
            targetPlayer = false,
            fast = true,
        });
    }
}
