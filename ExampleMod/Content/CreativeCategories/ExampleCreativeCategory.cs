using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace ExampleMod.Content.CreativeCategories
{
	// A ModCreativeCategory adds a button to the main Journey Mode power strip, next to vanilla's Duplication, Research, Time, Weather and Personal categories.
	// Clicking it opens the element returned by CreateContent, here a small panel with two buttons.
	// The icon on the button is ExampleCreativeCategory.png, found automatically because it's next to this file and has the same name.
	public class ExampleCreativeCategory : ModCreativeCategory
	{
		public static LocalizedText HealText { get; private set; }
		public static LocalizedText RestoreManaText { get; private set; }

		// Shown below the tooltip when the player has power descriptions turned on, like vanilla categories.
		public override LocalizedText Description => this.GetLocalization(nameof(Description));

		public override void SetStaticDefaults() {
			HealText = this.GetLocalization("Heal");
			RestoreManaText = this.GetLocalization("RestoreMana");
		}

		public override UIElement CreateContent() {
			var panel = new UIPanel {
				// ContentLeft places the panel beside the main strip, where vanilla opens its windows.
				Left = new StyleDimension(ContentLeft, 0f),
				VAlign = 0.5f,
				Width = new StyleDimension(220f, 0f),
				Height = new StyleDimension(100f, 0f)
			};

			// Stop clicks on the panel from also using the held item.
			panel.OnUpdate += element => {
				if (element.IsMouseHovering) {
					Main.LocalPlayer.mouseInterface = true;
				}
			};

			panel.Append(CreateButton(HealText, 0f, () => Main.LocalPlayer.Heal(Main.LocalPlayer.statLifeMax2)));
			panel.Append(CreateButton(RestoreManaText, 44f, () => {
				Player player = Main.LocalPlayer;
				player.ManaEffect(player.statManaMax2 - player.statMana);
				player.statMana = player.statManaMax2;
			}));

			return panel;
		}

		private static UITextPanel<LocalizedText> CreateButton(LocalizedText text, float top, Action onClick) {
			var button = new UITextPanel<LocalizedText>(text) {
				Width = new StyleDimension(0f, 1f),
				Height = new StyleDimension(36f, 0f),
				Top = new StyleDimension(top, 0f)
			};

			button.OnMouseOver += (_, _) => button.BackgroundColor = new Color(73, 94, 171);
			button.OnMouseOut += (_, _) => button.BackgroundColor = new Color(63, 82, 151) * 0.7f;
			button.OnLeftClick += (_, _) => {
				SoundEngine.PlaySound(SoundID.MenuTick);
				onClick();
			};

			return button;
		}
	}
}
