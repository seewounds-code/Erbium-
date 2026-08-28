using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HexTest.Content.Ore;
using HexTest.Content.Projectiles;

namespace HexTest.Content.Items.Spitfire
{
	public class InferniteBullet : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 8;
			Item.height = 16;

			Item.damage = 9;
			Item.DamageType = DamageClass.Ranged;
			Item.knockBack = 2f;
			Item.crit = 0;

			Item.maxStack = 999;
			Item.consumable = true;
			Item.ammo = AmmoID.Bullet;

			Item.shoot = ModContent.ProjectileType<InferniteBulletProjectile>();
			Item.shootSpeed = 16f;

			Item.value = Item.sellPrice(copper: 10);
			Item.rare = ItemRarityID.Orange;
		}

		public override void AddRecipes()
		{
			CreateRecipe(50)
				.AddIngredient(ItemID.MusketBall, 50)
				.AddIngredient(ModContent.ItemType<InferniteOre>())
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}