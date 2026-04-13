using Nickel;
using System.Collections.Generic;
using System.Reflection;
using AutumnMooncat.SpireCore.Cards.Defect;

namespace AutumnMooncat.SpireCore.Artifacts.Defect;

internal sealed class RunicCapacitor : Artifact, IRArtifact
{
    public static string ID => nameof(RunicCapacitor);
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
        //return s.route is Combat ? count : null;
    }

    public override void OnCombatEnd(State state) 
    {
        if (state.route is Combat c)
        {
            count = c.energy;
        }
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        if (count > 0)
        {
            combat.QueueImmediate(new AEnergy() {changeAmount = count});
        } 
    }
}
