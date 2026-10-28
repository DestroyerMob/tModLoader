using System.Collections.Generic;
using Terraria.ID;

namespace Terraria.ModLoader;

public static class CreativeCategoryLoader
{
	public static int CategoryCount => CreativeCategoryID.Count + categories.Count;

	internal static readonly List<ModCreativeCategory> categories = new List<ModCreativeCategory>();

	internal static int Add(ModCreativeCategory category)
	{
		categories.Add(category);
		return CategoryCount - 1;
	}

	public static ModCreativeCategory GetCategory(int type)
	{
		return type >= CreativeCategoryID.Count && type < CategoryCount ? categories[type - CreativeCategoryID.Count] : null;
	}

	internal static void FinishSetup()
	{
		// The menu is built once at startup, before mods load.
		Main.CreativeMenu.ResetForModReload();
	}

	internal static void Unload()
	{
		// Drop the menu before mods unload, so it no longer references their categories or any changes they made to it.
		Main.CreativeMenu.ResetForModReload();
		categories.Clear();
	}
}
