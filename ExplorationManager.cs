using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Stellaris
{
    public static class ExplorationManager
    {
        public static StarSystem starSystemPlayerAt;
        public static Planet planetPlayerAt;
        public static void ExploreSystem(Vector2 position , bool playerArrive)
        {

            var galaxyComponent = Current.Game.GetComponent<GalaxyComponent>();
            if (galaxyComponent?.ClusterData == null) return;

            var galaxy = galaxyComponent.ClusterData;

            // 标记为已探索
            if (galaxy.exploredSystems.ContainsKey(position))
            {
                galaxy.exploredSystems[position] = true;
            }

            // 如果系统不存在，生成它
            var existingSystem = galaxy.starSystems.FirstOrDefault(s => s.position == position);
            if (existingSystem == null)
            {
                var newSystem = GenerateStarSystem(position);
                newSystem.explored = true;
                galaxy.starSystems.Add(newSystem);

                // 生成新的相邻隐藏系统
                GenerateNewAdjacentSystems(position, galaxy);
            }
            else
            {
                existingSystem.explored = true;
            }
            if (playerArrive)
            {
                MovePlayerShipToAnotherSystem(position.GetStarSystem());
            }
        }

        public static void MovePlayerShipToAnotherSystem(StarSystem systemMoveTo)
        {
            GalaxyComponent comp = Current.Game.GetComponent<GalaxyComponent>();
            WorldShip ship = comp.ClusterData.universeObjects.Where(x => x is WorldShip && ((WorldShip)x).def == StellarisDefOf.UniverseMapParent_Ship) as WorldShip;
            if (ship != null)
            {
                ship.starSystem.universeObjects.Remove(ship);
                ship.starSystem = systemMoveTo;
                ship.planet = null;
                starSystemPlayerAt = systemMoveTo;
                systemMoveTo.universeObjects.Add(ship);
            }
            else
            {
                Log.Error("World ship don't exist.");
            }
        }

        public static StarSystem GenerateStarSystem(Vector2 position)
        {
            var system = new StarSystem
            {
                position = position,
                name = NameGenerator.GenerateSystemName(),
                explored = false
            };

            system.GenerateSystemContent();
            return system;
        }

        private static void GenerateNewAdjacentSystems(Vector2 center, GalaxyCluster galaxy)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    Vector2 newPos = center + new Vector2(x, y);
                    if (!galaxy.exploredSystems.ContainsKey(newPos))
                    {
                        galaxy.exploredSystems[newPos] = false;
                    }
                }
            }
        }
        public static StarSystem GetStarSystem(this Vector2 vector)
        {
            GalaxyComponent comp = Current.Game.GetComponent<GalaxyComponent>();
            foreach (StarSystem starSystem in comp.ClusterData.starSystems)
            {
                if (starSystem.position == vector)
                {
                    return starSystem;
                }
            }
            return null;
        }
    }
}
