
#nullable enable
namespace SandSimulator.Voxel
{
	internal static class Material
	{
		public static Position? PotentialMove(VoxelGrid grid, Position pos)
		{
			Position[]? moveTestOffsets = null;

			switch (grid[pos])
			{
				case VoxelType.Sand:
					moveTestOffsets = SandMoveOffsets;
					break;
				case VoxelType.Water:
					moveTestOffsets = WaterMoveOffsets;
					break;
				case VoxelType.Rock:
					// Do nothing
					break;
			}

			if (moveTestOffsets != null)
			{
				foreach (var offset in moveTestOffsets)
				{
					var targetPos = new Position
					{
						X = pos.X + offset.X,
						Y = pos.Y + offset.Y
					};

					// If we're sand and falling straight down into water, allow move
					if (grid[pos] == VoxelType.Sand && pos.Y == targetPos.Y + 1 && grid[targetPos] == VoxelType.Water)
					{
						return targetPos;
					}

					if (grid[targetPos] == VoxelType.None)
					{
						return targetPos;
					}
				}
			}
			return null;
		}

		private static Position[] SandMoveOffsets = new Position[] { new Position { X = 0, Y = -1 }, new Position { X = -1, Y = -1 }, new Position { X = 1, Y = -1 } };
		private static Position[] WaterMoveOffsets = new Position[] { new Position { X = 0, Y = -1 }, new Position { X = -1, Y = 0 }, new Position { X = 1, Y = 0 } };
	}
}
#nullable restore
