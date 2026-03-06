using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.Spirecore.Features;

namespace AutumnMooncat.Spirecore.Artifacts.Watcher;

internal sealed class Damaru : Artifact, IRArtifact
{
    public static string ID => nameof(Damaru);
    public static IArtifactEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Artifacts.RegisterArtifact(ID, new ArtifactConfiguration
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new ArtifactMeta
            {
                owner = Characters.Watcher.DeckEntry!.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = IRArtifact.LookUpSpr(Characters.Watcher.ArtifactAssetPath + ID)!.Value,
            Name = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "description"]).Localize
        });
    }

    public override List<Tooltip> GetExtraTooltips()
    {
        return [
            new GlossaryTooltip(MantraStatus.ID)
            {
                Icon = MantraStatus.Entry.Configuration.Definition.icon,
                TitleColor = Colors.status,
                Title = MainModFile.Loc(["status", MantraStatus.ID, "name"]),
                Description = MainModFile.Loc(["status", MantraStatus.ID, "description"]),
            }
        ];
    }
    
    public int count;

    public override int? GetDisplayNumber(State s)
    {
        return count;
    }

    public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition,
        int handCount)
    {
        if (deck == Characters.Watcher.DeckEntry.Deck)
        {
            count++;
            Pulse();
        }

        if (count < 5)
        {
            return;
        }

        count = 0;
        combat.QueueImmediate(new AStatus()
        {
            status = MantraStatus.Entry.Status,
            statusAmount = 2,
            targetPlayer = true
        });
    }
}
