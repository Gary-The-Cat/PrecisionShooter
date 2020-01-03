using Game.Example;
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

        public GameScreen(
            RenderWindow window,
            FloatRect configuration)
            : base(window, configuration)
        {
            taylorsCircle = new CircleShape(50);
            taylorsCircle.Origin = new Vector2f(50, 50);
            taylorsCircle.Position = new Vector2f(Configuration.Width / 2, Configuration.Height / 2);
        }
        
        /// <summary>
        /// Update - Here we add all our logic for updating components in this screen. 
        /// This includes checking for user input, updating the position of moving objects and more!
        /// </summary>
        /// <param name="deltaT">The amount of time that has passed since the last frame was drawn.</param>
        public override void Update(float deltaT)
        {
            var currentPosition = taylorsCircle.Position;
            taylorsCircle.Position = new Vector2f(currentPosition.X + 1, currentPosition.Y);
        }
        
        /// <summary>
        /// Draw - Here we don't update any of the components, only draw them in their current state to the screen.
        /// </summary>
        /// <param name="deltaT">The amount of time that has passed since the last frame was drawn.</param>
        public override void Draw(float deltaT)
        {
            window.Draw(taylorsCircle);
            frame++;
        }
    }
}