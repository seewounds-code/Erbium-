using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.NPCs.BloodSucker
{
	public class BloodSuckerTail : ModNPC
	{
		public override string Texture => "HexTest/Content/NPC/BloodSucker/BloodSuckerTail";

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
			NPC.width = 28;
			NPC.height = 28;
			NPC.lifeMax = 1;
			NPC.damage = 0;
			NPC.defense = 15;
			NPC.lavaImmune = true;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.chaseable = false;
			NPC.HitSound = SoundID.NPCHit18;
			NPC.aiStyle = -1;
		}

		public override bool? CanBeHitByItem(Player player, Item item) => false;

		public override bool? CanBeHitByProjectile(Projectile projectile) => false;

		public override bool CanBeHitByNPC(NPC attacker) => false;

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

		public override void AI()
		{
			FollowSegment();
		}

		private void FollowSegment()
		{
			int followIndex = (int)NPC.ai[0];
			if (followIndex < 0 || followIndex >= Main.maxNPCs || !Main.npc[followIndex].active)
			{
				NPC.active = false;
				return;
			}

			NPC front = Main.npc[followIndex];

			if (NPC.realLife >= 0)
			{
				int headIndex = NPC.realLife;
				if (headIndex < 0 || headIndex >= Main.maxNPCs || !Main.npc[headIndex].active)
				{
					NPC.active = false;
					return;
				}
			}

			Vector2 toFront = front.Center - NPC.Center;
			float dist = toFront.Length();
			Vector2 dir = dist > 0.01f ? toFront / dist : Vector2.UnitX;

			float desiredGap = (front.width + NPC.width) / 2f + 8f;
			Vector2 desiredPos = front.Center - dir * desiredGap;

			NPC.Center = Vector2.Lerp(NPC.Center, desiredPos, 0.55f);
			NPC.rotation = dir.ToRotation() + MathHelper.PiOver2;
			NPC.localAI[0]++;
			NPC.rotation += (float)Math.Sin(NPC.whoAmI * 0.5f + NPC.localAI[0] * 0.12f) * 0.05f;
		}
	}
}