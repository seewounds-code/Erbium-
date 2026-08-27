using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using HexTest.Content.Projectiles;
using HexTest.Content.Systems;

namespace HexTest.Content.Items
{
	public class testsword : ModItem
	{
		private bool fullSetBonusActive;

		public override void SetDefaults()
		{
			Item.damage = 3000;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 18;
			Item.useAnimation = 18;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 12f;
			Item.value = Item.buyPrice(gold: 999);
			Item.rare = ItemRarityID.Red;
			Item.UseSound = SoundID.Item60;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<SlashParticle>();
			Item.shootSpeed = 20f;
			Item.noMelee = false;
			Item.noUseGraphic = false;
			Item.crit = 30;
			Item.mana = 0;
			Item.ArmorPenetration = 10;
		}

		public override void UpdateEquip(Player player)
		{
			fullSetBonusActive = false;

			if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
			{
				bool hasHead = false;
				bool hasChest = false;
				bool hasLegs = false;

				for (int i = 0; i < 20; i++)
				{
					int type = player.armor[i].type;
					if (type == calamity.Find<ModItem>("GodSlayerHeadMelee").Type || type == calamity.Find<ModItem>("GodSlayerHeadRanged").Type || type == calamity.Find<ModItem>("GodSlayerHeadMagic").Type || type == calamity.Find<ModItem>("GodSlayerHeadSummon").Type || type == calamity.Find<ModItem>("GodSlayerHeadGeneric").Type || type == calamity.Find<ModItem>("GodSlayerHornedGreathelm").Type)
						hasHead = true;
					if (type == calamity.Find<ModItem>("GodSlayerBody").Type || type == calamity.Find<ModItem>("GodSlayerChestplate").Type)
						hasChest = true;
					if (type == calamity.Find<ModItem>("GodSlayerLegs").Type || type == calamity.Find<ModItem>("GodSlayerLeggings").Type)
						hasLegs = true;
				}

				if (hasHead && hasChest && hasLegs)
				{
					fullSetBonusActive = true;
					player.GetDamage(DamageClass.Melee) += 0.05f;
					player.GetArmorPenetration(DamageClass.Generic) += 5f;
				}
			}
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			var modPlayer = player.GetModPlayer<TestSwordPlayer>();
			modPlayer.pendingSlash = true;
			modPlayer.slashTimer = (int)(Item.useAnimation * 0.7f);
			modPlayer.slashSource = source;
			modPlayer.slashType = type;
			modPlayer.slashDamage = damage;
			modPlayer.slashKnockback = knockback;
			modPlayer.slashDirection = velocity;

			return false;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			float hue = (Main.GlobalTimeWrappedHourly * 0.5f) % 1f;
			Color rainbowColor = Main.hslToRgb(hue, 1f, 0.7f);

			foreach (TooltipLine line in tooltips)
			{
				if (line.Name == "DisplayName")
				{
					line.OverrideColor = rainbowColor;
				}
			}

			TooltipLine descLine = tooltips.Find(t => t.Name == "Tooltip" && t.Mod == "Terraria");
			if (descLine != null)
			{
				descLine.Text = "This blade has power channeled by ancient gods, it holds tremendous power. Wearing a specific godly armor may unlock hidden powers of the God Slayer Blade";
				descLine.OverrideColor = rainbowColor;
			}

			int insertIndex = tooltips.Count;
			for (int i = 0; i < tooltips.Count; i++)
			{
				if (tooltips[i].Name == "Tooltip" && tooltips[i].Mod == "Terraria")
				{
					insertIndex = i + 1;
					break;
				}
			}

			if (fullSetBonusActive)
			{
				TooltipLine synergyLine = new TooltipLine(Mod, "SynergyBonus", "[God Slayer Synergy Active] +5% Damage, +5 Armor Penetration");
				synergyLine.OverrideColor = Color.LimeGreen;
				tooltips.Insert(insertIndex, synergyLine);
			}
			else if (ModLoader.HasMod("CalamityMod"))
			{
				TooltipLine requirementLine = new TooltipLine(Mod, "SynergyRequirement", "Requires full God Slayer Armor for bonus stats");
				requirementLine.OverrideColor = Color.Gray;
				tooltips.Insert(insertIndex, requirementLine);
			}
		}

		public override void AddRecipes()
		{
			if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
			{
				int auricBar = calamity.Find<ModItem>("AuricBar").Type;
				int cosmiliteBar = calamity.Find<ModItem>("CosmiliteBar").Type;
				int exoPrism = calamity.Find<ModItem>("ExoPrism").Type;
				int nightmareFuel = calamity.Find<ModItem>("NightmareFuel").Type;
				int endothermicEnergy = calamity.Find<ModItem>("EndothermicEnergy").Type;
				int darksunFragment = calamity.Find<ModItem>("DarksunFragment").Type;
				int necroplasm = calamity.Find<ModItem>("Necroplasm").Type;

				Recipe recipe = CreateRecipe();
				recipe.AddIngredient(auricBar, 20);
				recipe.AddIngredient(cosmiliteBar, 45);
				recipe.AddIngredient(exoPrism, 5);
				recipe.AddIngredient(nightmareFuel, 3);
				recipe.AddIngredient(endothermicEnergy, 3);
				recipe.AddIngredient(darksunFragment, 3);
				recipe.AddIngredient(necroplasm, 3);
				recipe.AddIngredient(ItemID.PinkGel, 15);
				recipe.AddIngredient(ItemID.Fireblossom, 10);
				recipe.AddTile(calamity.Find<ModTile>("DraedonsForge").Type);
				recipe.Register();
			}
			else
			{
				Recipe recipe = CreateRecipe();
				recipe.AddIngredient(ItemID.LunarBar, 10);
				recipe.AddIngredient(ItemID.FragmentSolar, 20);
				recipe.AddIngredient(ItemID.FragmentNebula, 20);
				recipe.AddIngredient(ItemID.FragmentVortex, 20);
				recipe.AddIngredient(ItemID.FragmentStardust, 20);
				recipe.AddTile(TileID.LunarCraftingStation);
				recipe.Register();
			}
		}
	}
}
