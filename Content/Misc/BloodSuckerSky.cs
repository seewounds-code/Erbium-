using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using static Terraria.ModLoader.ModContent;
using HexTest.Content.NPCs.BloodSucker;

namespace HexTest.Content.Misc
{
	public class BloodSuckerSky : CustomSky
	{
		private const float FadeInRate = 0.025f;
		private const float FadeOutRate = 0.0056f;

		private float skyOpacity;

		public override void Update(GameTime gameTime)
		{
			if (NPC.AnyNPCs(NPCType<BloodSuckerHead>()))
			{
				skyOpacity = MathHelper.Clamp(skyOpacity + FadeInRate, 0f, 1f);
			}
			else
			{
				skyOpacity = MathHelper.Clamp(skyOpacity - FadeOutRate, 0f, 1f);
			}
			Opacity = skyOpacity;
		}

		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			if (skyOpacity <= 0f || minDepth >= 1f || maxDepth <= 0f)
			{
				return;
			}

			Color tint = new Color(190, 25, 15) * skyOpacity;
			spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), tint);
		}

		public override bool IsActive()
		{
			return skyOpacity > 0f;
		}

		public override void Reset()
		{
			skyOpacity = 0f;
		}

		public override void Activate(Vector2 position, params object[] args)
		{
		}

		public override void Deactivate(params object[] args)
		{
		}
	}
}