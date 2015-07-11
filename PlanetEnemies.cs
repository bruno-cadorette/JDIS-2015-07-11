using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIServer
{
    public class PlanetEnemies
    {
        public Planet Planet { get; set; }
        public List<Planet> Enemies { get; set; }

        public PlanetEnemies(Planet planet, IEnumerable<Planet> enemies)
        {
            Planet = planet;
            Enemies = enemies.ToList();
        }
    }
}
