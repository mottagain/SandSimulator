using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SandSimulator.Components;
using SandSimulator.Voxel;

namespace SandSimulator.Systems
{
	internal class CheckForActionSystem : EntityUpdateSystem
	{
		private ComponentMapper<CheckVoxelComponent> _checkPosMapper;
		private ComponentMapper<PositionComponent> _posMapper;
		private VoxelGrid _grid;

		public CheckForActionSystem(VoxelGrid grid)
			: base(Aspect.All(typeof(PositionComponent), typeof(CheckVoxelComponent)))
		{
			_grid = grid;
		}

		public override void Initialize(IComponentMapperService mapperService)
		{
			_checkPosMapper = mapperService.GetMapper<CheckVoxelComponent>();
			_posMapper = mapperService.GetMapper<PositionComponent>();
		}

		public override void Update(GameTime gameTime)
		{
			foreach (var entityId in ActiveEntities)
			{
				var entity = GetEntity(entityId);
				entity.Detach<CheckVoxelComponent>();

				var checkPos = _posMapper.Get(entityId);
				if (Material.PotentialMove(_grid, checkPos.Position) != null)
				{
					entity.Attach(new MovingVoxelComponent { SourceVoxel = _grid[checkPos.Position] });
				}
				else
				{
					entity.Detach<PositionComponent>();
					entity.Destroy();
				}
			}
		}

		private Position[] SandMoveOffsets = new Position[] { new Position { X = 0, Y = -1 }, new Position { X = -1, Y = -1 }, new Position { X = 1, Y = -1 } };
		private Position[] WaterMoveOffsets = new Position[] { new Position { X = 0, Y = -1}, new Position { X = -1,Y = 0 }, new Position { X = 1, Y = 0 } };
	}
}
