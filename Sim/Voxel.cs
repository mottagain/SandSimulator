
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SandSimulator.Sim
{
	internal class VoxelComparerByPosition : IComparer<Voxel>
	{
		public int Compare(Voxel lhs, Voxel rhs)
		{
			int result = lhs.Position.Y.CompareTo(rhs.Position.Y);
			if (result != 0) return result;

			result = lhs.Position.X.CompareTo(rhs.Position.X);
			return result;
		}
	}

	internal class Voxel
	{
		public IntVector2 Position { get; set; }

		public Color Color { get; set; }

		public virtual void Step(VoxelGrid grid)
		{
		}

		protected static Random _random = new Random();
	}

	internal class SolidVoxel : Voxel
	{

	}
	internal class LiquidVoxel : Voxel
	{
		public override void Step(VoxelGrid grid)
		{
			var targetPos = new IntVector2 { X = this.Position.X, Y = this.Position.Y - 1 };
			var targetCell = grid[targetPos];

			if (targetCell == null)
			{
				grid.Swap(this.Position, targetPos);
			}
			else 
			{
				int flip = Voxel._random.Next(0, 2);
				var offsets = (flip == 0) ? _offsets : _reverseOffsets;

				foreach (var offset in offsets)
				{
					targetPos = new IntVector2 { X = this.Position.X + offset.X, Y = this.Position.Y + offset.Y };
					targetCell = grid[targetPos];

					if (targetCell == null)
					{
						grid.Swap(this.Position, targetPos);
						break;
					}
				}
			}
		}

		private static IntVector2[] _offsets = new IntVector2[] { new IntVector2 { X = -1, Y = 0 }, new IntVector2 { X = 1, Y = 0 } };
		private static IntVector2[] _reverseOffsets = new IntVector2[] { new IntVector2 { X = 1, Y = 0 }, new IntVector2 { X = -1, Y = 0 } };
	}

	internal class GasVoxel : Voxel
	{
		public override void Step(VoxelGrid grid)
		{
			var targetPos = new IntVector2 { X = this.Position.X, Y = this.Position.Y + 1 };
			var targetCell = grid[targetPos];

			if (targetCell == null)
			{
				grid.Swap(this.Position, targetPos);
			}
			else
			{
				int flip = Voxel._random.Next(0, 2);
				var offsets = (flip == 0) ? _offsets : _reverseOffsets;

				foreach (var offset in offsets)
				{
					targetPos = new IntVector2 { X = this.Position.X + offset.X, Y = this.Position.Y + offset.Y };
					targetCell = grid[targetPos];

					if (targetCell == null)
					{
						grid.Swap(this.Position, targetPos);
						break;
					}
				}
			}
		}

		private static IntVector2[] _offsets = new IntVector2[] { new IntVector2 { X = -1, Y = 0 }, new IntVector2 { X = 1, Y = 0 } };
		private static IntVector2[] _reverseOffsets = new IntVector2[] { new IntVector2 { X = 1, Y = 0 }, new IntVector2 { X = -1, Y = 0 } };
	}

	internal class MovableSolidVoxel : SolidVoxel
	{
		public override void Step(VoxelGrid grid)
		{
			var targetPos = new IntVector2 { X = this.Position.X, Y = this.Position.Y - 1 };
			var targetCell = grid[targetPos];

			if (targetCell == null || targetCell is LiquidVoxel)
			{
				grid.Swap(this.Position, targetPos);
			}
			else
			{
				int flip = Voxel._random.Next(0, 2);
				var offsets = (flip == 0)? _offsets : _reverseOffsets;

				foreach (var offset in offsets)
				{
					targetPos = new IntVector2 { X = this.Position.X + offset.X, Y = this.Position.Y + offset.Y };
					targetCell = grid[targetPos];

					if (targetCell == null || targetCell is LiquidVoxel)
					{
						grid.Swap(this.Position, targetPos);
						break;
					}
				}
			}
		}

		private static IntVector2[] _offsets = new IntVector2[] { new IntVector2 { X = -1, Y = -1 }, new IntVector2 { X = 1, Y = -1 } };
		private static IntVector2[] _reverseOffsets = new IntVector2[] { new IntVector2 { X = 1, Y = -1 }, new IntVector2 { X = -1, Y = -1 } };
	}

	internal class ImmovableSolidVoxel : SolidVoxel
	{

	}

	internal class WaterVoxel : LiquidVoxel
	{
		public WaterVoxel()
		{
			this.Color = Color.Blue;
		}
	}

	internal class SandVoxel : MovableSolidVoxel
	{
		public SandVoxel() 
		{
			this.Color = Color.Yellow;
		}
	}

	internal class RockVoxel : ImmovableSolidVoxel
	{
		public RockVoxel()
		{
			this.Color = Color.Gray;
		}
	}

	internal class SmokeVoxel : GasVoxel
	{
		public SmokeVoxel()
		{
			this.Color = Color.LightGray;
		}
	}

}
