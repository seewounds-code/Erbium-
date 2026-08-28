using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Items
{
	public class CosmoPickaxe : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 80;
			Item.DamageType = DamageClass.Melee;
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 18;
			Item.useAnimation = 18;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5f;
			Item.value = Item.sellPrice(gold: 5);
			Item.rare = ItemRarityID.Cyan;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.useTurn = true;
			Item.scale = 1.1f;
			Item.pick = 210;
			Item.noMelee = false;
			Item.noUseGraphic = false;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Ore.CosmerianBar>(), 15)
				.AddIngredient(ItemID.LunarBar, 10)
				.AddIngredient(ItemID.FallenStar, 5)
				.AddTile(TileID.MythrilAnvil)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}
}
