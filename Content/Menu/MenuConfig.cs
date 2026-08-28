using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace HexTest.Content.Menu
{
	public class MenuConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[Header("$Mods.HexTest.Configs.MenuConfig.MenuHeader")]

		[DefaultValue(true)]
		public bool MenuBackground;

		[DefaultValue(true)]
		public bool MenuLogo;

		[DefaultValue(true)]
		public bool MenuSnow;
	}
}