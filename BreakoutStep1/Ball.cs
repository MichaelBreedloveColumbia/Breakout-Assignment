using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Sprite;
using MonoGameLibrary.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutStep1
{
    public enum BallState { OnPaddleStart, Playing, MarkedForRemoval }

    public class Ball : DrawableSprite
    {
        public BallState State { get; private set; }
        int CollisionCD = 0;
        float BallSpeed_Max = 500f;
        float BallSpeed_Min = 150f;

        GameConsole console;

        public Ball(Game game)
            : base(game)
        {
            this.State = BallState.OnPaddleStart;
            
            //Lazy load GameConsole
            console = (GameConsole)this.Game.Services.GetService(typeof(IGameConsole));
            if (console == null) //ohh no no console let's add a new one
            {
                console = new GameConsole(this.Game);
                this.Game.Components.Add(console);  //add a new game console to Game
            }
        }

        public void SetInitialLocation()
        {
            this.Location = new Vector2(200, 300);
        }

        /// <summary>
        /// Launches ball
        /// </summary>
        /// <param name="gameTime"></param>
        public void LaunchBall(GameTime gameTime)
        {
            this.Speed = 190;
            this.Direction = new Vector2(1, -1);
            this.State = BallState.Playing;
        }

        protected override void LoadContent()
        {
            this.spriteTexture = this.Game.Content.Load<Texture2D>("ballSmall");
            SetInitialLocation();
            base.LoadContent();
        }

        public void UpdateBall(GameTime gameTime, List<MonogameBlock> Blocks)
        {
            if (CollisionCD > 0)
            {
                CollisionCD--;
            }

            switch (this.State)
            {
                case BallState.OnPaddleStart:
                case BallState.MarkedForRemoval:
                    break;

                case BallState.Playing:
                    MoveBall(gameTime, Blocks);
                    break;
            }

            base.Update(gameTime);
        }

        public void MultiplySpeed(float amt)
        {
            Speed *= amt;

            if (Speed > BallSpeed_Max)
            {
                Speed = BallSpeed_Max;
            }

            if (Speed < BallSpeed_Min)
            {
                Speed = BallSpeed_Min;
            }
        }

        private void MoveBall(GameTime gameTime, List<MonogameBlock> Blocks)
        {
            this.Location += this.Direction * (this.Speed * gameTime.ElapsedGameTime.Milliseconds / 1000);

            //bounce off wall
            //Left and Right
            if ((this.Location.X + this.spriteTexture.Width > this.Game.GraphicsDevice.Viewport.Width)
                ||
                (this.Location.X < 0))
            {
                this.Direction.X *= -1;
            }
            //bottom Miss
            if (this.Location.Y + this.spriteTexture.Height > this.Game.GraphicsDevice.Viewport.Height)
            {
                State = BallState.MarkedForRemoval;
                this.Direction.Y *= -1;
            }

            //Top
            if (this.Location.Y < 0)
            {
                this.Direction.Y *= -1;
            }

            foreach(MonogameBlock block in Blocks)
            {
                if (Intersects(block) && CollisionCD < 1 && block.BlockState != BlockState.Broken)
                {
                    if (this.Location.Y + this.spriteTexture.Height >= block.Location.Y || this.Location.Y <= block.Location.Y + block.spriteTexture.Height)
                    {
                        this.Direction.Y *= -1;
                    }
                    else if (this.Location.X + this.spriteTexture.Width <= block.Location.X || this.Location.X >= block.Location.X + block.spriteTexture.Width)
                    {
                        this.Direction.X *= -1;
                    }

                    block.HitByBall(this);

                    CollisionCD = 4;

                    if (block.BlockState == BlockState.Broken)
                    {
                        MultiplySpeed(0.95f);
                    }
                }
            }
        }
    }
}
