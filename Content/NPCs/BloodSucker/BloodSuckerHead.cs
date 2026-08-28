using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using HexTest.Content.Items;
using HexTest.Content.Misc;
using HexTest.Content.Projectiles;

namespace HexTest.Content.NPCs.BloodSucker
{
	using NPC = Terraria.NPC;

	[AutoloadBossHead]
	public class BloodSuckerHead : ModNPC
	{
		public const int BodySegmentCount = 20;
		public const float Phase2Threshold = 0.5f;

		public const int Attack_BurrowingCharge = 1;
		public const int Attack_BloodSpitBarrage = 2;
		public const int Attack_CirclingStalk = 3;
		public const int Attack_FrenziedCharge = 4;
		public const int Attack_BloodRain = 5;
		public const int Attack_SpiralVortex = 6;
		public const int Attack_AggressiveCircle = 7;

		public override string Texture => "HexTest/Content/NPC/BloodSucker/BloodSuckerHead";
		public override string BossHeadTexture => "HexTest/Content/Misc/BloodSuckerMini";

		private int phaseTransitionPause;
		private int despawnTimer;
		private int attackStage;
		private int firedCount;
		private int lastShotTick;
		private float orbitAngle;
		private float orbitRadius;
		private Vector2 burstDirection;
		private bool burstSounded;
		private Vector2 previousVelocity; // last frame's velocity, used to clamp per-tick turns

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 1;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
			NPCID.Sets.BossBestiaryPriority.Add(Type);
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				CustomTexturePath = "HexTest/Content/NPC/BloodSucker/BloodSuckerHead",
				PortraitScale = 0.65f
			});
		}

		public override void SetDefaults()
		{
			NPC.width = 46;
			NPC.height = 46;
			NPC.lifeMax = 26000;
			NPC.damage = 90;
			NPC.defense = 30;
			NPC.lavaImmune = true;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.boss = true;
			NPC.chaseable = true;
			NPC.HitSound = SoundID.NPCHit18;
			NPC.DeathSound = SoundID.NPCDeath19;
			NPC.aiStyle = -1;

			for (int i = 0; i < BuffLoader.BuffCount; i++)
			{
				if (Main.debuff[i])
				{
					NPC.buffImmune[i] = true;
				}
			}
		}

		public override void AI()
		{
			NPC.TargetClosest(true);
			Player target = Main.player[NPC.target];

			bool targetInvalid = target == null || !target.active || target.dead;
			if (targetInvalid || Main.dayTime)
			{
				HandleDespawn();
				return;
			}

			if (NPC.localAI[1] == 0f)
			{
				SpawnSegments();
				NPC.localAI[1] = 1f;
			}

			if (NPC.ai[0] < 1f && NPC.life <= (int)(NPC.lifeMax * Phase2Threshold))
			{
				EnterPhase2();
			}

			Lighting.AddLight(NPC.Center, 1f, 0.24f, 0.08f);
			EmitBloodTrail(0.5f);

			if (phaseTransitionPause > 0)
			{
				phaseTransitionPause--;
				NPC.velocity *= 0.88f;
				return;
			}

			NPC.localAI[0]++;
			NPC.ai[1]++;

			int attack = (int)NPC.ai[2];
			if (attack <= 0)
			{
				StartAttack(NextAttack(0));
			}

			bool finished = UpdateAttack(attack, target);
			if (finished || NPC.ai[1] > CurrentAttackDuration(attack))
			{
				StartAttack(NextAttack(attack));
			}

			// ---- Inertial steering: keep the head's turns wide and smooth ----

			// >>> Adjustable per-tick values (tune to taste). <<<
			const float maxSpeed     = 22f;  // hard cap on head speed (pixels/tick)
			const float maxSteerTurn = 0.2f; // max radians the VELOCITY direction may change per tick (suggest 0.15-0.3)
			const float maxRotTurn   = 0.1f; // max radians the SPRITE rotation may change per tick (suggest 0.08-0.12)

			// 1) Cap speed first.
			Vector2 vel = NPC.velocity;
			float speed = vel.Length();
			if (speed > maxSpeed)
			{
				vel = vel.SafeNormalize(Vector2.Zero) * maxSpeed;
				NPC.velocity = vel;
				speed = maxSpeed;
			}

			// 2) Cap how much the velocity DIRECTION can rotate per tick, so the head
			//    arcs into turns instead of snapping toward the target instantly.
			if (speed > 0.01f && previousVelocity.LengthSquared() > 0.01f)
			{
				float newAngle = vel.ToRotation();
				float oldAngle = previousVelocity.ToRotation();
				float diff = MathHelper.WrapAngle(newAngle - oldAngle);
				float clamped = MathHelper.Clamp(diff, -maxSteerTurn, maxSteerTurn);
				vel = vel.RotatedBy(clamped - diff); // rotates 'vel' back inside the turn limit
				NPC.velocity = vel;
			}

			// 3) Ease the sprite rotation toward the velocity angle, clamped per tick.
			//    >>> "+ MathHelper.PiOver2" kept — the head sprite is drawn facing UP
			//        in the PNG (vertical). Omit the offset if yours faces RIGHT. <<<
			if (vel.LengthSquared() > 0.01f)
			{
				float targetRotation = vel.ToRotation() + MathHelper.PiOver2;
				float rotDiff = MathHelper.WrapAngle(targetRotation - NPC.rotation);
				NPC.rotation += MathHelper.Clamp(rotDiff, -maxRotTurn, maxRotTurn);
			}

			// Remember final velocity so next frame can limit the turn from here.
			previousVelocity = NPC.velocity;
		}

		private void StartAttack(int attack)
		{
			NPC.ai[2] = attack;
			NPC.ai[1] = 0f;
			NPC.localAI[0] = 0f;
			attackStage = 0;
			firedCount = 0;
			lastShotTick = 0;
			burstSounded = false;
			orbitAngle = NPC.AngleTo(Main.player[NPC.target].Center);
			orbitRadius = NPC.ai[0] >= 1f ? 300f : 340f;

			switch (attack)
			{
				case Attack_BurrowingCharge:
					SoundEngine.PlaySound(SoundID.WormDig, NPC.Center);
					break;
				case Attack_FrenziedCharge:
					SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
					break;
				case Attack_BloodRain:
					SoundEngine.PlaySound(SoundID.Splash, NPC.Center);
					break;
				case Attack_SpiralVortex:
					SoundEngine.PlaySound(SoundID.Item68, NPC.Center);
					break;
			}
		}

		private int CurrentAttackDuration(int attack)
		{
			switch (attack)
			{
				case Attack_BurrowingCharge: return 240;
				case Attack_BloodSpitBarrage: return 360;
				case Attack_CirclingStalk: return 480;
				case Attack_FrenziedCharge: return 150;
				case Attack_BloodRain: return 300;
				case Attack_SpiralVortex: return 360;
				case Attack_AggressiveCircle: return 300;
				default: return 60;
			}
		}

		private int NextAttack(int current)
		{
			if (NPC.ai[0] < 1f)
			{
				switch (current)
				{
					case Attack_BurrowingCharge: return Attack_BloodSpitBarrage;
					case Attack_BloodSpitBarrage: return Attack_CirclingStalk;
					default: return Attack_BurrowingCharge;
				}
			}
			switch (current)
			{
				case Attack_FrenziedCharge: return Attack_BloodRain;
				case Attack_BloodRain: return Attack_SpiralVortex;
				case Attack_SpiralVortex: return Attack_AggressiveCircle;
				default: return Attack_FrenziedCharge;
			}
		}

		private bool UpdateAttack(int attack, Player target)
		{
			switch (attack)
			{
				case Attack_BurrowingCharge: return BurrowingCharge(target);
				case Attack_BloodSpitBarrage: return BloodSpitBarrage(target);
				case Attack_CirclingStalk: return CirclingStalk(target);
				case Attack_FrenziedCharge: return FrenziedCharge(target);
				case Attack_BloodRain: return BloodRain(target);
				case Attack_SpiralVortex: return SpiralVortex(target);
				case Attack_AggressiveCircle: return AggressiveCircle(target);
				default: return true;
			}
		}

		private bool BurrowingCharge(Player target)
		{
			switch (attackStage)
			{
				case 0:
					{
						Vector2 diveGoal = new Vector2(target.Center.X, target.Bottom.Y + 260f);
						MoveToward(diveGoal, 10f, 0.07f);
						if (NPC.localAI[0] > 130f || IsWithin(NPC.Center, diveGoal, 48f))
						{
							attackStage = 1;
						}
					}
					break;
				case 1:
					{
						Vector2 emergeGoal = new Vector2(target.Center.X, target.Bottom.Y + 60f);
						MoveToward(emergeGoal, 8f, 0.05f);
						EmitDirtDust();
						if (NPC.localAI[0] % 30f == 0f)
						{
							SoundEngine.PlaySound(SoundID.WormDig, NPC.Center);
						}
						if (IsWithin(NPC.Center, emergeGoal, 40f))
						{
							attackStage = 2;
						}
					}
					break;
				case 2:
					{
						if (!burstSounded)
						{
							burstSounded = true;
							SoundEngine.PlaySound(SoundID.Item71, NPC.Center);
							burstDirection = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
						}
						MoveToward(target.Center + burstDirection * 140f, 14f, 0.06f);
						EmitDirtDust();
						if (NPC.localAI[0] > 210f || IsWithin(NPC.Center, target.Center, 60f))
						{
							return true;
						}
					}
					break;
			}
			return false;
		}

		private bool BloodSpitBarrage(Player target)
		{
			if (NPC.localAI[0] == 1f)
			{
				orbitAngle = NPC.AngleTo(target.Center);
			}
			orbitAngle += 0.015f;
			Vector2 hold = target.Center + new Vector2((float)Math.Cos(orbitAngle), (float)Math.Sin(orbitAngle) * 0.6f) * 260f;
			MoveToward(hold, 6f, 0.06f);
			NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.AngleTo(target.Center) + MathHelper.PiOver2, 0.25f);

			if (firedCount < 5 && NPC.localAI[0] - lastShotTick >= 30f)
			{
				lastShotTick = (int)NPC.localAI[0];
				float baseAngle = NPC.AngleTo(target.Center);
				float spread = (firedCount - 2) * 0.16f;
				Vector2 dir = new Vector2((float)Math.Cos(baseAngle + spread), (float)Math.Sin(baseAngle + spread));
				SpawnProjectile(NPC.Center + dir * 40f, dir * 13f, ModContent.ProjectileType<BloodOrb>(), 12);
				SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
				firedCount++;
			}
			return false;
		}

		private bool CirclingStalk(Player target)
		{
			if (NPC.localAI[0] == 1f)
			{
				orbitAngle = NPC.AngleTo(target.Center);
				orbitRadius = 340f;
			}
			orbitAngle += 7f / orbitRadius * 0.9f;
			orbitRadius = Math.Max(200f, orbitRadius - 0.12f);
			MoveToward(OrbitPosition(target, orbitRadius), 7f, 0.08f);
			return false;
		}

		private bool FrenziedCharge(Player target)
		{
			bool brakeDone = firedCount >= 1 && NPC.localAI[0] > 130f;
			switch (attackStage)
			{
				case 0:
					{
						NPC.velocity *= 0.9f;
						NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.AngleTo(target.Center) + MathHelper.PiOver2, 0.2f);
						if (NPC.localAI[0] > 25f)
						{
							attackStage = 1;
						}
					}
					break;
				case 1:
					{
						burstDirection = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
						NPC.velocity = burstDirection * 18f;
						SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
						SoundEngine.PlaySound(SoundID.Item74, NPC.Center);
						attackStage = 2;
					}
					break;
				case 2:
					{
						NPC.velocity = burstDirection * 18f;
						EmitBloodTrail(1.2f);
						if (NPC.localAI[0] > 70f || IsWithin(NPC.Center, target.Center, 40f))
						{
							attackStage = 3;
						}
					}
					break;
				case 3:
					{
						NPC.velocity *= 0.86f;
						if (firedCount == 0 && NPC.localAI[0] > 95f)
						{
							firedCount = 1;
							attackStage = 1;
						}
					}
					break;
			}
			return brakeDone;
		}

		private bool BloodRain(Player target)
		{
			if (attackStage == 0)
			{
				MoveToward(target.Center - new Vector2(0f, 320f), 8f, 0.07f);
				if (NPC.localAI[0] > 45f)
				{
					attackStage = 1;
				}
			}
			else
			{
				MoveToward(target.Center - new Vector2(0f, 320f), 6f, 0.05f);
				if (firedCount < 15 && NPC.localAI[0] % 6f == 0f)
				{
					float offX = Main.rand.NextFloat(-440f, 440f);
					Vector2 pos = new Vector2(target.Center.X + offX, target.Center.Y - 520f);
					Vector2 vel = new Vector2(Main.rand.NextFloat(-2f, 2f), 4f);
					SpawnProjectile(pos, vel, ModContent.ProjectileType<BloodDrop>(), 15);
					SoundEngine.PlaySound(SoundID.Splash, pos);
					firedCount++;
				}
				if (firedCount >= 15)
				{
					return true;
				}
			}
			return false;
		}

		private bool SpiralVortex(Player target)
		{
			MoveToward(target.Center - new Vector2(0f, 180f), 5f, 0.045f);
			if (firedCount < 12 && NPC.localAI[0] % 8f == 0f)
			{
				float angle = orbitAngle;
				orbitAngle += 0.55f;
				Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
				SpawnProjectile(NPC.Center + dir * 36f, dir * 5.5f + NPC.velocity * 0.35f, ModContent.ProjectileType<BloodOrb>(), 12);
				firedCount++;
			}
			return firedCount >= 12 && NPC.localAI[0] > 130f;
		}

		private bool AggressiveCircle(Player target)
		{
			if (NPC.localAI[0] == 1f)
			{
				orbitAngle = NPC.AngleTo(target.Center);
				orbitRadius = 300f;
			}
			orbitAngle += 14f / orbitRadius * 0.85f;
			orbitRadius = Math.Max(130f, orbitRadius - 1.2f);
			Vector2 orbitPos = target.Center + new Vector2((float)Math.Cos(orbitAngle), (float)Math.Sin(orbitAngle) * 0.72f) * orbitRadius;
			MoveToward(orbitPos, 14f, 0.12f);
			EmitBloodTrail(0.8f);
			return false;
		}

		private Vector2 OrbitPosition(Player target, float radius)
		{
			return target.Center + new Vector2((float)Math.Cos(orbitAngle), (float)Math.Sin(orbitAngle) * 0.68f) * radius;
		}

		private void MoveToward(Vector2 goal, float maxSpeed, float accel)
		{
			Vector2 toGoal = goal - NPC.Center;
			float dist = toGoal.Length();
			if (dist < 2f)
			{
				NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Zero, 0.08f);
				return;
			}
			Vector2 desired = toGoal / dist * maxSpeed;
			NPC.velocity = Vector2.Lerp(NPC.velocity, desired, accel);
			float cap = maxSpeed * 1.35f;
			float speedSqr = NPC.velocity.LengthSquared();
			if (speedSqr > cap * cap)
			{
				NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero) * cap;
			}
		}

		private bool IsWithin(Vector2 a, Vector2 b, float range)
		{
			return (a - b).Length() < range;
		}

		private void SnapRotation(Vector2 facing)
		{
			if (facing.LengthSquared() > 0.01f)
			{
				NPC.rotation = MathHelper.Lerp(NPC.rotation, facing.ToRotation() + MathHelper.PiOver2, 0.15f);
			}
		}

		private void SpawnProjectile(Vector2 position, Vector2 velocity, int type, int damage)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				return;
			}
			Projectile.NewProjectile(NPC.GetSource_FromThis(), position, velocity, type, damage, 0f, Main.myPlayer);
		}

		private void SpawnSegments()
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				return;
			}
			NPC.realLife = NPC.whoAmI; // the head owns the boss's shared health bar
			int previous = NPC.whoAmI;
			for (int i = 0; i < BodySegmentCount; i++)
			{
				int type = i == BodySegmentCount - 1 ? ModContent.NPCType<BloodSuckerTail>() : ModContent.NPCType<BloodSuckerBody>();
				int index = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, type);
				if (index >= 0 && index < Main.maxNPCs && Main.npc[index].active)
				{
					Main.npc[index].realLife = NPC.whoAmI;
					Main.npc[index].ai[0] = NPC.whoAmI;   // HP owner = the head (AI re-asserts realLife from this)
					Main.npc[index].ai[1] = previous;     // segment to follow = the one spawned in front
					Main.npc[index].netUpdate = true;
					previous = index;
				}
			}
		}

		private void EnterPhase2()
		{
			NPC.ai[0] = 1f;
			phaseTransitionPause = 90;
			Main.NewText(Language.GetTextValue("Mods.HexTest.NPCs.BloodSuckerHead.PhaseShift"), 220, 30, 10);
			SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
			for (int i = 0; i < 36; i++)
			{
				Vector2 dir = new Vector2(1f, 0f).RotatedBy(MathHelper.TwoPi * i / 36f);
				Dust dust = Dust.NewDustDirect(NPC.Center - Vector2.One * 6, 12, 12, DustID.Blood, dir.X * 5f, dir.Y * 5f, 120, default, 2f);
				dust.noGravity = true;
				dust.color = new Color(230, 40, 10);
				dust.fadeIn = 1.5f;
			}
		}

		private void HandleDespawn()
		{
			NPC.ai[2] = -1f;
			NPC.velocity *= 0.9f;
			despawnTimer++;
			if (despawnTimer >= 90)
			{
				NPC.active = false;
				NPC.netUpdate = true;
			}
		}

		private void EmitBloodTrail(float density)
		{
			for (int i = 0; i < (int)(3 * density); i++)
			{
				int dustType = Main.rand.Next(3) < 2 ? DustID.Blood : DustID.Crimson;
				Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(NPC.width * 0.5f, NPC.height * 0.5f);
				Dust dust = Dust.NewDustDirect(pos - Vector2.One * 4, 8, 8, dustType, 0f, 0f, 90, default, Main.rand.NextFloat(0.8f, 1.6f));
				dust.noGravity = true;
				dust.velocity = Main.rand.NextVector2Circular(2f, 2f);
				dust.color = Color.Lerp(Color.White, Color.Red, Main.rand.NextFloat());
				dust.fadeIn = 1.2f;
			}
		}

		private void EmitDirtDust()
		{
			Point p = NPC.Center.ToTileCoordinates();
			if (WorldGen.InWorld(p.X, p.Y) && Main.tile[p.X, p.Y].HasTile)
			{
				for (int i = 0; i < 2; i++)
				{
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Dirt, -NPC.velocity.X * 0.1f, -NPC.velocity.Y * 0.1f, 0, default, 1.1f);
					dust.noGravity = true;
				}
			}
		}

		public override void OnKill()
		{
			BloodSuckerSystem.downedBloodSucker = true;
			Player healer = Main.player[NPC.FindClosestPlayer()];
			if (healer != null && healer.active && !healer.dead)
			{
				healer.Heal(50);
				healer.HealEffect(50);
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BloodSuckerTreasureBag>()));
			npcLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 1, 5, 10));
			// Trophy drops ONLY from the corpse (1 in 10 chance). Never also put it in the bag.
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodSuckerTrophy>(), 10));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodSuckerMask>(), 4));
			// Potions: 5-10 Lesser Healing (heal 50), 2-5 Healing (heal 100); 1 in 1 chance.
			npcLoot.Add(ItemDropRule.Common(ItemID.LesserHealingPotion, 1, 5, 10));
			npcLoot.Add(ItemDropRule.Common(ItemID.HealingPotion, 1, 2, 5));
		}
	}
}