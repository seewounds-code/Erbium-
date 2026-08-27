using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Ore
{
	public class CosmerianBar : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 24;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.buyPrice(gold: 1);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<CosmerianOre>(), 2)
				.AddIngredient(ItemID.LunarOre, 2)
				.AddTile(TileID.AdamantiteForge)
				.Register();
		}
	}
}
