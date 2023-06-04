
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SandSimulator.Sim
{
	internal enum VoxelType : byte
	{
		None,
		Rock,
		Sand,
		Water,
		Steam,
	};

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
		public Voxel(VoxelType type) {
			this.Type = type;
		}

		public VoxelType Type { get; set; }

		public IntVector2 Position { get; set; }

		public Color Color
		{ 
			get 
			{
				return _voxelColors[(int)this.Type];
			}
		}

		public virtual void Step(VoxelGrid grid)
		{
		}

		protected static Random _random = new Random();

		private static Color[] _voxelColors = new Color[] { 
			Color.Black,		// None
			Color.Gray, 		// Rock
			Color.Yellow, 		// Sand
			Color.Blue,			// Water
			Color.LightGray  	// Steam
		};
	}

	internal class SolidVoxel : Voxel
	{
		public SolidVoxel(VoxelType type) : base(type) 
		{
		}
	}
	internal class LiquidVoxel : Voxel
	{
		public LiquidVoxel(VoxelType type) : base(type) 
		{
		}

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
		public GasVoxel(VoxelType type) : base(type) 
		{
		}

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
		public MovableSolidVoxel(VoxelType type) : base(type) 
		{
		}

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
		public ImmovableSolidVoxel(VoxelType type) : base(type) 
		{
		}
	}

	internal class WaterVoxel : LiquidVoxel
	{
		public WaterVoxel() : base(VoxelType.Water)
		{
		}
	}

	internal class SandVoxel : MovableSolidVoxel
	{
		public SandVoxel() : base(VoxelType.Sand)
		{
		}
	}

	internal class RockVoxel : ImmovableSolidVoxel
	{
		public RockVoxel() : base(VoxelType.Rock)
		{
		}
	}

	internal class SteamVoxel : GasVoxel
	{
		public SteamVoxel() : base(VoxelType.Steam)
		{
		}
	}
}
