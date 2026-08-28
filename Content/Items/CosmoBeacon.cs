using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HexTest.Content.CosmoNpcs;
using HexTest.Content.Ore;

namespace HexTest.Content.Items
{
	using NPC = Terraria.NPC;

	public class CosmoBeacon : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 52;
			Item.maxStack = 20;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.buyPrice(gold: 10);
			Item.useAnimation = 45;
			Item.useTime = 45;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.consumable = true;
			Item.UseSound = SoundID.Roar;
		}

		public override bool CanUseItem(Player player)
		{
			return !NPC.AnyNPCs(ModContent.NPCType<CosmoUfo>());
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<CosmoUfo>());
			}

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<CosmerianBar>(), 5)
				.AddIngredient(ItemID.Wire, 10)
				.AddIngredient(ItemID.Glass, 5)
				.AddTile(TileID.AdamantiteForge)
				.Register();
		}
	}
}