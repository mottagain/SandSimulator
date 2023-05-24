using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SandSimulator.Components;
using SandSimulator.Voxel;
using System.Linq;

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
			foreach (var entityId in ActiveEntities.OrderBy(id => _posMapper.Get(id).Position))
			{
				var entity = GetEntity(entityId);
				entity.Detach<CheckVoxelComponent>();

				var currentPos = _posMapper.Get(entityId).Position;
				var targetPos = Material.PotentialMove(_grid, currentPos);
				if (targetPos != null)
				{
					entity.Attach(new MovingVoxelComponent { 
						SourceVoxel = _grid[currentPos],
						Direction = targetPos.Value - currentPos,
						Speed = 2,
					});;
				}
				else
				{
					entity.Detach<PositionComponent>();
					entity.Destroy();
				}
			}
		}

		private IntVector2[] SandMoveOffsets = new IntVector2[] { new IntVector2 { X = 0, Y = -1 }, new IntVector2 { X = -1, Y = -1 }, new IntVector2 { X = 1, Y = -1 } };
		private IntVector2[] WaterMoveOffsets = new IntVector2[] { new IntVector2 { X = 0, Y = -1}, new IntVector2 { X = -1,Y = 0 }, new IntVector2 { X = 1, Y = 0 } };
	}
}
