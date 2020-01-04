using Game.CollisionData.Shapes;
using SFML.Graphics;
using SFML.System;
namespace Game.Entities
{
    public class Projectile
    {
        public Vector2f Position;
        public Vector2f Velocity;
        public Vector2f Acceleration;
        public CircleShape Sprite;
        public Circle Body;

        public Projectile(Vector2f position)
        {
            Position = position;
            Sprite = new CircleShape(50);
            Sprite.Position = Position;
            Body = new Circle(Position.X, Position.Y, 50);
        }

        public void Update(float deltaT)
        {
            Acceleration = new Vector2f(0, 9.8f) * 80;

            Velocity = Velocity + deltaT * Acceleration;

            Position = Position + deltaT * Velocity;

            Sprite.Position = Position;

            Body.Position = Position;
        }

        public void Draw(RenderWindow window)
        {
            window.Draw(Sprite);
        }
    }
}
