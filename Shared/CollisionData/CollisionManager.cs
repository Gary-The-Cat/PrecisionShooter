using System;
using System.Collections.Generic;
using System.Linq;
using Game.CollisionData.Shapes;
using Game.ExtensionMethods;
using SFML.System;

namespace Game.CollisionData
{
    public static class CollisionManager
    {
        private static int MaxGjkIterations = 30;
        private static int MaxEpaIterations = 30;
        private static float GjkEpsilon = 0.001f;
        private static float EpaEpsilon = 0.1f;

        public static Collision CheckCollision(IShape componentA, IShape componentB)
        {
            Collision collision = null;
            var shapeA = componentA;
            var shapeB = componentB;

            if (shapeA is Circle && shapeB is Rectangle)
            {
                collision = CheckCollision((Circle)shapeA, (Rectangle)shapeB);
            }

            if (shapeB is Circle && shapeA is Rectangle)
            {
                collision = CheckCollision((Circle)shapeB, (Rectangle)shapeA);
            }

            if (shapeB is Circle && shapeA is Circle)
            {
                collision = CheckCollision((Circle)shapeB, (Circle)shapeA);
            }

            if (collision == null)
            {
                collision = CheckCollisionEpa(shapeA, shapeB);
            }
            
            return collision;
        }

        public static Collision CheckCollisionEpa(IShape shapeA, IShape shapeB)
        {
            var gjkSimplex = new List<Vector2f>();
            // Check simple/quick GJK overlap
            if (!OverlapsGjk(shapeA, shapeB, ref gjkSimplex))
            {
                return null;
            }

            // EPA simplex is a queue of edges, prioritised on distance to origin.
            var simplex = new Queue<(Vector2f, Vector2f, int)>();

            // Consume GJK termination simplex.
            var winding = GetWindingOrder(gjkSimplex);
            simplex.Enqueue((gjkSimplex[0], gjkSimplex[1], winding));
            simplex.Enqueue((gjkSimplex[1], gjkSimplex[2], winding));
            simplex.Enqueue((gjkSimplex[2], gjkSimplex[0], winding));

            // Start the EPA algorithm.
            for (int i = 0; i < MaxEpaIterations; i++)
            {
                // Get the simplex edge closest to origin.
                var vertices = simplex.Peek();
                var edge = vertices.Item2 - vertices.Item1;

                // Calculate new support vertex in direction of origin.
                var support = GetSupportVertex(shapeA, shapeB, edge.PerendicularClockwise());

                // Terminate if support vertex is not significantly closer to origin.
                float distance = support.Dot(edge.PerendicularClockwise());
                if ((distance - edge.Magnitude()) < EpaEpsilon)
                {
                    return new Collision
                    {
                        Normal = edge.PerendicularClockwise(),
                        Depth = distance
                    };
                }

                // Use new support vertex to split closest edge in two.
                simplex.Dequeue();
                simplex.Enqueue((vertices.Item1, support, winding));
                simplex.Enqueue((support, vertices.Item2, winding));
            }

            var topVertices = simplex.Peek();
            var topEdge = topVertices.Item2 - topVertices.Item1;

            return new Collision
            {
                Normal = topEdge,
                Depth = topEdge.Magnitude()
            };
        }


        private static bool OverlapsGjk(IShape shapeA, IShape shapeB, ref List<Vector2f> simplex)
        {
            var direction = new Vector2f();

            int vertexCount = 0;

            for(int i = 0; i < MaxGjkIterations; i++)
            {
                switch (vertexCount)
                {
                    case 0:
                        direction = shapeB.GetPosition() - shapeA.GetPosition();
                        if (direction.IsZero())
                        {
                            direction.X = 1;
                        }
                        break;
                    case 1:
                        direction *= -1;
                        break;
                    case 2:
                    {
                        var xy = simplex[0] - simplex[1];
                        var xo = simplex[1] * -1;
                        direction = GetTripleProduct(xy, xo, xy);

                        if (direction.Magnitude() < float.Epsilon)
                        {
                            direction = xy.PerendicularCounterClockwise();
                        }
                        break;
                    }
                    case 3:
                    {
                        // Testing whether simplex contains origin..
                        var xy = simplex[1] - simplex[2];
                        var xz = simplex[0] - simplex[2];
                        var xo = simplex[2] * -1.0f;

                        // Origin outside simplex to right of xz, remove y and reset search direction.
                        var xzPerp = GetTripleProduct(xy, xz, xz);
                        if (xzPerp.Dot(xo) >= 0.0f)
                        {
                            simplex[1] = simplex[2];
                            direction = xzPerp;
                            vertexCount--;
                            simplex.Remove(simplex.Last());
                            break;
                        }

                        // Origin outside simplex to left of xy, remove z and reset search direction.
                        var xyPerp = GetTripleProduct(xz, xy, xy);
                        if (xyPerp.Dot(xo) >= 0.0f)
                        {
                            simplex[0] = simplex[1];
                            simplex[1] = simplex[2];
                            direction = xyPerp;
                            vertexCount--;
                            simplex.Remove(simplex.Last());
                            break;
                        }

                        // Origin inside simplex, overlap found!
                        return true;
                    }
                    default:
                        break;
                }

                // Calculate support vertex in search direction.
                var support = GetSupportVertex(shapeA, shapeB, direction);

                // Support vertex did not pass origin, no overlap!
                if (support.Dot(direction) <= GjkEpsilon)
                {
                    return false;
                }

                // Add support vertex to simplex.
                simplex.Add(support);
                vertexCount++;
            }

            return false;
        }

