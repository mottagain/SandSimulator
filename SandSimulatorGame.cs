using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Entities;
using SandSimulator.Systems;
using SandSimulator.Voxel;
using System.Diagnostics;

namespace SandSimulator
{
	public class SandSimulatorGame : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private World _world;
		private VoxelSimulation _simulation;

		public SandSimulatorGame()
		{
			_graphics = new GraphicsDeviceManager(this);
			_graphics.PreferredBackBufferWidth = 1600;
			_graphics.PreferredBackBufferHeight = 900;
			_world = null;
			_simulation = null;
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			var grid = new VoxelGrid(160, 90);

			_world = new WorldBuilder()
				.AddSystem(new VoxelMoveSystem(grid))
				.AddSystem(new CheckForActionSystem(grid))
				.AddSystem(new RenderSystem(GraphicsDevice, grid))
				.Build();

			_simulation = new VoxelSimulation(grid, _world.CreateEntity);

			for (int i = 0; i < _simulation.Width; i++)
			{
				_simulation.AddVoxel(i, _simulation.Height - 1, VoxelType.Rock);
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
				CreateVoxelAtCursor(VoxelType.Sand);
			}
			if (Keyboard.GetState().IsKeyDown(Keys.R))
			{
				CreateVoxelAtCursor(VoxelType.Rock);
			}
			if (Keyboard.GetState().IsKeyDown(Keys.D))
			{
				Debug.WriteLine("{0} entities tracked.", _world.EntityCount);
			}

			_world.Update(gameTime);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			_world.Draw(gameTime);

			base.Draw(gameTime);
		}

		private void CreateVoxelAtCursor(VoxelType voxelType)
		{
			var mouseState = Mouse.GetState();

			var cellWidth = Window.ClientBounds.Width / _simulation.Width;
			var cellHeight = Window.ClientBounds.Height / _simulation.Height;

			var posX = mouseState.X / cellWidth;
			var posY = mouseState.Y / cellHeight;

			if (_simulation.Grid[posX, posY] == VoxelType.None)
			{
				_simulation.AddVoxel(posX, posY, voxelType);
			}
		}
	}
}