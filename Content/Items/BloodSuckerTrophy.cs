using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Items
{
	public class BloodSuckerTrophy : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 34;
			Item.maxStack = 99;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(gold: 1);
		}
	}
}