
#nullable enable
using System;

namespace SandSimulator.Voxel
{
	internal static class Material
	{
		private static Random random = new Random();

		public static Position? PotentialMove(VoxelGrid grid, Position pos)
		{
			Position[]? moveTestOffsets = null;

			bool flip = random.Next(0, 2) == 0;

			switch (grid[pos])
			{
				case VoxelType.Sand:
					moveTestOffsets = flip ? SandMoveOffsetsLeftRight : SandMoveOffsetsRightLeft;
					break;
				case VoxelType.Water:
					moveTestOffsets = flip ? WaterMoveOffsetsLeftRight : WaterMoveOffsetsRightLeft;
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

		private static Position[] SandMoveOffsetsLeftRight = new Position[] { new Position { X = 0, Y = -1 }, new Position { X = -1, Y = -1 }, new Position { X = 1, Y = -1 } };
		private static Position[] SandMoveOffsetsRightLeft = new Position[] { new Position { X = 0, Y = -1 }, new Position { X = 1, Y = -1 }, new Position { X = -1, Y = -1 } };
		private static Position[] WaterMoveOffsetsLeftRight = new Position[] { new Position { X = 0, Y = -1 }, new Position { X = -1, Y = 0 }, new Position { X = 1, Y = 0 } };
		private static Position[] WaterMoveOffsetsRightLeft = new Position[] { new Position { X = 0, Y = -1 }, new Position { X = 1, Y = 0 }, new Position { X = -1, Y = 0 } };
	}
}
#nullable restore
