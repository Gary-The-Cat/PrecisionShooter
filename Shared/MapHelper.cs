using SFML.System;
using SFML.Window;
using Shared.DataStructures;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Shared
{
    public static class MapHelper
    {
        public static Vector2f TitleBarSize = new Vector2f(12, 57);

        public static Vector2f GetCentreOfGrid(Vector2i windowPosition)
        {
            return GetCentreFromIndex(GetIndexFromPosition(GetMousePosition(windowPosition)));
        }

        public static Vector2f GetBottomLeftOfGrid(Vector2i windowPosition)
        {
            var centre = GetCentreFromIndex(GetIndexFromPosition(GetMousePosition(windowPosition)));
            return new Vector2f(centre.X - Configuration.GridSize / 2, centre.Y - Configuration.GridSize / 2);
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

        public static void SerialiseMap(Map map)
        {
            // Create a stream to serialize the object to.  
            var ms = new MemoryStream();

            // Serializer the User object to the stream.  
            new DataContractJsonSerializer(typeof(Map)).WriteObject(ms, map);

            File.WriteAllBytes(GetUniqueMapLocation(), ms.ToArray());
        }

        public static Map DeserialiseMap(string mapLocation)
        {
            // Create a deserialiser
            var deser = new DataContractJsonSerializer(typeof(Map));

            // Get the byte data from the map file
            byte[] byteArray = Encoding.ASCII.GetBytes(File.ReadAllText(mapLocation));

            // Create a memory stream from the byte array
            MemoryStream ms = new MemoryStream(byteArray);

            // Read the map object out of the memory stream
            return deser.ReadObject(ms) as Map;
        }

        private static string GetUniqueMapLocation()
        {
            int map = 0;
            var rootDirectory = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\Resources\Maps");
            var mapLocation = rootDirectory + @"\" + map;
            while (File.Exists(mapLocation))
            {
                map++; 
                mapLocation = rootDirectory + @"\" + map;
            }

            return mapLocation;
        }
    }
}
