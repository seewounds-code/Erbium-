using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ModLoader;

namespace HexTest.Content.NPC
{
	using NPC = Terraria.NPC;

	public class CosmoUfoBossBar : ModBossBar
	{
		public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
		{
			return ModContent.Request<Texture2D>("HexTest/Content/Misc/CosmoUfoMini");
		}

		public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)
		{
			NPC npc = Main.npc[info.npcIndexToAimAt];
			if (!npc.active)
				return false;

			life = npc.life;
			lifeMax = npc.lifeMax;

			return true;
		}
	}
}