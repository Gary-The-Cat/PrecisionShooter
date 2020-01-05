using SFML.Graphics;
using SFML.System;
using Shared.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Shared.DataStructures
{
    [DataContract]
    public class Map
    {
        public Map()
        {
            var resourcesFolder = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\Resources");
            var goalTexture = new Texture(resourcesFolder + @"\Goal.png");
            var startTexture = new Texture(resourcesFolder + @"\Start.png");

            Goal = new RectangleShape(new Vector2f(128, 96)) { Position = GoalPosition, Texture = goalTexture };
            Start = new RectangleShape(new Vector2f(128, 128)) { Position = StartPosition, Texture = startTexture };
        }

        [DataMember]
        public List<Component> Components { get; set; }
            = new List<Component>();

        [DataMember]
        public (float, float) ShootingRange { get; set; }

        [DataMember]
        public Vector2f StartPosition { get; set; }

        [DataMember]
        public Vector2f GoalPosition { get; set; }

        public RectangleShape Goal { get; set; }

        public RectangleShape Start { get; set; }

        public void Load()
        {
            foreach (var component in Components)
            {
                component.Load();
            }


            var resourcesFolder = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\Resources");
            var goalTexture = new Texture(resourcesFolder + @"\Goal.png");
            var startTexture = new Texture(resourcesFolder + @"\Start.png");

            Goal = new RectangleShape(new Vector2f(128, 96)) { Position = GoalPosition, Texture = goalTexture };
            Start = new RectangleShape(new Vector2f(128, 128)) { Position = StartPosition, Texture = startTexture };
        }

        public void Draw(RenderWindow window)
        {
            foreach (var component in Components)
            {
                window.Draw(component.Visual);
            }

            window.Draw(Goal);
            window.Draw(Start);
        }
    }
}
