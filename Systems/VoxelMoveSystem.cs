using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SandSimulator.Components;
using SandSimulator.Voxel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SandSimulator.Systems
{
	internal class VoxelMoveSystem : EntityUpdateSystem
	{
		private ComponentMapper<PositionComponent> _posMapper;
		private ComponentMapper<MovingVoxelComponent> _movingVoxelMapper;
		private VoxelGrid _grid;
		private World _world;

		public VoxelMoveSystem(VoxelGrid grid)
			: base(Aspect.All(typeof(PositionComponent), typeof(MovingVoxelComponent)))
		{
			_grid = grid;
		}

		public override void Initialize(World world)
		{
			_world = world;

			base.Initialize(world);
		}

		public override void Initialize(IComponentMapperService mapperService)
		{
			_posMapper = mapperService.GetMapper<PositionComponent>();
			_movingVoxelMapper = mapperService.GetMapper<MovingVoxelComponent>();
		}

		public override void Update(GameTime gameTime)
		{
			var potentialCheckPositions = new HashSet<IntVector2>();
			var occupiedPositions = new HashSet<IntVector2>();

			foreach (var entityId in ActiveEntities.OrderBy(id => _posMapper.Get(id).Position))
			{
				var posComponent = _posMapper.Get(entityId);
				var moveComponent = _movingVoxelMapper.Get(entityId);

				var oldPos = posComponent.Position;
				var oldDirection = moveComponent.Direction;

				bool stillMoving = _grid[oldPos] == moveComponent.SourceVoxel;
				IntVector2 endPos = new IntVector2 { X = 0, Y = 0 };
				IntVector2 endDirection = new IntVector2 { X = 0, Y = 0 };

				if (stillMoving)
				{
					(stillMoving, endPos, endDirection) = MoveSteps((int)Math.Round(moveComponent.Speed), oldPos, oldDirection, occupiedPositions, potentialCheckPositions);
				}

				if ( stillMoving)
				{
					posComponent.Position = endPos;
					_posMapper.Put(entityId, posComponent);

					moveComponent.Direction = endDirection;
					_movingVoxelMapper.Put(entityId, moveComponent);
				}
				else
				{
					// Move is no longer valid, remove it.
					var entity = GetEntity(entityId);
					entity.Detach<PositionComponent>();
					entity.Detach<MovingVoxelComponent>();
					entity.Destroy();
				}
			}

			foreach (var pos in potentialCheckPositions)
			{
				if (!occupiedPositions.Contains(pos))
				{
					var entity = CreateEntity();
					entity.Attach(new PositionComponent { Position = pos });
					entity.Attach(new CheckVoxelComponent());
				}
			}
		}

		private (bool stillMoving, IntVector2 endPos, IntVector2 endDirection) MoveSteps(int steps, IntVector2 startPos, IntVector2 startDirection, HashSet<IntVector2> occupiedPositions, HashSet<IntVector2> potentialCheckPositions)
		{
			var currentPos = startPos;
			var currentDirection = startDirection;

			for (int i = 0; i < steps; i++)
			{
				var newPos = Material.PotentialMove(_grid, currentPos);
				if (newPos == null) return (false, currentPos, currentDirection);

				var newDirection = newPos.Value - currentPos;

				// If the last velocity is a valid lateral move, use it
				if (currentDirection.Y == 0 && newDirection.Y == 0 && Material.IsValidMove(_grid, currentPos, currentPos + currentDirection))
				{
					newPos = currentPos + currentDirection;
					newDirection = currentDirection;
				}

				occupiedPositions.Add(newPos.Value);
				_grid.Swap(currentPos, newPos.Value);

				// Check all positions around this one to see if they may be impacted
				// Including the new destination position
				foreach (var offset in Offsets)
				{
					var testPos = new IntVector2 { X = currentPos.X + offset.X, Y = currentPos.Y + offset.Y };
					if (_grid[testPos] != VoxelType.None)
					{
						potentialCheckPositions.Add(testPos);
					}
				}

				currentPos = newPos.Value;
				currentDirection = newDirection;
			}

			return (true, currentPos, currentDirection);
		}

		private IntVector2[] Offsets = new IntVector2[] {
			new IntVector2 { X = -1, Y = 0 },
			new IntVector2 { X = 0, Y = -1 },
			new IntVector2 { X = 1, Y = 0 },
			new IntVector2 { X = 0, Y = 1 },
			new IntVector2 { X = -1, Y = -1 },
			new IntVector2 { X = 1, Y = -1 },
			new IntVector2 { X = -1, Y = 1 },
			new IntVector2 { X = 1, Y = 1 },
			};
	}
}
