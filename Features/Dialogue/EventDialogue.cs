using System.Collections.Generic;
using AutumnMooncat.SpireCore.Characters;
using Nickel;

namespace AutumnMooncat.SpireCore.Features.Dialogue;

internal sealed class EventDialogue : IRDialogue
{
	public static void Register(IModHelper helper)
	{
		var loc = IRDialogue.GetLoc(locale => MainModFile.GetFile($"i18n/dialogue-event-{locale}.json").OpenRead());
		var ironcladNodes = MakeICNodes();
		var ironcladHard = MakeICHard();
		var ironcladSay = MakeICSay();
		var silentNodes = MakeSilentNodes();
		var silentHard = MakeSilentHard();
		var silentSay = MakeSilentSay();
		var defectNodes = MakeDefectNodes();
		var defectHard = MakeDefectHard();
		var defectSay = MakeDefectSay();
		var watcherNodes = MakeWatcherNodes();
		var watcherHard = MakeWatcherHard();
		var watcherSay = MakeWatcherSay();
		
		helper.Events.OnModLoadPhaseFinished += (_, phase) =>
		{
			if (phase != ModLoadPhase.AfterDbInit)
				return;
			IRDialogue.InjectStory(Ironclad.ID, CType.Ironclad, ironcladNodes, ironcladHard, ironcladSay, NodeType.@event);
			IRDialogue.InjectStory(Silent.ID, CType.Silent, silentNodes, silentHard, silentSay, NodeType.@event);
			IRDialogue.InjectStory(Defect.ID, CType.Defect, defectNodes, defectHard, defectSay, NodeType.@event);
			IRDialogue.InjectStory(Watcher.ID, CType.Watcher, watcherNodes, watcherHard, watcherSay, NodeType.@event);
		};
		
		helper.Events.OnLoadStringsForLocale += (_, e) =>
		{
			IRDialogue.InjectLocalizations(Ironclad.ID, CType.Ironclad, loc, ironcladNodes, ironcladHard, ironcladSay, e);
			IRDialogue.InjectLocalizations(Silent.ID, CType.Silent, loc, silentNodes, silentHard, silentSay, e);
			IRDialogue.InjectLocalizations(Defect.ID, CType.Defect, loc, defectNodes, defectHard, defectSay, e);
			IRDialogue.InjectLocalizations(Watcher.ID, CType.Watcher, loc, watcherNodes, watcherHard, watcherSay, e);
		};
	}
	
	public static Dictionary<IReadOnlyList<string>, StoryNode> MakeICNodes()
	{
		var newNodes = new Dictionary<IReadOnlyList<string>, StoryNode>();
		
		newNodes[["Shop", "0"]] = new()
		{
			lookup = ["shopBefore"],
			bg = nameof(BGShop),
			allPresent = [CType.Ironclad],
			lines = [
				new Say { who = CType.Ironclad, loopTag = Anim.Neutral },
				new Say { who = CType.Cleo, loopTag = Anim.Neutral, flipped = true },
				new Jump { key = "NewShop" }
			],
		};
		
		newNodes[["Shop", "1"]] = new()
		{
			lookup = ["shopBefore"],
			bg = nameof(BGShop),
			allPresent = [CType.Ironclad],
			lines = [
				new Say { who = CType.Ironclad, loopTag = Anim.Neutral },
				new Say { who = CType.Cleo, loopTag = Anim.Neutral, flipped = true },
				new Jump { key = "NewShop" }
			],
		};
		
		return newNodes;
	}
	
