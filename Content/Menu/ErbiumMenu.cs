using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace HexTest.Content.Menu
{
	public class ErbiumMenu : ModMenu
	{
		private const int MaxSnow = 140;

		private struct Snowflake
		{
			public Vector2 Position;
			public float FallSpeed;
			public float DriftSpeed;
			public float Sway;
			public float Scale;
			public float Alpha;
			public float Phase;
		}

		private Asset<Texture2D> logo;
		private Asset<Texture2D> background;
		private Asset<Texture2D> snowTexture;

		private Snowflake[] snowflakes = new Snowflake[MaxSnow];
		private bool initialized;
		private float timer;

		public override Asset<Texture2D> Logo => logo;

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Music/MenuMusic");

		public override void Load()
		{
			logo = ModContent.Request<Texture2D>("HexTest/Content/Menu/ErbiumLogo");
			background = ModContent.Request<Texture2D>("HexTest/Content/Menu/Background");
			snowTexture = ModContent.Request<Texture2D>("HexTest/Content/Menu/Snow");
		}

		public override void OnSelected()
		{
			ResetSnow();
		}

		public override void Update(bool isOnTitleScreen)
		{
			MenuConfig config = ModContent.GetInstance<MenuConfig>();
			if (!config.MenuSnow)
				return;

			if (!initialized)
				ResetSnow();

			timer += 1f / 60f;

			for (int i = 0; i < MaxSnow; i++)
			{
				ref Snowflake flake = ref snowflakes[i];

				flake.Phase += flake.Sway;
				flake.Position.X += (float)Math.Sin(flake.Phase) * flake.DriftSpeed / 4f;
				flake.Position.Y += flake.FallSpeed;

				if (flake.Position.Y > Main.screenHeight + 24f)
				{
					flake.Position.Y = -24f;
					flake.Position.X = Main.rand.Next(0, Main.screenWidth + 1);
					flake.Alpha = Main.rand.NextFloat(0.35f, 1f);
				}

				if (flake.Position.X < -32f)
					flake.Position.X = -32f;
				else if (flake.Position.X > Main.screenWidth + 32f)
					flake.Position.X = Main.screenWidth + 32f;
			}
		}

		public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
		{
			MenuConfig config = ModContent.GetInstance<MenuConfig>();
			if (config.MenuBackground && background != null)
			{
				DrawBackgroundCover(spriteBatch, background.Value);
			}

			Texture2D logoTexture = logo?.Value;
			if (logoTexture != null)
			{
				float maxWidth = Main.screenWidth * 0.28f;
				float maxHeight = Main.screenHeight * 0.2f;
				logoScale = Math.Min(maxWidth / logoTexture.Width, maxHeight / logoTexture.Height);
				float scaledHeight = logoTexture.Height * logoScale;
				logoDrawCenter = new Vector2(Main.screenWidth / 2f, Main.screenHeight * 0.06f + scaledHeight * 0.5f);
				drawColor = Color.White;
			}

			return config.MenuLogo;
		}

		public override void PostDrawLogo(SpriteBatch spriteBatch, Vector2 logoDrawCenter, float logoRotation, float logoScale, Color drawColor)
		{
			MenuConfig config = ModContent.GetInstance<MenuConfig>();
			if (!config.MenuSnow || snowTexture == null)
				return;

			Texture2D texture = snowTexture.Value;
			float baseScale = 6f / texture.Width;
			Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);

			for (int i = 0; i < MaxSnow; i++)
			{
				Snowflake flake = snowflakes[i];
				if (flake.Alpha <= 0f)
					continue;

				spriteBatch.Draw(texture, flake.Position, null, Color.White * flake.Alpha, flake.Phase * 0.5f,
					origin, baseScale * flake.Scale, SpriteEffects.None, 0f);
			}
		}

		private void ResetSnow()
		{
			initialized = true;

			for (int i = 0; i < MaxSnow; i++)
			{
				snowflakes[i] = new Snowflake
				{
					Position = new Vector2(Main.rand.Next(0, Main.screenWidth + 1), Main.rand.Next(-Main.screenHeight, 0)),
					FallSpeed = Main.rand.NextFloat(0.6f, 1.9f),
					DriftSpeed = Main.rand.NextFloat(0.5f, 1.6f),
					Sway = Main.rand.NextFloat(0.015f, 0.05f),
					Scale = Main.rand.NextFloat(0.5f, 1.4f),
					Alpha = Main.rand.NextFloat(0.35f, 1f),
					Phase = Main.rand.NextFloat(0f, MathHelper.TwoPi),
				};
			}
		}

		private void DrawBackgroundCover(SpriteBatch spriteBatch, Texture2D texture)
		{
			float scale = Math.Max((float)Main.screenWidth / texture.Width, (float)Main.screenHeight / texture.Height);
			int width = (int)(texture.Width * scale);
			int height = (int)(texture.Height * scale);
			Vector2 position = new Vector2((Main.screenWidth - width) * 0.5f, (Main.screenHeight - height) * 0.5f);

			spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, width, height), Color.White);
		}
	}
}