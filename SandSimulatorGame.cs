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
		private Statistics _statistics;
		private InspectState _inspectState;

		public SandSimulatorGame()
		{
			_graphics = new GraphicsDeviceManager(this);
			_graphics.PreferredBackBufferWidth = 1600;
			_graphics.PreferredBackBufferHeight = 900;
			_world = null;
			_simulation = null;
			_statistics = new Statistics();
			_inspectState = new InspectState();
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			var grid = new VoxelGrid(160, 90);

			_world = new WorldBuilder()
				.AddSystem(new StatisticsSystem(grid, _statistics))
				.AddSystem(new InspectSystem(grid, _inspectState))
				.AddSystem(new VoxelMoveSystem(grid))
				.AddSystem(new CheckForActionSystem(grid))
				.AddSystem(new VoxelRenderSystem(GraphicsDevice, grid))
				.Build();

			Components.Add(_world);

			_simulation = new VoxelSimulation(grid, _world.CreateEntity);

			for (int i = 0; i < _simulation.Width; i++)
			{
				_simulation.AddVoxel(i, 0, VoxelType.Rock);
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
			if (Keyboard.GetState().IsKeyDown(Keys.W))
			{
				CreateVoxelAtCursor(VoxelType.Water);
			}
			if (Keyboard.GetState().IsKeyDown(Keys.D))
			{
				Debug.WriteLine("{0}: {1} entities, {2} Position Components, {3} Check Voxel Components, {4} Moving Voxel Components tracked.", 
					_statistics.Frame, _statistics.Entities, _statistics.PositionComponents, _statistics.CheckVoxelComponents, _statistics.MovingVoxelComponents);
			}

			if (Keyboard.GetState().IsKeyDown(Keys.I))
			{
				if (_inspectState.TargetPosition != null)
				{
					Debug.WriteLine($"{_inspectState}");
				}
				else
				{
					_inspectState.TargetPosition = GetCursorPosInGrid();
				}
			}
			else
			{
				_inspectState.TargetPosition = null;
			}

			_world.Update(gameTime);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			_world.Draw(gameTime);

			base.Draw(gameTime);
		}

		private void CreateVoxelAtCursor(VoxelType voxelType)
		{
			var pos = GetCursorPosInGrid();

			if (_simulation.Grid[pos] == VoxelType.None)
			{
				_simulation.AddVoxel(pos.X, pos.Y, voxelType);
			}
		}

		private Position GetCursorPosInGrid()
		{
			var mouseState = Mouse.GetState();

			var cellWidth = Window.ClientBounds.Width / _simulation.Width;
			var cellHeight = Window.ClientBounds.Height / _simulation.Height;

			var posX = mouseState.X / cellWidth;
			var posY = _simulation.Grid.Height - (mouseState.Y / cellHeight) - 1;

			return  new Position { X = posX, Y = posY };
		}
	}
}