	public static Dictionary<IReadOnlyList<string>, StoryNode> MakeICHard()
	{
		var newNodes = new Dictionary<IReadOnlyList<string>, StoryNode>();
		
		newNodes[["LoseCharacterCard_{{CharacterType}}"]] = new()
		{
			oncePerRun = true,
			bg = nameof(BGSupernova),
			allPresent = [CType.Ironclad],
			lines = [
				new Say { who = CType.Ironclad, loopTag = Anim.Neutral }
			]
		};
		
		newNodes[["CrystallizedFriendEvent_{{CharacterType}}"]] = new()
		{
			oncePerRun = true,
			bg = nameof(BGCrystalizedFriend),
			allPresent = [CType.Ironclad],
			lines = [
				new Wait() { secs = 1.5 },
				new Say { who = CType.Ironclad, loopTag = Anim.Neutral }
			]
		};
		
		newNodes[["ChoiceCardRewardOfYourColorChoice_{{CharacterType}}"]] = new()
		{
			oncePerRun = true,
			bg = nameof(BGBootSequence),
			allPresent = [CType.Ironclad],
			lines = [
				new Say { who = CType.Ironclad, loopTag = Anim.Squint },
				new Say { who = CType.CAT, loopTag = Anim.Neutral }
			]
		};
		
		return newNodes;
	}

	public static Dictionary<IReadOnlyList<string>, Say> MakeICSay()
	{
		var newSay = new Dictionary<IReadOnlyList<string>, Say>();
		newSay[["GrandmaShop"]] = new()
		{
			who = CType.Ironclad,
			loopTag = Anim.Neutral
		};
		newSay[["LoseCharacterCard"]] = new()
		{
			who = CType.Ironclad,
			loopTag = Anim.Neutral
		};
		newSay[["CrystallizedFriendEvent"]] = new()
		{
			who = CType.Ironclad,
			loopTag = Anim.Neutral
		};
		newSay[["ShopKeepBattleInsult"]] = new()
		{
			who = CType.Ironclad,
			loopTag = Anim.Neutral
		};
		return newSay;
	}
	
	public static Dictionary<IReadOnlyList<string>, StoryNode> MakeSilentNodes()
	{
		var newNodes = new Dictionary<IReadOnlyList<string>, StoryNode>();
		return newNodes;
	}
	
	public static Dictionary<IReadOnlyList<string>, StoryNode> MakeSilentHard()
	{
		var newNodes = new Dictionary<IReadOnlyList<string>, StoryNode>();
		return newNodes;
	}
	
	public static Dictionary<IReadOnlyList<string>, Say> MakeSilentSay()
	{
		var newSay = new Dictionary<IReadOnlyList<string>, Say>();
		return newSay;
	}
	
	public static Dictionary<IReadOnlyList<string>, StoryNode> MakeDefectNodes()
	{
		var newNodes = new Dictionary<IReadOnlyList<string>, StoryNode>();
		return newNodes;
	}
	
	public static Dictionary<IReadOnlyList<string>, StoryNode> MakeDefectHard()
	{
		var newNodes = new Dictionary<IReadOnlyList<string>, StoryNode>();
		return newNodes;
	}
	
	public static Dictionary<IReadOnlyList<string>, Say> MakeDefectSay()
	{
		var newSay = new Dictionary<IReadOnlyList<string>, Say>();
		return newSay;
	}
	
	public static Dictionary<IReadOnlyList<string>, StoryNode> MakeWatcherNodes()
	{
		var newNodes = new Dictionary<IReadOnlyList<string>, StoryNode>();
		return newNodes;
	}
	
	public static Dictionary<IReadOnlyList<string>, StoryNode> MakeWatcherHard()
	{
		var newNodes = new Dictionary<IReadOnlyList<string>, StoryNode>();
		return newNodes;
	}
	
	public static Dictionary<IReadOnlyList<string>, Say> MakeWatcherSay()
	{
		var newSay = new Dictionary<IReadOnlyList<string>, Say>();
		return newSay;
	}
	
