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
        public const string name = "Blue Waffle2";

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
            while (ourPlanets.Count > 0)
            {
                ourPlanets.RemoveAll(x=>toDelete.Contains(x.Planet.Id));
                foreach (var planet in ourPlanets)
                {

                    planet.Enemies.RemoveAll(x => usedPlanet.Any(u => u.Id == x.Id));
                    bool used = planet.Enemies.Count > 0;
                    foreach (var enemy in planet.Enemies)
                    {
                        if (planet.Planet.ShipCount > enemy.ShipCount)
                        {
                            usedPlanet.Add(enemy);
                            Send(planet.Planet, enemy, 2);
                            
                            planet.Planet.ShipCount -= enemy.ShipCount + 1;
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

        void Send(Planet owner, Planet target, int size)
        {
            for (int i = 0; i < size; i++)
            {
                Game.AttackPlanet(owner, target, Convert.ToInt32(Math.Ceiling((double)(target.ShipCount + 1) / size)));
            }
        }


        public void set_name()
        {
            Console.Out.WriteLine("Setting name");
            Game.SetName(name);
        }
    }
}
