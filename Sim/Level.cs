using System.Collections.Generic;

namespace SandSimulator.Sim
{
	internal class Level
	{
		public Level(int width, int height)
		{
			_width = width;
			_height = height;
			_tiles = new Dictionary<IntVector2, VoxelTile>();
			_tileSize = new IntVector2 { X = 20, Y = 20 };
			_center = new IntVector2 { X = _width / 2, Y = _height / 2 };
		}

		public int Width { get { return _width; } }
		public int Height { get { return _height; } }
		public IntVector2 Center { get { return _center; } set { _center = value; } }

		public void AddVoxel(VoxelType type, IntVector2 pos)
		{
			var tilePos = TilePositionOfVoxel(pos);

			VoxelTile tile;
			if (!_tiles.TryGetValue(tilePos, out tile))
			{
				tile = new VoxelTile(_tileSize.X, _tileSize.Y);
				_tiles.Add(tilePos, tile);
			}

			var posInTile = PositionInTile(pos);
			tile[posInTile] = new Voxel(type);
		}

		public IEnumerable<(IntVector2, Voxel)> TraverseSimDist()
		{
			for (int j = 0; j < _height; j++)
			{
				for (int i = 0; i < _width; i++)
				{
					var targetPos = this.Center + new IntVector2 { X = i - (_width / 2), Y = j - (_height /2) };

					var tileOffset = TilePositionOfVoxel(targetPos);
					var posInTile = PositionInTile(targetPos);

					this._tiles.TryGetValue(tileOffset, out var tile);
					if (tile != null)
					{
						var voxel = tile[posInTile];
						if (voxel != null)
						{
							yield return (targetPos, voxel);
						}
					}
				}
			}
		}

		public int Step()
		{
			int updates = 0;

			foreach (var tile in _tiles.Values)
			{
				foreach ((var pos, var voxel) in tile.Traverse())
				{
					if (voxel.Step(pos, tile))
					{
						updates++;
					}
				}
			}

			return updates;
		}

		private IntVector2 TilePositionOfVoxel(IntVector2 pos)
		{
			return new IntVector2 { X = pos.X / _tileSize.X, Y = pos.Y / _tileSize.Y };
		}

		private IntVector2 PositionInTile(IntVector2 pos)
		{
			return new IntVector2 { X = pos.X % _tileSize.X, Y = pos.Y % _tileSize.Y };
		}

		private int _width;
		private int _height;
		private IntVector2 _center;
		private Dictionary<IntVector2, VoxelTile> _tiles;
		private IntVector2 _tileSize;
	}
}
