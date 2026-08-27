using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HexTest.Content.Ore;
using HexTest.Content.Systems;

namespace HexTest.Content.Items
{
	public class CosmoBoot : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 28;
			Item.value = Item.sellPrice(gold: 5);
			Item.rare = ItemRarityID.Cyan;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.moveSpeed += 0.12f;
			player.maxRunSpeed = 5f;
			player.runAcceleration *= 1.5f;

			player.wingTimeMax += 60;

			player.iceSkate = true;
			player.waterWalk = true;
			player.waterWalk2 = true;
			player.lavaMax = 420;

			player.buffImmune[BuffID.Burning] = true;

			CosmoBootPlayer modPlayer = player.GetModPlayer<CosmoBootPlayer>();
			modPlayer.HasCosmoBoot = true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			float hue = (Main.GlobalTimeWrappedHourly * 0.5f) % 1f;
			Color rainbowColor = Main.hslToRgb(hue, 1f, 0.7f);

			foreach (TooltipLine line in tooltips)
			{
				if (line.Name == "DisplayName")
				{
					line.OverrideColor = rainbowColor;
				}
			}

			TooltipLine descLine = tooltips.Find(t => t.Name == "Tooltip" && t.Mod == "Terraria");
			if (descLine != null)
			{
				descLine.Text = "+12% movement speed\n" +
					"+50% running acceleration and a maximum running speed of 50 mph\n" +
					"+10% flight time\n" +
					"The ability to fly with double jump\n" +
					"Improved mobility on Ice Blocks\n" +
					"The ability to walk on liquids\n" +
					"Immunity to the Burning debuff\n" +
					"7 seconds of lava immunity";
				descLine.OverrideColor = rainbowColor;
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.LightningBoots)
				.AddIngredient(ItemID.Wire, 5)
				.AddIngredient(ItemID.PlatinumBar, 5)
				.AddIngredient(ItemID.WaterBucket)
				.AddIngredient(ItemID.LavaBucket)
				.AddIngredient(ModContent.ItemType<CosmerianBar>(), 30)
				.AddIngredient(ItemID.HoneyBucket)
				.AddTile(TileID.TinkerersWorkbench)
				.Register();
		}
	}
}
