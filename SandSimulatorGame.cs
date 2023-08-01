using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using SandSimulator.Sim;
using SandSimulator.Util;

namespace SandSimulator
{
	public class SandSimulatorGame : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private VoxelSimulation _simulation;
		private SpriteFont _font;
		private SmoothFramerate _frameRate;

		public SandSimulatorGame()
		{
			_graphics = new GraphicsDeviceManager(this);
			_graphics.PreferredBackBufferWidth = 1600;
			_graphics.PreferredBackBufferHeight = 900;
			_simulation = null;
			_font = null;
			_frameRate = new SmoothFramerate(10);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
//			_graphics.IsFullScreen = true;
//			_graphics.ApplyChanges();

			var grid = new VoxelTile(320, 180);

			_simulation = new VoxelSimulation(grid);

			for (int i = 0; i < _simulation.Width; i++)
			{
				_simulation.AddVoxel(VoxelType.Rock, new IntVector2 { X = i, Y = 0 });
				_simulation.AddVoxel(VoxelType.Rock, new IntVector2 { X = i, Y = _simulation.Height - 1 });
			}

			for (int i = 0; i < _simulation.Height; i++)
			{
				_simulation.AddVoxel(VoxelType.Rock, new IntVector2 { X = 0, Y = i });
				_simulation.AddVoxel(VoxelType.Rock, new IntVector2 { X = _simulation.Width - 1, Y = i });
			}

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			_font = this.Content.Load<SpriteFont>("Arial");
		}

		protected override void Update(GameTime gameTime)
		{
			this._frameRate.Update(gameTime.ElapsedGameTime.TotalSeconds);

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			if (Keyboard.GetState().IsKeyDown(Keys.A))
			{
				this.CreateVoxelAtCursor(VoxelType.Acid);
			}
			if (Keyboard.GetState().IsKeyDown(Keys.R))
			{
				this.CreateVoxelAtCursor(VoxelType.Rock);
			}
			if (Keyboard.GetState().IsKeyDown(Keys.S))
			{
				this.CreateVoxelAtCursor(VoxelType.Sand);
			}
			if (Keyboard.GetState().IsKeyDown(Keys.G))
			{
				this.CreateVoxelAtCursor(VoxelType.Steam);
			}
			if (Keyboard.GetState().IsKeyDown(Keys.L))
			{
				this.CreateVoxelAtCursor(VoxelType.Water);
			}
			if (Keyboard.GetState().IsKeyDown(Keys.W))
			{
				this.CreateVoxelAtCursor(VoxelType.Wood);
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

			_spriteBatch.DrawString(_font, $"FPS: {_frameRate.Framerate:0000} Updates: {_simulation.UpdatesLastFrame}", new Vector2(10, 10), Color.Green);

			_spriteBatch.End();
			base.Draw(gameTime);
		}

		private void CreateVoxelAtCursor(VoxelType type)
		{
			var mouseState = Mouse.GetState();
			var mousePos = new Point(mouseState.X, mouseState.Y);
			if (Window.ClientBounds.Contains(mousePos + Window.Position))
			{
				var pos = GetCursorPosInGrid(mousePos);

				if (_simulation.Grid[pos] == null)
				{
					_simulation.AddVoxel(type, pos);
				}
			}
		}

		private IntVector2 GetCursorPosInGrid(Point mousePos)
		{
			var cellWidth = Window.ClientBounds.Width / _simulation.Width;
			var cellHeight = Window.ClientBounds.Height / _simulation.Height;

			var posX = mousePos.X / cellWidth;
			var posY = _simulation.Grid.Height - (mousePos.Y / cellHeight) - 1;

			return  new IntVector2 { X = posX, Y = posY };
		}
	}
}