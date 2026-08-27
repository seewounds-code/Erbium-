using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Ore
{
	public class CosmerianOre : ModItem
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
			Item.createTile = ModContent.TileType<CosmerianOreBlock>();
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.buyPrice(silver: 50);
		}
	}
}
