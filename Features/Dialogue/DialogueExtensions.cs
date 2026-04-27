using System;
using System.Collections.Generic;
using System.Linq;
using Nickel;

namespace AutumnMooncat.SpireCore.Features.Dialogue;

internal static class DialogueExt
{
	public static SaySwitch SplitWith(this Say say, params string[] thatIs)
	{
		//MainModFile.Log("AllPlayable: {}", CType.AllPlayable.Aggregate((a,b) => a+", "+b));
		return new SaySwitch()
		{
			lines = say.CopyWith(thatIs)
		}.WithSplitFlag();
	}
	
	public static SaySwitch SplitWithout(this Say say, params string[] thatIsnt)
	{
		return new SaySwitch()
		{
			lines = say.CopyWithout(thatIsnt)
		}.WithSplitFlag();
	}

	public static SaySwitch WithSplitFlag(this SaySwitch say)
	{
		return say.WithData("SplitSwitch", true);
	}

	public static bool HasSplitFlag(this SaySwitch say)
	{
		return say.GetData("SplitSwitch", out bool has) && has;
	}
	
	public static Say WithSilentFlag(this Say say)
	{
		return say.WithData("IsSilent", true);
	}

	public static bool HasSilentFlag(this Say say)
	{
		return say.GetData("IsSilent", out bool has) && has;
	}
	
	public static Say WithNotSilentFlag(this Say say)
	{
		return say.WithData("IsSilent", false);
	}

	public static bool HasNotSilentFlag(this Say say)
	{
		return say.GetData("IsSilent", out bool has) && !has;
	}
	
	public static Shout WithSilentFlag(this Shout shout)
	{
		return shout.WithData("IsSilent", true);
	}

	public static bool HasSilentFlag(this Shout shout)
	{
		return shout.GetData("IsSilent", out bool has) && has;
	}

	public static List<Say> CopyWith(this Say say, params string[] thatIs)
	{
		return CType.AllPlayable.Where(thatIs.Contains)
			.Select(s => Mutil.DeepCopy(say).WithCopyFlag()
				.EditThis(thiz => thiz.who = s))
			.ToList()
			.EditThis(thiz => thiz.FirstOrDefault()?.WithCopyFlag(false));
	}

	public static List<Say> CopyWithout(this Say say, params string[] thatIsnt)
	{
		return CType.AllPlayable.Where(s => !thatIsnt.Contains(s))
			.Select(s => Mutil.DeepCopy(say).WithCopyFlag()
				.EditThis(thiz => thiz.who = s))
			.ToList()
			.EditThis(thiz => thiz.FirstOrDefault()?.WithCopyFlag(false));
	}

	public static Say WithCopyFlag(this Say say, bool val = true)
	{
		return say.WithData("CopySay", val);
	}

	public static bool HasCopyFlag(this Say say)
	{
		return say.GetData("CopySay", out bool has) && has;
	}

	public static Say WithOnExecute(this Say say, Records.OnExecutePayload epl)
	{
		var data = say.GetOrMakeData("OnExecute", new List<Records.OnExecutePayload>());
		data.Add(epl);
		return say;
	}

	public static bool GetOnExecutes(this Say say, out List<Records.OnExecutePayload> data)
	{
		return say.GetData("OnExecute", out data);
	}

	public static StoryNode WithRequirement(this StoryNode node, Records.RequirementPayload cpl)
	{
		var data = node.GetOrMakeData("ExtraRequirements", new List<Records.RequirementPayload>());
		data.Add(cpl);
		return node;
	}

	public static bool GetRequirements(this StoryNode node, out List<Records.RequirementPayload> data)
	{
		return node.GetData("ExtraRequirements", out data);
	}

	public static void AddPersistentData(this StoryVars vars, string key, object data)
	{
		vars.SetData(key, data);
	}
	
	public static void AddRunData(this StoryVars vars, string key, object data)
	{
		vars.SetData(key, data);
		var list = vars.GetOrMakeData("RunResettingData", new List<string>());
		list.Add(key);
	}
	
	public static void AddCombatData(this StoryVars vars, string key, object data)
	{
		vars.SetData(key, data);
		var list = vars.GetOrMakeData("CombatResettingData", new List<string>());
		list.Add(key);
	}
	
