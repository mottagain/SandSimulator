
namespace SandSimulator.Sim
{
	internal class VoxelSimulation
	{
		private VoxelTile _grid;

		public VoxelSimulation(VoxelTile grid)
		{
			_grid = grid;
		}

		public int Width { get { return _grid.Width; } }
		public int Height { get { return _grid.Height;} }

		public int UpdatesLastFrame { get; set; }

		public VoxelTile Grid { get { return _grid; } }

		public void AddVoxel(VoxelType type, IntVector2 pos)
		{
			var voxel = new Voxel(type);
			voxel.Position = pos;
			this.Grid[pos] = voxel;
		}

		public void Step()
		{
			this.UpdatesLastFrame = 0;

			foreach (var voxel in this.Grid.Traverse())
			{
				if (voxel.Step(this.Grid))
				{
					this.UpdatesLastFrame++;
				}
			}
		}
	}
}
