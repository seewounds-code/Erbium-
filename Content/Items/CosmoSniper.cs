using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using HexTest.Content.Projectiles;
using HexTest.Content.Ore;

namespace HexTest.Content.Items
{
	public class CosmoSniperPlayer : ModPlayer
	{
		public int burstCounter;
		public int cooldownTimer;
		public bool justFired;

		public override void ResetEffects()
		{
			justFired = false;
		}

		public override void UpdateEquips()
		{
			if (cooldownTimer > 0)
				cooldownTimer--;
		}
	}

	public class CosmoSniper : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 6000;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 50;
			Item.height = 28;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 7.5f;
			Item.value = Item.buyPrice(gold: 999);
			Item.rare = ItemRarityID.Red;
			Item.UseSound = SoundID.Item5;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<CosmoSniperLaser>();
			Item.shootSpeed = 30f;
			Item.noMelee = true;
			Item.useAmmo = AmmoID.Bullet;
			Item.consumeAmmoOnLastShotOnly = true;
			Item.mana = 0;
			Item.crit = 20;
			Item.ArmorPenetration = 15;
		}

		public override bool CanUseItem(Player player)
		{
			CosmoSniperPlayer modPlayer = player.GetModPlayer<CosmoSniperPlayer>();

			if (modPlayer.cooldownTimer > 0)
			{
				modPlayer.justFired = true;
				return false;
			}

			if (modPlayer.burstCounter >= 5)
			{
				modPlayer.burstCounter = 0;
				modPlayer.cooldownTimer = 120;
				SoundEngine.PlaySound(new SoundStyle("HexTest/Content/Items/ReloadCosmo") with { Volume = 0.8f, Pitch = 0f }, player.position);
				modPlayer.justFired = true;
				return false;
			}

			if (modPlayer.burstCounter == 0 && !modPlayer.justFired)
			{
				Item.useTime = 1;
				Item.useAnimation = 1;
			}
			else
			{
				Item.useTime = 15;
				Item.useAnimation = 15;
			}

			return true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			CosmoSniperPlayer modPlayer = player.GetModPlayer<CosmoSniperPlayer>();
			modPlayer.justFired = true;

			float pitch = 0.1f * modPlayer.burstCounter;
			SoundEngine.PlaySound(new SoundStyle("HexTest/Content/Items/LaserCanon") with { Volume = 0.7f, Pitch = pitch }, player.position);

			Vector2 muzzleOffset = velocity.SafeNormalize(Vector2.UnitX) * 40f;
			Vector2 spawnPos = player.Center + muzzleOffset;

			for (int i = 0; i < 5; i++)
			{
				Vector2 dustVel = velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(4f, 8f);
				Dust dust = Dust.NewDustDirect(spawnPos - new Vector2(4), 8, 8, DustID.Vortex, dustVel.X, dustVel.Y, 150, default, 0.8f);
				dust.noGravity = true;
				dust.color = new Color(100, 0, 255);
			}

			Projectile.NewProjectile(source, spawnPos, velocity, type, damage, knockback, player.whoAmI);

			modPlayer.burstCounter++;

			return false;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			position = position + velocity.SafeNormalize(Vector2.UnitX) * 45f;
		}

		public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
		{
			float hue = (Main.GlobalTimeWrappedHourly * 0.4f + 0.65f) % 1f;
			Color cosmicColor = Main.hslToRgb(hue, 1f, 0.7f);

			foreach (TooltipLine line in tooltips)
			{
				if (line.Name == "DisplayName")
				{
					line.OverrideColor = cosmicColor;
				}
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<CosmerianBar>(), 20)
				.AddIngredient(ItemID.LunarBar, 10)
				.AddIngredient(ItemID.Glass, 5)
				.AddIngredient(ItemID.Wire, 5)
				.AddTile(TileID.AdamantiteForge)
				.Register();
		}
	}
}
