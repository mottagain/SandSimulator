
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
