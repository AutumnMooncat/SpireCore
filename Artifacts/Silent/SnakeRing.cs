using System.Linq;
using Nickel;
using System.Reflection;
using AutumnMooncat.SpireCore.Actions;
using AutumnMooncat.SpireCore.Features;

namespace AutumnMooncat.SpireCore.Artifacts.Silent;

internal sealed class SnakeRing : Artifact, IRArtifact
{
    public static string ID => nameof(SnakeRing);
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
    
    public override void OnTurnStart(State state, Combat combat)
    {
        if (combat.turn != 1)
        {
            return;
        }
        combat.QueueImmediate(new AMoveRandom()
        {
            count = 2
        });
    }

    private class AMoveRandom : CardAction
    {
        public int count;
        
        public override void Begin(G g, State s, Combat c)
        {
            var cards = s.deck.Where(card => card.GetMeta().deck == Entry.Configuration.Meta.owner).ToList();
            if (count > cards.Count)
            {
                count = cards.Count;
            }
            
            for (int i = 0 ; i < count ; i++)
            {
                c.QueueImmediate(new MoveSelectedCardToPile()
                {
                    selectedCard = cards[i]
                });
            }
        }
    }
}
