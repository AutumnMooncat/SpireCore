using Nickel;
using System.Reflection;

namespace AutumnMooncat.SpireCore.Artifacts.Defect;

internal sealed class EmotionChip : Artifact, IRArtifact
{
    public static string ID => nameof(EmotionChip);
    public static IArtifactEntry Entry { get; set; }
    public static Spr OnSpr { get; set; }
    public static Spr OffSpr { get; set; }

    public static void Register(IModHelper helper)
    {
        OnSpr = IRArtifact.LookUpSpr(Characters.Defect.ArtifactAssetPath + ID)!.Value;
        OffSpr = IRArtifact.LookUpSpr(Characters.Defect.ArtifactAssetPath + ID + "Off")!.Value;
        Entry = helper.Content.Artifacts.RegisterArtifact(ID, new ArtifactConfiguration
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new ArtifactMeta
            {
                owner = Characters.Defect.DeckEntry!.Deck,
                pools = [ArtifactPool.Boss],
                extraGlossary = ["status.perfectShield"]
            },
            Sprite = OnSpr,
            Name = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "description"]).Localize
        });
    }
    
    public bool alreadyActivated;

    public override Spr GetSprite()
    {
        return alreadyActivated ? OffSpr : OnSpr;
    }

    public override void OnPlayerLoseHull(State state, Combat combat, int amount)
    {
        if (alreadyActivated)
            return;
        alreadyActivated = true;
        combat.QueueImmediate(new AStatus()
        {
            status = Status.perfectShield,
            statusAmount = 1,
            targetPlayer = true
        });
    }

    public override void OnCombatEnd(State state)
    {
        alreadyActivated = false;
    }
}
