using SFML.Graphics;
using SFML.System;
using Shared;

namespace LevelEditor
{
    public class AllignmentGrid
    {
        public CircleShape[,] GridVisuals { get; set; }

        public AllignmentGrid()
        {
            GridVisuals = new CircleShape[Configuration.GridWidth + 1, Configuration.GridHeight + 1];

            for (int x = 0; x < Configuration.GridWidth + 1; x++)
            {
                for (int y = 0; y < Configuration.GridHeight + 1; y++)
                {
                    var radius = 1;
                    var position = new Vector2f(x * Configuration.GridSize - radius, y * Configuration.GridSize - radius);
                    GridVisuals[x, y] = new CircleShape(radius) { Position = position, FillColor = new Color(0xe1, 0xe1, 0xe1) };
                }
            }
        }

        public void Draw(RenderWindow window) 
        {

            for (int x = 0; x < Configuration.GridWidth + 1; x++)
            {
                for (int y = 0; y < Configuration.GridHeight + 1; y++)
                {
                    window.Draw(GridVisuals[x, y]);
                }
            }
        }
    }
}
