using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace HexTest.Content.Ore
{
	public class CosmerianWorldGen : ModSystem
	{
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
				list.Insert(index + 1, new CosmerianHousePass());
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

	public class CosmerianHousePass : GenPass
	{
		private const string StructureFile = "Content/Structures/house.shstruct";

		public CosmerianHousePass() : base("CosmerianHouse", 100f)
		{
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Growing cosmerian houses";

			if (!ModLoader.TryGetMod("StructureHelper", out Mod structureHelper))
			{
				return;
			}

			int structWidth = 20;
			int structHeight = 15;

			try
			{
				object sizeResult = structureHelper.Call("GetSize", StructureFile);
				if (sizeResult is Vector2 v)
				{
					structWidth = (int)v.X;
					structHeight = (int)v.Y;
				}
			}
			catch
			{
			}

			PlaceInBiome(structureHelper, progress, structWidth, structHeight, TileID.MarbleBlock);
			PlaceInBiome(structureHelper, progress, structWidth, structHeight, TileID.MushroomGrass);
		}

		private void PlaceInBiome(Mod structureHelper, GenerationProgress progress, int structWidth, int structHeight, int biomeTile)
		{
			int placed = 0;
			int maxAttempts = 500;
			int maxPlacements = 3;
			int biomeTileCount = 0;

			for (int x = 20; x < Main.maxTilesX - structWidth - 20; x++)
			{
				for (int y = (int)Main.rockLayer; y < Main.maxTilesY - structHeight - 20; y++)
				{
					Tile tile = Main.tile[x, y];
					if (tile.HasTile && tile.TileType == biomeTile)
					{
						biomeTileCount++;
					}
				}
			}

			if (biomeTileCount == 0)
			{
				return;
			}

			int attempts = 0;
			HashSet<Point> used = new HashSet<Point>();

			while (placed < maxPlacements && attempts < maxAttempts)
			{
				attempts++;

				int px = WorldGen.genRand.Next(20, Main.maxTilesX - structWidth - 20);
				int py = WorldGen.genRand.Next((int)Main.rockLayer + 10, Main.maxTilesY - structHeight - 20);

				bool nearBiome = false;
				for (int bx = px - 5; bx <= px + structWidth + 5; bx++)
				{
					for (int by = py - 5; by <= py + structHeight + 5; by++)
					{
						if (!WorldGen.InWorld(bx, by))
							continue;

						Tile tile = Main.tile[bx, by];
						if (tile.HasTile && tile.TileType == biomeTile)
						{
							nearBiome = true;
							break;
						}
					}
					if (nearBiome) break;
				}

				if (!nearBiome)
				{
					continue;
				}

				bool tooClose = false;
				foreach (Point usedPoint in used)
				{
					if (Math.Abs(usedPoint.X - px) < structWidth + 30 && Math.Abs(usedPoint.Y - py) < structHeight + 30)
					{
						tooClose = true;
						break;
					}
				}

				if (tooClose)
				{
					continue;
				}

				bool hasSpace = true;
				for (int sx = px - 2; sx < px + structWidth + 2; sx++)
				{
					for (int sy = py - 2; sy < py + structHeight + 2; sy++)
					{
						if (!WorldGen.InWorld(sx, sy))
						{
							hasSpace = false;
							break;
						}
					}
					if (!hasSpace) break;
				}

				if (!hasSpace)
				{
					continue;
				}

				try
				{
					structureHelper.Call("Place", StructureFile, new Point16(px, py));
					used.Add(new Point(px, py));
					placed++;

					FillChestsInArea(px, py, structWidth, structHeight);

					progress.Value = (double)placed / maxPlacements;
				}
				catch
				{
				}
			}
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
