using System.Collections.Generic;
using System.Linq;
using Nickel;
using System.Reflection;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Artifacts.Defect;

internal sealed class SymbioticVirus : Artifact, IRArtifact
{
    public static string ID => nameof(SymbioticVirus);
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
    
    public override List<Tooltip> GetExtraTooltips()
    {
        return [
            ITooltipHelper.MakeTooltip("stuff", DarkObject.ID, DarkObject.icon, Colors.drone, null, new {DarkObject.Damage}),
            ChargeStatus.GetTooltip
        ];
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        List<int> source = [];
        for (int key = state.ship.x - 1; key < state.ship.x + state.ship.parts.Count() + 1; ++key)
        {
            if (!combat.stuff.ContainsKey(key))
                source.Add(key);
        }
        var list = source.Shuffle(state.rngActions).Take(2).ToList();
        foreach (int num in list)
        {
            var stuff = new DarkObject()
            {
                x = num,
                xLerped = num,
                targetPlayer = false
            };
            combat.stuff.Add(num, stuff);
        }
        if (list.Count <= 0)
            return;
        Pulse();
    }
}
