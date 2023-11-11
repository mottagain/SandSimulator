using System;
using System.Collections.Generic;
using System.Linq;

namespace SandSimulator.Sim
{
	// X increases toward the right
	// Y increases toward the top
	internal class VoxelTile
	{
		private int _width;
		private int _height;
		private Dictionary<IntVector2, Voxel> _data;

		public VoxelTile(int width, int height)
		{
			_width = width;
			_height = height;
			_data = new Dictionary<IntVector2, Voxel>();
		}

		public int Width { get { return _width; } }
		public int Height { get { return _height; } }

		public IntVector2 Size { get { return new IntVector2 { X = _width, Y = _height }; } }

		public Voxel this[int x, int y]
		{
			get
			{
				return this[new IntVector2 { X = x, Y = y }];
			}
			set
			{
				this[new IntVector2 { X = x, Y = y }] = value;
			}
		}

		public Voxel this[IntVector2 pos]
		{
			get
			{
				if (this._data.TryGetValue(pos, out var result))
				{
					return result;
				}

				return null;
			}
			set
			{
				if (pos.X < 0 || pos.X >= this._width || pos.Y < 0 || pos.Y >= this._height)
				{
					throw new InvalidOperationException("Position is out of bounds.");
				}

				Voxel result;

				if (this._data.TryGetValue(pos, out result))
				{
					this._data.Remove(pos);
				}
				this._data.Add(pos, value);
			}
		}

		public IEnumerable<(IntVector2, Voxel)> Traverse()
		{
			// Snapshot the container so we can modify the original collection while enumerating.
			var snapshot = this._data.Select(kvp => (kvp.Key, kvp.Value)).ToList();

			foreach (var item in snapshot)
			{
				yield return item;
			}
		}

		public static void Swap(VoxelTile tileA, IntVector2 a, VoxelTile tileB, IntVector2 b)
		{
			Voxel search = new Voxel(VoxelType.None);

			Voxel atPosA;
			if (tileA._data.TryGetValue(a, out atPosA))
			{
				tileA._data.Remove(a);
			}

			Voxel atPosB;
			if (tileB._data.TryGetValue(b, out atPosB))
			{
				tileB._data.Remove(b);
			}

			if (atPosA != null)
			{
				if (b.X >= 0 && b.X < tileA._width && b.Y >= 0 && b.Y < tileA._height)
				{
					tileA._data.Add(b, atPosA);
				}
			}

			if (atPosB != null)
			{
				if (a.X >= 0 && a.X < tileB._width && a.Y >= 0 && a.Y < tileB._height)
				{
					tileB._data.Add(a, atPosB);
				}
			}
		}
	}
}
