using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using HexTest.Content.NPCs.BloodSucker;

namespace HexTest.Content.Items
{
	public class BloodSpores : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 20;
			Item.maxStack = 20;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.sellPrice(gold: 3);
			Item.useAnimation = 45;
			Item.useTime = 45;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.consumable = true;
			Item.UseSound = SoundID.Roar;
		}

		public override bool CanUseItem(Player player)
		{
			if (Main.dayTime)
			{
				Main.NewText(Language.GetTextValue("Mods.HexTest.Items.BloodSpores.DayMessage"), 220, 30, 10);
				return false;
			}
			if (NPC.AnyNPCs(ModContent.NPCType<BloodSuckerHead>()))
			{
				Main.NewText(Language.GetTextValue("Mods.HexTest.Items.BloodSpores.AlreadyActive"), 220, 30, 10);
				return false;
			}
			return true;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<BloodSuckerHead>());
			}
			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.LesserHealingPotion, 5)
				.AddIngredient(ModContent.ItemType<RedHusk>())
				.AddIngredient(ItemID.Deathweed, 5)
				.AddTile(TileID.DemonAltar)
				.Register();
		}
	}
}