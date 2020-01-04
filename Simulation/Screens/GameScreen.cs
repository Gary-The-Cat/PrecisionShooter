using Game.CollisionData;
using Game.CollisionData.Shapes;
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
        private CircleShape taylorsCircle;
        private RectangleShape lukesCircle;

        public GameScreen(
            RenderWindow window,
            FloatRect configuration)
            : base(window, configuration)
        {
            taylorsCircle = new CircleShape(50);
            taylorsCircle.Origin = new Vector2f(50, 50);
            taylorsCircle.Position = new Vector2f(Configuration.Width / 2, Configuration.Height / 2);

            lukesCircle = new RectangleShape(new Vector2f(50, 50));
            lukesCircle.Origin = new Vector2f(25, 25);
            lukesCircle.Position = new Vector2f(Configuration.Width / 2, Configuration.Height / 2);
        }
        
        /// <summary>
        /// Update - Here we add all our logic for updating components in this screen. 
        /// This includes checking for user input, updating the position of moving objects and more!
        /// </summary>
        /// <param name="deltaT">The amount of time that has passed since the last frame was drawn.</param>
        public override void Update(float deltaT)
        {
            taylorsCircle.Position = MapHelper.GetMousePosition(window.Position);
        }
        
        /// <summary>
        /// Draw - Here we don't update any of the components, only draw them in their current state to the screen.
        /// </summary>
        /// <param name="deltaT">The amount of time that has passed since the last frame was drawn.</param>
        public override void Draw(float deltaT)
        {
            var lukesRectangle = new Rectangle(lukesCircle.Position.X, lukesCircle.Position.Y, lukesCircle.Size.X / 2, lukesCircle.Size.Y / 2);
            var taylorsCircle = new Circle(this.taylorsCircle.Position.X - this.taylorsCircle.Origin.X, this.taylorsCircle.Position.Y - this.taylorsCircle.Origin.Y, this.taylorsCircle.Radius);

            var collitision = CollisionManager.CheckCollision(lukesRectangle, taylorsCircle);

            if(collitision != null)
            {
                lukesCircle.FillColor = Color.Red;
                lukesCircle.Position -= collitision.Normal.Normalize() * collitision.Depth;
                this.taylorsCircle.FillColor = Color.Red;
            }
            else
            {
                lukesCircle.FillColor = Color.White;
                this.taylorsCircle.FillColor = Color.White;
            }

            window.Draw(this.taylorsCircle);
            window.Draw(lukesCircle);
            frame++;
        }
    }
}