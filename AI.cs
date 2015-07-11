using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIServer
{
    public class AI
    {
        public TCPServer Game { get; private set; }

        // Set your team Name here!!!
        public const string name = "Blue Waffle";

        public AI(TCPServer server)
        {
            Game = server;
        }

        public void update(UpdateContainer container)
        {
            var helper = new AIHelper(container);
            var ourPlanets = helper.OurPlanet().Select(planet => new PlanetEnemies(planet, helper.EnemyPlanetsByDistance(planet))).ToList();
            var usedPlanet = new List<Planet>();
            var toDelete = new List<int>();

            if (helper.DeathStar().Owner != name && helper.DeathStar().Owner != string.Empty && helper.TotalPlanetArmySize() > helper.DeathStar().ShipCount)
            {
                foreach (var planet in ourPlanets)
                {
                    Send(planet.Planet, helper.DeathStar(), 1, planet.Planet.ShipCount);
                }
                return;
            }
            if (helper.DeathStar().Owner == name && helper.DeathStar().DeathStarCharge >= 1)
                Game.DeathstarDestroyPlanet(helper.DeathStar(), helper.PlanetWithHighestPop());

            //Defence(helper);
            while (ourPlanets.Count > 0)
            {
                ourPlanets.RemoveAll(x=>toDelete.Contains(x.Planet.Id));
                foreach (var planet in ourPlanets)
                {

                    planet.Enemies.RemoveAll(x => usedPlanet.Any(u => u.Id == x.Id));
                    bool used = planet.Enemies.Count > 0;
                    foreach (var enemy in planet.Enemies)
                    {
                        if (planet.Planet.ShipCount > enemy.ShipCount+1 && helper.NbShipsAttacking(enemy) <= enemy.ShipCount + 1)
                        {
                            int additionalShips = 0;
                            if (enemy.Owner == String.Empty)
                                additionalShips = 2;
                            else
                                additionalShips = 6;

                            usedPlanet.Add(enemy);
                            Send(planet.Planet, enemy, 2, enemy.ShipCount + additionalShips);
                            
                            planet.Planet.ShipCount -= enemy.ShipCount + additionalShips;
                            used = true;
                            break;
                        }
                        used = false;
                    }
                    if (!used)
                    {
                        toDelete.Add(planet.Planet.Id);
                    }
                }
            }
            Console.Out.WriteLine("Updating");
        }
        void Defence(AIHelper helper)
        {
            double rayon = helper.Rayon;
            var ourPlanets = helper.OurPlanet();
            var enemies = helper.EnemyPlanets();
            var planets = helper.Partition(ourPlanets, x => helper.EnemyInRayon(x, rayon));

            // Item1 == ceux en danger
            // Item2 == a envoyer
            foreach (var planet in planets.Item2.Where(x=>!helper.IsSafe(x)))
            {
                var ally = helper.ClosestPlanet(planet, planets.Item1);
                Send(planet, ally, 2, planet.ShipCount / 2);
            }
        }

        void Send(Planet owner, Planet target, int chunks, int size)
        {
            for (int i = 0; i < chunks; i++)
            {
                Game.AttackPlanet(owner, target, Convert.ToInt32(Math.Floor((double)size / chunks)));
            }
        }


        public void set_name()
        {
            Console.Out.WriteLine("Setting name");
            Game.SetName(name);
        }
    }
}
