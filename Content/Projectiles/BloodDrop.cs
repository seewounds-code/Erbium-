using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Projectiles
{
	public class BloodDrop : ModProjectile
	{
		public override string Texture => "HexTest/Content/Projectiles/BloodDrop";

		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 1;
			ProjectileID.Sets.TrailCacheLength[Type] = 6;
			ProjectileID.Sets.TrailingMode[Type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 20;
			Projectile.hostile = true;
			Projectile.friendly = false;
			Projectile.damage = 15;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.alpha = 30;
			Projectile.aiStyle = -1;
		}

		public override void AI()
		{
			Projectile.velocity.Y = MathHelper.Clamp(Projectile.velocity.Y + 0.45f, -16f, 16f);
			Projectile.velocity.X *= 0.995f;

			Player target = Main.player[Main.myPlayer];
			if (target != null && target.active && !target.dead)
			{
				float offset = target.Center.X - Projectile.Center.X;
				Projectile.velocity.X += MathHelper.Clamp(offset * 0.004f, -0.5f, 0.5f);
			}

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 60, default, Main.rand.NextFloat(0.7f, 1.2f));
				dust.noGravity = true;
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Burst();
			return true;
		}

		public override void OnKill(int timeLeft)
		{
			Burst();
		}

		private void Burst()
		{
			SoundEngine.PlaySound(SoundID.Splash, Projectile.Center);
			for (int i = 0; i < 10; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-2f, 2f), 70, default, 1.5f);
				dust.noGravity = true;
			}
		}
	}
}