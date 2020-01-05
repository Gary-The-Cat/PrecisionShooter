using Game.CollisionData;
using Game.CollisionData.Shapes;
using Game.Entities;
using Game.Example;
using Game.ExtensionMethods;
using Game.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared;
using Shared.DataStructures;
using System;
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

        private Map map;

        public GameScreen(
            RenderWindow window,
            FloatRect configuration)
            : base(window, configuration)
        {
            ground = new RectangleShape(new Vector2f(Configuration.Width, 10));
            ground.Position = new Vector2f(0, Configuration.Height - 10);
            groundBody = new Rectangle(Configuration.Width / 2, Configuration.Height - 5, Configuration.Width / 2, 5);
            lukesProjectile = new Projectile(new Vector2f(Configuration.Width / 2 - 250, Configuration.Height / 2));

            map = MapHelper.LoadMap($"{Configuration.MapLocations}\\0");
            map.Load();
        }
        
        /// <summary>
        /// Update - Here we add all our logic for updating components in this screen. 
        /// This includes checking for user input, updating the position of moving objects and more!
        /// </summary>
        /// <param name="deltaT">The amount of time that has passed since the last frame was drawn.</param>
        public override void Update(float deltaT)
        {
            lukesProjectile.Update(deltaT);

            if (Keyboard.IsKeyPressed(Keyboard.Key.P))
            {
                lukesProjectile.Position = MapHelper.GetMousePosition(window.Position);
                lukesProjectile.Velocity = new Vector2f(500, -600);
            }

            CheckCollisions();
        }

        /// <summary>
        /// Draw - Here we don't update any of the components, only draw them in their current state to the screen.
        /// </summary>
        /// <param name="deltaT">The amount of time that has passed since the last frame was drawn.</param>
        public override void Draw(float deltaT)
        {
            lukesProjectile.Draw(window);
            window.Draw(ground);
            map.Draw(window);
            frame++;
        }

        private void CheckCollisions()
        {
            Collision collision = null;
            Collision maxCollision = null;
            float maxDepth = float.MinValue;

            foreach(var component in map.Components)
            {
                collision = CollisionManager.CheckCollision(component.Body, lukesProjectile.Body);

                if (collision != null)
                {

                    if(collision.Depth > maxDepth)
                    {
                        maxDepth = collision.Depth;
                        maxCollision = collision;
                    }
                }
            }

            if (maxCollision != null)
            {
                lukesProjectile.Position = lukesProjectile.Position + maxCollision.Normal * maxCollision.Depth;
                lukesProjectile.Velocity = GetNewVelocity(lukesProjectile.Velocity, maxCollision);
            }
        }

        private Vector2f GetNewVelocity(Vector2f velocity, Collision collision)
        {
            var currentAngle = Math.Atan2(-velocity.Y, -velocity.X);
            var collisionAngle = Math.Atan2(collision.Normal.Y, collision.Normal.X);
            var exitAngle = collisionAngle + (collisionAngle - currentAngle);

            var normalisedVelocity = velocity.Normalize();

            var currentSpeed = velocity.Magnitude() * 0.45f;
            var exitVector = new Vector2f(MathExtensions.Cos(exitAngle), MathExtensions.Sin(exitAngle));
            exitVector *= currentSpeed;
            return exitVector;
        }
    }
}