using MonoGame.Extended.Entities;
using SandSimulator.Components;
using System;

namespace SandSimulator.Voxel
{
	internal class VoxelSimulation
	{
		private VoxelGrid _grid;
		private Func<Entity> _createEntity;

		public VoxelSimulation(VoxelGrid grid, Func<Entity> createEntity) 
		{
			_grid = grid;
			_createEntity = createEntity;
		}

		public int Width { get { return _grid.Width; } }
		public int Height { get { return _grid.Height;} }

		public void AddVoxel(int x, int y, VoxelType type)
		{
			_grid[x, y] = type;
			var entity = _createEntity();
			entity.Attach(new PositionComponent { Position = new Position { X = x, Y = y } });
			entity.Attach(new CheckVoxelComponent());

			Console.Error.WriteLine("Add voxel {0}", type);
		}

		public VoxelGrid Grid { get { return _grid; } }
	}
}
