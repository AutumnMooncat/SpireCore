using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace AutumnMooncat.Spirecore.Artifacts.Ironclad;

internal sealed class MarkOfPain : Artifact, IRArtifact
{
    public static string ID => nameof(MarkOfPain);
    public static IArtifactEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Artifacts.RegisterArtifact(ID, new ArtifactConfiguration
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new ArtifactMeta
            {
                owner = Characters.Ironclad.DeckEntry!.Deck,
                pools = [ArtifactPool.Boss]
            },
            Sprite = IRArtifact.LookUpSpr(Characters.Ironclad.ArtifactAssetPath + ID)!.Value,
            Name = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "description"]).Localize
        });
    }

    public override List<Tooltip> GetExtraTooltips()
    {
        return [
            new TTCard
            {
                card = new TrashUnplayable()
            }
        ];
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        combat.QueueImmediate(new AAddCard
        {
            card = new TrashUnplayable(),
            destination = CardDestination.Deck,
            insertRandomly = true,
            amount = 2
        });
    }

    public override void OnReceiveArtifact(State state)
    {
        state.ship.baseEnergy++;
    }

    public override void OnRemoveArtifact(State state)
    {
        state.ship.baseEnergy--;
    }
}
