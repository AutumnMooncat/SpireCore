using Nickel;

namespace AutumnMooncat.SpireCore.Features.Dialogue;

internal sealed class EventDialogue : IRDialogue
{
	public static void Register(IModHelper helper)
	{
		var loc = IRDialogue.GetLoc(locale => MainModFile.GetFile($"i18n/dialogue-event-{locale}.json").OpenRead());
		
		var normal = MakeNormalNodes();
		var hardcoded = MakeHardcodedNodes();
		var sayswitch = MakeSaySwitches();
		var replacers = MakeHashReplacements();
		
		helper.Events.OnModLoadPhaseFinished += (_, phase) =>
		{
			if (phase != ModLoadPhase.AfterDbInit)
				return;
			IRDialogue.InjectStory(normal, NodeType.@event);
			IRDialogue.InjectStory(hardcoded, NodeType.@event);
			IRDialogue.InjectSwitches(sayswitch, NodeType.@event);
			IRDialogue.InjectHashReplacements(replacers, NodeType.@event);
		};
		
		helper.Events.OnLoadStringsForLocale += (_, e) =>
		{
			IRDialogue.LocalizeStory(loc, normal, e);
			IRDialogue.LocalizeStory(loc, hardcoded, e);
			IRDialogue.LocalizeSwitches(loc, sayswitch, e);
			IRDialogue.LocalizeHashReplacements(loc, replacers, e);
		};
	}

	public static IRDialogue.DialogueRegistry<StoryNode> MakeNormalNodes()
	{
		var reg = IRDialogue.MakeNormalRegistry();
		reg.Register(["Shop"], new()
		{
			lookup = ["shopBefore"],
			bg = nameof(BGShop),
			allPresent = [CType.Ironclad],
			lines = [
				new Say { who = CType.Cleo, loopTag = Anim.Neutral, flipped = true },
				new Say { who = CType.Ironclad, loopTag = Anim.Ironclad.Sword },
				new Jump { key = "NewShop" }
			],
		});
		
		reg.Register(["Shop"], new()
		{
			lookup = ["shopBefore"],
			bg = nameof(BGShop),
			allPresent = [CType.Ironclad],
			lines = [
				new Say { who = CType.Cleo, loopTag = Anim.Neutral, flipped = true },
				new Say { who = CType.Ironclad, loopTag = Anim.Neutral },
				new Say { who = CType.CAT, loopTag = Anim.Squint },
				new Jump { key = "NewShop" }
			],
		});
		return reg;
	}
	
	public static IRDialogue.DialogueRegistry<StoryNode> MakeHardcodedNodes()
	{
		var reg = IRDialogue.MakeHardcodedRegistry();
		
		reg.Register([CType.Ironclad, "LoseCharacterCard_{{CharacterType}}"], new()
		{
			oncePerRun = true,
			bg = nameof(BGSupernova),
			allPresent = [CType.Ironclad],
			lines = [
				new Say { who = CType.Ironclad, loopTag = Anim.Neutral }
			]
		});
		
		reg.Register([CType.Ironclad, "LoseCharacterCard_{{CharacterType}}"], new()
		{
			oncePerRun = true,
			bg = nameof(BGSupernova),
			allPresent = [CType.Defect],
			lines = [
				new Say { who = CType.Defect, loopTag = Anim.Defect.Sad }
			]
		});
		
		reg.Register([CType.Ironclad, "CrystallizedFriendEvent_{{CharacterType}}"], new()
		{
			oncePerRun = true,
			bg = nameof(BGCrystalizedFriend),
			allPresent = [CType.Ironclad],
			lines = [
				new Wait() { secs = 1.5 },
				new Say { who = CType.Ironclad, loopTag = Anim.Ironclad.Sword }
			]
		});
		
		reg.Register([CType.Ironclad, "CrystallizedFriendEvent_{{CharacterType}}"], new()
		{
			oncePerRun = true,
			bg = nameof(BGCrystalizedFriend),
			allPresent = [CType.Defect],
			lines = [
				new Wait() { secs = 1.5 },
				new Say { who = CType.Defect, loopTag = Anim.Defect.Happy }
			]
		});
		
		reg.Register([CType.Ironclad, "ChoiceCardRewardOfYourColorChoice_{{CharacterType}}"], new()
		{
			oncePerRun = true,
			bg = nameof(BGBootSequence),
			allPresent = [CType.Ironclad],
			lines = [
				new Say { who = CType.Ironclad, loopTag = Anim.Ironclad.Mad },
				new Say { who = CType.CAT, loopTag = Anim.Neutral }
			]
		});
		return reg;
	}
	
	public static IRDialogue.DialogueRegistry<Say> MakeSaySwitches()
	{
		var reg = IRDialogue.MakeSaySwitchRegistry();
		
		reg.Register(["GrandmaShop"], new Say()
		{
			who = CType.Ironclad,
			loopTag = Anim.Neutral
		}.WithOnExecute(new Records.OnExecutePayload()
		{
			key = StoryTags.ICHasCupcakes, 
			lifetime = Records.OnExecutePayload.Lifetime.Run, 
			value = true
		}));

		reg.Register(["GrandmaShop"], new Say()
		{
			who = CType.Defect,
			loopTag = Anim.Neutral
		});
		
		
		reg.Register(["LoseCharacterCard"], new Say()
		{
			who = CType.Ironclad,
			loopTag = Anim.Neutral
		});
		
		reg.Register(["CrystallizedFriendEvent"], new Say()
		{
			who = CType.Ironclad,
			loopTag = Anim.Neutral
		});
		
		reg.Register(["ShopKeepBattleInsult"], new Say()
		{
			who = CType.Ironclad,
			loopTag = Anim.Neutral
		});
		return reg;
	}

	public static IRDialogue.DialogueRegistry<Say> MakeHashReplacements()
	{
		var reg = IRDialogue.MakeHashReplacerRegistry();
		reg.Register(["Replacements", "ChoiceCardRewardOfYourColorChoice"], new Say()
		{
			who = CType.Defect,
			loopTag = Anim.Squint,
			hash = "1ef5ed18"
		});
		reg.Register(["Replacements", "ChoiceCardRewardOfYourColorChoice"], new Say()
		{
			who = CType.Defect,
			loopTag = Anim.Squint,
			hash = "3afc930f"
		});
		reg.Register(["Replacements", "ChoiceCardRewardOfYourColorChoice"], new Say()
		{
			who = CType.Silent,
			loopTag = Anim.Squint,
			hash = "1ef5ed18"
		});
		reg.Register(["Replacements", "ChoiceCardRewardOfYourColorChoice"], new Say()
		{
			who = CType.Silent,
			loopTag = Anim.Squint,
			hash = "3afc930f"
		});
		return reg;
	}
}