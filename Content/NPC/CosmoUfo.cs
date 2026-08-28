using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using HexTest.Content.Ore;
using HexTest.Content.Systems;

namespace HexTest.Content.CosmoNpcs
{
	using NPC = Terraria.NPC;

	[AutoloadBossHead]
	public class CosmoUfo : ModNPC
	{
		public override string Texture => "HexTest/Content/NPC/CosmoUfo";

		private Vector2 dashDir;
		private bool phase2Signaled;

		public override string BossHeadTexture => "HexTest/Content/Misc/CosmoUfoMini";

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 2;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
			NPCID.Sets.BossBestiaryPriority.Add(Type);
		}

		public override void SetDefaults()
		{
			NPC.width = 120;
			NPC.height = 100;
			NPC.lifeMax = 50000;
			NPC.damage = 75;
			NPC.defense = 34;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.boss = true;
			NPC.chaseable = true;
			NPC.friendly = false;
			NPC.dontTakeDamage = false;
			NPC.aiStyle = -1;
			NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.NPCDeath14;
			NPC.BossBar = ModContent.GetInstance<CosmoUfoBossBar>();
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter++;
			if (NPC.frameCounter >= 8)
			{
				NPC.frameCounter = 0;
				NPC.frame.Y += frameHeight;
				if (NPC.frame.Y >= Main.npcFrameCount[Type] * frameHeight)
				{
					NPC.frame.Y = 0;
				}
			}
		}

		private float HoverHeight()
		{
			return 275f + 25f * (float)Math.Sin(NPC.ai[1] * 0.04f + NPC.whoAmI * 1.7f);
		}

		public override void AI()
		{
			NPC.TargetClosest(true);
			Player player = Main.player[NPC.target];

			if (!player.active || player.dead)
			{
				NPC.velocity.X *= 0.94f;
				NPC.velocity.Y -= 0.35f;
				return;
			}

			bool phase2 = (double)NPC.life <= NPC.lifeMax * 0.5;

			if (phase2 && !phase2Signaled)
			{
				phase2Signaled = true;
				AI_PhaseShiftRing();
			}

			if (phase2 && NPC.ai[2] == 0f && NPC.ai[1] <= 0f)
			{
				NPC.ai[2] = 1f;
				NPC.ai[3] = 30f;
			}

			int dashState = (int)NPC.ai[2];

			switch (dashState)
			{
				case 1:
					{
						Vector2 toPlayer = player.Center - NPC.Center;
						Vector2 alignVel = new Vector2(MathHelper.Clamp(toPlayer.X * 0.05f, -14f, 14f), 0f);
						NPC.velocity = Vector2.Lerp(NPC.velocity, alignVel, 0.15f);
						AI_AmbientDust(1f);

						NPC.ai[3]--;
						if (NPC.ai[3] <= 0f)
						{
							NPC.ai[2] = 2f;
							NPC.ai[3] = 36f;
							dashDir = toPlayer.SafeNormalize(Vector2.UnitX);
							NPC.velocity = dashDir * 30f;
						}
					}
					break;

				case 2:
					{
						NPC.velocity = dashDir * 32f;
						AI_TrailDust(2f);

						NPC.ai[3]--;
						if (NPC.ai[3] <= 0f)
						{
							NPC.ai[2] = 3f;
							NPC.velocity *= 0.25f;
							AI_RadialBoltRing();
						}
					}
					break;

				case 3:
					{
						NPC.velocity *= 0.9f;
						AI_AmbientDust(0.5f);
						NPC.ai[2] = 0f;
						NPC.ai[1] = 180f;
					}
					break;
			}

			if (dashState == 0)
			{
				Vector2 desiredPos = player.Center - new Vector2(0f, HoverHeight());
				Vector2 toDesired = desiredPos - NPC.Center;
				float maxSpeed = phase2 ? 16f : 9f;
				Vector2 moveVel = Vector2.Clamp(toDesired * 0.07f, -Vector2.One * maxSpeed, Vector2.One * maxSpeed);
				NPC.velocity = Vector2.Lerp(NPC.velocity, moveVel, phase2 ? 0.14f : 0.1f);

				AI_AmbientDust(1f);

				if (NPC.ai[0] <= 0f)
				{
					NPC.ai[0] = 90f;
					AI_FireLaserSpread(player);
				}
			}

			NPC.ai[0]--;
			NPC.ai[1]--;
		}

		private void AI_FireLaserSpread(Player player)
		{
			Vector2 baseDir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);

			for (int i = -1; i <= 1; i++)
			{
				Vector2 dir = baseDir.RotatedBy(i * 0.25f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + dir * 60f, dir * 16f, ProjectileID.SaucerLaser, 60, 3f, Main.myPlayer);
			}
		}

		private void AI_RadialBoltRing()
		{
			for (int i = 0; i < 8; i++)
			{
				Vector2 dir = new Vector2(1f, 0f).RotatedBy(MathHelper.TwoPi * i / 8f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + dir * 40f, dir * 5f, ProjectileID.VortexLightning, 55, 2f, Main.myPlayer);
			}
		}

		private void AI_PhaseShiftRing()
		{
			for (int i = 0; i < 40; i++)
			{
				Vector2 dir = new Vector2(1f, 0f).RotatedBy(MathHelper.TwoPi * i / 40f);
				int dustType = i % 2 == 0 ? DustID.Vortex : DustID.Shadowflame;
				Color color = i % 2 == 0 ? new Color(80, 220, 255) : new Color(220, 90, 255);

				Dust dust = Dust.NewDustDirect(NPC.Center - Vector2.One * 6, 12, 12, dustType, dir.X * 5f, dir.Y * 5f, 120, default, 2.2f);
				dust.noGravity = true;
				dust.color = color;
				dust.fadeIn = 1.6f;
			}
		}

		private void AI_AmbientDust(float intensity)
		{
			for (int i = 0; i < (int)(3 * intensity); i++)
			{
				int dustType = Main.rand.Next(3) < 2 ? DustID.Vortex : DustID.Shadowflame;
				Color color = Main.rand.Next(2) == 0 ? new Color(80, 220, 255) : new Color(190, 90, 255);
				Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(NPC.width * 0.45f, NPC.height * 0.45f);

				Dust dust = Dust.NewDustDirect(pos - Vector2.One * 4, 8, 8, dustType, 0f, 0f, 90, default, Main.rand.NextFloat(0.7f, 1.5f));
				dust.noGravity = true;
				dust.velocity = Main.rand.NextVector2Circular(1.5f, 1.5f);
				dust.color = color;
				dust.fadeIn = 1.3f;
			}
		}

		private void AI_TrailDust(float density)
		{
			for (int i = 0; i < (int)(10 * density); i++)
			{
				int dustType = Main.rand.Next(2) == 0 ? DustID.Electric : DustID.Vortex;
				Color color = Color.Lerp(new Color(70, 210, 255), new Color(230, 90, 255), Main.rand.NextFloat());
				Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(26f, 16f);

				Dust dust = Dust.NewDustDirect(pos - Vector2.One * 4, 8, 8, dustType, 0f, 0f, 110, default, Main.rand.NextFloat(1f, 2f));
				dust.noGravity = true;
				dust.velocity = Main.rand.NextVector2Circular(5f, 5f) + NPC.velocity * -0.06f;
				dust.color = color;
				dust.fadeIn = 1.5f;
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CosmerianOre>(), 1, 40, 40));
		}

		public override void OnKill()
		{
			BossChecklistSystem.downedCosmoUfo = true;
		}
	}
}