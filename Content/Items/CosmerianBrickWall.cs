using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HexTest.Content.Walls;

namespace HexTest.Content.Items
{
	public class CosmerianBrickWall : ModItem
	{
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
			Item.createWall = ModContent.WallType<CosmerianBrickWallTile>();
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.buyPrice(silver: 10);
		}

		public override void AddRecipes()
		{
			CreateRecipe(4)
				.AddIngredient(ModContent.ItemType<CosmerianBrick>(), 1)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}