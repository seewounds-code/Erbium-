using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using HexTest.Content.Items;

namespace HexTest.Content.Walls
{
	public class InferniteBrickWallTile : ModWall
	{
		public override string Texture => "HexTest/Content/Walls/InferniteBrickWall";

		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;

			RegisterItemDrop(ModContent.ItemType<InferniteBrickWall>());

			AddMapEntry(new Color(200, 60, 20));
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Texture2D tex = ModContent.Request<Texture2D>("HexTest/Content/Walls/InferniteBrickWall").Value;
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
			Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + zero;
			Color color = Lighting.GetColor(i, j);
			spriteBatch.Draw(tex, drawPos, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			return false;
		}
	}
}
