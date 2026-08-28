using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using HexTest.Content.NPCs.BloodSucker;

namespace HexTest.Content.Items
{
	public class BloodSuckerTreasureBag : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.BossBag[Type] = true;
			ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;
			Item.ResearchUnlockCount = 3;
		}

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 28;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.rare = ItemRarityID.Expert;
			Item.expert = true;
			Item.value = Item.sellPrice(gold: 5);
		}

		public override bool CanRightClick() => true;

		public override void ModifyItemLoot(ItemLoot itemLoot)
		{
			// 1x Boss Mask — guaranteed, exactly one.
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodSuckerMask>(), 1, 1, 1));

			// 5-10x Healing Potion (heals 100 HP) — guaranteed.
			itemLoot.Add(ItemDropRule.Common(ItemID.HealingPotion, 1, 5, 10));

			// 1-3x Gold Coin — guaranteed.
			itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 1, 1, 3));

			// IMPORTANT: do NOT add the Trophy here. BloodSuckerTrophy drops directly
			// from the boss corpse in BloodSuckerHead.ModifyNPCLoot (1-in-10). Putting it
			// in the bag too would duplicate it in Expert/Master Mode.
		}
	}
}