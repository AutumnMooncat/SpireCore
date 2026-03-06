using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace AutumnMooncat.Spirecore.Artifacts.Silent;

internal sealed class WristBlade : Artifact, IRArtifact
{
    public static string ID => nameof(WristBlade);
    public static IArtifactEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Artifacts.RegisterArtifact(ID, new ArtifactConfiguration
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new ArtifactMeta
            {
                owner = Characters.Silent.DeckEntry!.Deck,
                pools = [ArtifactPool.Boss]
            },
            Sprite = IRArtifact.LookUpSpr(Characters.Silent.ArtifactAssetPath + ID)!.Value,
            Name = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "description"]).Localize
        });
    }

    private bool _lock;
    
    public override int ModifyBaseDamage(int baseDamage, Card card, State state, Combat combat, bool fromPlayer)
    {
        if (!fromPlayer || _lock || card == null)
        {
            return 0;
        }

        _lock = true;
        var data = card.GetDataWithOverrides(state);
        _lock = false;
        return data.cost == 0 ? 1 : 0;
    }
}
