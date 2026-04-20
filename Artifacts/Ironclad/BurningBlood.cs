using System.Collections.Generic;
using Nickel;
using System.Reflection;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Artifacts.Ironclad;

internal sealed class BurningBlood : Artifact, IRArtifact
{
    public static string ID => nameof(BurningBlood);
    public static IArtifactEntry Entry { get; set; }
    public static Spr OnSpr { get; set; }
    public static Spr OffSpr { get; set; }
    
    public static void Register(IModHelper helper)
    {
        OnSpr = IRArtifact.LookUpSpr(Characters.Ironclad.ArtifactAssetPath + ID+"2")!.Value;
        OffSpr = IRArtifact.LookUpSpr(Characters.Ironclad.ArtifactAssetPath + ID+"2Off")!.Value;
        Entry = helper.Content.Artifacts.RegisterArtifact(ID, new ArtifactConfiguration
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new ArtifactMeta
            {
                owner = Characters.Ironclad.DeckEntry!.Deck,
                pools = [ArtifactPool.Common],
            },
            Sprite = OnSpr,
            Name = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "name"]).Localize,
            Description = MainModFile.Instance.AnyLocalizations.Bind(["artifact", ID, "description"]).Localize
        });
    }

    public bool triggered;
    
    public override List<Tooltip> GetExtraTooltips()
    {
        return [
            new GlossaryTooltip(HeatCapStatus.ID)
            {
                Icon = HeatCapStatus.Entry.Configuration.Definition.icon,
                TitleColor = Colors.status,
                Title = MainModFile.Loc(["status", HeatCapStatus.ID, "name"]),
                Description = MainModFile.Loc(["status", HeatCapStatus.ID, "descriptionAlt"]),
            }
        ];
    }

    public override Spr GetSprite()
    {
        return triggered ? OffSpr : OnSpr;
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
                new AHeal()
                {
                    healAmount = 1,
                    targetPlayer = true,
                    artifactPulse = Key()
                },
                new AStatus()
                {
                    status = HeatCapStatus.Entry.Status,
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
