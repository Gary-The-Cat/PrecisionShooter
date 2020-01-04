using Game.CollisionData;
using Game.CollisionData.Shapes;
using Game.Entities;
using Game.Example;
using Game.ExtensionMethods;
using Game.Interfaces;
using SFML.Graphics;
using SFML.System;
using Shared;
using System.Collections.Generic;

namespace Game.Screens
{
    public class GameScreen : Screen
    {
        private int frame;

        private Projectile lukesProjectile;
        private Projectile taylorsProjectile;

        private RectangleShape ground;

        private Rectangle groundBody;

        public GameScreen(
            RenderWindow window,
            FloatRect configuration)
            : base(window, configuration)
        {
            ground = new RectangleShape(new Vector2f(Configuration.Width, 10));
            ground.Position = new Vector2f(0, Configuration.Height - 10);
            groundBody = new Rectangle(Configuration.Width / 2, Configuration.Height - 5, Configuration.Width / 2, 5);
            lukesProjectile = new Projectile(new Vector2f(Configuration.Width / 2, Configuration.Height / 2));
        }
        
        /// <summary>
        /// Update - Here we add all our logic for updating components in this screen. 
        /// This includes checking for user input, updating the position of moving objects and more!
        /// </summary>
        /// <param name="deltaT">The amount of time that has passed since the last frame was drawn.</param>
        public override void Update(float deltaT)
        {
            lukesProjectile.Update(deltaT);

            Collision collision = CollisionManager.CheckCollision(groundBody, lukesProjectile.Body);

            if(collision != null)
            {
                lukesProjectile.Position = lukesProjectile.Position + collision.Normal * collision.Depth;
                lukesProjectile.Velocity = new Vector2f(-10/ deltaT, -500);
            }
        }
        
        /// <summary>
        /// Draw - Here we don't update any of the components, only draw them in their current state to the screen.
        /// </summary>
        /// <param name="deltaT">The amount of time that has passed since the last frame was drawn.</param>
        public override void Draw(float deltaT)
        {
            lukesProjectile.Draw(window);
            window.Draw(ground);
            frame++;
        }
    }
}