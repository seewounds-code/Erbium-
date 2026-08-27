using Terraria;
using Terraria.ModLoader;
using HexTest.Content.Items;

namespace HexTest.Content.Systems
{
	public class GiveTestSword : ModPlayer
	{
		public override void OnEnterWorld()
		{
			Player.QuickSpawnItem(Player.GetSource_FromThis(), ModContent.ItemType<testsword>());
		}
	}
}
