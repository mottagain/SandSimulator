using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SandSimulator.Components;
using SandSimulator.Voxel;
using System.Collections.Generic;

namespace SandSimulator.Systems
{
	internal class VoxelMoveSystem : EntityUpdateSystem
	{
		private ComponentMapper<PositionComponent> _posMapper;
		private ComponentMapper<MovingVoxelComponent> _movingVoxelMapper;
		private VoxelGrid _grid;

		public VoxelMoveSystem(VoxelGrid grid)
			: base(Aspect.All(typeof(PositionComponent), typeof(MovingVoxelComponent)))
		{
			_grid = grid;
		}

		public override void Initialize(IComponentMapperService mapperService)
		{
			_posMapper = mapperService.GetMapper<PositionComponent>();
			_movingVoxelMapper = mapperService.GetMapper<MovingVoxelComponent>();
		}

		public override void Update(GameTime gameTime)
		{
			var checkPositions = new HashSet<Position>();
			foreach (var entityId in ActiveEntities)
			{
				var posComponent = _posMapper.Get(entityId);
				var oldPos = posComponent.Position;

				// Check all positions around this one to see if they may be impacted
				// Including the new destination position
				foreach (var offset in Offsets)
				{
					var testPos = new Position { X = oldPos.X + offset.X, Y = oldPos.Y + offset.Y };
					if (_grid[testPos] != VoxelType.None)
					{
						checkPositions.Add(testPos);
					}
				}
			}

			foreach (var entityId in ActiveEntities)
			{
				var posComponent = _posMapper.Get(entityId);
				var moveComponent = _movingVoxelMapper.Get(entityId);

				var oldPos = posComponent.Position;
				checkPositions.Remove(oldPos);

				var newPos = Material.PotentialMove(_grid, oldPos);
				if (newPos != null)
				{
					checkPositions.Remove(newPos);
					_grid.Swap(oldPos, newPos);
					posComponent.Position = newPos;
				}
				else
				{
					DestroyEntity(entityId);
				}
			}

			foreach(var pos in checkPositions)
			{
				var entity = CreateEntity();
				entity.Attach(new PositionComponent { Position = pos });
				entity.Attach(new CheckVoxelComponent());
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