	/*public EventDialogue() : base(locale => ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"i18n/dialogue-event-{locale}.json").OpenRead())
	{
		var johnsonDeck = ModEntry.Instance.JohnsonDeck.CType;
		var johnsonType = ModEntry.Instance.JohnsonCharacter.CharacterType;
		var newNodes = new Dictionary<IReadOnlyList<string>, StoryNode>();
		var newHardcodedNodes = new Dictionary<IReadOnlyList<string>, StoryNode>();
		var saySwitchNodes = new Dictionary<IReadOnlyList<string>, Say>();

		ModEntry.Instance.Helper.Events.OnModLoadPhaseFinished += (_, phase) =>
		{
			if (phase != ModLoadPhase.AfterDbInit)
				return;
			InjectStory(newNodes, newHardcodedNodes, saySwitchNodes, NodeType.@event);
		};
		ModEntry.Instance.Helper.Events.OnLoadStringsForLocale += (_, e) => InjectLocalizations(newNodes, newHardcodedNodes, saySwitchNodes, e);

		newNodes[["Shop", "0"]] = new()
		{
			lookup = ["shopBefore"],
			bg = typeof(BGShop).Name,
			allPresent = [johnsonType],
			lines = [
				new Say { who = johnsonType, loopTag = "flashing" },
				new Say { who = "nerd", loopTag = "neutral", flipped = true },
				new Jump() { key = "NewShop" }
			],
		};
		newNodes[["Shop", "1"]] = new()
		{
			lookup = ["shopBefore"],
			bg = typeof(BGShop).Name,
			allPresent = [johnsonType],
			lines = [
				new Say { who = johnsonType, loopTag = "neutral" },
				new Say { who = "nerd", loopTag = "neutral", flipped = true },
				new Jump() { key = "NewShop" }
			],
		};

		newHardcodedNodes[["LoseCharacterCard_{{CharacterType}}"]] = new()
		{
			oncePerRun = true,
			bg = typeof(BGSupernova).Name,
			allPresent = [johnsonType],
			lines = [
				new Say { who = johnsonType, loopTag = "neutral" },
			],
		};
		newHardcodedNodes[["CrystallizedFriendEvent_{{CharacterType}}"]] = new()
		{
			oncePerRun = true,
			bg = typeof(BGCrystalizedFriend).Name,
			allPresent = [johnsonType],
			lines = [
				new Wait() { secs = 1.5 },
				new Say { who = johnsonType, loopTag = "fiddling" },
			],
		};
		newHardcodedNodes[["ChoiceCardRewardOfYourColorChoice_{{CharacterType}}"]] = new()
		{
			oncePerRun = true,
			bg = typeof(BGBootSequence).Name,
			allPresent = [johnsonType],
			lines = [
				new Say { who = johnsonType, loopTag = "squint" },
				new Say { who = "comp", loopTag = "neutral" },
			],
		};

		saySwitchNodes[["GrandmaShop"]] = new()
		{
			who = johnsonType,
			loopTag = "neutral"
		};
		saySwitchNodes[["LoseCharacterCard"]] = new()
		{
			who = johnsonType,
			loopTag = "neutral"
		};
		saySwitchNodes[["CrystallizedFriendEvent"]] = new()
		{
			who = johnsonType,
			loopTag = "fiddling"
		};
		saySwitchNodes[["ShopKeepBattleInsult"]] = new()
		{
			who = johnsonType,
			loopTag = "fiddling"
		};
		saySwitchNodes[["DraculaTime"]] = new()
		{
			who = johnsonType,
			loopTag = "squint"
		};
		saySwitchNodes[["Soggins_Infinite"]] = new()
		{
			who = johnsonType,
			loopTag = "flashing"
		};
		saySwitchNodes[["Soggins_Missile_Shout_1"]] = new()
		{
			who = johnsonType,
			loopTag = "neutral"
		};
		saySwitchNodes[["SogginsEscapeIntent_1"]] = new()
		{
			who = johnsonType,
			loopTag = "neutral"
		};
		saySwitchNodes[["SogginsEscape_1"]] = new()
		{
			who = johnsonType,
			loopTag = "fiddling"
		};
		saySwitchNodes[["AbandonedShipyard_Repaired"]] = new()
		{
			who = johnsonType,
			loopTag = "neutral"
		};
	}*/
}