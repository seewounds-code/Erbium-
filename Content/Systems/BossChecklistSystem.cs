using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using HexTest.Content.Items;
using HexTest.Content.CosmoNpcs;
using HexTest.Content.Ore;

namespace HexTest.Content.Systems
{
	public class BossChecklistSystem : ModSystem
	{
		public static bool downedCosmoUfo;

		public override void PostSetupContent()
		{
			if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklist))
				return;

			if (bossChecklist.Version < new Version(1, 6))
				return;

			bossChecklist.Call(
				"LogBoss",
				Mod,
				"CosmoUfo",
				11.5f,
				() => downedCosmoUfo,
				ModContent.NPCType<CosmoUfo>(),
				new Dictionary<string, object>()
				{
					["spawnItems"] = ModContent.ItemType<CosmoBeacon>(),
					["collectibles"] = new List<int>() { ModContent.ItemType<CosmerianOre>() },
					["spawnInfo"] = Language.GetOrRegister("Mods.HexTest.BossChecklistIntegration.CosmoUfo.SpawnInfo", () => "Use a [i:HexTest/CosmoBeacon] anywhere and anytime.")
				});
		}
	}
}