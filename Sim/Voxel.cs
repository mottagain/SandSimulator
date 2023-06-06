
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
			var primaryOffset = _primaryOffsets[(int)this.Type];
			var secondaryOffsets = _secondaryOffsets[(int)this.Type];

			var targetPos = this.Position + primaryOffset;
			var targetCell = grid[targetPos]; // TODO: Chagne grid to return VoxelType.None instead of null
			var targetType = targetCell != null ? targetCell.Type : VoxelType.None;

			if (_swapsWith[(int)this.Type][(int)targetType])
			{
				grid.Swap(this.Position, targetPos);
			}
			else
			{
				var handleOffset = (IntVector2 offset) => {
					targetPos = new IntVector2 { X = this.Position.X + offset.X, Y = this.Position.Y + offset.Y };
					targetCell = grid[targetPos];
					targetType = targetCell != null ? targetCell.Type : VoxelType.None;

					if (_swapsWith[(int)this.Type][(int)targetType])
					{
						grid.Swap(this.Position, targetPos);
						return false;
					}
					return true;
				};

				int flip = Voxel._random.Next(0, 2);
				if (flip == 0)
				{
					foreach (var offset in secondaryOffsets)
					{
						if (!handleOffset(offset))
						{
							break;
						}
					}
				}
				else
				{
					for (int i = secondaryOffsets.Length - 1; i >= 0; i--)
					{
						if (!handleOffset(secondaryOffsets[i]))
						{
							break;
						}
					}
				}
			}
		}


		protected static Random _random = new Random();

		private static Color[] _voxelColors = new Color[] { 
			Color.Black,		// None
			Color.Gray, 		// Rock
			Color.Yellow, 		// Sand
			Color.Blue,			// Water
			Color.LightGray  	// Steam
		};

		protected static IntVector2[] _primaryOffsets = new IntVector2[] {
			new IntVector2 { X = 0, Y = 0 },	// None
			new IntVector2 { X = 0, Y = 0 },	// Rock
			new IntVector2 { X = 0, Y = -1 },	// Sand
			new IntVector2 { X = 0, Y = -1 },	// Water
			new IntVector2 { X = 0, Y = 1 },	// Steam
		};

		protected static IntVector2[][] _secondaryOffsets = new IntVector2[][] {
			new IntVector2[] { },																		// None
			new IntVector2[] { },																		// Rock
			new IntVector2[] { new IntVector2 { X = -1, Y = -1 }, new IntVector2 { X = 1, Y = -1 } },	// Sand
			new IntVector2[] { new IntVector2 { X = -1, Y = 0 }, new IntVector2 { X = 1, Y = 0 } },		// Water
			new IntVector2[] { new IntVector2 { X = -1, Y = 0 }, new IntVector2 { X = 1, Y = 0 } },		// Steam
		};

		protected static bool[][] _swapsWith = new bool[][] {
					 //  None,  Rock,  Sand,  Water, Steam
			new bool[] { false, false, false, false, false },	// None
			new bool[] { false, false, false, false, false },	// Rock
			new bool[] { true,  false, false, true,  true  },	// Sand
			new bool[] { true,  false, false, false, true },	// Water
			new bool[] { true,  false, false, false, false },	// Steam
		};
	}

	internal class WaterVoxel : Voxel
	{
		public WaterVoxel() : base(VoxelType.Water)
		{
		}
	}

	internal class SandVoxel : Voxel
	{
		public SandVoxel() : base(VoxelType.Sand)
		{
		}
	}

	internal class RockVoxel : Voxel
	{
		public RockVoxel() : base(VoxelType.Rock)
		{
		}
	}

	internal class SteamVoxel : Voxel
	{
		public SteamVoxel() : base(VoxelType.Steam)
		{
		}
	}
}
