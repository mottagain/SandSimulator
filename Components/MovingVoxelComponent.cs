
using SandSimulator.Voxel;

namespace SandSimulator.Components
{
	internal class MovingVoxelComponent
	{
		public VoxelType SourceVoxel { get; set; }

		public IntVector2? Velocity { get ; set; }
	}
}
