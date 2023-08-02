using Microsoft.Xna.Framework.Graphics;
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
			_tileSize = new IntVector2 { X = _width / 2, Y = _height / 2 };
		}

		public int Width { get { return _width; } }
		public int Height { get { return _height; } }

		public Voxel this[IntVector2 pos]
		{
			get
			{
				var tilePos = new IntVector2 { X = pos.X / _tileSize.X, Y = pos.Y / _tileSize.Y };

				VoxelTile tile;
				if (_tiles.TryGetValue(tilePos, out tile))
				{
					return tile[pos.X % _tileSize.X, pos.Y % _tileSize.Y];
				}

				return null;
			}
			set
			{
				var tilePos = new IntVector2 { X = pos.X / _tileSize.X, Y = pos.Y / _tileSize.Y };

				VoxelTile tile;
				if (!_tiles.TryGetValue(tilePos, out tile))
				{
					tile = new VoxelTile(_tileSize.X, _tileSize.Y);
					_tiles.Add(tilePos, tile);
				}
				var posInTile = new IntVector2 { X = pos.X % _tileSize.X, Y = pos.Y % _tileSize.Y };
				tile[posInTile] = value;
			}
		}

		public IEnumerable<(IntVector2, Voxel)> Traverse()
		{
			foreach ((var tileOffset, var tile) in _tiles)
			{
				foreach ((var pos, var voxel) in tile.Traverse())
				{
					var offsetPos = new IntVector2 { X = tileOffset.X * _tileSize.X + pos.X, Y = tileOffset.Y * _tileSize.Y + pos.Y };
					yield return (offsetPos, voxel);
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

		private int _width;
		private int _height;
		private Dictionary<IntVector2, VoxelTile> _tiles;
		private IntVector2 _tileSize;
	}
}
