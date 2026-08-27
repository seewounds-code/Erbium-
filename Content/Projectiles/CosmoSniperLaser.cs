using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Projectiles
{
	public class CosmoSniperLaser : ModProjectile
	{
		private int trailLength = 40;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = trailLength;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 4;
			Projectile.height = 4;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 120;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.light = 0f;
			Projectile.extraUpdates = 5;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 3;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			for (int i = 0; i < Projectile.oldPos.Length; i++)
			{
				if (Projectile.oldPos[i] == Vector2.Zero) continue;

				float progress = (float)i / Projectile.oldPos.Length;
				float scale = MathHelper.Lerp(1.6f, 0.2f, progress);
				Vector2 dustPos = Projectile.oldPos[i] + Projectile.Size / 2f;

				Color beamColor = Color.Lerp(new Color(30, 150, 255), new Color(190, 40, 255), progress);

				for (int j = 0; j < 2; j++)
				{
					Dust dust = Dust.NewDustDirect(dustPos - Vector2.One * 4, 8, 8, DustID.Vortex, 0f, 0f, 50, default, scale);
					dust.noGravity = true;
					dust.velocity = Projectile.velocity * -0.02f + Main.rand.NextVector2Circular(0.3f, 0.3f);
					dust.color = beamColor;
					dust.fadeIn = 1.5f;
				}

				if (i % 2 == 0)
				{
					Color coreColor = Color.Lerp(new Color(120, 220, 255), new Color(255, 140, 255), progress);
					Dust core = Dust.NewDustDirect(dustPos - Vector2.One * 3, 6, 6, DustID.Shadowflame, 0f, 0f, 60, default, scale * 0.6f);
					core.noGravity = true;
					core.velocity = Projectile.velocity * -0.01f;
					core.color = coreColor;
				}

				if (i % 3 == 0)
				{
					Vector2 perp = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2);
					float offset = Main.rand.NextFloat(-1.5f, 1.5f);
					Color sparkColor = Color.Lerp(new Color(80, 220, 255), new Color(240, 100, 255), progress);
					Dust spark = Dust.NewDustDirect(dustPos + perp * offset - Vector2.One * 3, 6, 6, DustID.Electric, 0f, 0f, 80, default, scale * 0.7f);
					spark.noGravity = true;
					spark.velocity = Projectile.velocity * 0.2f + perp * offset * 0.2f;
					spark.color = sparkColor;
					spark.fadeIn = 1.3f;
				}
			}

			for (int i = 0; i < 4; i++)
			{
				float t = Main.rand.NextFloat();
				Color headColor = Color.Lerp(new Color(20, 160, 255), new Color(180, 40, 255), t);
				Dust dust = Dust.NewDustDirect(Projectile.Center - Vector2.One * 4, 8, 8, DustID.Vortex, 0f, 0f, 40, default, Main.rand.NextFloat(1.2f, 1.8f));
				dust.noGravity = true;
				dust.velocity = Projectile.velocity * -0.08f + Main.rand.NextVector2Circular(0.5f, 0.5f);
				dust.color = headColor;
				dust.fadeIn = 1.6f;
			}

			for (int i = 0; i < 2; i++)
			{
				Color headCore = Color.Lerp(new Color(140, 240, 255), new Color(255, 180, 255), Main.rand.NextFloat());
				Dust core = Dust.NewDustDirect(Projectile.Center - Vector2.One * 3, 6, 6, DustID.Shadowflame, 0f, 0f, 50, default, 0.5f);
				core.noGravity = true;
				core.velocity = Projectile.velocity * -0.03f;
				core.color = headCore;
			}

			Vector2 perpHead = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2);
			for (int i = 0; i < 2; i++)
			{
				float offset = Main.rand.NextFloat(-2f, 2f);
				Color sparkHead = Color.Lerp(new Color(100, 240, 255), new Color(255, 140, 255), Main.rand.NextFloat());
				Dust spark = Dust.NewDustDirect(Projectile.Center + perpHead * offset - Vector2.One * 3, 6, 6, DustID.Electric, 0f, 0f, 70, default, Main.rand.NextFloat(0.7f, 1.1f));
				spark.noGravity = true;
				spark.velocity = Projectile.velocity * 0.4f + perpHead * offset * 0.3f;
				spark.color = sparkHead;
				spark.fadeIn = 1.4f;
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.Ichor, 300);
			target.AddBuff(BuffID.Confused, 60);

			SoundEngine.PlaySound(SoundID.NPCHit4 with { Pitch = 0.5f }, target.position);

			for (int i = 0; i < 24; i++)
			{
				Vector2 vel = Main.rand.NextVector2Circular(8f, 8f);
				float t = Main.rand.NextFloat();
				Color hitColor = Color.Lerp(new Color(40, 180, 255), new Color(220, 60, 255), t);

				Dust dust = Dust.NewDustDirect(target.position, target.width, target.height, DustID.Vortex, vel.X, vel.Y, 80, default, Main.rand.NextFloat(1.4f, 2f));
				dust.noGravity = true;
				dust.color = hitColor;
				dust.fadeIn = 1.6f;
			}

			for (int i = 0; i < 12; i++)
			{
				float t = Main.rand.NextFloat();
				Color sparkColor = Color.Lerp(new Color(120, 240, 255), new Color(255, 140, 255), t);
				Dust spark = Dust.NewDustDirect(target.position, target.width, target.height, DustID.Electric, 0f, 0f, 100, default, Main.rand.NextFloat(1f, 1.5f));
				spark.noGravity = true;
				spark.velocity = Main.rand.NextVector2Circular(12f, 12f);
				spark.color = sparkColor;
			}
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item10 with { Pitch = 0.3f, Volume = 0.5f }, Projectile.position);

			for (int i = 0; i < 30; i++)
			{
				Vector2 vel = Main.rand.NextVector2Circular(7f, 7f);
				float t = Main.rand.NextFloat();
				Color killColor = Color.Lerp(new Color(30, 160, 255), new Color(220, 40, 255), t);

				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, vel.X, vel.Y, 80, default, Main.rand.NextFloat(1.2f, 2f));
				dust.noGravity = true;
				dust.color = killColor;
				dust.fadeIn = 1.8f;
			}

			for (int i = 0; i < 18; i++)
			{
				float t = Main.rand.NextFloat();
				Color sparkColor = Color.Lerp(new Color(80, 220, 255), new Color(240, 100, 255), t);
				Dust spark = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 100, default, Main.rand.NextFloat(1f, 1.6f));
				spark.noGravity = true;
				spark.velocity = Main.rand.NextVector2Circular(12f, 12f);
				spark.color = sparkColor;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			return false;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.Transparent;
		}
	}
}
