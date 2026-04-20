using System.Collections.Generic;
using Nickel;
using System.Reflection;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Artifacts.Silent;

internal sealed class NinjaScroll : Artifact, IRArtifact
{
    public static string ID => nameof(NinjaScroll);
    public static IArtifactEntry Entry { get; set; }
    
    public static void Register(IModHelper helper)
    {
        Entry = helper.Content.Artifacts.RegisterArtifact(ID, new ArtifactConfiguration
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new ArtifactMeta
            {
                owner = Characters.Silent.DeckEntry!.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = IRArtifact.LookUpSpr(Characters.Silent.ArtifactAssetPath + ID)!.Value,
            Name = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "description"]).Localize
        });
    }

    public override List<Tooltip> GetExtraTooltips()
    {
        return [ShivStatus.GetTooltip];
    }
    
    public override void OnTurnStart(State state, Combat combat)
    {
        if (combat.turn != 1)
        {
            return;
        }
        combat.QueueImmediate(new AStatus()
        {
            status = ShivStatus.Entry.Status,
            statusAmount = 3,
            targetPlayer = true
        });
    }
}
