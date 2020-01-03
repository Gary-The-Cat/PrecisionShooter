using System;
using SFML.System;

namespace Game.CollisionData.Shapes
{
    public class Polygon : IShape
    {
        public Vector2f GetPosition()
        {
            throw new NotImplementedException();
        }

        public Vector2f GetVertex(Vector2f direction)
        {
            return new Vector2f(0, 0);
        }

        public void SetPosition(float x, float y)
        {
            throw new NotImplementedException();
        }

        public void SetPosition(Vector2f position)
        {
            throw new NotImplementedException();
        }
    }
}
