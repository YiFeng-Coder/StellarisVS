using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Stellaris
{
    public static class PlanetGenerateUtility
    {
        public static List<BiomeDef> GetLavePlanetBiomeDefs()
        { 
            List<BiomeDef> list = new List<BiomeDef>();
            //list.Add(BiomeDefOf.Glowforest);
            list.Add(StellarisDefOf.StellarisLavaPlanetBiome);
            return list;
        }
    }
}
