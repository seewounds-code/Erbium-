using Terraria;
using Terraria.ModLoader;

namespace HexTest.Content.Buffs
{
	/// <summary>
	/// Inferniskin buff: +20% damage, +5% crit chance, −8 defense.
	/// Applied by the Inferniskin Potion.
	/// </summary>
	public class InferniskinBuff : ModBuff
	{
		public override string Texture => "HexTest/Content/Misc/InferniskinBuff";

		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = false;
			Main.buffNoTimeDisplay[Type] = false;
			Main.buffTooltip[Type] = "Increases damage by 20% and critical strike chance by 5%, but reduces defense by 8";
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetDamage(DamageClass.Generic) += 0.20f;
			player.GetCritChance(DamageClass.Generic) += 5f;
			player.statDefense -= 8;
		}
	}
}