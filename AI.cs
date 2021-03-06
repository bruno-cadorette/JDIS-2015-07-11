﻿using System;
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

            if (helper.DeathStar().Owner != name && helper.DeathStar().Owner != string.Empty)
            {
                foreach (var planet in ourPlanets)
                {
                    Send(planet.Planet, helper.DeathStar(), 1, planet.Planet.ShipCount);
                }
                return;
            }
            if (helper.DeathStar().Owner == name && helper.DeathStar().DeathStarCharge >= 1)
                Game.DeathstarDestroyPlanet(helper.DeathStar(), helper.PlanetWithHighestSize());

            while (ourPlanets.Count > 0)
            {
                ourPlanets.RemoveAll(x=>toDelete.Contains(x.Planet.Id));
                foreach (var planet in ourPlanets)
                {
                    planet.Enemies.RemoveAll(x => usedPlanet.Any(u => u.Id == x.Id));
                    bool used = planet.Enemies.Count > 0;
                    if (helper.IncomingForce(planet.Planet) <= planet.Planet.ShipCount)
                    {
                        planet.Planet.ShipCount -= helper.IncomingForce(planet.Planet);
                    }
                    foreach (var enemy in planet.Enemies)
                    {
                        if (enemy.Owner != String.Empty)
                        {
                            Send(planet.Planet, enemy, 2, planet.Planet.ShipCount / 2);
                            planet.Planet.ShipCount /= 2;
                        }
                        else if (enemy.Owner == String.Empty && planet.Planet.ShipCount > enemy.ShipCount+1 && helper.NbShipsAttacking(enemy) <= enemy.ShipCount + 1)
                        {
                            int additionalShips = 2;

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
