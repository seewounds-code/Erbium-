using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Items
{
	/// <summary>
	/// Scaffolding Boots: Hermes-style run speed plus auto-placing platforms
	/// from the player's inventory beneath their feet while moving over gaps.
	/// </summary>
	public class ScaffoldingBoots : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 20;
			Item.maxStack = 1;

			Item.accessory = true;

			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ItemRarityID.Green;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// Hermes Boots run speed.
			player.accRunSpeed = 6f;

			player.GetModPlayer<ScaffoldingBootsPlayer>().hasScaffoldingBoots = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.BambooBlock, 15)
				.AddIngredient(ItemID.RichMahogany, 20)
				.AddIngredient(ItemID.HermesBoots)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}