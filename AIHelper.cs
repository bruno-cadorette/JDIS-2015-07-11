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
            return EnemyPlanets().OrderBy(x => DistanceBetweenPlanets(a, x)).ThenByDescending(x => x.Size);
        }

        public IEnumerable<Planet> EnemyPlanets()
        {
            return Container.Planets.Where(x => x.Owner != AI.name);
        }
        public IEnumerable<Planet> WeakEnemyPlanets(Planet ourPlanet, IEnumerable<Planet> enemies)
        {
            return enemies.Where(x => ourPlanet.ShipCount > x.ShipCount);
        }

        public double DistanceToClosestEnemyPlanet(Planet planet)
        {
            var p = EnemyPlanets().Where(x => x.Id != planet.Id).OrderBy(x => DistanceBetweenPlanets(planet, x)).First();
            return DistanceBetweenPlanets(planet,p);
        }

        public Planet PlanetInDanger()
        {
            return OurPlanet().OrderBy(DistanceToClosestEnemyPlanet).First();
        }
        //public IEnumerable<KeyValuePair<Planet, Planet>> WeakEnemyPlanets(IEnumerable<PlanetEnemies> ourPlanets)
        //{
            
        public int NbShipsAttacking(Planet destination)
        {
            return Container.Ships.Where(x => x.Owner == AI.name && x.TargetId == destination.Id).Sum(x => x.ShipCount);
        }

        public double DistanceBetweenPlanets(Planet a, Planet b)
        {
            var intermediare = Math.Pow((b.PosX - a.PosX),2) + Math.Pow((b.PosY - a.PosY),2);
            return Math.Sqrt(intermediare);
        }
    }
}
