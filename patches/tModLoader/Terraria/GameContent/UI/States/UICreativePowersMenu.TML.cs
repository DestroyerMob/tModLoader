using System.Collections.Generic;
using Terraria.GameContent.Creative;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Terraria.GameContent.UI.States;

public partial class UICreativePowersMenu
{
	private readonly Dictionary<int, UIElement> _modCategoryContent = new Dictionary<int, UIElement>();
	private int _lastMainCategory;

	internal int CurrentMainCategory => _mainCategory.CurrentOption;

	private void AddModCategoryButtons(CreativePowerUIElementRequestInfo request, List<UIElement> list)
	{
		foreach (ModCreativeCategory category in CreativeCategoryLoader.categories) {
			GroupOptionButton<int> button = CreativePowersHelper.CreateCategoryButton(request, category.Type, 0);
			UIElement icon = category.CreateIcon();
			icon.IgnoresMouseInteraction = true;
			button.Append(icon);
			button.OnLeftClick += MainCategoryButtonClick;
			button.OnUpdate += affectedElement => ModCategoryButton_OnUpdate(affectedElement, category);
			_mainCategory.Buttons.Add(category.Type, button);
			list.Add(button);
		}
	}

	private static void ModCategoryButton_OnUpdate(UIElement affectedElement, ModCreativeCategory category)
	{
		if (!affectedElement.IsMouseHovering)
			return;

		bool isSelected = ((GroupOptionButton<int>)affectedElement).IsSelected;
		string text = (isSelected ? category.CloseTooltip : category.OpenTooltip).Value;
		string description = category.Description.Value;
		if (CreativePowerSettings.ShouldPowersBeElaborated && !string.IsNullOrEmpty(description))
			text += "\n" + description;

		Main.instance.MouseTextNoOverride(text);
	}

	private void RefreshModCategoryContent()
	{
		int option = _mainCategory.CurrentOption;
		if (option != _lastMainCategory) {
			CreativeCategoryLoader.GetCategory(_lastMainCategory)?.OnClose();
			CreativeCategoryLoader.GetCategory(option)?.OnOpen();
			_lastMainCategory = option;
		}

		if (CreativeCategoryLoader.GetCategory(option) is not ModCreativeCategory category)
			return;

		if (!_modCategoryContent.TryGetValue(option, out UIElement content))
			_modCategoryContent[option] = content = category.CreateContent();

		if (content != null)
			_container.Append(content);
	}
}
