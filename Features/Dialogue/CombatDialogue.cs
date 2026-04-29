using AutumnMooncat.SpireCore.Characters;
using Nickel;

namespace AutumnMooncat.SpireCore.Features.Dialogue;

internal sealed class CombatDialogue : IRDialogue
{
	public static void Register(IModHelper helper)
	{
		var loc = IRDialogue.GetLoc(locale => MainModFile.GetFile($"i18n/dialogue-combat-{locale}.json").OpenRead());
		var normal = MakeNormalNodes();
		var sayswitch = MakeSaySwitches();
		
		helper.Events.OnModLoadPhaseFinished += (_, phase) =>
		{
			if (phase != ModLoadPhase.AfterDbInit)
				return;
			IRDialogue.InjectStory(normal, NodeType.combat);
			IRDialogue.InjectSwitches(sayswitch, NodeType.combat);
		};
		
		helper.Events.OnLoadStringsForLocale += (_, e) =>
		{
			IRDialogue.LocalizeStory(loc, normal, e);
			IRDialogue.LocalizeSwitches(loc, sayswitch, e);
		};
	}
	
	public static IRDialogue.DialogueRegistry<StoryNode> MakeNormalNodes()
	{
		var reg = IRDialogue.MakeNormalRegistry();
		
		AddArtifactNodes(reg);
		AddEnemyNodes(reg);
		AddWeShotNodes(reg);
		AddTheyShotNodes(reg);
		AddPositionNodes(reg);
		AddStateNodes(reg);
		AddStatusNodes(reg);
		AddHandNodes(reg);
		
		return reg;
	}

	private static void AddArtifactNodes(IRDialogue.DialogueRegistry<StoryNode> reg)
	{
		
	}
	
