using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Projectiles
{
	public class BloodOrb : ModProjectile
	{
		public override string Texture => "HexTest/Content/Projectiles/BloodOrb";

		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 1;
			ProjectileID.Sets.TrailCacheLength[Type] = 8;
			ProjectileID.Sets.TrailingMode[Type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.hostile = true;
			Projectile.friendly = false;
			Projectile.damage = 12;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 240;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.alpha = 40;
			Projectile.aiStyle = -1;
		}

		public override void AI()
		{
			Projectile.rotation += 0.15f;
			Lighting.AddLight(Projectile.Center, 0.8f, 0.1f, 0.05f);

			Player target = Main.player[Main.myPlayer];
			if (target != null && target.active && !target.dead)
			{
				Vector2 toTarget = target.Center - Projectile.Center;
				float len = toTarget.Length();
				if (len > 60f && len < 720f)
				{
					Vector2 dir = toTarget / len;
					Projectile.velocity = Vector2.Lerp(Projectile.velocity, dir * Projectile.velocity.Length(), 0.045f);
				}
			}

			Projectile.velocity *= 0.996f;

			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 60, default, Main.rand.NextFloat(0.7f, 1.2f));
				dust.noGravity = true;
			}
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 8; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 70, default, 1.4f);
				dust.noGravity = true;
			}
		}
	}
}