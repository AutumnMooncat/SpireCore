using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace AutumnMooncat.Spirecore.Artifacts.Ironclad;

internal sealed class RedSkull : Artifact, IRArtifact
{
    public static string ID => nameof(RedSkull);
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
                extraGlossary = ["status.shieldAlt"]
            },
            Sprite = IRArtifact.LookUpSpr(Characters.Ironclad.ArtifactAssetPath + ID)!.Value,
            Name = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "description"]).Localize
        });
    }

    public override int ModifyBaseDamage(int baseDamage, Card card, State state, Combat combat, bool fromPlayer)
    {
        if (!fromPlayer || state.ship.Get(Status.shield) > 0)
        {
            return 0;
        }
        return card?.GetMeta().deck == Entry.Configuration.Meta.owner ? 1 : 0;
    }
}
