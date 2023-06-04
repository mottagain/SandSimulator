using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using SandSimulator.Sim;

namespace SandSimulator
{
	public class SandSimulatorGame : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private VoxelSimulation _simulation;

		public SandSimulatorGame()
		{
			_graphics = new GraphicsDeviceManager(this);
			_graphics.PreferredBackBufferWidth = 1600;
			_graphics.PreferredBackBufferHeight = 900;
			_simulation = null;
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			var grid = new VoxelGrid(160, 90);

			_simulation = new VoxelSimulation(grid);

			for (int i = 0; i < _simulation.Width; i++)
			{
				_simulation.AddVoxel<RockVoxel>(new IntVector2 { X = i, Y = 0 });
			}

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			if (Keyboard.GetState().IsKeyDown(Keys.S))
			{
				this.CreateVoxelAtCursor<SandVoxel>();
			}
			if (Keyboard.GetState().IsKeyDown(Keys.W))
			{
				this.CreateVoxelAtCursor<WaterVoxel>();
			}
			if (Keyboard.GetState().IsKeyDown(Keys.G))
			{
				this.CreateVoxelAtCursor<SmokeVoxel>();
			}

			this._simulation.Step();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			_spriteBatch.Begin();

			var cellWidth = GraphicsDevice.Viewport.Width / _simulation.Width;
			var cellHeight = GraphicsDevice.Viewport.Height / _simulation.Height;

			foreach (var voxel in this._simulation.Grid.Traverse())
			{
				var posX = voxel.Position.X * cellWidth;
				var posY = GraphicsDevice.Viewport.Height - (voxel.Position.Y + 1) * cellHeight;
				_spriteBatch.FillRectangle(new Rectangle(posX, posY, cellWidth, cellHeight), voxel.Color);
			}

			_spriteBatch.End();

			base.Draw(gameTime);
		}

		private void CreateVoxelAtCursor<T>() where T : Voxel, new()
		{
			var pos = GetCursorPosInGrid();

			if (_simulation.Grid[pos] == null)
			{
				_simulation.AddVoxel<T>(pos);
			}
		}

		private IntVector2 GetCursorPosInGrid()
		{
			var mouseState = Mouse.GetState();

			var cellWidth = Window.ClientBounds.Width / _simulation.Width;
			var cellHeight = Window.ClientBounds.Height / _simulation.Height;

			var posX = mouseState.X / cellWidth;
			var posY = _simulation.Grid.Height - (mouseState.Y / cellHeight) - 1;

			return  new IntVector2 { X = posX, Y = posY };
		}
	}
}