using SFML.Graphics;
using SFML.System;
using Shared.Components;
using System.Collections.Generic;
using System.IO;

namespace Shared.Helpers
{
    public static class ComponentLoader
    {
        public static List<Component> GetComponents()
        {
            var components = new List<Component>();

            var rootDirectory = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\Resources\Components");

            var componentFolders = Directory.GetDirectories(rootDirectory);

            foreach (var componentFolder in componentFolders)
            {
                if(File.Exists(componentFolder + @"\Image.png"))
                {
                    components.Add(LoadComponent(componentFolder));
                }
            }

            return components;
        }

        private static Component LoadComponent(string componentFolder)
        {
            return new Component(Path.GetFileName(componentFolder), new Vector2f(0, 0));
        }
    }
}