	private static void AddEnemyNodes(IRDialogue.DialogueRegistry<StoryNode> reg)
	{
		#region StartedBattle

		reg.Register(["Enemies", "StartedBattle", "AgainstDrake"], new StoryNode
		{
			turnStart = true,
			oncePerRun = true,
			oncePerCombat = true,
			maxTurnsThisCombat = 1,
			allPresent = [CType.Ironclad, CType.Enemies.DrakePirate],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Neutral},
				new Say {who = CType.Enemies.DrakePirate, loopTag = Anim.Drake.Sly},
			]
		});

		reg.Register(["Enemies", "StartedBattle", "AgainstWizbo"], new StoryNode
		{
			turnStart = true,
			oncePerRun = true,
			oncePerCombat = true,
			maxTurnsThisCombat = 1,
			allPresent = [CType.Ironclad, CType.Enemies.Wizbo],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Ironclad.Mad},
			]
		});
		
		#endregion
	}
	
	private static void AddWeShotNodes(IRDialogue.DialogueRegistry<StoryNode> reg)
	{
		#region Missed

		reg.Register(["WeShot", "Missed"], new StoryNode
		{
			playerShotJustMissed = true,
			oncePerCombat = true,
			doesNotHaveArtifacts = [nameof(Recalibrator), nameof(GrazerBeam)],
			allPresent = [CType.Ironclad],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Squint},
			]
		});

		#endregion

		#region HitArmor

		

		#endregion

		#region HitArmorALot

		

		#endregion

		#region HitArmorAndPierced

		

		#endregion

		#region DidDamage

		#region Generic

		reg.Register(["WeShot", "DidDamage", "Generic"], new StoryNode
		{
			playerShotJustHit = true,
			minDamageDealtToEnemyThisAction = 1,
			allPresent = [CType.Ironclad],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Neutral},
				new SaySwitch()
				{
					lines = [
						new Say {who = CType.Silent, loopTag = Anim.Neutral},
						new Say {who = CType.Riggs, loopTag = Anim.Neutral},
					]
				}
			]
		});

		reg.Register(["WeShot", "DidDamage", "Generic"], new StoryNode
		{
			playerShotJustHit = true,
			minDamageDealtToEnemyThisAction = 1,
			allPresent = [CType.Ironclad],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Neutral},
			]
		});

		reg.Register(["WeShot", "DidDamage", "Generic"], new StoryNode
		{
			playerShotJustHit = true,
			minDamageDealtToEnemyThisAction = 1,
			allPresent = [CType.Ironclad],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Ironclad.Mad},
			]
		});
		
		reg.Register(["WeShot", "DidDamage", "Generic"], new StoryNode
		{
			playerShotJustHit = true,
			oncePerRun = true,
			minDamageDealtToEnemyThisAction = 1,
			allPresent = [CType.Ironclad, CType.Isaac],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Neutral},
				new Say {who = CType.Isaac, loopTag = Anim.Neutral},
				new Say {who = CType.Ironclad, loopTag = Anim.Neutral},
			]
		});
		
		reg.Register(["WeShot", "DidDamage", "Generic"], new StoryNode
		{
			playerShotJustHit = true,
			minDamageDealtToEnemyThisAction = 1,
			allPresent = [CType.Ironclad],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Neutral},
			]
		});

		#endregion

		#region Specific

		// Ironclad
		reg.Register(["WeShot", "DidDamage", "Ironclad"], new StoryNode
		{
			playerShotJustHit = true,
			minDamageDealtToEnemyThisAction = 1,
			whoDidThat = DType.Ironclad,
			allPresent = [CType.Ironclad],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Neutral},
				new SaySwitch()
				{
					lines = [
						new Say {who = CType.Max, loopTag = Anim.Max.Smile},
						new Say {who = CType.CAT, loopTag = Anim.Squint},
					]
				}
			]
		});
		
		reg.Register(["WeShot", "DidDamage", "Ironclad"], new StoryNode
		{
			playerShotJustHit = true,
			minDamageDealtToEnemyThisAction = 1,
			whoDidThat = DType.Ironclad,
			allPresent = [CType.Ironclad, CType.Peri],
			lines = [
				new Say {who = CType.Peri, loopTag = Anim.Peri.Vengeful},
				new Say {who = CType.Ironclad, loopTag = Anim.Neutral},
			]
		});
		
		// Silent
		
		// Defect
		
		// Watcher
		
		// Riggs
		
		// Dizzy
		reg.Register(["WeShot", "DidDamage", "Dizzy"], new StoryNode
		{
			playerShotJustHit = true,
			minDamageDealtToEnemyThisAction = 1,
			whoDidThat = DType.Dizzy,
			allPresent = [CType.Ironclad, CType.Dizzy],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Ironclad.Mad},
				new Say {who = CType.Dizzy, loopTag = Anim.Dizzy.Serious},
			]
		});
		
		// Peri
		
		// Isaac
		
		// Drake
		
		// Max
		
		// Books
		reg.Register(["WeShot", "DidDamage", "Books"], new StoryNode
		{
			playerShotJustHit = true,
			minDamageDealtToEnemyThisAction = 1,
			whoDidThat = DType.Books,
			allPresent = [CType.Ironclad, CType.Books],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Ironclad.Happy},
				new Say {who = CType.Books, loopTag = Anim.Books.Blush},
			]
		});
		
		// CAT

		#endregion
		
		#endregion

		#region DidBigDamage

		reg.Register(["WeShot", "DidBigDamage", "Ironclad"], new StoryNode
		{
			playerShotJustHit = true,
			minDamageDealtToEnemyThisAction = 3,
			whoDidThat = DType.Ironclad,
			allPresent = [CType.Ironclad],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Squint},
			]
		});

		#endregion

		#region DidOverThree

		reg.Register(["WeShot", "DidOverThree"], new StoryNode
		{
			playerShotJustHit = true,
			minDamageDealtToEnemyThisAction = 4,
			allPresent = [CType.Ironclad],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Squint},
			]
		});

		#endregion

		#region DidOverFive

		reg.Register(["WeShot", "DidOverFive"], new StoryNode
		{
			playerShotJustHit = true,
			minDamageDealtToEnemyThisAction = 6,
			allPresent = [CType.Ironclad],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Squint},
			]
		});

		#endregion
	}
	
	private static void AddTheyShotNodes(IRDialogue.DialogueRegistry<StoryNode> reg)
	{
		#region Missed

		

		#endregion

		#region TookZero

		reg.Register(["TheyShot", "TookZero"], new StoryNode
		{
			enemyShotJustHit = true,
			maxDamageDealtToPlayerThisTurn = 0,
			allPresent = [CType.Ironclad],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Neutral},
			]
		});

		#endregion

		#region TookZeroLowHP

		reg.Register(["TheyShot", "TookZeroLowHP"], new StoryNode
		{
			enemyShotJustHit = true,
			maxDamageDealtToPlayerThisTurn = 0,
			maxHull = 2,
			allPresent = [CType.Ironclad],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Squint},
			]
		});

		#endregion

		#region TookOne

		reg.Register(["TheyShot", "TookOne"], new StoryNode
		{
			enemyShotJustHit = true,
			minDamageDealtToPlayerThisTurn = 1,
			maxDamageDealtToPlayerThisTurn = 1,
			allPresent = [CType.Ironclad],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Squint},
				new SaySwitch()
				{
					lines = [
						new Say {who = CType.Dizzy, loopTag = Anim.Dizzy.Explains}
					]
				}
			]
		});

		#endregion

		#region InArmor

		

		#endregion

		#region InArmorALot

		

		#endregion

		#region AboutToDie

		reg.Register(["TheyShot", "AboutToDie"], new StoryNode
		{
			enemyShotJustHit = true,
			maxHull = 2,
			oncePerCombatTags = [StoryTags.AboutToDie],
			oncePerRun = true,
			allPresent = [CType.Ironclad, CType.Isaac],
			lines = [
				new Say {who = CType.Isaac, loopTag = Anim.Isaac.Panic},
				new Say {who = CType.Ironclad, loopTag = Anim.Neutral},
			]
		});
		
		reg.Register(["TheyShot", "AboutToDie"], new StoryNode
		{
			enemyShotJustHit = true,
			maxHull = 2,
			oncePerCombatTags = [StoryTags.AboutToDie],
			oncePerRun = true,
			allPresent = [CType.Ironclad],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Squint},
				new SaySwitch()
				{
					lines = [
						new Say {who = CType.Dizzy, loopTag = Anim.Dizzy.Intense},
						new Say {who = CType.Max, loopTag = Anim.Squint}
					]
				}
			]
		});

		#endregion

		#region OneHPThisIsFine

		reg.Register(["TheyShot", "OneHPThisIsFine"], new StoryNode
		{
			enemyShotJustHit = true,
			maxHull = 1,
			oncePerCombatTags = [StoryTags.AboutToDie],
			oncePerRun = true,
			allPresent = [CType.Ironclad, CType.Books],
			lines = [
				new Say {who = CType.Books, loopTag = Anim.Books.Paws},
				new Say {who = CType.Ironclad, loopTag = Anim.Ironclad.Intense},
			]
		});

		#endregion
	}
	
	private static void AddPositionNodes(IRDialogue.DialogueRegistry<StoryNode> reg)
	{
		#region NoOverlap

		reg.Register(["Position", "NoOverlap"], new StoryNode
		{
			priority = true,
			shipsDontOverlapAtAll = true,
			oncePerRun = true,
			oncePerCombatTags = [StoryTags.NoOverlap],
			allPresent = [CType.Ironclad],
			nonePresent = [CType.Enemies.Brac, CType.Enemies.Robots.Scrap],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Neutral},
			]
		});

		#endregion
		
		#region NoOverlapCrab

		reg.Register(["Position", "NoOverlapCrab"], new StoryNode
		{
			priority = true,
			shipsDontOverlapAtAll = true,
			oncePerRun = true,
			oncePerCombatTags = [StoryTags.NoOverlap],
			allPresent = [CType.Ironclad, CType.Enemies.Brac],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Squint},
			]
		});

		#endregion

		#region NoOverlapButSeeker

		reg.Register(["Position", "NoOverlapButSeeker"], new StoryNode
		{
			priority = true,
			shipsDontOverlapAtAll = true,
			oncePerRun = true,
			oncePerCombatTags = [StoryTags.NoOverlapSeeker],
			anyDronesHostile = ["missile_seeker"],
			allPresent = [CType.Ironclad],
			nonePresent = [CType.Enemies.Brac],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Squint},
			]
		});

		#endregion

		#region MovingALot

		

		#endregion
	}
	
	private static void AddStateNodes(IRDialogue.DialogueRegistry<StoryNode> reg)
	{
		#region WeLostALotOfHPTimeForTheBet
		
		// IC Drake Bet Starts
		reg.Register(["State", "WeLostALotOfHPTimeForTheBet"], new StoryNode
		{
			enemyShotJustHit = true,
			minDamageDealtToPlayerThisTurn = 3,
			allPresent = [CType.Ironclad, CType.Drake],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Squint},
				new Say {who = CType.Drake, loopTag = Anim.Drake.Sly},
				new Say {who = CType.Ironclad, loopTag = Anim.Neutral},
				new Say {who = CType.Ironclad, loopTag = Anim.Squint}.WithOnExecute(new Records.OnExecutePayload
				{
					key = StoryTags.ICDrakeTheBetIsOn, 
					value = true, 
					lifetime = Records.OnExecutePayload.Lifetime.Persistent
				}),
			]
		}.WithRequirements(new Records.StoryDataPayload()
		{
			key = StoryTags.ICDrakeTheBetIsOn, 
			value = null
		}));
		
		// IC Drake 1-0
		reg.Register(["State", "WeLostALotOfHPTimeForTheBet"], new StoryNode
		{
			enemyShotJustHit = true,
			minDamageDealtToPlayerThisTurn = 3,
			oncePerRun = true,
			allPresent = [CType.Ironclad, CType.Drake],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Squint},
				new Say {who = CType.Drake, loopTag = Anim.Drake.SlyBlush},
			]
		}.WithRequirements(
			new Records.StoryDataPayload()
			{
				key = StoryTags.ICDrakeTheBetIsOn, 
				value = true
			},
			new Records.StoryDataPayload()
			{
				key = StoryTags.ICBeatDrakeCounter, value = 0, not = true
			},
			new Records.StoryDataPayload()
			{
				key = StoryTags.DrakeBeatICCounter, value = 0
			}));
		
		// IC Drake 0-1
		reg.Register(["State", "WeLostALotOfHPTimeForTheBet"], new StoryNode
		{
			enemyShotJustHit = true,
			minDamageDealtToPlayerThisTurn = 3,
			oncePerRun = true,
			allPresent = [CType.Ironclad, CType.Drake],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Squint},
				new Say {who = CType.Drake, loopTag = Anim.Drake.Sly},
			]
		}.WithRequirements(
			new Records.StoryDataPayload()
			{
				key = StoryTags.ICDrakeTheBetIsOn, 
				value = true
			},
			new Records.StoryDataPayload()
			{
				key = StoryTags.ICBeatDrakeCounter, value = 0
			},
			new Records.StoryDataPayload()
			{
				key = StoryTags.DrakeBeatICCounter, value = 0, not = true
			}));
		
		// IC Drake 1-1
		reg.Register(["State", "WeLostALotOfHPTimeForTheBet"], new StoryNode
		{
			enemyShotJustHit = true,
			minDamageDealtToPlayerThisTurn = 3,
			oncePerRun = true,
			allPresent = [CType.Ironclad, CType.Drake],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Squint},
				new Say {who = CType.Drake, loopTag = Anim.Drake.Sly},
			]
		}.WithRequirements(
			new Records.StoryDataPayload()
			{
				key = StoryTags.ICDrakeTheBetIsOn, 
				value = true
			},
			new Records.StoryDataPayload()
			{
				key = StoryTags.ICBeatDrakeCounter, value = 0, not = true
			},
			new Records.StoryDataPayload()
			{
				key = StoryTags.DrakeBeatICCounter, value = 0, not = true
			}));

		#endregion

		#region WeLostALotOfHP
		
		reg.Register(["State", "WeLostALotOfHP"], new StoryNode
		{
			enemyShotJustHit = true,
			minDamageDealtToPlayerThisTurn = 3,
			allPresent = [CType.Ironclad],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Squint},
				new SaySwitch()
				{
					lines = [
						new Say {who = CType.Dizzy, loopTag = Anim.Dizzy.Intense}
					]
				}
			]
		});
		
		reg.Register(["State", "WeLostALotOfHP"], new StoryNode
		{
			enemyShotJustHit = true,
			minDamageDealtToPlayerThisTurn = 3,
			allPresent = [CType.Ironclad],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Squint},
				new SaySwitch()
				{
					lines = [
						new Say {who = CType.Riggs, loopTag = Anim.Neutral}
					]
				}
			]
		});
		
		reg.Register(["State", "WeLostALotOfHP"], new StoryNode
		{
			enemyShotJustHit = true,
			minDamageDealtToPlayerThisTurn = 3,
			allPresent = [CType.Ironclad],
			lines = [
				new SaySwitch()
				{
					lines = [
						new Say {who = CType.Peri, loopTag = Anim.Peri.Mad}
					]
				},
				new Say {who = CType.Ironclad, loopTag = Anim.Ironclad.Intense},
			]
		}.WithRequirements(new Records.StoryDataPayload()
		{
			key = StoryTags.ICHasCupcakes, 
			value = true
		}));
		
		reg.Register(["State", "WeLostALotOfHP"], new StoryNode
		{
			enemyShotJustHit = true,
			minDamageDealtToPlayerThisTurn = 3,
			allPresent = [CType.Peri],
			lines = [
				new Say {who = CType.Peri, loopTag = Anim.Peri.Mad},
				new SaySwitch()
				{
					lines = [
						new Say {who = CType.Ironclad, loopTag = Anim.Squint},
						new Say {who = CType.Watcher, loopTag = Anim.Neutral}
					]
				}
			]
		}.WithRequirements(new Records.AtLeastOnePresentPayload()
		{
			chars = [CType.Ironclad, CType.Watcher]
		}));

		#endregion

		#region TheyLostALotOfHP

		reg.Register(["State", "TheyLostALotOfHP"], new StoryNode
		{
			playerShotJustHit = true,
			minDamageDealtToEnemyThisTurn = 10,
			allPresent = [CType.Ironclad],
			lines =
			[
				new Say { who = CType.Ironclad, loopTag = Anim.Neutral },
			]
		});

		#endregion

		#region WentMissing
		
		// Ironclad
		
		// Silent
		
		// Defect
		reg.Register(["State", "WentMissing", "Defect"], new StoryNode
		{
			priority = true,
			lastTurnPlayerStatuses = [Defect.Entry.MissingStatus.Status],
			oncePerCombatTags = [StoryTags.WentMissing.Defect],
			oncePerRun = true,
			lines = [
				new Say { who = CType.Defect, loopTag = Anim.Neutral },
			],
		});
		
		// Watcher
		
		// Riggs
		
		// Dizzy
		
		// Peri
		
		// Isaac
		
		// Drake
		
		// Max
		
		// Books
		
		// CAT

		#endregion

		#region NoLongerMissing

		reg.Register(["State", "NoLongerMissing"], new StoryNode
		{
			priority = true,
			lookup = [LookupKeys.IroncladReturnedFromMissing],
			oncePerRun = true,
			lines = [
				new Say { who = CType.Ironclad, loopTag = Anim.Squint },
			],
		});

		#endregion

		#region EnemyHasBrittle

		

		#endregion

		#region EnemyHasWeak

		

		#endregion

		#region LongCombat

		reg.Register(["State", "LongCombat"], new StoryNode
		{
			turnStart = true,
			oncePerRun = true,
			oncePerCombatTags = ["manyTurns"],
			minTurnsThisCombat = 9,
			allPresent = [CType.Ironclad],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Squint},
			]
		});

		#endregion

		#region VeryLongCombat

		reg.Register(["State", "VeryLongCombat"], new StoryNode
		{
			turnStart = true,
			oncePerRun = true,
			oncePerCombatTags = ["veryManyTurns"],
			minTurnsThisCombat = 20,
			allPresent = [CType.Ironclad],
			lines = [
				new Say {who = CType.Ironclad, loopTag = Anim.Squint},
			]
		});

		#endregion
	}
	
	private static void AddStatusNodes(IRDialogue.DialogueRegistry<StoryNode> reg)
	{
		#region OverheatingIroncladsFault

		

		#endregion

		#region OverheatingIroncladFix

		

		#endregion

		#region OverheatingGeneric

		

		#endregion

		#region JustOverheated

		

		#endregion

		#region JustGainedHeatIroncladHere

		

		#endregion
	}
	
	private static void AddHandNodes(IRDialogue.DialogueRegistry<StoryNode> reg)
	{
		#region Empty

		

		#endregion

		#region EmptyWithEnergy

		

		#endregion

		#region OnlyTrash

		

		#endregion

		#region OnlyUnplayable

		

		#endregion

		#region PlayedManyCards

		

		#endregion

		#region PlayedCheapCard

		

		#endregion

		#region PlayedExpensiveCard

		

		#endregion

		#region ManyFlips

		

		#endregion
	}
	
	public static IRDialogue.DialogueRegistry<Say> MakeSaySwitches()
	{
		var reg = IRDialogue.MakeSaySwitchRegistry();
		reg.Register(["SaySwitches", "CrabFacts1_Multi_0", "1"], new Say()
		{
			who = CType.Ironclad,
			loopTag = Anim.Neutral
		});
		
		reg.Register(["SaySwitches", "CrabFacts2_Multi_0", "1"], new Say()
		{
			who = CType.Ironclad,
			loopTag = Anim.Neutral
		});
		return reg;
	}
}