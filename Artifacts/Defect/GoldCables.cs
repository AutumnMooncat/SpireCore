using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Cards.Defect;

namespace AutumnMooncat.Spirecore.Artifacts.Defect;

internal sealed class GoldCables : Artifact, IRArtifact
{
    public static string ID => nameof(GoldCables);
    public static IArtifactEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Artifacts.RegisterArtifact(ID, new ArtifactConfiguration
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new ArtifactMeta
            {
                owner = Characters.Defect.DeckEntry!.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = IRArtifact.LookUpSpr(Characters.Defect.ArtifactAssetPath + ID)!.Value,
            Name = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "description"]).Localize
        });
    }

    public int count;

    public override int? GetDisplayNumber(State s)
    {
        return count;
    }

    public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition,
        int handCount)
    {
        if (deck == Characters.Defect.DeckEntry.Deck)
        {
            count++;
            Pulse();
        }

        if (count < 4)
        {
            return;
        }

        count = 0;
        combat.QueueImmediate(new ADrawCard(){count = 1});
    }
}
