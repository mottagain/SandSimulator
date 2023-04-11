
using System;

namespace SandSimulator.Voxel
{
	internal enum VoxelType
	{
		None = 0,
		Sand = 1,
		Rock = 2,
		Water = 3,
	}

	// X increases toward the right
	// Y increases toward the top
	internal class VoxelGrid
	{
		private int _width;
		private int _height;
		private VoxelType[] _data;

		public VoxelGrid(int width, int height)
		{
			_width = width;
			_height = height;
			_data = new VoxelType[width * height];
		}

		public int Width { get { return _width; } }
		public int Height { get { return _height; } }

		public VoxelType this[int x, int y]
		{
			get
			{
				if (x < 0 || y < 0 || x >= _width || y >= _height)
				{
					return VoxelType.Rock;
				}
				return _data[y * Width + x];
			}
			set
			{
				if (x < 0 || y < 0 || x >= _width || y >= _height)
				{
					throw new InvalidOperationException();
				}

				_data[y * Width + x] = value;
			}
		}

		public VoxelType this[Position pos]
		{
			get
			{
				return this[pos.X, pos.Y];
			}
		}
	}
}
