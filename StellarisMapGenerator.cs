using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Stellaris
{
    public static class StellarisMapGenerator
    {
        public static Map GenerateMapOnPlayerPlanet(IntVec3 mapSize, MapParent parent, MapGeneratorDef mapGenerator, IEnumerable<GenStepWithParams> extraGenStepDefs = null, Action<Map> extraInitBeforeContentGen = null, bool isPocketMap = false, bool stepDebugger = false)
        {
            if (ExplorationManager.planetPlayerAt == GalaxyCluster.initialPlanet || mapGenerator == StellarisDefOf.StellarisSpace)
            {
                return MapGenerator.GenerateMap(mapSize,parent, mapGenerator, extraGenStepDefs,extraInitBeforeContentGen,isPocketMap);
            }
            else
            {
                Map map;
                switch (ExplorationManager.planetPlayerAt.type)
                {
                    default:
                        map = MapGenerator.GenerateMap(mapSize,parent,StellarisDefOf.StellarisCommonPlanetGenerator, extraGenStepDefs, extraInitBeforeContentGen, isPocketMap);
                        break;
                }
                return map;
            }
        }
    }
}
