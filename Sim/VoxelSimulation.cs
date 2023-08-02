
namespace SandSimulator.Sim
{
	internal class VoxelSimulation
	{
		private Level _level;

		public VoxelSimulation(Level level)
		{
			_level = level;
		}

		public int Width { get { return _level.Width; } }
		public int Height { get { return _level.Height;} }

		public int UpdatesLastFrame { get; set; }

		public Level Level { get { return _level; } }

		public void AddVoxel(VoxelType type, IntVector2 pos)
		{
			var voxel = new Voxel(type);
			this.Level[pos] = voxel;
		}

		public void Step()
		{
			this.UpdatesLastFrame = this.Level.Step();
		}
	}
}
