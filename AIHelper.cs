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

        public double Rayon
        {
            get
            {
                return Container.Planets.Average(planet => DistanceBetweenPlanets(planet, ClosestPlanet(planet,Container.Planets))) * 2;
            }
        }

        public AIHelper(UpdateContainer container)
        {
            Container = container;
        }

        public int TotalPlanetArmySize()
        {
            return Container.Planets.Where(x => x.Owner == AI.name).Sum(x => x.ShipCount);
        }
        public Planet DeathStar()
        {
            foreach (Planet planet in Container.Planets)
            {
                if (planet.IsDeathStar)
                    return planet;
            }
            return null;
        }
        public Planet PlanetWithHighestPop()
        {
            int highest = 0;
            Planet target = DeathStar();
            foreach (Planet planet in Container.Planets.Where(x => x.Owner != AI.name && x.Owner != String.Empty))
            {
                if (planet.ShipCount > highest)
                {
                    highest = planet.ShipCount;
                    target = planet;
                }
            }
            return target;
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
            return Container.Planets.Where(x => x.Owner != AI.name).Where(x => x.Owner != String.Empty || x.ShipCount <= 6);
        }
        public IEnumerable<Planet> WeakEnemyPlanets(Planet ourPlanet, IEnumerable<Planet> enemies)
        {
            return enemies.Where(x => ourPlanet.ShipCount > x.ShipCount);
        }

        public double DistanceToClosestEnemyPlanet(Planet planet)
        {
            var p = EnemyPlanets().Where(x => x.Id != planet.Id).OrderBy(x => DistanceBetweenPlanets(planet, x)).First();
            return DistanceBetweenPlanets(planet, p);
        }

        public Planet PlanetInDanger(Planet planet, IEnumerable<Planet> planets)
        {
            return planets.OrderBy(DistanceToClosestEnemyPlanet).First();
        }
            
        public int NbShipsAttacking(Planet destination)
        {
            return Container.Ships.Where(x => x.Owner == AI.name && x.TargetId == destination.Id).Sum(x => x.ShipCount);
        }

        public bool EnemyInRayon(Planet home, double rayon)
        {
            return EnemyPlanets().Any(enemy => DistanceBetweenPlanets(home, enemy) > rayon);
        }

        public double DistanceBetweenPlanets(Planet a, Planet b)
        {
            var intermediare = Math.Pow((b.PosX - a.PosX), 2) + Math.Pow((b.PosY - a.PosY), 2);
            return Math.Sqrt(intermediare);
        }

        public Planet ClosestPlanet(Planet home, IEnumerable<Planet> planets)
        {
            return planets.Where(x => x.Id != home.Id).OrderBy(x => DistanceBetweenPlanets(home, x)).First();
        }

        public Planet ClosestPlanetInDanger(Planet home, IEnumerable<Planet> planets)
        {

            return planets.Where(x => x.Id != home.Id).OrderBy(x => DistanceBetweenPlanets(home, x)).FirstOrDefault(x => !IsSafe(x));
        }

        public Tuple<IEnumerable<T>, IEnumerable<T>> Partition<T>(IEnumerable<T> collection, Func<T, bool> predicate)
        {
            return Tuple.Create(collection.Where(predicate), collection.Where(x => !predicate(x)));
        }

        public bool IsSafe(Planet ourPlanet)
        {
            return EnemyPlanets().Where(x=>x.Owner!="").OrderBy(x => DistanceBetweenPlanets(ourPlanet, x)).Take(5).All(x => x.Owner == AI.name);
        }
    }
}
