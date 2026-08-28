using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HexTest.Content.Items;

namespace HexTest.Content.Tiles
{
	public class InferniteBrickTile : ModTile
	{
		public override string Texture => "HexTest/Content/Tiles/InferniteBrick";

		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

			RegisterItemDrop(ModContent.ItemType<InferniteBrick>());

			AddMapEntry(new Color(255, 80, 30));
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Tile tile = Main.tile[i, j];
			tile.TileFrameX = 0;
			tile.TileFrameY = 0;
			return false;
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			if (LavaNearby(i, j))
			{
				fail = true;
				effectOnly = false;
			}
		}

		private static bool LavaNearby(int i, int j)
		{
			for (int dx = -1; dx <= 1; dx++)
			{
				for (int dy = -1; dy <= 1; dy++)
				{
					int tx = i + dx;
					int ty = j + dy;
					if (WorldGen.InWorld(tx, ty))
					{
						Tile tile = Main.tile[tx, ty];
						if (tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Lava)
							return true;
					}
				}
			}
			return false;
		}
	}
}
