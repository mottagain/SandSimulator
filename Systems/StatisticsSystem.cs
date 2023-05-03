using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SandSimulator.Components;
using SandSimulator.Voxel;

namespace SandSimulator.Systems
{
	internal class Statistics
	{
		public long Frame { get; set; }

		public int Entities { get; set; }

		public int PositionComponents { get; set; }

		public int CheckVoxelComponents { get; set; }

		public int MovingVoxelComponents { get; set; }
	}

	internal class StatisticsSystem : EntityUpdateSystem
	{
		private ComponentMapper<PositionComponent> _posMapper;
		private ComponentMapper<MovingVoxelComponent> _movingVoxelMapper;
		private ComponentMapper<CheckVoxelComponent> _checkVoxelMapper;
		private long _frameNum;
		private World _world;
		private VoxelGrid _grid;

		public StatisticsSystem(VoxelGrid grid, Statistics statistics)
			: base(Aspect.One(typeof(PositionComponent), typeof(MovingVoxelComponent), typeof(CheckVoxelComponent)))
		{
			_frameNum = 0;
			_grid = grid;
			this.Statistics = statistics;
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

			var entityCount = _world.EntityCount;
			var positionCount = _posMapper.Components.Count;
			var movingVoxelCount = _movingVoxelMapper.Components.Count;
			var checkPosCount = _checkVoxelMapper.Components.Count;

			this.Statistics.Frame = _frameNum;
			this.Statistics.Entities = entityCount;
			this.Statistics.PositionComponents = 0;
			this.Statistics.CheckVoxelComponents = 0;
			this.Statistics.MovingVoxelComponents = 0;
				
			foreach (var entity in ActiveEntities)
			{
				if (_posMapper.Get(entity) != null)
				{
					this.Statistics.PositionComponents++;
				}
				if (_checkVoxelMapper.Get(entity) != null)
				{
					this.Statistics.CheckVoxelComponents++;
				}
				if (_movingVoxelMapper.Get(entity) != null)
				{
					this.Statistics.MovingVoxelComponents++;
				}
			}
		}

		public Statistics Statistics { get; private set; }
	}
}
