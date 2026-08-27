using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Projectiles
{
	public class CosmoSun : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 450;
			Projectile.ignoreWater = false;
			Projectile.tileCollide = false;
			Projectile.light = 0.7f;
			Projectile.extraUpdates = 2;
		}

		public override void AI()
		{
			Projectile.rotation += 0.4f * Projectile.direction;

			Projectile.ai[0]++;

			float pinkR = 1.0f, pinkG = 0.2f, pinkB = 0.8f;
			float blueR = 0.2f, blueG = 0.8f, blueB = 1.0f;
			float blend = (float)Math.Sin(Projectile.ai[0] * 0.1f) * 0.5f + 0.5f;
			float lr = pinkR * blend + blueR * (1f - blend);
			float lg = pinkG * blend + blueG * (1f - blend);
			float lb = pinkB * blend + blueB * (1f - blend);
			Lighting.AddLight(Projectile.Center, lr * 0.9f, lg * 0.9f, lb * 0.9f);

			if (Main.rand.NextBool(2))
			{
				float dustScale = 0.7f + Main.rand.NextFloat() * 0.5f;
				Color dustColor = Color.Lerp(new Color(255, 50, 200), new Color(50, 200, 255), Main.rand.NextFloat());
				Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 8, 8, DustID.Vortex, 0f, 0f, 100, default, dustScale);
				dust.velocity = Projectile.velocity * 0.3f + Main.rand.NextVector2Circular(1f, 1f);
				dust.color = dustColor;
				dust.noGravity = true;
				dust.fadeIn = 0.8f;
			}

			if (Projectile.ai[1] == 1f)
			{
				Projectile.velocity *= 0f;
				Projectile.tileCollide = false;
				if (Projectile.timeLeft < 60)
				{
					Projectile.alpha = (int)(255f * (1f - Projectile.timeLeft / 60f));
					Projectile.scale = 0.5f + 0.5f * (Projectile.timeLeft / 60f);
				}
				return;
			}

			if (Projectile.ai[1] == 1f)
			{
				Projectile.tileCollide = false;
				Projectile.velocity *= 0f;

				if (Projectile.timeLeft < 180)
				{
					Projectile.alpha = (int)(255f * (1f - Projectile.timeLeft / 180f));
					Projectile.scale = 0.5f + 0.5f * (Projectile.timeLeft / 180f);
				}
				return;
			}

			NPC target = FindNearestTarget();
			if (target != null)
			{
				Vector2 toTarget = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
				float speed = Math.Max(Projectile.velocity.Length(), 12f);
				Vector2 desiredVelocity = toTarget * speed;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.12f);
			}
			else
			{
				Projectile.velocity *= 0.97f;
			}
		}

		private NPC FindNearestTarget()
		{
			NPC closest = null;
			float closestDist = 1100f;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.lifeMax > 5)
				{
					float dist = Vector2.Distance(Projectile.Center, npc.Center);
					if (dist < closestDist)
					{
						closestDist = dist;
						closest = npc;
					}
				}
			}
			return closest;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.immune[Projectile.owner] = 6;
		}

		public override bool? CanHitNPC(NPC target)
		{
			if (Projectile.ai[1] == 1f)
				return null;
			return null;
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 15; i++)
			{
				Color color = Color.Lerp(new Color(255, 50, 200), new Color(50, 200, 255), Main.rand.NextFloat());
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0f, 0f, 100, default, 1.2f);
				dust.velocity = Main.rand.NextVector2Circular(4f, 4f);
				dust.color = color;
				dust.noGravity = true;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = ModContent.Request<Texture2D>("HexTest/Content/Projectiles/CosmoSun").Value;
			Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

			for (int i = 0; i < Projectile.oldPos.Length; i++)
			{
				if (Projectile.oldPos[i] == Vector2.Zero)
					continue;

				float progress = 1f - (float)i / Projectile.oldPos.Length;
				Color trailColor = Color.Lerp(
					new Color(255, 50, 200, 100),
					new Color(50, 200, 255, 100),
					progress
				) * progress * 0.5f;

				Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
				Main.EntitySpriteDraw(texture, drawPos, null, trailColor, Projectile.oldRot[i], origin, Projectile.scale * progress, SpriteEffects.None, 0f);
			}

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
			return false;
		}
	}
}
