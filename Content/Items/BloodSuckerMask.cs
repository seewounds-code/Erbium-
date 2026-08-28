using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Items
{
	public class BloodSuckerMask : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 18;
			Item.maxStack = 99;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(gold: 1);
		}
	}
}