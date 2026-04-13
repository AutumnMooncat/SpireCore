using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Features;
using AutumnMooncat.SpireCore.Util;

namespace AutumnMooncat.SpireCore.Artifacts.Watcher;

internal sealed class Duality : Artifact, IRArtifact
{
    public static string ID => nameof(Duality);
    public static IArtifactEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Artifacts.RegisterArtifact(ID, new ArtifactConfiguration
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new ArtifactMeta
            {
                owner = Characters.Watcher.DeckEntry!.Deck,
                pools = [ArtifactPool.Boss],
                extraGlossary = ["status.tempShieldAlt"]
            },
            Sprite = IRArtifact.LookUpSpr(Characters.Watcher.ArtifactAssetPath + ID)!.Value,
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
        if (Wiz.IsAttack(card, state, combat))
        {
            count++;
            Pulse();
        }

        if (count < 2)
        {
            return;
        }

        count = 0;
        combat.QueueImmediate(new AStatus()
        {
            status = Status.tempShield,
            statusAmount = 1,
            targetPlayer = true
        });
    }
}
