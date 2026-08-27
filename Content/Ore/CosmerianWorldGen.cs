using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
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
}
