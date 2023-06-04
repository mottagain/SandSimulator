
namespace SandSimulator.Sim
{
	internal class VoxelSimulation
	{
		private VoxelGrid _grid;

		public VoxelSimulation(VoxelGrid grid)
		{
			_grid = grid;
		}

		public int Width { get { return _grid.Width; } }
		public int Height { get { return _grid.Height;} }

		public VoxelGrid Grid { get { return _grid; } }

		public void AddVoxel<T>(IntVector2 pos) where T : Voxel, new()
		{
			var voxel = new T();
			voxel.Position = pos;
			this.Grid[pos] = voxel;
		}

		public void Step()
		{
			foreach (var voxel in this.Grid.Traverse())
			{
				voxel.Step(this.Grid);
			}
		}
	}
}
