using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Ore
{
	public class CosmerianOreBlock : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileMergeDirt[Type] = false;
			TileID.Sets.ForcedDirtMerging[Type] = false;
			MinPick = 180;

			AddMapEntry(new Color(150, 50, 255));
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
			if (!fail && !effectOnly)
			{
				Item.NewItem(null, i * 16, j * 16, 16, 16, ModContent.ItemType<CosmerianOre>());
			}
		}
	}
}
