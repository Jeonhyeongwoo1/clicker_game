using Clicker.Entity;

namespace Clicker.Manager
{
    public class MapManager
    {
        public Map CurrentMap => _currentMap;
        private Map _currentMap;

        public void SetMap(Map map)
        {
            _currentMap = map;
        }
    }
}