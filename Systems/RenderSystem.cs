
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SandSimulator.Components;
using SandSimulator.Voxel;

namespace SandSimulator.Systems
{
	internal class RenderSystem : EntityDrawSystem
	{
		private GraphicsDevice _device;
		private SpriteBatch _batch;
		private VoxelGrid _grid;

		public RenderSystem(GraphicsDevice device, VoxelGrid grid)
			: base(Aspect.All(typeof(CheckPosition)))
		{
			_device = device;
			_batch = new SpriteBatch(device);
			_grid = grid;
		}

		public override void Initialize(IComponentMapperService mapperService)
		{
			// Do nothing
		}

		public override void Draw(GameTime gameTime)
		{
			_batch.Begin();

			var cellWidth = _device.Viewport.Width / _grid.Width;
			var cellHeight = _device.Viewport.Height / _grid.Height;

			for (int j = 0; j < _grid.Height; j++)
			{
				for (int i = 0; i < _grid.Width; i++)
				{
					var posX = i * cellWidth;
					var posY = j * cellHeight;

					var voxelType = _grid[i, j];
					var color = GetColorFromVoxelType(voxelType);
					_batch.FillRectangle(new Rectangle(posX, posY, cellWidth, cellHeight), color);
				}
			}

			_batch.End();
		}
		private Color GetColorFromVoxelType(VoxelType type)
		{
			switch (type)
			{
				case VoxelType.None: return Color.Black;
				case VoxelType.Sand: return Color.Yellow;
				case VoxelType.Rock: return Color.Gray;
				default: return Color.Red;
			}
		}
	}
}
