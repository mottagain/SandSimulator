using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SandSimulator.Components;
using SandSimulator.Voxel;

namespace SandSimulator.Systems
{
	internal class VoxelMoveSystem : EntityUpdateSystem
	{
		private ComponentMapper<VoxelMove> _voxelMoveMapper;
		private VoxelGrid _grid;

		public VoxelMoveSystem(VoxelGrid grid)
			: base(Aspect.All(typeof(VoxelMove)))
		{
			_grid = grid;
		}

		public override void Initialize(IComponentMapperService mapperService)
		{
			_voxelMoveMapper = mapperService.GetMapper<VoxelMove>();
		}

		public override void Update(GameTime gameTime)
		{
			foreach (var entityId in ActiveEntities)
			{
				var voxelMove = _voxelMoveMapper.Get(entityId);

				var type = _grid[voxelMove.From.X, voxelMove.From.Y];
				_grid[voxelMove.From.X, voxelMove.From.Y] = VoxelType.None;
				_grid[voxelMove.To.X, voxelMove.To.Y] = type;
				DestroyEntity(entityId);

				// Check all positions around this one to see if they may be impacted
				// Including the new destination position
				foreach (var offset in Offsets)
				{
					var targetX = voxelMove.From.X + offset.X;
					var targetY = voxelMove.From.Y + offset.Y;
					if (_grid[targetX, targetY] != VoxelType.None)
					{
						var entity = CreateEntity();
						entity.Attach(new CheckPosition { Position = new Position { X = targetX, Y = targetY } });
					}
				}
			}
		}

		private Position[] Offsets = new Position[] { 
			new Position { X = -1, Y = 0 }, 
			new Position { X = 0, Y = -1 }, 
			new Position { X = 1, Y = 0 }, 
			new Position { X = 0, Y = 1 },
			new Position { X = -1, Y = -1 }, 
			new Position { X = 1, Y = -1 }, 
			new Position { X = -1, Y = 1 }, 
			new Position { X = 1, Y = 1 } };
	}
}
