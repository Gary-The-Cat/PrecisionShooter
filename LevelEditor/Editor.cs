using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared;

namespace LevelEditor
{
    public class Editor
    {
        RenderWindow window;
        Clock clock;

        public Editor()
        {

            // Create the main window
            window = new RenderWindow(new VideoMode(Configuration.Width, Configuration.Height), "Editor");
            window.SetFramerateLimit(60);

            window.Closed += OnClose;

            clock = new Clock();
        }

        public void Run()
        {
            while (window.IsOpen)
            {
                float deltaT = Configuration.IsDebugFrameTime
                    ? Configuration.DebugFrameTime
                    : clock.Restart().AsMicroseconds() / 1000000f;

                // Clear the previous frame
                window.Clear(Configuration.Background);

                // Process events
                window.DispatchEvents();

                this.Update(deltaT);

                this.Draw();

                // Update the window
                window.Display();
            }
        }

        private void Update(float deltaT)
        {
        }

        private void Draw()
        {

            var position = MapHelper.GetCentreOfGrid(window.Position);

            var rectangle = new RectangleShape(new Vector2f(Configuration.GridSize, Configuration.GridSize));
            rectangle.Origin = new Vector2f(Configuration.GridSize / 2, Configuration.GridSize / 2);
            rectangle.Position = position;

            window.Draw(rectangle);
        }

        private static void OnClose(object sender, EventArgs e)
        {
            // Close the window when OnClose event is received
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

    }
}
