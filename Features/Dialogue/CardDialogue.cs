using System.Collections.Generic;
using AutumnMooncat.Spirecore.Cards.Ironclad;
using AutumnMooncat.Spirecore.Characters;
using Nickel;

namespace AutumnMooncat.Spirecore.Features.Dialogue;

internal sealed class CardDialogue : IRDialogue
{
	public static void Register(IModHelper helper)
	{
		var loc = IRDialogue.GetLoc(locale => MainModFile.GetFile($"i18n/dialogue-card-{locale}.json").OpenRead());
		var ironcladNodes = MakeICNodes();
		var silentNodes = MakeSilentNodes();
		var defectNodes = MakeDefectNodes();
		var watcherNodes = MakeWatcherNodes();
		
		helper.Events.OnModLoadPhaseFinished += (_, phase) =>
		{
			if (phase != ModLoadPhase.AfterDbInit)
				return;
			IRDialogue.InjectStory(Ironclad.ID, CType.Ironclad, ironcladNodes, [], [], NodeType.combat);
			IRDialogue.InjectStory(Silent.ID, CType.Silent, silentNodes, [], [], NodeType.combat);
			IRDialogue.InjectStory(Defect.ID, CType.Defect, defectNodes, [], [], NodeType.combat);
			IRDialogue.InjectStory(Watcher.ID, CType.Watcher, watcherNodes, [], [], NodeType.combat);
		};
		
		helper.Events.OnLoadStringsForLocale += (_, e) =>
		{
			IRDialogue.InjectLocalizations(Ironclad.ID, CType.Ironclad, loc, ironcladNodes, [], [], e);
			IRDialogue.InjectLocalizations(Silent.ID, CType.Silent, loc, silentNodes, [], [], e);
			IRDialogue.InjectLocalizations(Defect.ID, CType.Defect, loc, defectNodes, [], [], e);
			IRDialogue.InjectLocalizations(Watcher.ID, CType.Watcher, loc, watcherNodes, [], [], e);
		};
	}

	public static Dictionary<IReadOnlyList<string>, StoryNode> MakeICNodes()
	{
		var newNodes = new Dictionary<IReadOnlyList<string>, StoryNode>();
		
		newNodes[["Played", WarCry.ID, "0"]] = new()
		{
			lookup = [$"Played::{WarCry.ID}"],
			priority = true,
			oncePerRun = true,
			allPresent = [CType.Ironclad],
			lines = [
				new Say { who = CType.Ironclad, loopTag = Anim.Squint },
			],
		};
		
		newNodes[["Played", Juggernaut.ID]] = new()
		{
			lookup = [$"Played::{Juggernaut.ID}"],
			priority = true,
			oncePerRun = true,
			oncePerCombatTags = [$"Played::{Juggernaut.ID}"],
			allPresent = [CType.Ironclad],
			lines = [
				new Say { who = CType.Ironclad, loopTag = Anim.Neutral },
				new Say { who = CType.RandomCrew, loopTag = Anim.Neutral }
			],
		};
		
		return newNodes;
	}
	
	public static Dictionary<IReadOnlyList<string>, StoryNode> MakeSilentNodes()
	{
		var newNodes = new Dictionary<IReadOnlyList<string>, StoryNode>();
		return newNodes;
	}
	
	public static Dictionary<IReadOnlyList<string>, StoryNode> MakeDefectNodes()
	{
		var newNodes = new Dictionary<IReadOnlyList<string>, StoryNode>();
		return newNodes;
	}
	
	public static Dictionary<IReadOnlyList<string>, StoryNode> MakeWatcherNodes()
	{
		var newNodes = new Dictionary<IReadOnlyList<string>, StoryNode>();
		return newNodes;
	}

	/*public CardDialogue() : base(locale => ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"i18n/dialogue-card-{locale}.json").OpenRead())
	{
		var johnsonDeck = ModEntry.Instance.JohnsonDeck.CType;
		var johnsonType = ModEntry.Instance.JohnsonCharacter.CharacterType;
		var newNodes = new Dictionary<IReadOnlyList<string>, StoryNode>();

		ModEntry.Instance.Helper.Events.OnModLoadPhaseFinished += (_, phase) =>
		{
			if (phase != ModLoadPhase.AfterDbInit)
				return;
			InjectStory(newNodes, [], [], NodeType.combat);
		};
		ModEntry.Instance.Helper.Events.OnLoadStringsForLocale += (_, e) => InjectLocalizations(newNodes, [], [], e);

		newNodes[["Played", "Quarter1"]] = new()
		{
			lookup = [$"Played::{new Quarter1Card().Key()}"],
			priority = true,
			oncePerRun = true,
			allPresent = [johnsonType],
			lines = [
				new Say { who = johnsonType, loopTag = "fiddling" },
			],
		};

		for (var i = 0; i < 3; i++)
			newNodes[["Played", "Deadline", i.ToString()]] = new()
			{
				lookup = [$"Played::{new DeadlineCard().Key()}"],
				priority = true,
				oncePerRun = true,
				oncePerCombatTags = [$"Played::{new DeadlineCard().Key()}"],
				allPresent = [johnsonType],
				lines = [
					new Say { who = johnsonType, loopTag = "fiddling" },
				],
			};

		for (var i = 0; i < 3; i++)
			newNodes[["Played", "LayoutOrStrategize", i.ToString()]] = new()
			{
				lookup = [$"Played::{ModEntry.Instance.Package.Manifest.UniqueName}::LayoutOrStrategize"],
				priority = true,
				oncePerRun = true,
				oncePerCombatTags = [$"Played::{ModEntry.Instance.Package.Manifest.UniqueName}::LayoutOrStrategize"],
				allPresent = [johnsonType],
				lines = [
					new Say { who = johnsonType, loopTag = "neutral" },
				],
			};

		for (var i = 0; i < 2; i++)
			newNodes[["Played", "Downsize", i.ToString()]] = new()
			{
				lookup = [$"Played::{new DownsizeCard().Key()}"],
				priority = true,
				oncePerRun = true,
				oncePerCombatTags = [$"Played::{new DownsizeCard().Key()}"],
				allPresent = [johnsonType],
				lines = [
					new Say { who = johnsonType, loopTag = "fiddling" },
				],
			};
	}*/
}