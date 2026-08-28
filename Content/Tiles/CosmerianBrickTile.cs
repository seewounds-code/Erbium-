using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using HexTest.Content.Items;

namespace HexTest.Content.Tiles
{
	public class CosmerianBrickTile : ModTile
	{
		public override string Texture => "HexTest/Content/Tiles/CosmerianBrick";

		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

			RegisterItemDrop(ModContent.ItemType<CosmerianBrick>());

			AddMapEntry(new Color(150, 50, 255));
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Tile tile = Main.tile[i, j];
			tile.TileFrameX = 0;
			tile.TileFrameY = 0;
			return false;
		}
	}
}