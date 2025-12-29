using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Stellaris
{
    public class GalaxyCluster : IExposable
    {
        public List<StarSystem> starSystems = new List<StarSystem>();
        public Dictionary<Vector2, bool> exploredSystems = new Dictionary<Vector2, bool>();
        public Vector2 initialSystemPosition;
        // 注意：StarSystem 和 Planet 是 WorldObject，它们在新生成的 World 中通常会失效。
        // 如果你需要跨存档保留它们，它们必须是纯数据类 (Plain C# Class)，不能继承自 WorldComponent 或 GameComponent。
        // 如果 StarSystem 是纯数据类，下面代码是安全的。
        public static StarSystem initialSystem;
        public static Planet initialPlanet;
        // IUniversable 需要确保它的实现类可以被 Deep Save
        public List<IUniversable> universeObjects = new List<IUniversable>();
        public static Scenario initialScenario;
        public void ExposeData()
        {
            Scribe_Collections.Look(ref starSystems, "starSystems", LookMode.Deep);
            Scribe_Collections.Look(ref exploredSystems, "exploredSystems", LookMode.Value, LookMode.Value);
            Scribe_Values.Look(ref initialSystemPosition, "initialSystemPosition");
            Scribe_Deep.Look(ref initialPlanet, "initialPlanet");
            Scribe_Deep.Look(ref initialSystem, "initialSystem");
            Scribe_Deep.Look(ref initialScenario, "initialScenario");
            // 如果 IUniversable 是多态的，需要处理多态保存
            Scribe_Collections.Look(ref universeObjects, "universeObjects", LookMode.Deep);

        }

        public void GenerateInitialCluster()
        {
            starSystems = new List<StarSystem>();
            exploredSystems = new Dictionary<Vector2, bool>();

            // 生成初始恒星系

            initialSystem = ExplorationManager.GenerateStarSystem(Vector2.zero);
            initialPlanet = PlanetGenerator.GeneratePlanetWithType(PlanetGenerator.GenerateOrbitSlots(initialSystem.planets.Count+1, initialSystem.star).First(), initialSystem.star, initialSystem.planets.Count + 1, PlanetType.Terrestrial);
            initialPlanet.resources.Add(ThingDefOf.Plasteel);
            initialPlanet.resources.Add(ThingDefOf.ComponentIndustrial);
            initialPlanet.habitable = true;
            initialPlanet.temperature = 300f;
            initialPlanet.name = Find.World.info.seedString;
            initialSystem.planets.Add(initialPlanet);
            starSystems.Add(initialSystem);
            exploredSystems[Vector2.zero] = true;
            initialSystemPosition = Vector2.zero;
            ExplorationManager.starSystemPlayerAt = initialSystem;
            ExplorationManager.planetPlayerAt = initialPlanet;
            // 生成周围的隐藏恒星系

            GenerateSurroundingSystems(Vector2.zero, 3);
        }
        /*
         
         GenerateSurroundingSystems方法等待调整
         
         
         */
        private void GenerateSurroundingSystems(Vector2 center, int radius)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    Vector2 pos = new Vector2(x, y) + center;
                    if (!exploredSystems.ContainsKey(pos) && Vector2.Distance(center, pos) <= radius)
                    {
                        exploredSystems[pos] = false;
                    }
                }
            }
        }
    }
}
