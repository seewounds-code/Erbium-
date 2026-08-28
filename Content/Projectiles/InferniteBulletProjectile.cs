using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Projectiles
{
	public class InferniteBulletProjectile : ModProjectile
	{
		public override string Texture => "HexTest/Content/Items/Spitfire/InferniteBullet";

		public override void SetDefaults()
		{
			Projectile.width = 4;
			Projectile.height = 4;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 0;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = true;
			Projectile.aiStyle = ProjAIStyleID.Arrow;
		}

		public override void AI()
		{
			if (Main.rand.NextBool(2))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position - new Vector2(4), 8, 8, DustID.Torch, -Projectile.velocity.X * 0.1f, -Projectile.velocity.Y * 0.1f, 90, default, Main.rand.NextFloat(0.6f, 0.9f));
				dust.noGravity = true;
			}
			base.AI();
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.OnFire, 180);

			for (int i = 0; i < Main.rand.Next(3, 6); i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 100, default, 1.2f);
				dust.noGravity = true;
			}
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

			for (int i = 0; i < 5; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 100, default, Main.rand.NextFloat(1f, 1.5f));
				dust.noGravity = true;
			}
		}
	}
}