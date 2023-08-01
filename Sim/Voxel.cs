
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SandSimulator.Sim
{
	internal enum VoxelType : byte
	{
		None,
		Acid,
		Rock,
		Sand,
		Water,
		Wood,
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
			this.Speed = _startingSpeed[(int)type];
			this.FreeFall = true;
			this.Momentum = _random.Next(0, 2) == 0;
		}

		public VoxelType Type { get; set; }

		public float Speed { get; set; }

		public bool FreeFall { get; set; }

		public bool Momentum { get; set;}

		public IntVector2 Position { get; set; }

		public Color Color
		{ 
			get 
			{
				return _voxelColors[(int)this.Type];
			}
		}

		public bool Step(VoxelTile grid)
		{
			bool moved = false;

			if (this.FreeFall)
			{
				this.Speed += _gravity[(int)this.Type];
			}

			for (int step = 0; step < (int)Math.Round(this.Speed); step++) 
			{

				var primaryOffset = _primaryOffsets[(int)this.Type];
				var secondaryOffsets = _secondaryOffsets[(int)this.Type];

				if (this.ApplyOffset(grid, primaryOffset))
				{
					moved = true;
					this.FreeFall = true;
				} 
				else
				{
					this.FreeFall = false;
					
					if (this.Momentum)
					{
						foreach (var offset in secondaryOffsets)
						{
							if (this.ApplyOffset(grid, offset))
							{
								moved = true;
								break;
							}
						}
					}
					else
					{
						for (int i = secondaryOffsets.Length - 1; i >= 0; i--)
						{
							if (this.ApplyOffset(grid, secondaryOffsets[i]))
							{
								moved = true;
								break;
							}
						}
					}
				}
			}
			return moved;
		}

		private bool ApplyOffset(VoxelTile grid, IntVector2 offset) 
		{
			var targetPos = this.Position + offset;
			var targetCell = grid[targetPos];
			var targetType = targetCell != null ? targetCell.Type : VoxelType.None;

			if (_swapsWith[(int)this.Type][(int)targetType])
			{
				if (targetType != VoxelType.None)
				{
					targetCell.Speed = _startingSpeed[(int)targetType];
					targetCell.Momentum = _random.Next(0, 2) == 0;
					targetCell.FreeFall = true;
				}

				grid.Swap(this.Position, targetPos);

				var belowVoxel = this.Position + new IntVector2 { X = 0, Y = -1 };
				var belowCell = grid[belowVoxel];
				var belowType = belowCell != null ? belowCell.Type : VoxelType.None;
				var friction = _friction[(int)this.Type][(int)belowType];
				this.Speed -= friction;
				
				return true;
			} 
			return false;
		}

		private static Random _random = new Random();

		private static Color[] _voxelColors = new Color[] { 
			Color.Black,		// None
			Color.Green,		// Acid
			Color.Gray, 		// Rock
			Color.Yellow, 		// Sand
			Color.Blue,			// Water
			Color.SaddleBrown,	// Wood
			Color.LightGray  	// Steam
		};

		private static byte[] _startingSpeed = new byte[] {
			0,	// None
			0,	// Acid
			0,	// Rock
			0,	// Sand
			0,	// Water
			0,	// Wood
			1,	// Steam
		};

		private static float[] _gravity = new float[] {
			0.0f,		// None
			0.653333f,  // Acid
			0.0f,		// Rock
			0.653333f,	// Sand
			0.653333f,	// Water
			0.0f,		// Wood
			0.0f,		// Steam
		};

		private static IntVector2[] _primaryOffsets = new IntVector2[] {
			new IntVector2 { X = 0, Y = 0 },	// None
			new IntVector2 { X = 0, Y = -1 },	// Acid
			new IntVector2 { X = 0, Y = 0 },	// Rock
			new IntVector2 { X = 0, Y = -1 },	// Sand
			new IntVector2 { X = 0, Y = -1 },	// Water
			new IntVector2 { X = 0, Y = 0 },	// Wood
			new IntVector2 { X = 0, Y = 1 },	// Steam
		};

		private static IntVector2[][] _secondaryOffsets = new IntVector2[][] {
			new IntVector2[] { },																		// None
			new IntVector2[] { new IntVector2 { X = -1, Y = 0 }, new IntVector2 { X = 1, Y = 0 } },		// Acid
			new IntVector2[] { },																		// Rock
			new IntVector2[] { new IntVector2 { X = -1, Y = -1 }, new IntVector2 { X = 1, Y = -1 } },	// Sand
			new IntVector2[] { new IntVector2 { X = -1, Y = 0 }, new IntVector2 { X = 1, Y = 0 } },		// Water
			new IntVector2[] { },																		// Wood
			new IntVector2[] { new IntVector2 { X = -1, Y = 0 }, new IntVector2 { X = 1, Y = 0 } },		// Steam
		};

		private static bool[][] _swapsWith = new bool[][] {
					 //  None,  Acid, Rock,  Sand,  Water, Wood,  Steam
			new bool[] { false, false, false, false, false, false, false },	// None
			new bool[] { true,  false, false, false, true,  false, true },	// Acid
			new bool[] { false, false, false, false, false, false, false },	// Rock
			new bool[] { true,  true,  false, false, true,  false, true  },	// Sand
			new bool[] { true,  false, false, false, false, false, true  },	// Water
			new bool[] { false, false, false, false, false, false, false },	// Wood
			new bool[] { true,  false, false, false, false, false, false },	// Steam
		};

		private static float[][] _friction = new float[][] {
					 //   None, Acid, Rock,  Sand,  Water, Wood,  Steam
			new float[] { 0.0f, 0.0f,   0.0f,  0.0f,  0.0f, 0.0f,  0.0f },	// None
			new float[] { 0.0f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.0f },	// Acid
			new float[] { 0.0f, 0.0f,   0.0f,  0.0f,  0.0f, 0.0f,  0.0f },	// Rock
			new float[] { 0.0f, 0.06f, 0.06f, 0.05f,  0.0f, 0.06f, 0.0f },	// Sand
			new float[] { 0.0f,0.005f,0.005f,0.005f,0.005f, 0.005f,0.0f },	// Water
			new float[] { 0.0f, 0.0f,   0.0f,  0.0f,  0.0f, 0.0f,  0.0f },	// Wood
			new float[] { 0.0f, 0.01f, 0.01f,  0.0f,  0.0f, 0.01f, 0.0f },	// Steam
		};
	}
}