	public static void AddTurnData(this StoryVars vars, string key, object data)
	{
		vars.SetData(key, data);
		var list = vars.GetOrMakeData("TurnResettingData", new List<string>());
		list.Add(key);
	}

	public static void ClearRunData(this StoryVars vars)
	{
		if (vars.GetData("RunResettingData", out List<string> data))
		{
			foreach (var key in data)
			{
				vars.RemoveData(key);
			}
			vars.RemoveData("RunResettingData");
		}
	}
	
	public static void ClearCombatData(this StoryVars vars)
	{
		if (vars.GetData("CombatResettingData", out List<string> data))
		{
			foreach (var key in data)
			{
				vars.RemoveData(key);
			}
			vars.RemoveData("CombatResettingData");
		}
	}
	
	public static void ClearTurnData(this StoryVars vars)
	{
		if (vars.GetData("TurnResettingData", out List<string> data))
		{
			foreach (var key in data)
			{
				vars.RemoveData(key);
			}
			vars.RemoveData("TurnResettingData");
		}
	}
	
	public static int GetMinShieldLostThisTurn(this StoryNode node)
		=> MainModFile.GetHelper().ModData.GetModDataOrDefault<int>(node, "MinShieldLostThisTurn");

	public static StoryNode SetMinShieldLostThisTurn(this StoryNode node, int value)
	{
		MainModFile.GetHelper().ModData.SetModData(node, "MinShieldLostThisTurn", value);
		return node;
	}

	public static int GetShieldLostThisTurn(this StoryVars vars)
		=> MainModFile.GetHelper().ModData.GetModDataOrDefault<int>(vars, "ShieldLostThisTurn");

	public static void SetShieldLostThisTurn(this StoryVars vars, int value)
		=> MainModFile.GetHelper().ModData.SetModData(vars, "ShieldLostThisTurn", value);

	public static HashSet<int> GetLastCardIdsInDeck(this Combat combat)
		=> MainModFile.GetHelper().ModData.ObtainModData<HashSet<int>>(combat, "LastCardIdsInDeck");
	
	public static bool? GetJustPlayedRecycleCard(this StoryNode node)
		=> MainModFile.GetHelper().ModData.GetOptionalModData<bool>(node, "JustPlayedRecycleCard");

	public static StoryNode SetJustPlayedRecycleCard(this StoryNode node, bool? value)
	{
		MainModFile.GetHelper().ModData.SetOptionalModData(node, "JustPlayedRecycleCard", value);
		return node;
	}
	
	public static bool GetJustPlayedRecycleCard(this StoryVars vars)
		=> MainModFile.GetHelper().ModData.GetModDataOrDefault<bool>(vars, "JustPlayedRecycleCard");

	public static void SetJustPlayedRecycleCard(this StoryVars vars, bool value)
		=> MainModFile.GetHelper().ModData.SetModData(vars, "JustPlayedRecycleCard", value);
	
	public static bool? GetStrengthened(this StoryNode node)
		=> MainModFile.GetHelper().ModData.GetOptionalModData<bool>(node, "Strengthened");

	public static StoryNode SetStrengthened(this StoryNode node, bool? value)
	{
		MainModFile.GetHelper().ModData.SetOptionalModData(node, "Strengthened", value);
		return node;
	}
	
	public static bool GetStrengthened(this StoryVars vars)
		=> MainModFile.GetHelper().ModData.GetModDataOrDefault<bool>(vars, "Strengthened");

	public static void SetStrengthened(this StoryVars vars, bool value)
		=> MainModFile.GetHelper().ModData.SetModData(vars, "Strengthened", value);
	
	public static bool? GetDiscounted(this StoryNode node)
		=> MainModFile.GetHelper().ModData.GetOptionalModData<bool>(node, "Discounted");

	public static StoryNode SetDiscounted(this StoryNode node, bool? value)
	{
		MainModFile.GetHelper().ModData.SetOptionalModData(node, "Discounted", value);
		return node;
	}
	
	public static bool GetDiscounted(this StoryVars vars)
		=> MainModFile.GetHelper().ModData.GetModDataOrDefault<bool>(vars, "Discounted");

	public static void SetDiscounted(this StoryVars vars, bool value)
		=> MainModFile.GetHelper().ModData.SetModData(vars, "Discounted", value);
}