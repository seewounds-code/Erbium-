using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Items
{
	public class CosmoBook : ModItem
	{
		private bool rightClick;

		public override void SetDefaults()
		{
			Item.damage = 130;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 10;
			Item.width = 34;
			Item.height = 34;
			Item.useTime = 14;
			Item.useAnimation = 14;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 5f;
			Item.crit = 12;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.CosmoSun>();
			Item.shootSpeed = 18f;
			Item.autoReuse = true;
			Item.rare = ItemRarityID.Cyan;
			Item.value = Item.buyPrice(gold: 35);
			Item.UseSound = new SoundStyle("HexTest/Content/Items/CosmoBookAttackSound")
			{
				SoundLimitBehavior = SoundLimitBehavior.IgnoreNew,
				MaxInstances = 3
			};
		}

		public override bool AltFunctionUse(Player player) => true;

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				rightClick = true;
				Item.damage = 110;
				Item.mana = 15;
				Item.useTime = 30;
				Item.useAnimation = 30;
				Item.shootSpeed = 14f;
				Item.autoReuse = false;
			}
			else
			{
				rightClick = false;
				Item.damage = 130;
				Item.mana = 10;
				Item.useTime = 14;
				Item.useAnimation = 14;
				Item.shootSpeed = 18f;
				Item.autoReuse = true;
			}
			return true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (rightClick)
			{
				Vector2 target = Main.MouseWorld;
				Projectile.NewProjectile(source, target, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 1f);
				return false;
			}
			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Book)
				.AddIngredient(ItemID.Ectoplasm, 10)
				.AddIngredient(ItemID.ChlorophyteBar, 12)
				.AddIngredient(ModContent.ItemType<Content.Ore.CosmerianBar>(), 10)
				.AddTile(TileID.LunarCraftingStation)
				.Register();

			CreateRecipe()
				.AddIngredient(ItemID.Book)
				.AddIngredient(ItemID.Ectoplasm, 10)
				.AddIngredient(ItemID.ChlorophyteBar, 12)
				.AddIngredient(ModContent.ItemType<Content.Ore.CosmerianBar>(), 10)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}

	public class CosmoBookPlayer : ModPlayer
	{
		public override void UpdateEquips()
		{
			if (Player.HeldItem.type == ModContent.ItemType<CosmoBook>())
			{
				Player.manaRegenBonus += 20;
				Player.manaRegenDelay = 0;
			}
		}
	}
}
