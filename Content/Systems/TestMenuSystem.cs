using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace HexTest.Content.Systems
{
	public class TestMenuSystem : ModSystem
	{
		public static ModKeybind openMenuKey;

		public override void Load()
		{
			openMenuKey = KeybindLoader.RegisterKeybind(Mod, "OpenTestMenu", "OemSemicolon");
		}

		public override void PostUpdateWorld()
		{
			if (openMenuKey != null && openMenuKey.JustPressed)
			{
				Player player = Main.LocalPlayer;
				NPC.NewNPC(player.GetSource_Misc("TestMenu"), (int)player.Center.X, (int)player.Center.Y - 100, NPCID.KingSlime);
				SoundEngine.PlaySound(SoundID.Roar, player.position);
				Main.NewText("Spawned: King Slime", Color.Red);
			}
		}
	}
}
