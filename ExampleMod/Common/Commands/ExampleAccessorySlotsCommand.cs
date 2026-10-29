using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;

namespace ExampleMod.Common.Commands
{
	// This command lists the items in a player's ModAccessorySlots, including the items stored in their other equipment loadouts.
	// ModAccessorySlot.FunctionalItem and similar properties only access Main.CurrentPlayer, so ModAccessorySlotPlayer is used instead to access any player.
	public class ExampleAccessorySlotsCommand : ModCommand
	{
		public static LocalizedText UsageText { get; private set; }
		public static LocalizedText DescriptionText { get; private set; }
		public static LocalizedText PlayerNotFoundText { get; private set; }
		public static LocalizedText HeaderText { get; private set; }
		public static LocalizedText LoadoutText { get; private set; }

		public override void SetStaticDefaults() {
			string key = $"Commands.{nameof(ExampleAccessorySlotsCommand)}.";
			UsageText = Mod.GetLocalization($"{key}Usage");
			DescriptionText = Mod.GetLocalization($"{key}Description");
			PlayerNotFoundText = Mod.GetLocalization($"{key}PlayerNotFound");
			HeaderText = Mod.GetLocalization($"{key}Header");
			LoadoutText = Mod.GetLocalization($"{key}Loadout");
		}

		// CommandType.Chat means that command can be used in Chat in SP and MP
		public override CommandType Type
			=> CommandType.Chat;

		// The desired text to trigger this command
		public override string Command
			=> "modslots";

		// A short usage explanation for this command
		public override string Usage
			=> UsageText.Value;

		// A short description of this command
		public override string Description
			=> DescriptionText.Value;

		public override void Action(CommandCaller caller, string input, string[] args) {
			// Any player can be inspected, not just the one using the command. In multiplayer, other players' slots are synced to every client.
			Player player = caller.Player;
			if (args.Length > 0) {
				string name = string.Join(" ", args);
				player = Main.player.FirstOrDefault(p => p.active && p.name == name);
				if (player == null)
					throw new UsageException(PlayerNotFoundText.Format(name));
			}

			ModAccessorySlotPlayer slotPlayer = player.GetModPlayer<ModAccessorySlotPlayer>();
			AccessorySlotLoader loader = LoaderManager.Get<AccessorySlotLoader>();

			// The items currently equipped. Each collection is indexed by ModAccessorySlot.Type.
			// Only loaded slots are listed, indices at or above LoadedSlotCount belong to unloaded slots, which only exist on the local client.
			caller.Reply(HeaderText.Format(player.name));
			for (int slot = 0; slot < slotPlayer.LoadedSlotCount; slot++) {
				caller.Reply($"{loader.Get(slot, player).Name}: {Tag(slotPlayer.FunctionalItems[slot])} {Tag(slotPlayer.VanityItems[slot])} {Tag(slotPlayer.DyeItems[slot])}");
			}

			// The items stored in the other loadouts. The selected loadout is skipped, since it is empty while its items are equipped.
			foreach (ModAccessorySlotPlayer.ExEquipmentLoadout loadout in slotPlayer.Loadouts) {
				if (loadout.LoadoutIndex == player.CurrentLoadoutIndex)
					continue;

				var tags = new List<string>();
				for (int slot = 0; slot < slotPlayer.LoadedSlotCount; slot++) {
					tags.Add($"{Tag(loadout.FunctionalItems[slot])} {Tag(loadout.VanityItems[slot])} {Tag(loadout.DyeItems[slot])}");
				}
				caller.Reply(LoadoutText.Format(loadout.LoadoutIndex + 1, string.Join(" | ", tags)));
			}
		}

		private static string Tag(Item item) => item.IsAir ? "-" : ItemTagHandler.GenerateTag(item);
	}
}
