using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using HexTest.Content.Ore;

namespace HexTest.Content.Tiles
{
	public class InferniteBarTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileShine[Type] = 1100;
			Main.tileSolid[Type] = true;
			Main.tileSolidTop[Type] = true;
			Main.tileFrameImportant[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(255, 80, 30));

			HitSound = SoundID.Tink;
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
