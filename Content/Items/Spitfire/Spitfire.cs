using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using HexTest.Content.Ore;

namespace HexTest.Content.Items.Spitfire
{
	public class Spitfire : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 92;
			Item.height = 44;

			Item.damage = 15;
			Item.DamageType = DamageClass.Ranged;
			Item.knockBack = 1.5f;
			Item.crit = 0;

			Item.useTime = 6;
			Item.useAnimation = 6;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.holdStyle = 0;

			Item.shoot = ProjectileID.Bullet;
			Item.shootSpeed = 14f;
			Item.useAmmo = AmmoID.Bullet;
			Item.UseSound = SoundID.Item11;

			Item.value = Item.sellPrice(gold: 5);
			Item.rare = ItemRarityID.Orange;
		}

		public override bool CanConsumeAmmo(Item ammo, Player player)
		{
			return !Main.rand.NextBool(2);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Item.UseSound = SoundID.Item11 with { Pitch = Main.rand.NextFloat(-0.15f, 0.15f) };
			return true;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(14f, 6f);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Minishark)
				.AddIngredient(ModContent.ItemType<InferniteBar>(), 5)
				.AddIngredient(ItemID.Fireblossom, 2)
				.AddIngredient(ItemID.LavaBucket)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}