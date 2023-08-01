
using System.Collections.Generic;

namespace SandSimulator.Sim
{
	// X increases toward the right
	// Y increases toward the top
	internal class VoxelTile
	{
		private int _width;
		private int _height;
		private SortedSet<Voxel> _data;

		public VoxelTile(int width, int height)
		{
			_width = width;
			_height = height;
			_data = new SortedSet<Voxel>(new VoxelComparerByPosition());
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
				Voxel search = new Voxel(VoxelType.None);
				search.Position = pos;

				Voxel result;
				if (this._data.TryGetValue(search, out result))
				{
					return result;
				}
				return null;
			}
			set
			{
				Voxel result;
				Voxel search = new Voxel(VoxelType.None);
				search.Position = pos;
				if (this._data.TryGetValue(search, out result))
				{
					this._data.Remove(result);
				}
				if (value != null)
				{
					this._data.Add(value);
				}
			}
		}

		public IEnumerable<Voxel> Traverse()
		{
			var inOrderVoxels = new Voxel[this._data.Count];
			this._data.CopyTo(inOrderVoxels);

			foreach (var voxel in inOrderVoxels)
			{
				yield return voxel;
			}
		}

		public void Swap(IntVector2 a, IntVector2 b)
		{
			Voxel search = new Voxel(VoxelType.None);

			search.Position = a;
			Voxel atPosA;
			if (this._data.TryGetValue(search, out atPosA))
			{
				this._data.Remove(atPosA);
			}

			search.Position = b;
			Voxel atPosB;
			if (this._data.TryGetValue(search, out atPosB))
			{
				this._data.Remove(atPosB);
			}

			if (atPosA != null)
			{
				atPosA.Position = b;
				this._data.Add(atPosA);
			}

			if (atPosB != null)
			{
				atPosB.Position = a;
				this._data.Add(atPosB);
			}
		}
	}
}
