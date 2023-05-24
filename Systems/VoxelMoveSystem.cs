using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SandSimulator.Components;
using SandSimulator.Voxel;
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

				var newPos = Material.PotentialMove(_grid, oldPos);

				// Addresses corner case where swaping pixels on a successful move can move it out from underneath an active move.
				if (_grid[oldPos] == moveComponent.SourceVoxel && newPos != null)
				{
					var newDirection = newPos.Value - oldPos;

					// If the last velocity is a valid lateral move, use it
					if (oldDirection.Y == 0 && newDirection.Y == 0 && Material.IsValidMove(_grid, oldPos, oldPos + oldDirection))
					{
						newPos = oldPos + oldDirection;
						newDirection = oldDirection;
					}

					// Update direction
					moveComponent.Direction = newDirection;
					_movingVoxelMapper.Put(entityId, moveComponent);

					// Update position
					occupiedPositions.Add(newPos.Value);
					_grid.Swap(oldPos, newPos.Value);
					posComponent.Position = newPos.Value;
					_posMapper.Put(entityId, posComponent);

					// Check all positions around this one to see if they may be impacted
					// Including the new destination position
					foreach (var offset in Offsets)
					{
						var testPos = new IntVector2 { X = oldPos.X + offset.X, Y = oldPos.Y + offset.Y };
						if (_grid[testPos] != VoxelType.None)
						{
							potentialCheckPositions.Add(testPos);
						}
					}
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
