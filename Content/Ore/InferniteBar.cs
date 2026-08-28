using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HexTest.Content.Tiles;

namespace HexTest.Content.Ore
{
	public class InferniteBar : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 24;
			Item.maxStack = Item.CommonMaxStack;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<InferniteBarTile>();
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(gold: 1);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<InferniteOre>(), 2)
				.AddIngredient(ItemID.TitaniumOre, 2)
				.AddTile(TileID.AdamantiteForge)
				.Register();
		}
	}
}
