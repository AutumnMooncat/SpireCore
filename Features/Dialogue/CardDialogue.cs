using System;
using System.Collections.Generic;
using AutumnMooncat.SpireCore.Cards.Ironclad;
using Nickel;

namespace AutumnMooncat.SpireCore.Features.Dialogue;

internal sealed class CardDialogue : IRDialogue
{
	public static readonly Dictionary<Type, string> CardLookups = [];

	public static string RegisterLookup(Type t)
	{
		return CardLookups.GetOrAddValue(t, $"Played::{t.FullName}".PrefixID());
	}
	
	public static void Register(IModHelper helper)
	{
		var loc = IRDialogue.GetLoc(locale => MainModFile.GetFile($"i18n/dialogue-card-{locale}.json").OpenRead());
		var cardNodes = MakeCardNodes();
		
		helper.Events.OnModLoadPhaseFinished += (_, phase) =>
		{
			if (phase != ModLoadPhase.AfterDbInit)
				return;
			IRDialogue.InjectStory("Played", null, cardNodes, [], [], NodeType.combat);
		};
		
		helper.Events.OnLoadStringsForLocale += (_, e) =>
		{
			IRDialogue.InjectLocalizations("Played", null, loc, cardNodes, [], [], e);
		};
	}

	public static Dictionary<IReadOnlyList<string>, StoryNode> MakeCardNodes()
	{
		var newNodes = new Dictionary<IReadOnlyList<string>, StoryNode>();

		/*RegisterLookup(typeof(Anger));
		newNodes[[nameof(Anger)]] = new()
		{
			lookup = [CardLookups[typeof(Anger)]],
			priority = true,
			allPresent = [CType.Ironclad],
			lines = [
				new Say { who = CType.Ironclad, loopTag = Anim.Ironclad.Mad },
				new SaySwitch()
				{
					lines = [
						new Say { who = CType.Defect, loopTag = Anim.Squint },
						new Say { who = CType.Silent, loopTag = Anim.Squint },
						..new Say { loopTag = Anim.Neutral}.CopyWithout(CType.Ironclad, CType.Defect, CType.Silent)
					]
				}
			],
		};*/

		RegisterLookup(typeof(Juggernaut));
		newNodes[[nameof(Juggernaut)]] = new()
		{
			lookup = [CardLookups[typeof(Juggernaut)]],
			priority = true,
			oncePerRun = true,
			allPresent = [CType.Ironclad],
			lines = [
				new Say { who = CType.Ironclad, loopTag = Anim.Neutral },
				new Say { loopTag = Anim.Squint }.SplitWithout(CType.Ironclad)
			],
		};
		
		RegisterLookup(typeof(ShieldGun));
		newNodes[[nameof(ShieldGun)]] = new()
		{
			lookup = [CardLookups[typeof(ShieldGun)]],
			priority = true,
			oncePerRun = true,
			allPresent = [CType.Ironclad, CType.Dizzy],
			lines = [
				new Say { who = CType.Ironclad, loopTag = Anim.Squint },
				new Say { who = CType.Dizzy, loopTag = Anim.Neutral }
			],
		};
		
		return newNodes;
	}
}