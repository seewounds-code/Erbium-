using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HexTest.Content.Ore;
using HexTest.Content.Tiles;

namespace HexTest.Content.Items
{
	public class InferniteBrick : ModItem
	{
		public override string Texture => "HexTest/Content/Tiles/InferniteBrick";

		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = Item.CommonMaxStack;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<InferniteBrickTile>();
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(silver: 50);
		}

		public override void AddRecipes()
		{
			CreateRecipe(2)
				.AddIngredient(ModContent.ItemType<InferniteOre>(), 1)
				.AddTile(TileID.Furnaces)
				.Register();
		}
	}
}
