using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIServer
{
    public class AIHelper
    {
        private UpdateContainer Container;

        public class PlanetEnemies
        {
            Planet planet;
            IEnumerable<KeyValuePair<Planet, double>> enemies;

            public PlanetEnemies(Planet planet, IEnumerable<Planet> enemies)
            {
                this.planet = planet;
                this.enemies = enemies;
            }
        }

        public AIHelper(UpdateContainer container)
        {
            Container = container;
        }

        public float TotalArmySize()
        {
            return Container.Planets.Sum(x => x.Size);
        }
        public IEnumerable<Planet> OurPlanet()
        {
            return Container.Planets.Where(x => x.Owner == AI.name);
        }

        public IEnumerable<Planet> EnemyPlanetsByDistance(Planet a)
        {
            return Container.Planets.Where(x => x.Owner != AI.name).OrderBy(x => DistanceBetweenPlanets(a, x)).ThenByDescending(x=>x.Size);
        }

        public IEnumerable<Planet> WeakEnemyPlanets(Planet ourPlanet, IEnumerable<Planet> enemies)
        {
            return enemies.Where(x => ourPlanet.ShipCount > x.ShipCount);
        }
        //public IEnumerable<KeyValuePair<Planet, Planet>> WeakEnemyPlanets(IEnumerable<PlanetEnemies> ourPlanets)
        //{
            
        //    //return enemies.Where(x => ourPlanet.ShipCount > x.ShipCount);
        //}

        private double DistanceBetweenPlanets(Planet a, Planet b)
        {
            var intermediare = Math.Pow((b.PosX - a.PosX),2) + Math.Pow((b.PosY - a.PosY),2);
            return Math.Sqrt(intermediare);
        }
    }
}
