using Game.CollisionData.Shapes;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Components
{
    [DataContract]
    public class Component
    {
        public Component(string componentType, Vector2f position)
        {
            ComponentType = componentType;
            Position = position;
            Load();
        }

        public void Load()
        {
            var componentsFolder = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\Resources\Components");

            var texture = new Texture(componentsFolder + @"\" + ComponentType + @"\Image.png");
            var size = texture.Size;
            var shape = new RectangleShape(new Vector2f(size.X, size.Y));
            shape.Texture = texture;
            shape.Origin = new Vector2f(size.X / 2, size.Y / 2);

            this.Visual = shape;
            this.Visual.Position = this.Position;
            var pos = shape.Position;
            this.Body = new Rectangle(pos.X, pos.Y, shape.Size.X / 2, shape.Size.Y / 2);
        }

        public Rectangle Body { get; set; }

        public RectangleShape Visual { get; set; }

        [DataMember]
        public string ComponentType { get; set; }

        [DataMember]
        public Vector2f Position { get; set; }

        public Component Clone()
        {
            return new Component(this.ComponentType, this.Position);
        }
    }
}
