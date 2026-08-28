using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Systems
{
	public class CosmoPickaxePlayer : ModPlayer
	{
		private static bool isMiningExtra;

		public override void PostUpdate()
		{
			if (isMiningExtra)
				return;

			bool holdingCosmoPickaxe = false;
			for (int i = 0; i < 50; i++)
			{
				if (Player.inventory[i].type == ModContent.ItemType<Items.CosmoPickaxe>() && Player.selectedItem == i)
				{
					holdingCosmoPickaxe = true;
					break;
				}
			}

			if (!holdingCosmoPickaxe)
				return;

			if (Player.itemAnimation <= 0 || Player.HeldItem.pick <= 0)
				return;

			int targetX = Player.tileTargetX;
			int targetY = Player.tileTargetY;

			if (!WorldGen.InWorld(targetX, targetY))
				return;

			Tile centerTile = Main.tile[targetX, targetY];
			if (!centerTile.HasTile)
				return;

			int[] sideOffsets = { -1, 1 };

			foreach (int dx in sideOffsets)
			{
				int tx = targetX + dx;
				int ty = targetY;

				if (!WorldGen.InWorld(tx, ty))
					continue;

				Tile tile = Main.tile[tx, ty];
				if (!tile.HasTile)
					continue;

				if (TileID.Sets.BasicChest[tile.TileType] || TileID.Sets.Torch[tile.TileType])
					continue;

				isMiningExtra = true;
				Player.PickTile(tx, ty, Player.HeldItem.pick);
				isMiningExtra = false;

				SpawnMiningParticles(tx, ty);
			}

			SpawnMiningParticles(targetX, targetY);
			Lighting.AddLight(targetX, targetY, 0.6f, 0.2f, 0.6f);
		}

		private void SpawnMiningParticles(int x, int y)
		{
			Vector2 pos = new Vector2(x * 16 + 8, y * 16 + 8);

			for (int i = 0; i < 6; i++)
			{
				Dust dust = Dust.NewDustDirect(pos - new Vector2(8), 16, 16, DustID.Vortex, 0f, 0f, 100, default, 0.9f);
				dust.noGravity = true;
				dust.velocity = Main.rand.NextVector2Circular(3f, 3f);
				dust.color = Main.rand.NextBool() ? new Color(255, 100, 200) : new Color(100, 200, 255);
			}

			for (int i = 0; i < 4; i++)
			{
				Dust dust = Dust.NewDustDirect(pos - new Vector2(8), 16, 16, DustID.TerraBlade, 0f, 0f, 150, default, 0.6f);
				dust.noGravity = true;
				dust.velocity = Main.rand.NextVector2Circular(2f, 2f);
			}

			Lighting.AddLight(x, y, 0.5f, 0.3f, 0.5f);
		}
	}
}