        private static Vector2f GetSupportVertex(IShape shapeA, IShape shapeB, Vector2f direction)
        {
            var vertexA = shapeA.GetVertex(direction);
            var vertexB = shapeB.GetVertex(-direction);
            return vertexA - vertexB;
        }

        private static Vector2f GetTripleProduct(Vector2f a, Vector2f b, Vector2f c)
        {
            // Simplified triple product (b x (a . c)) - (a x (b . c))
            float dot = a.X * b.Y - a.Y * b.X;
            float x = -c.Y * dot;
            float y = c.X * dot;
            return new Vector2f(x, y);
        }

        private static int GetWindingOrder(List<Vector2f> simplex)
        {
            float e0 = (simplex[1].X - simplex[0].X) * (simplex[1].Y + simplex[0].Y);
            float e1 = (simplex[2].X - simplex[1].X) * (simplex[2].Y + simplex[1].Y);
            float e2 = (simplex[0].X - simplex[2].X) * (simplex[0].Y + simplex[2].Y);
            return (e0 + e1 + e2 >= 0) ? 1 : -1;
        }

        public static Collision CheckCollision(Rectangle a, Rectangle b)
        {
            Collision collision = null;

            // Vector from A to B.
            var n = b.GetCentre() - a.GetCentre();

            // Rectangles collide if they overlap in both x and y axis.
            float xOverlap = a.GetHalfSize().X + b.GetHalfSize().X - MathExtensions.Abs(n.X);
            if (xOverlap > 0)
            {
                float yOverlap = a.GetHalfSize().Y + b.GetHalfSize().Y - MathExtensions.Abs(n.Y);
                if (yOverlap > 0)
                {
                    // Resolve collision along axis of least overlap.
                    if (xOverlap < yOverlap)
                    {
                        collision = new Collision();
                        collision.Normal = (n.X < 0) ? new Vector2f(-1, 0) : new Vector2f(1, 0);
                        collision.Depth = xOverlap;
                        return collision;
                    }
                    else
                    {
                        collision = new Collision();
                        collision.Normal = (n.Y < 0) ? new Vector2f(0, -1) : new Vector2f(0, 1);
                        collision.Depth = yOverlap;
                        return collision;
                    }
                }
            }
            return collision;
        }

        public static Collision CheckCollision(Circle a, Circle b)
        {
            Collision collision = null;

            // Vector from A to B.
            var n = b.GetPosition() - a.GetPosition();

            // No collision if distance greater than combined radii.
            float d = n.MagnitudeSquared();
            float r = a.GetRadius() + b.GetRadius();
            if (d > r * r)
            {
                return null;
            }

            // Avoided square root until needed.
            d = MathExtensions.Sqrt(d);

            // Collision resolution.
            if (d == 0)
            {
                // Circles are in the same position, set arbitrary collision normal.
                collision.Normal = new Vector2f(1, 0);
                collision.Depth = a.GetRadius();
            }
            else
            {
                collision.Normal = n / d;
                collision.Depth = r - d;
            }

            return collision;
        }

        public static Collision CheckCollision(Rectangle a, Circle b)
        {
            Collision collision = new Collision();
            // Vector from A to B.
            var n = b.GetPosition() + b.GetHalfSize() - a.GetCentre();

            // Get rectangle-vertex closest to circle center by clamping vector to rectangle bounds.
            var closest = new Vector2f(n.X, n.Y);
            closest.X = MathExtensions.Clamp(closest.X, -a.GetHalfSize().X, a.GetHalfSize().X);
            closest.Y = MathExtensions.Clamp(closest.Y, -a.GetHalfSize().Y, a.GetHalfSize().Y);

            // If clamping vector had no effect, then circle center is inside rectangle.
            bool inside = (n == closest);

            // Recalculate rectangle-vertex closest to circle center.
            if (inside)
            {
                if (MathExtensions.Abs(n.X) > MathExtensions.Abs(n.Y))
                {
                    closest.X = (closest.X > 0) ? a.GetHalfSize().X : -a.GetHalfSize().X;
                }
                else
                {
                    closest.Y = (closest.Y > 0) ? a.GetHalfSize().Y : -a.GetHalfSize().Y;
                }
            }

            // Calculate vector from circle center to closest rectangle-vertex.
            var nn = n - closest;
            float d = nn.MagnitudeSquared();
            float r = b.GetRadius();

            // No collision if vector is greater than circle radius.
            if (d > (r * r) && !inside)
            {
                return null;
            }

            // Avoided square root until needed.
            d = MathExtensions.Sqrt(d);

            // Collision resolution.
            if (inside)
            {
                collision.Normal = -1.0f * nn / d;
                collision.Depth = r + d;
            }
            else
            {
                collision.Normal = nn / d;
                collision.Depth = r - d;
            }

            return collision;
        }

        public static Collision CheckCollision(Circle a, Rectangle b)
        {
            return CheckCollision(b, a);
        }

        public static Vector2f? CheckCollision(Vector2f p0, Vector2f p1, Vector2f p2, Vector2f p3)
        {
            float s1_x = p1.X - p0.X; 
            float s1_y = p1.Y - p0.Y;
            float s2_x = p3.X - p2.X;
            float s2_y = p3.Y - p2.Y;

            float s = (-s1_y * (p0.X - p2.X) + s1_x * (p0.Y - p2.Y)) / (-s2_x * s1_y + s1_x * s2_y);
            float t = (s2_x * (p0.Y - p2.Y) - s2_y * (p0.X - p2.X)) / (-s2_x * s1_y + s1_x * s2_y);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                // Collision detected
                return new Vector2f(p0.X + (t * s1_x), p0.Y + (t * s1_y));
            }

            return null; // No collision
        }
    }
}
