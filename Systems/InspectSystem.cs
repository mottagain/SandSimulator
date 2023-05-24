using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SandSimulator.Components;
using SandSimulator.Voxel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandSimulator.Systems
{
	internal class InspectState
	{
		public InspectState()
		{
			PositionData = new Dictionary<IntVector2, (PosEcsState, VoxelType)>();
		}

		public IntVector2? TargetPosition { get; set; }

		public long Frame { get; set; }

		public Dictionary<IntVector2, (PosEcsState, VoxelType)> PositionData { get; private set; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine($"{Frame}:");

			foreach (var (pos, (ecsState, voxelType)) in PositionData)
			{
				sb.AppendLine($"({pos.X}, {pos.Y}):  ECS State: {ecsState}, Voxel Type: {voxelType}");
			}

			return sb.ToString();
		}
	}

	[Flags]
	internal enum PosEcsState
	{
		None = 0,
		Check = 1,
		Moving = 2,
	}

	internal class InspectSystem : EntityUpdateSystem
	{
		private ComponentMapper<PositionComponent> _posMapper;
		private ComponentMapper<MovingVoxelComponent> _movingVoxelMapper;
		private ComponentMapper<CheckVoxelComponent> _checkVoxelMapper;
		private long _frameNum;
		private World _world;
		private VoxelGrid _grid;

		public InspectSystem(VoxelGrid grid, InspectState inspectState)
			: base(Aspect.One(typeof(PositionComponent), typeof(MovingVoxelComponent), typeof(CheckVoxelComponent)))
		{
			_frameNum = 0;
			_grid = grid;
			this.InspectState = inspectState;
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
			_checkVoxelMapper = mapperService.GetMapper<CheckVoxelComponent>();
		}

		public override void Update(GameTime gameTime)
		{
			_frameNum++;

			if (this.InspectState.TargetPosition.HasValue) {
				var inspectPos = this.InspectState.TargetPosition.Value;

				this.InspectState.Frame = _frameNum;
				this.InspectState.PositionData.Clear();

				PosEcsState state = PosEcsState.None;

				foreach (var entityId in ActiveEntities)
				{
					var position = _posMapper.Get(entityId).Position;

					var distance = Math.Sqrt(Math.Pow(position.X - inspectPos.X, 2) + Math.Pow(position.Y - inspectPos.Y, 2));
					if (distance < 5)
					{
						if (_movingVoxelMapper.Has(entityId))
						{
							state |= PosEcsState.Moving;
						}

						if (_checkVoxelMapper.Has(entityId))
						{
							state |= PosEcsState.Check;
						}

						if (state == PosEcsState.None)
						{
							throw new InvalidOperationException();
						}

						this.InspectState.PositionData.Add(position, (state, _grid[position]));
					}
				}
			}
		}

		public InspectState InspectState { get; private set; }
	}
}
