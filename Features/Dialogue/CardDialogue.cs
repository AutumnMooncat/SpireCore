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
		var normal = MakeNormalNodes();
		
		helper.Events.OnModLoadPhaseFinished += (_, phase) =>
		{
			if (phase != ModLoadPhase.AfterDbInit)
				return;
			IRDialogue.InjectStory(normal, NodeType.combat);
		};
		
		helper.Events.OnLoadStringsForLocale += (_, e) =>
		{
			IRDialogue.LocalizeStory(loc, normal, e);
		};
	}
	
	public static IRDialogue.DialogueRegistry<StoryNode> MakeNormalNodes()
	{
		var reg = IRDialogue.MakeNormalRegistry();
		RegisterLookup(typeof(Anger));
		reg.Register(["Played", nameof(Anger)], new()
		{
			lookup = [CardLookups[typeof(Anger)]],
			priority = true,
			allPresent = [CType.Ironclad],
			lines = [
				new Say { who = CType.Ironclad, loopTag = Anim.Ironclad.Mad },
				new SaySwitch()
				{
					lines = [
						new Say { who = CType.Defect, loopTag = Anim.Neutral },
						new Say { who = CType.Silent, loopTag = Anim.Squint },
						..new Say { loopTag = Anim.Neutral}.CopyWithout(CType.Ironclad, CType.Defect, CType.Silent)
					]
				}
			],
		});

		RegisterLookup(typeof(Juggernaut));
		reg.Register(["Played", nameof(Juggernaut)], new()
		{
			lookup = [CardLookups[typeof(Juggernaut)]],
			priority = true,
			oncePerRun = true,
			allPresent = [CType.Ironclad],
			lines = [
				new Say { who = CType.Ironclad, loopTag = Anim.Neutral },
				new SaySwitch()
				{
					lines = [
						new Say { who = CType.Defect, loopTag = Anim.Squint },
						..new Say { loopTag = Anim.Squint }.CopyWithout(CType.Ironclad, CType.Defect)
					]
				}
			],
		});
		
		RegisterLookup(typeof(ShieldGun));
		reg.Register(["Played", nameof(ShieldGun)], new()
		{
			lookup = [CardLookups[typeof(ShieldGun)]],
			priority = true,
			oncePerRun = true,
			allPresent = [CType.Ironclad, CType.Dizzy],
			lines = [
				new Say { who = CType.Ironclad, loopTag = Anim.Squint },
				new Say { who = CType.Dizzy, loopTag = Anim.Neutral }
			],
		});
		return reg;
	}
}