using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Projectiles
{
	public class CosmoMonster : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = 3;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = false;
			Projectile.timeLeft = 180;
			Projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];

			Projectile.ai[1]++;

			if (Projectile.ai[0] == 0f)
			{
				Projectile.rotation += 0.4f * Projectile.direction;

				float maxDistance = 400f;
				if (Projectile.ai[1] > 30f && (Projectile.Distance(owner.Center) > maxDistance || !owner.active || owner.dead))
				{
					Projectile.ai[0] = 1f;
					Projectile.ai[1] = 0f;
					Projectile.tileCollide = false;
					Projectile.netUpdate = true;
				}
			}
			else
			{
				Projectile.tileCollide = false;

				float returnSpeed = 14f;
				float acceleration = 1.2f;

				Vector2 toOwner = owner.Center - Projectile.Center;
				float dist = toOwner.Length();
				toOwner.Normalize();
				toOwner *= returnSpeed;

				Projectile.velocity = (Projectile.velocity * (acceleration - 1f) + toOwner) / acceleration;

				Projectile.rotation += 0.4f * Projectile.direction;

				if (dist < 20f)
				{
					Projectile.Kill();
				}
			}

			if (Main.rand.NextBool(2))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0f, 0f, 100, default, 0.8f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
				dust.color = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.7f);
			}
		}

		public override void OnHitNPC(Terraria.NPC target, Terraria.NPC.HitInfo hit, int damageDone)
		{
			target.immune[Projectile.owner] = 8;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.ai[0] = 1f;
			Projectile.ai[1] = 0f;
			Projectile.tileCollide = false;
			Projectile.netUpdate = true;
			return false;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			for (int i = 0; i < Projectile.oldPos.Length; i++)
			{
				if (Projectile.oldPos[i] == Vector2.Zero)
					continue;

				Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
				Color color = lightColor * (1f - (float)i / Projectile.oldPos.Length) * 0.5f;
				Main.EntitySpriteDraw(ModContent.Request<Texture2D>("HexTest/Content/Projectiles/CosmoMonster").Value, drawPos, null, color, Projectile.rotation, Projectile.Size / 2f, 1f, SpriteEffects.None, 0f);
			}
			return true;
		}
	}
}
