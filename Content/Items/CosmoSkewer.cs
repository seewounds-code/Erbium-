using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Items
{
	public class CosmoSkewer : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 32;
			Item.damage = 85;
			Item.knockBack = 4f;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.rare = ItemRarityID.Cyan;
			Item.value = Item.sellPrice(gold: 5);
			Item.shoot = ModContent.ProjectileType<Projectiles.CosmoMonster>();
			Item.shootSpeed = 12f;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.DamageType = DamageClass.Throwing;

			if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
			{
				try
				{
					DamageClass rogue = calamity.Find<DamageClass>("RogueDamageClass");
					if (rogue != null)
						Item.DamageType = rogue;
				}
				catch
				{
					try
					{
						DamageClass rogue = calamity.Find<DamageClass>("Rogue");
						if (rogue != null)
							Item.DamageType = rogue;
					}
					catch { }
				}
			}
		}

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.CosmoMonster>()] < 3;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Ore.CosmerianBar>(), 12)
				.AddIngredient(ItemID.HellstoneBar, 8)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}
