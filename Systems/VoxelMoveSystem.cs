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
			: base(Aspect.All(typeof(PositionComponent)).One(typeof(MovingVoxelComponent), typeof(CheckVoxelComponent)))
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
			var potentialCheckPositions = new HashSet<Position>();

			foreach (var entityId in ActiveEntities)
			{
				var posComponent = _posMapper.Get(entityId);
				var moveComponent = _movingVoxelMapper.Get(entityId);

				if (moveComponent != null)
				{
					var oldPos = posComponent.Position;
					potentialCheckPositions.Remove(oldPos);

					var newPos = Material.PotentialMove(_grid, oldPos);
					if (newPos != null)
					{
						potentialCheckPositions.Remove(newPos);
						_grid.Swap(oldPos, newPos);
						posComponent.Position = newPos;

						// Check all positions around this one to see if they may be impacted
						// Including the new destination position
						foreach (var offset in Offsets)
						{
							var testPos = new Position { X = oldPos.X + offset.X, Y = oldPos.Y + offset.Y };
							if (_grid[testPos] != VoxelType.None)
							{
								potentialCheckPositions.Add(testPos);
							}
						}
					}
					else
					{
						DestroyEntity(entityId);
					}
				}
			}

			// Remove all potential check positions that we're already tracking
			foreach (var entityId in ActiveEntities)
			{
				var posComponent = _posMapper.Get(entityId);
				potentialCheckPositions.Remove(posComponent.Position);
			}

			foreach (var pos in potentialCheckPositions)
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
