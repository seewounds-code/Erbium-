using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Systems
{
	public class CosmoBootPlayer : ModPlayer
	{
		public bool HasCosmoBoot;
		private int extraJumps;
		private bool wasJumping;

		public override void ResetEffects()
		{
			HasCosmoBoot = false;
		}

		public override void PreUpdateMovement()
		{
			if (Player.velocity.Y == 0f)
			{
				extraJumps = 0;
				wasJumping = false;
				return;
			}

			bool jumpJustPressed = Player.controlJump && !wasJumping;
			wasJumping = Player.controlJump;

			if (HasCosmoBoot && jumpJustPressed && extraJumps < 2)
			{
				Player.velocity.Y = -Player.jumpSpeed * Player.gravDir;
				Player.jump = Player.jumpHeight / 2;
				extraJumps++;

				Player.wingTime = Player.wingTimeMax;

				for (int i = 0; i < 10; i++)
				{
					Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Vortex, 0f, 0f, 100, default, 1.2f);
					dust.noGravity = true;
					dust.velocity = Main.rand.NextVector2Circular(4f, 4f);
					dust.color = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.7f);
				}
			}
		}
	}
}
