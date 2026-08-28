using System;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ModLoader;
using HexTest.Content.Items;
using HexTest.Content.NPCs.BloodSucker;

namespace HexTest.Content.Misc
{
	public class BloodSuckerBossLog : ModSystem
	{
		public override void PostSetupContent()
		{
			if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklist))
			{
				return;
			}
			if (bossChecklist.Version < new Version(1, 6))
			{
				return;
			}

			bossChecklist.Call(
				"LogBoss",
				Mod,
				nameof(BloodSuckerHead),
				3.5f,
				() => BloodSuckerSystem.downedBloodSucker,
				ModContent.NPCType<BloodSuckerHead>(),
				new Dictionary<string, object>()
				{
					["spawnItems"] = ModContent.ItemType<BloodSpores>(),
					["spawnInfo"] = Language.GetOrRegister("Mods.HexTest.BossChecklistIntegration.BloodSucker.SpawnInfo", () => "Use a [i:HexTest/BloodSpores] at night."),
					["collectibles"] = new List<int>()
					{
						ModContent.ItemType<BloodSuckerTrophy>(),
						ModContent.ItemType<BloodSuckerMask>()
					}
				});
		}
	}
}