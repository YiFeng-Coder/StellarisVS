using RimWorld;
using Stellaris.DevTools;
using Stellaris.PlanetTravel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Stellaris
{
    public class GenStep_ArchaeologicalSite : GenStep
    {

        public override int SeedPart => 5152384;

        public override void Generate(Map map, GenStepParams parms)
        {
            TerrainGrid terrainGrid = map.terrainGrid;
            foreach (IntVec3 c in map.AllCells)
            {
                terrainGrid.SetTerrain(c, TerrainDefOf.Concrete);
            }
            foreach (Thing thing in map.listerThings.AllThings.ToList())
            {
                if (thing.def.destroyable)
                {
                    thing.Destroy();
                }
                else
                {
                    thing.DeSpawn();
                }
            }
            foreach (var item in map.AllCells)
            {
                map.roofGrid.SetRoof(item, null);
            }
            //AreaLoader.LoadAreaFromXml(parms.sitePart.def.tags.First(), map,IntVec3.Zero);
            ShipTransporter.DeserializeAndReconstruct(map,false, ShipTransporter.SITE_SNAPSHOT);
        }
    }
}
