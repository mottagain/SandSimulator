
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

		public void Swap(IntVector2 a, IntVector2 b)
		{
			Voxel search = new Voxel(VoxelType.None);

			Voxel atPosA;
			if (this._data.TryGetValue(a, out atPosA))
			{
				this._data.Remove(a);
			}

			Voxel atPosB;
			if (this._data.TryGetValue(b, out atPosB))
			{
				this._data.Remove(b);
			}

			if (atPosA != null)
			{
				this._data.Add(b, atPosA);
			}

			if (atPosB != null)
			{
				this._data.Add(a, atPosB);
			}
		}
	}
}
