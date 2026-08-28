using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using HexTest.Content.Items;

namespace HexTest.Content.Walls
{
	public class CosmerianBrickWallTile : ModWall
	{
		public override string Texture => "HexTest/Content/Walls/CosmerianBrickWall";

		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;

			RegisterItemDrop(ModContent.ItemType<CosmerianBrickWall>());

			AddMapEntry(new Color(100, 30, 180));
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Texture2D tex = ModContent.Request<Texture2D>("HexTest/Content/Walls/CosmerianBrickWall").Value;
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
			Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + zero;
			Color color = Lighting.GetColor(i, j);
			spriteBatch.Draw(tex, drawPos, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			return false;
		}
	}
}