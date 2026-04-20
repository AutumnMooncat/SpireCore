using Nickel;
using System.Reflection;

namespace AutumnMooncat.SpireCore.Artifacts.Ironclad;

internal sealed class RedSkull : Artifact, IRArtifact
{
    public static string ID => nameof(RedSkull);
    public static IArtifactEntry Entry { get; set; }
    public static Spr OnSpr { get; set; }
    public static Spr OffSpr { get; set; }
    
    public static void Register(IModHelper helper)
    {
        OnSpr = IRArtifact.LookUpSpr(Characters.Ironclad.ArtifactAssetPath + ID+"Triggered")!.Value;
        OffSpr = IRArtifact.LookUpSpr(Characters.Ironclad.ArtifactAssetPath + ID)!.Value;
        Entry = helper.Content.Artifacts.RegisterArtifact(ID, new ArtifactConfiguration
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new ArtifactMeta
            {
                owner = Characters.Ironclad.DeckEntry!.Deck,
                pools = [ArtifactPool.Common],
                extraGlossary = ["status.powerdriveAlt"]
            },
            Sprite = OffSpr,
            Name = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "description"]).Localize
        });
    }

    public bool triggered;

    public override Spr GetSprite()
    {
        return triggered ? OnSpr : OffSpr;
    }

    public override void OnCombatEnd(State state)
    {
        triggered = false;
    }

    public override void AfterPlayerOverheat(State state, Combat combat)
    {
        if (!triggered)
        {
            triggered = true;
            combat.QueueImmediate([
                new AStatus()
                {
                    status = Status.powerdrive,
                    statusAmount = 1,
                    targetPlayer = true
                }
            ]);
        }
    }

    /*public override int ModifyBaseDamage(int baseDamage, Card card, State state, Combat combat, bool fromPlayer)
    {
        if (!fromPlayer || state.ship.Get(Status.shield) > 0)
        {
            return 0;
        }
        return card?.GetMeta().deck == Entry.Configuration.Meta.owner ? 1 : 0;
    }*/
}
