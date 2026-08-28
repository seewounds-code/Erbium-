using Terraria;
using Terraria.ModLoader;
using HexTest.Content.NPCs.BloodSucker;

namespace HexTest.Content.Music
{
	public class BloodSuckerSceneEffect : ModSceneEffect
	{
		public override bool IsSceneEffectActive(Player player)
		{
			return NPC.AnyNPCs(ModContent.NPCType<BloodSuckerHead>());
		}

		public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

		public override int Music
		{
			get
			{
				int head = NPC.FindFirstNPC(ModContent.NPCType<BloodSuckerHead>());
				bool phase2 = head != -1 && Main.npc[head].ai[0] >= 1f;
				return MusicLoader.GetMusicSlot(Mod, phase2 ? "Content/Music/BloodSucker/Phase2" : "Content/Music/BloodSucker/Phase1");
			}
		}
	}
}