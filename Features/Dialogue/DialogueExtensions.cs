using System.Collections.Generic;
using Nickel;

namespace AutumnMooncat.SpireCore.Features.Dialogue;

internal static class DialogueExt
{
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