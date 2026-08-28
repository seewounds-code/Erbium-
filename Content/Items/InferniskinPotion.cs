using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HexTest.Content.Buffs;
using HexTest.Content.Ore;

namespace HexTest.Content.Items
{
	/// <summary>
	/// Inferniskin Potion: grants the Inferniskin buff for 5 minutes.
	/// Crafted from Bottled Water, Fireblossom, Deathweed, and Infernite Ore.
	/// </summary>
	public class InferniskinPotion : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 14;
			Item.height = 24;

			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.useTurn = true;
			Item.UseSound = SoundID.Item3;

			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;

			Item.rare = ItemRarityID.Orange;
			Item.value = Item.sellPrice(silver: 5);

			Item.buffType = ModContent.BuffType<InferniskinBuff>();
			Item.buffTime = 18000;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater)
				.AddIngredient(ItemID.Fireblossom)
				.AddIngredient(ItemID.Deathweed)
				.AddIngredient(ModContent.ItemType<InferniteOre>())
				.AddTile(TileID.Bottles)
				.Register();
		}
	}
}