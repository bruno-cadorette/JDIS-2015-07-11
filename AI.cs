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
            foreach (var planet in helper.OurPlanet())
            {
                var enemies = helper.EnemyPlanetsByDistance(planet);
                var weaks = helper.WeakEnemyPlanets(planet, enemies).ToArray();
                int shipCount = planet.ShipCount;





                for (int i=0;i < weaks.Length ;i++)
                {
                    if (shipCount > weaks[i].ShipCount)
                    {
                        Game.AttackPlanet(planet, weaks[i], weaks[i].ShipCount + 1);
                        shipCount -= weaks[i].ShipCount + 1;
                    }
                }
            }
            Console.Out.WriteLine("Updating");
        }

        public void set_name()
        {
            Console.Out.WriteLine("Setting name");
            Game.SetName(name);
        }
    }
}
