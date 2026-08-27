using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Projectiles
{
	public class SlashParticle : ModProjectile
	{
		private Color rainbowColor;

		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 90;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.light = 1f;
			Projectile.extraUpdates = 3;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 5;
			rainbowColor = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.7f);
		}

		public override void AI()
		{
			Projectile.ai[0] += 1f;

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);

			Projectile.velocity *= 1.02f;

			if (Projectile.ai[0] % 3 == 0)
			{
				rainbowColor = Main.hslToRgb((Projectile.ai[0] / 30f) % 1f, 1f, 0.7f);
			}

			for (int i = 0; i < 4; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2, 0f, 0f, 100, default, 1.3f);
				dust.noGravity = true;
				dust.velocity = Projectile.velocity * -0.3f + Main.rand.NextVector2Circular(2f, 2f);
				dust.color = rainbowColor;
				dust.fadeIn = 1.2f;
			}

			Dust trailDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond, 0f, 0f, 150, default, 0.8f);
			trailDust.noGravity = true;
			trailDust.velocity = Projectile.velocity * -0.5f;
			trailDust.color = rainbowColor;

			if (Projectile.ai[0] % 5 == 0)
			{
				SoundEngine.PlaySound(SoundID.Item15 with { Pitch = 0.5f, Volume = 0.3f }, Projectile.position);
			}
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item14 with { Pitch = 0.3f }, Projectile.position);
			SoundEngine.PlaySound(SoundID.Item60 with { Pitch = -0.2f, Volume = 0.6f }, Projectile.position);

			for (int i = 0; i < 30; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position - Projectile.velocity * 5f, Projectile.width * 3, Projectile.height * 3, DustID.RainbowMk2, 0f, 0f, 100, default, Main.rand.NextFloat(1.5f, 2.5f));
				dust.noGravity = true;
				dust.velocity = Main.rand.NextVector2Circular(12f, 12f) + Projectile.velocity * 0.5f;
				dust.color = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.7f);
				dust.fadeIn = 1.5f;
			}

			for (int i = 0; i < 20; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond, 0f, 0f, 150, default, Main.rand.NextFloat(1f, 2f));
				dust.noGravity = false;
				dust.velocity = Main.rand.NextVector2Circular(8f, 8f) + Projectile.velocity * 0.3f;
				dust.color = rainbowColor;
			}

			for (int i = 0; i < 15; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, Main.rand.NextFloat(1f, 1.5f));
				dust.noGravity = false;
				dust.velocity = Main.rand.NextVector2Circular(6f, 6f);
				dust.color = Color.Orange;
			}

			for (int i = 0; i < 8; i++)
			{
				int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud, 0f, 0f, 100, default, 1.5f);
				Main.dust[dustIndex].velocity = Main.rand.NextVector2Circular(5f, 5f);
				Main.dust[dustIndex].noGravity = true;
			}

			for (int i = 0; i < 5; i++)
			{
				Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Main.rand.NextVector2Circular(5f, 5f), Main.rand.Next(61, 64), 1.2f);
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.Ichor, 600);
			target.AddBuff(BuffID.BrokenArmor, 600);
			target.AddBuff(BuffID.Confused, 120);
			target.AddBuff(BuffID.Frostburn2, 300);
			target.AddBuff(BuffID.OnFire3, 300);

			SoundEngine.PlaySound(SoundID.NPCDeath11 with { Pitch = 0.3f }, target.position);

			for (int i = 0; i < 20; i++)
			{
				Dust dust = Dust.NewDustDirect(target.position, target.width, target.height, DustID.RainbowMk2, 0f, 0f, 100, default, 1.8f);
				dust.noGravity = true;
				dust.velocity = Main.rand.NextVector2Circular(10f, 10f);
				dust.color = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.7f);
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return rainbowColor * 0.9f;
		}
	}
}
