using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using HexTest.Content.Items;

namespace HexTest.Content.Systems
{
	public class TestSwordPlayer : ModPlayer
	{
		public bool HasSwordOut;
		private int extraJumps;

		public bool pendingSlash;
		public int slashTimer;
		public EntitySource_ItemUse_WithAmmo slashSource;
		public int slashType;
		public int slashDamage;
		public float slashKnockback;
		public Vector2 slashDirection;

		public override void ResetEffects()
		{
			HasSwordOut = false;
		}

		public override void PostUpdate()
		{
			if (HasSwordOut)
			{
				Player.moveSpeed += 0.8f;
				Player.maxRunSpeed *= 1.8f;
				Player.accRunSpeed *= 1.8f;
			}

			if (pendingSlash)
			{
				slashTimer--;
				if (slashTimer <= 0)
				{
					pendingSlash = false;
					FireSlash();
				}
			}
		}

		private void FireSlash()
		{
			Vector2 mousePos = Main.MouseWorld;
			Vector2 spawnPos = Player.Center + slashDirection * 2f;
			Vector2 shootVelocity = (mousePos - spawnPos).SafeNormalize(Vector2.UnitY) * 20f;

			for (int i = 0; i < 3; i++)
			{
				Vector2 spread = shootVelocity.RotatedByRandom(MathHelper.ToRadians(8));
				Projectile.NewProjectile(slashSource, spawnPos, spread, slashType, slashDamage, slashKnockback, Player.whoAmI);
			}

			SoundEngine.PlaySound(SoundID.Item92, Player.position);
			SoundEngine.PlaySound(SoundID.Item60, Player.position);

			for (int i = 0; i < 15; i++)
			{
				Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.RainbowMk2, 0f, 0f, 100, default, 1.5f);
				dust.noGravity = true;
				dust.velocity = Main.rand.NextVector2Circular(8f, 8f);
				dust.color = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.7f);
			}
		}

		public override void PreUpdateMovement()
		{
			if (HasSwordOut && Player.controlJump && Player.releaseJump && Player.velocity.Y != 0f)
			{
				if (extraJumps < 2)
				{
					Player.velocity.Y = -Player.jumpSpeed * Player.gravDir;
					Player.jump = Player.jumpHeight / 2;
					extraJumps++;

					for (int i = 0; i < 10; i++)
					{
						Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.RainbowMk2, 0f, 0f, 100, default, 1.2f);
						dust.noGravity = true;
						dust.velocity = Main.rand.NextVector2Circular(4f, 4f);
						dust.color = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.7f);
					}
				}
			}

			if (Player.velocity.Y == 0f)
			{
				extraJumps = 0;
			}
		}

		public override void PostUpdateEquips()
		{
			for (int i = 0; i < 50; i++)
			{
				if (Player.inventory[i].type == ModContent.ItemType<testsword>() && Player.selectedItem == i)
				{
					HasSwordOut = true;
					break;
				}
			}
		}
	}
}
