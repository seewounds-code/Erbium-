using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace HexTest.Content.Ore
{
	public class CosmerianWorldGen : ModSystem
	{
		public static List<Point> HousePositions = new List<Point>();
		public static ModKeybind TeleportKey;

		public override void Load()
		{
			TeleportKey = KeybindLoader.RegisterKeybind(Mod, "TeleportToHouse", "H");
		}

		public override void Unload()
		{
			TeleportKey = null;
		}

		public override void ModifyWorldGenTasks(List<GenPass> list, ref double totalWeight)
		{
			int index = list.FindIndex(genpass => genpass.Name == "Micro Biomes");
			if (index == -1)
			{
				index = list.FindIndex(genpass => genpass.Name == "Final Cleanup");
			}

			if (index != -1)
			{
				list.Insert(index, new CosmerianOrePass());
				list.Insert(index + 1, new CaveHousePass());
			}

			// Infernite replaces Calamity's Brimstone Slag, which is generated in the
			// "Brimstone Crag" pass that runs near the very end of worldgen. Anchor our
			// pass right after it so the slag tiles actually exist to be replaced.
			int cragIndex = list.FindIndex(genpass => genpass.Name == "Brimstone Crag");
			if (cragIndex != -1)
			{
				list.Insert(cragIndex + 1, new InferniteWorldGen());
			}
			else
			{
				int cleanupIndex = list.FindIndex(genpass => genpass.Name == "Final Cleanup");
				if (cleanupIndex != -1)
				{
					list.Insert(cleanupIndex + 1, new InferniteWorldGen());
				}
				else
				{
					list.Add(new InferniteWorldGen());
				}
			}
		}

		public override void PostUpdateWorld()
		{
			if (TeleportKey != null && TeleportKey.JustPressed && HousePositions.Count > 0)
			{
				Player player = Main.LocalPlayer;
				Point nearest = HousePositions[0];
				float nearestDist = Vector2.Distance(player.Center, nearest.ToVector2() * 16);

				for (int i = 1; i < HousePositions.Count; i++)
				{
					float dist = Vector2.Distance(player.Center, HousePositions[i].ToVector2() * 16);
					if (dist < nearestDist)
					{
						nearest = HousePositions[i];
						nearestDist = dist;
					}
				}

				player.Center = new Vector2(nearest.X * 16 + 8, nearest.Y * 16);
				player.velocity = Vector2.Zero;
				Terraria.Audio.SoundEngine.PlaySound(SoundID.Item6, player.position);
				Main.NewText("Teleported to Cosmerian Cave House!", new Color(150, 100, 255));
			}
		}

		public override void SaveWorldData(TagCompound tag)
		{
			List<int> xList = new List<int>();
			List<int> yList = new List<int>();
			foreach (Point p in HousePositions)
			{
				xList.Add(p.X);
				yList.Add(p.Y);
			}
			tag["houseX"] = xList;
			tag["houseY"] = yList;
		}

		public override void LoadWorldData(TagCompound tag)
		{
			HousePositions.Clear();
			if (tag.ContainsKey("houseX") && tag.ContainsKey("houseY"))
			{
				IList<int> xList = tag.GetList<int>("houseX");
				IList<int> yList = tag.GetList<int>("houseY");
				int count = Math.Min(xList.Count, yList.Count);
				for (int i = 0; i < count; i++)
				{
					HousePositions.Add(new Point(xList[i], yList[i]));
				}
			}
		}
	}

	public class CosmerianOrePass : GenPass
	{
		public CosmerianOrePass() : base("CosmerianOre", 100f)
		{
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Mining cosmerian";

			int targetType = ModContent.TileType<CosmerianOreBlock>();

			List<Point> granitePositions = new List<Point>();

			for (int x = 0; x < Main.maxTilesX; x++)
			{
				for (int y = 0; y < Main.maxTilesY; y++)
				{
					Tile tile = Main.tile[x, y];
					if (tile.HasTile && tile.TileType == TileID.Granite)
					{
						granitePositions.Add(new Point(x, y));
					}
				}
			}

			if (granitePositions.Count == 0)
			{
				return;
			}

			HashSet<Point> graniteSet = new HashSet<Point>(granitePositions);
			HashSet<Point> visited = new HashSet<Point>();
			List<List<Point>> clusters = new List<List<Point>>();

			for (int i = 0; i < granitePositions.Count; i++)
			{
				Point start = granitePositions[i];
				if (visited.Contains(start))
				{
					continue;
				}

				List<Point> cluster = new List<Point>();
				Queue<Point> queue = new Queue<Point>();
				queue.Enqueue(start);
				visited.Add(start);

				while (queue.Count > 0)
				{
					Point current = queue.Dequeue();
					cluster.Add(current);

					for (int dx = -40; dx <= 40; dx++)
					{
						for (int dy = -40; dy <= 40; dy++)
						{
							if (dx == 0 && dy == 0)
							{
								continue;
							}

							Point neighbor = new Point(current.X + dx, current.Y + dy);
							if (!visited.Contains(neighbor) && graniteSet.Contains(neighbor))
							{
								visited.Add(neighbor);
								queue.Enqueue(neighbor);
							}
						}
					}
				}

				clusters.Add(cluster);
			}

			int clustersProcessed = 0;

			foreach (List<Point> cluster in clusters)
			{
				int veinCount = WorldGen.genRand.Next(1, 4);

				for (int v = 0; v < veinCount; v++)
				{
					Point center = cluster[WorldGen.genRand.Next(cluster.Count)];
					int radius = WorldGen.genRand.Next(5, 9);

					for (int dx = -radius; dx <= radius; dx++)
					{
						for (int dy = -radius; dy <= radius; dy++)
						{
							int tx = center.X + dx;
							int ty = center.Y + dy;

							if (!WorldGen.InWorld(tx, ty))
							{
								continue;
							}

							Tile tile = Main.tile[tx, ty];
							if (tile.HasTile && tile.TileType == TileID.Granite)
							{
								tile.TileType = (ushort)targetType;
								tile.TileFrameX = 0;
								tile.TileFrameY = 0;
							}
						}
					}
				}

				clustersProcessed++;
				progress.Value = (double)clustersProcessed / clusters.Count;
			}
		}
	}

	public class CaveHousePass : GenPass
	{
		private const string StructureFile = "Content/Structures/cavehouse.shstruct";
		private const int StructWidth = 16;
		private const int StructHeight = 9;

		public CaveHousePass() : base("CaveHouse", 100f)
		{
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Growing cosmerian cave houses";

			if (!ModLoader.TryGetMod("StructureHelper", out Mod structureHelper))
			{
				return;
			}

			int placed = 0;
			int maxAttempts = 2000;
			int maxPlacements = 5;
			int minCavernY = (int)(Main.worldSurface + 50);
			int maxCavernY = (int)(Main.rockLayer * 2.5);

			if (maxCavernY > Main.maxTilesY - 30)
				maxCavernY = Main.maxTilesY - 30;

			HashSet<Point> used = new HashSet<Point>();
			int attempts = 0;

			while (placed < maxPlacements && attempts < maxAttempts)
			{
				attempts++;

				int px = WorldGen.genRand.Next(30, Main.maxTilesX - StructWidth - 30);
				int py = WorldGen.genRand.Next(minCavernY, maxCavernY);

				if (!WorldGen.InWorld(px, py) || !WorldGen.InWorld(px + StructWidth, py + StructHeight))
					continue;

				if (Math.Abs(px + StructWidth / 2 - Main.spawnTileX) < 80)
					continue;

				if (!IsNearBiome(px, py))
					continue;

				if (!HasEnoughOpenSpace(px, py, 5))
					continue;

				bool tooClose = false;
				foreach (Point usedPoint in used)
				{
					if (Math.Abs(usedPoint.X - px) < StructWidth + 40 && Math.Abs(usedPoint.Y - py) < StructHeight + 40)
					{
						tooClose = true;
						break;
					}
				}

				if (tooClose)
					continue;

				if (!IsMostlySolid(px, py))
					continue;

				try
				{
					structureHelper.Call("Place", StructureFile, new Point16(px, py));
					used.Add(new Point(px, py));
					CosmerianWorldGen.HousePositions.Add(new Point(px + StructWidth / 2, py + StructHeight));
					placed++;

					FillChestsInArea(px, py, StructWidth, StructHeight);

					progress.Value = (double)placed / maxPlacements;
				}
				catch
				{
				}
			}
		}

		private bool IsNearBiome(int x, int y)
		{
			int scanRadius = 20;

			for (int dx = -scanRadius; dx <= scanRadius + StructWidth; dx++)
			{
				for (int dy = -scanRadius; dy <= scanRadius + StructHeight; dy++)
				{
					int tx = x + dx;
					int ty = y + dy;

					if (!WorldGen.InWorld(tx, ty))
						continue;

					Tile tile = Main.tile[tx, ty];
					if (tile.HasTile && (tile.TileType == TileID.Granite || tile.TileType == TileID.Marble))
						return true;

					if (tile.WallType == WallID.GraniteUnsafe || tile.WallType == WallID.MarbleUnsafe || tile.WallType == WallID.Granite || tile.WallType == WallID.Marble)
						return true;
				}
			}

			return false;
		}

		private bool HasEnoughOpenSpace(int x, int y, int openThreshold)
		{
			int openCount = 0;
			int checkSize = 6;

			for (int dx = 0; dx < StructWidth; dx += checkSize)
			{
				for (int dy = 0; dy < StructHeight; dy += checkSize)
				{
					int tx = x + dx;
					int ty = y + dy;

					if (!WorldGen.InWorld(tx, ty))
						return false;

					Tile tile = Main.tile[tx, ty];
					if (!tile.HasTile || !Main.tileSolid[tile.TileType])
						openCount++;
				}
			}

			return openCount >= openThreshold;
		}

		private bool IsMostlySolid(int x, int y)
		{
			int solidCount = 0;
			int total = 0;

			for (int dx = 0; dx < StructWidth; dx += 3)
			{
				for (int dy = 0; dy < StructHeight; dy += 3)
				{
					int tx = x + dx;
					int ty = y + dy + StructHeight;

					if (!WorldGen.InWorld(tx, ty))
						continue;

					Tile tile = Main.tile[tx, ty];
					total++;
					if (tile.HasTile && (Main.tileSolid[tile.TileType] || tile.TileType == TileID.Granite || tile.TileType == TileID.Marble || tile.TileType == TileID.Dirt || tile.TileType == TileID.Stone))
						solidCount++;
				}
			}

			return total > 0 && solidCount >= total / 2;
		}

		private void FillChestsInArea(int x, int y, int width, int height)
		{
			for (int tx = x; tx < x + width; tx++)
			{
				for (int ty = y; ty < y + height; ty++)
				{
					if (!WorldGen.InWorld(tx, ty))
						continue;

					Tile tile = Main.tile[tx, ty];
					if (tile.HasTile && (tile.TileType == TileID.Containers || tile.TileType == TileID.Containers2))
					{
						int chestIndex = Chest.FindChest(tx, ty);
						if (chestIndex >= 0)
						{
							Chest chest = Main.chest[chestIndex];
							if (chest != null)
							{
								int oreCount = WorldGen.genRand.Next(11);
								for (int s = 0; s < chest.item.Length; s++)
								{
									if (chest.item[s].type == ItemID.None && oreCount > 0)
									{
										chest.item[s].SetDefaults(ModContent.ItemType<CosmerianOre>());
										chest.item[s].stack = oreCount;
										oreCount = 0;
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
