
#nullable enable
using System;

namespace SandSimulator.Voxel
{
	internal static class Material
	{
		private static Random random = new Random();

		public static IntVector2? PotentialMove(VoxelGrid grid, IntVector2 pos)
		{
			IntVector2[]? moveTestOffsets = null;

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
					var targetPos = pos + offset;

					if (IsValidMove(grid, pos, targetPos))
					{
						return targetPos;
					}
				}
			}
			return null;
		}

		public static bool IsValidMove(VoxelGrid grid, IntVector2 fromPos, IntVector2 toPos)
		{
			if (grid[fromPos] != grid[toPos])
			{
				//If we're sand and falling straight down into water, allow move
				if (grid[fromPos] == VoxelType.Sand && fromPos.Y == toPos.Y + 1 && grid[toPos] == VoxelType.Water)
				{
					return true;
				}

				if (grid[toPos] == VoxelType.None)
				{
					return true;
				}
			}

			return false;
		}

		private static IntVector2[] SandMoveOffsetsLeftRight = new IntVector2[] { new IntVector2 { X = 0, Y = -1 }, new IntVector2 { X = -1, Y = -1 }, new IntVector2 { X = 1, Y = -1 } };
		private static IntVector2[] SandMoveOffsetsRightLeft = new IntVector2[] { new IntVector2 { X = 0, Y = -1 }, new IntVector2 { X = 1, Y = -1 }, new IntVector2 { X = -1, Y = -1 } };
		private static IntVector2[] WaterMoveOffsetsLeftRight = new IntVector2[] { new IntVector2 { X = 0, Y = -1 }, new IntVector2 { X = -1, Y = 0 }, new IntVector2 { X = 1, Y = 0 } };
		private static IntVector2[] WaterMoveOffsetsRightLeft = new IntVector2[] { new IntVector2 { X = 0, Y = -1 }, new IntVector2 { X = 1, Y = 0 }, new IntVector2 { X = -1, Y = 0 } };
	}
}
#nullable restore
