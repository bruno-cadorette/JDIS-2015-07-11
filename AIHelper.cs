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
                return Container.Planets.Average(planet => DistanceBetweenPlanets(planet, ClosestPlanet(planet, Container.Planets))) * 2;
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
        public Planet PlanetWithHighestSize()
        {
            float highest = 0;
            Planet target = DeathStar();
            foreach (Planet planet in Container.Planets.Where(x => x.Owner != AI.name && x.Owner != String.Empty))
            {
                if (planet.Size > highest)
                {
                    highest = planet.Size;
                    target = planet;
                }
            }
            return target;
        }
        public int NbEnemyPlanet()
        {
            return Container.Planets.Where(x => x.Owner != AI.name && x.Owner != String.Empty).Sum(x => 1);
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
            //return Container.Planets.Where(x => x.Owner != AI.name && x.Owner != String.Empty);
            return Container.Planets.Where(x => x.Owner != AI.name).Where(x => x.Owner != String.Empty || x.ShipCount <= 6);
        }

        public double DistanceToClosestEnemyPlanet(Planet planet)
        {
            var p = EnemyPlanets().Where(x => x.Id != planet.Id).OrderBy(x => DistanceBetweenPlanets(planet, x)).First();
            return DistanceBetweenPlanets(planet, p);
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
        public int IncomingForce(Planet home)
        {
            int enemy = IncomingCount(s => s.Owner != AI.name, home);
            int reinforcement = IncomingCount(s => s.Owner == AI.name, home);
            return enemy - reinforcement;
        }
        private int IncomingCount(Func<Ship, bool> controller, Planet home)
        {
            return Container.Ships.Where(controller).Where(s => s.TargetId == home.Id).Sum(s => s.ShipCount);
        }
    }
}
