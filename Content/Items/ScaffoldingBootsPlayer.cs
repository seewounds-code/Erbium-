using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Items
{
	/// <summary>
	/// Per-player data for the Scaffolding Boots. While the boots are equipped
	/// and the player moves horizontally over empty space, a platform is pulled
	/// from the player's inventory (held item first, then the hotbar/inventory)
	/// and placed underneath their feet.
	/// </summary>
	public class ScaffoldingBootsPlayer : ModPlayer
	{
		/// <summary>True while the Scaffolding Boots are equipped.</summary>
		public bool hasScaffoldingBoots;

		public override void ResetEffects()
		{
			hasScaffoldingBoots = false;
		}

		public override void PostUpdate()
		{
			if (!hasScaffoldingBoots)
				return;

			// Only scaffold while the player is actually moving horizontally.
			if (Math.Abs(Player.velocity.X) <= 0.1f)
				return;

			// Tile one block directly below the center of the player's feet.
			int tileX = (int)(Player.position.X + Player.width / 2f) / 16;
			int tileY = (int)(Player.position.Y + Player.height + 2f) / 16;

			if (!WorldGen.InWorld(tileX, tileY))
				return;

			// Only scaffold into empty (air) tiles.
			if (Main.tile[tileX, tileY].HasTile)
				return;

			Item platform = FindPlatform();
			if (platform == null)
				return;

			if (WorldGen.PlaceTile(tileX, tileY, platform.createTile, mute: false, style: platform.placeStyle))
			{
				// Consume one platform.
				platform.stack--;
				if (platform.stack <= 0)
					platform.TurnToAir();

				SoundEngine.PlaySound(SoundID.Dig, Player.position);

				// Sync the placed tile to other clients in multiplayer.
				if (Main.netMode == NetmodeID.MultiplayerClient)
					NetMessage.SendTileSquare(-1, tileX, tileY, 1);
			}
		}

		/// <summary>
		/// Returns the first valid platform item to use: the held item if it is
		/// a platform, otherwise the first platform found in the inventory.
		/// </summary>
		private Item FindPlatform()
		{
			if (IsPlatform(Player.HeldItem) && Player.HeldItem.stack > 0)
				return Player.HeldItem;

			for (int i = 0; i <= 50 && i < Player.inventory.Length; i++)
			{
				Item item = Player.inventory[i];
				if (item != null && IsPlatform(item) && item.stack > 0)
					return item;
			}

			return null;
		}

		/// <summary>True if the item places a platform tile.</summary>
		private static bool IsPlatform(Item item)
		{
			if (item == null || item.IsAir)
				return false;

			int tileType = item.createTile;
			return tileType >= 0 && tileType < TileID.Sets.Platforms.Length && TileID.Sets.Platforms[tileType];
		}
	}
}