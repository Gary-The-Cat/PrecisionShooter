using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared;
using Shared.Components;
using Shared.DataStructures;
using Shared.Helpers;
using static SFML.Window.Keyboard;

namespace LevelEditor
{
    public class Editor
    {
        RenderWindow window;
        Clock clock;
        AllignmentGrid grid;
        List<Component> components;
        int componentindex;
        Component currentComponent => components[componentindex];
        Vertex[] background;
        Map map;

        List<Component> selectedComponents;

        bool IsPlacingGoal => IsKeyPressed(Key.G);
        bool IsPlacingStart => IsKeyPressed(Key.S);
        bool IsDeleting => IsKeyPressed(Key.D);

        int frame = 0;
        public Editor(string[] args)
        {
            // Create the main window.
            window = new RenderWindow(new VideoMode(Configuration.Width, Configuration.Height), "Level Editor");
            window.SetFramerateLimit(60);

            // Listen for window events.
            window.Closed += OnClose;
            window.MouseWheelScrolled += OnScroll;
            window.MouseButtonPressed += OnPress;
            window.KeyPressed += KeyPressed;

            // Load all the components that we have created.
            components = ComponentLoader.GetComponents();
            selectedComponents = new List<Component>();

            background = new Vertex[4]
            {
                new Vertex(new Vector2f(0, 0), new Color(0x3E, 0x5A, 0x8E)),
                new Vertex(new Vector2f(Configuration.Width, 0), new Color(0x3E, 0x5A, 0x8E)),
                new Vertex(new Vector2f(Configuration.Width, Configuration.Height), new Color(0x82, 0xAB, 0xE3)),
                new Vertex(new Vector2f(0, Configuration.Height), new Color(0x82, 0xAB, 0xE3))
            };

            clock = new Clock();
            grid = new AllignmentGrid();
            map = new Map();

            if (args.Any())
            {
                var rootDirectory = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\Resources\Maps");
                map = MapHelper.DeserialiseMap($"{rootDirectory}\\{args.First()}");
                map.Load();
            }
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.P)
            {
                MapHelper.SerialiseMap(map);
            }
        }

        private void OnPress(object sender, MouseButtonEventArgs e)
        {
            if(e.Button == Mouse.Button.Left)
            {
                if (IsDeleting)
                {
                    foreach (var component in selectedComponents)
                    {
                        map.Components.Remove(component);
                    }
                }
                else
                {
                    map.Components.Add(currentComponent.Clone());
                }
            }
        }

        private void OnScroll(object sender, MouseWheelScrollEventArgs e)
        {
            if(e.Delta > 0)
            {
                componentindex++;
                if (componentindex >= components.Count())
                {
                    componentindex = 0;
                }
            }
            else
            {
                componentindex--;
                if (componentindex < 0)
                {
                    componentindex = components.Count() - 1;
                }
            }
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
            if (IsPlacingGoal)
            {
                map.GoalPosition = MapHelper.GetBottomLeftOfGrid(window.Position);
                map.Goal.Position = map.GoalPosition;
            }
            else if (IsPlacingStart)
            {
                map.StartPosition = MapHelper.GetBottomLeftOfGrid(window.Position);
                map.Start.Position = map.StartPosition;
            }
            else if (IsDeleting)
            {
                selectedComponents.Clear();
                foreach (var component in map.Components)
                {
                    var mousePos = MapHelper.GetBottomLeftOfGrid(window.Position);
                    if (component.Body.GetGlobalBounds().Contains(mousePos.X, mousePos.Y))
                    {
                        component.Body.Scale = new Vector2f(1.2f, 1.2f);
                        selectedComponents.Add(component);
                    }
                    else
                    {
                        component.Body.Scale = new Vector2f(1f, 1f);
                    }
                }
            }
            else
            {
                currentComponent.Position = MapHelper.GetBottomLeftOfGrid(window.Position);
                currentComponent.Body.Position = currentComponent.Position;
            }

            if(frame % 60 == 0)
            {
                components = ComponentLoader.GetComponents();
            }
        }

        private void Draw()
        {
            window.Draw(background, 0, 4, PrimitiveType.Quads);

            grid.Draw(window);

            map.Draw(window);

            if (!IsPlacingGoal && !IsPlacingStart && !IsDeleting)
            {
                window.Draw(currentComponent.Body);
            }

            frame++;
        }

        private static void OnClose(object sender, EventArgs e)
        {
            // Close the window when OnClose event is received
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

    }
}
