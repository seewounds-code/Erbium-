using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HexTest.Content.Items
{
	/// <summary>
	/// A frosty saber that chills enemies on contact.
	/// Early-game melee weapon crafted from wood, ice, and bars.
	/// </summary>
	public class FrostSaber : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 36;

			Item.damage = 16;
			Item.DamageType = DamageClass.Melee;
			Item.knockBack = 4.5f;
			Item.useTurn = true;
			Item.autoReuse = true;

			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;

			Item.value = Item.buyPrice(copper: 60);
			Item.rare = ItemRarityID.White;

			Item.UseSound = SoundID.Item1;
			Item.scale = 1.1f;
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.Frostburn, 150);
		}

		public override void OnHitPvp(Player player, Player target, Player.HurtInfo info)
		{
			target.AddBuff(BuffID.Frostburn, 150);
		}

		public override void AddRecipes()
		{
			int[] iceBlocks = new int[] {
				ItemID.IceBlock,
				ItemID.PurpleIceBlock,
				ItemID.PinkIceBlock,
				ItemID.RedIceBlock
			};

			int[] bars = new int[] {
				ItemID.IronBar,
				ItemID.LeadBar
			};

			foreach (int ice in iceBlocks) {
				foreach (int bar in bars) {
					CreateRecipe()
						.AddIngredient(ItemID.Wood, 5)
						.AddIngredient(ice, 20)
						.AddIngredient(bar, 10)
						.AddTile(TileID.Anvils)
						.Register();
				}
			}
		}
	}
}