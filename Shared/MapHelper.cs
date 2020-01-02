using SFML.System;
using SFML.Window;
using System;

namespace Shared
{
    public static class MapHelper
    {
        public static Vector2f TitleBarSize = new Vector2f(12, 57);


        public static Vector2f GetCentreOfGrid(Vector2i windowPosition)
        {
            return GetCentreFromIndex(GetIndexFromPosition(GetMousePosition(windowPosition)));
        }

        public static Vector2i GetIndexFromPosition(Vector2f position)
        {
            var x = position.X - Configuration.GridSize / 2;
            var y = position.Y - Configuration.GridSize / 2;

            return new Vector2i((int)Math.Round(x / Configuration.GridSize, 0), (int)Math.Round(y / Configuration.GridSize, 0));
        }

        public static Vector2f GetCentreFromIndex(Vector2i index)
        {
            var x = index.X * Configuration.GridSize + Configuration.GridSize / 2;
            var y = index.Y * Configuration.GridSize + Configuration.GridSize / 2;

            return new Vector2f(x, y);
        }

        public static Vector2f GetMousePosition(Vector2i windowPosition)
        {
            var position = Mouse.GetPosition() - windowPosition;

            return new Vector2f(position.X - TitleBarSize.X, position.Y - TitleBarSize.Y);
        }
    }
}
