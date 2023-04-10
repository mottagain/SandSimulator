using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SandSimulator.Components;
using SandSimulator.Voxel;

namespace SandSimulator.Systems
{
	internal class CheckForActionSystem : EntityUpdateSystem
	{
		private ComponentMapper<CheckPosition> _gridPosMapper;
		private VoxelGrid _grid;

		public CheckForActionSystem(VoxelGrid grid)
			: base(Aspect.All(typeof(CheckPosition)))
		{
			_grid = grid;
		}

		public override void Initialize(IComponentMapperService mapperService)
		{
			_gridPosMapper = mapperService.GetMapper<CheckPosition>();
		}

		public override void Update(GameTime gameTime)
		{
			foreach (var entityId in ActiveEntities)
			{
				var checkPos = _gridPosMapper.Get(entityId);

				switch (_grid[checkPos.Position.X, checkPos.Position.Y])
				{
					case VoxelType.Sand:
						foreach (var offset in SandMoveOffsets)
						{
							var targetPos = new Position
							{
								X = checkPos.Position.X + offset.X,
								Y = checkPos.Position.Y + offset.Y
							};

							if (_grid[targetPos] == VoxelType.None)
							{
								var entity = CreateEntity();
								entity.Attach(new VoxelMove { From = checkPos.Position, To = targetPos });
								break;
							}
						}
						break;
					case VoxelType.Rock:
						// Do nothing
						break;
				}
				DestroyEntity(entityId);
			}
		}

		private Position[] SandMoveOffsets = new Position[] { new Position { X = 0, Y = 1 }, new Position { X = -1, Y = 1 }, new Position { X = 1, Y = 1 } };
	}
}
