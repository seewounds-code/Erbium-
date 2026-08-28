using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace HexTest.Content.Misc
{
	public class BloodSuckerSystem : ModSystem
	{
		public static bool downedBloodSucker;

		public static BloodSuckerSky BloodSuckerSky { get; private set; }

		public override void OnModLoad()
		{
			if (Main.dedServ)
			{
				return;
			}

			BloodSuckerSky = new BloodSuckerSky();
			SkyManager.Instance["HexTest:BloodSuckerSky"] = BloodSuckerSky;
			SkyManager.Instance.Activate("HexTest:BloodSuckerSky", Vector2.Zero);
		}

		public override void SaveWorldData(TagCompound tag)
		{
			tag["downedBloodSucker"] = downedBloodSucker;
		}

		public override void LoadWorldData(TagCompound tag)
		{
			downedBloodSucker = tag.GetBool("downedBloodSucker");
		}
	}
}