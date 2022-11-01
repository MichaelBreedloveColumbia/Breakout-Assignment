using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Util;
using System.Collections.Generic;

namespace BreakoutStep1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        Ball ball;
        BlockManager manager;

        Paddle paddle;
        InputHandler input;

        int score = 0;
        int lives = 3;

        bool Active = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            input = (InputHandler)this.Services.GetService(typeof(IInputHandler));

            NewGame(true, false);
        }

        void NewGame(bool newPaddle = false, bool overridePaddleBall = true)
        {
            manager = new BlockManager(this);
            GenerateLayout();

            SpawnBall(overridePaddleBall);

            if (newPaddle)
            {
                paddle = new Paddle(this, ball);
                this.Components.Add(paddle);
            }

            score = 0;
            lives = 3;

            Active = true;
        }

        void EndGame()
        {
            Active = false;
            foreach(MonogameBlock block in manager.Blocks)
            {
                this.Components.Remove(block);
            }

            manager.RemoveAll();
            this.Components.Remove(ball);
        }

        void GenerateLayout()
        {
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    MonogameBlock block = new MonogameBlock(this);
                    block.Location = new Vector2(40 + (10 * j) + i * 60, 50 + 50 * j);
                    this.Components.Add(block);
                    manager.AddBlock(block);
                }
            }
        }

        void SpawnBall(bool OverridePaddleBall = false)
        {
            ball = new Ball(this);
            this.Components.Add(ball);

            if (OverridePaddleBall)
            {
                paddle.ball = ball;
                paddle.controller.ball = ball;
            }
        }

        void AddScore()
        {
            score++;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Arial");

            // TODO: use this.Content to load your game content here

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            foreach(MonogameBlock block in manager.Blocks)
            {
                if (block.BlockState == BlockState.Broken)
                {
                    AddScore();
                    this.Components.Remove(block);
                }
            }
            
            manager.UpdateManager();

            if (ball.State == BallState.MarkedForRemoval)
            {
                this.Components.Remove(ball);
                lives--;
                SpawnBall(true);
            }

            ball.UpdateBall(gameTime, manager.Blocks);
            
            if (manager.State == ManagerStatus.Inactive && Active || lives < 1)
            {
                EndGame();
            }

            if (!Active && Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                NewGame();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            if (manager.State == ManagerStatus.Active)
            {
                spriteBatch.DrawString(font, $"Score: {score}        Lives: {lives}        Current Ball Speed: {ball.Speed}", new Vector2(0, 0), Color.White);
            }
            else
            {
                if (lives < 1)
                {
                    spriteBatch.DrawString(font, "Game over! Press space to play again.", new Vector2(0, 0), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(font, "You won! Press space to play again.", new Vector2(0, 0), Color.White);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
