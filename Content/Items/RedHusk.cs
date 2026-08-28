using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Items
{
	public class RedHusk : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 14;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.sellPrice(silver: 15);
		}
	}
}