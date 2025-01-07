using System;

namespace Clicker.Entity
{
    [Serializable]
    public class MapCollisionData
    {
        public int minX;
        public int maxX;
        public int minY;
        public int maxY;

        public int[,] map;
    }
}