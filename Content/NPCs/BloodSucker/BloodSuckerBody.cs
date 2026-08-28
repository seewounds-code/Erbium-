using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.NPCs.BloodSucker
{
	public class BloodSuckerBody : ModNPC
	{
		public override string Texture => "HexTest/Content/NPC/BloodSucker/BloodSuckerBody";

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 1;
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Hide = true
			});
		}

		public override void SetDefaults()
		{
			NPC.width = 38;
			NPC.height = 38;
			NPC.lifeMax = 1;
			NPC.damage = 55;
			NPC.defense = 20;
			NPC.lavaImmune = true;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.chaseable = false;
			NPC.HitSound = SoundID.NPCHit18;
			NPC.aiStyle = -1;
			// Shared-health with the head: the AI() re-asserts realLife = ai[0] every frame.
			NPC.dontTakeDamage = false;
			NPC.realLife = -1;
		}

		public override bool? CanBeHitByItem(Player player, Item item) => true;

		public override bool? CanBeHitByProjectile(Projectile projectile) => true;

		public override bool CanBeHitByNPC(NPC attacker) => false;

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

		public override void AI()
		{
			// ai[0] = the head's whoAmI (set in SpawnSegments). Re-assert every frame so
			// a hit on this segment always redirects to — and reduces — the boss's real HP.
			int headIndex = (int)NPC.ai[0];
			if (headIndex < 0 || headIndex >= Main.maxNPCs || !Main.npc[headIndex].active)
			{
				NPC.active = false; // head is gone -> this segment dies with it
				return;
			}
			NPC.realLife = headIndex;

			// ai[1] = the segment to follow (the one spawned directly in front of us).
			int leaderIndex = (int)NPC.ai[1];
			if (leaderIndex < 0 || leaderIndex >= Main.maxNPCs || !Main.npc[leaderIndex].active)
			{
				NPC.active = false;
				return;
			}
			NPC leader = Main.npc[leaderIndex];

			// Direction vector this segment uses to "see" its leader.
			Vector2 distanceVector = leader.Center - NPC.Center;

			// Rotate smoothly (ease, don't snap) to face the leader.
			// >>> If the segment sprite is drawn facing RIGHT in the PNG, drop the
			//     "+ MathHelper.PiOver2". Drawn facing UP (vertical, like the head)? Keep it. <<<
			NPC.rotation = MathHelper.Lerp(NPC.rotation, distanceVector.ToRotation() + MathHelper.PiOver2, 0.15f);

			// >>> spacing = pixel distance between segment centers. Lower = pack the worm
			//     tighter, higher = spread it out. Roughly your sprite's height. <<<
			float spacing = 24f;

			// Lock this segment exactly `spacing` pixels BEHIND the leader so the body
			// bends naturally around turns instead of lagging into a straight stick.
			if (distanceVector.LengthSquared() > 0.01f) // avoid Normalize of a zero vector
			{
				NPC.Center = leader.Center - Vector2.Normalize(distanceVector) * spacing;
			}

			NPC.localAI[0]++; // optional animation timer
		}
	}
}