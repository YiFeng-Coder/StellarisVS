using HarmonyLib;
using LudeonTK;
using RimWorld;
using RimWorld.BaseGen;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Stellaris
{
    public static class SiteUtility
    {
        [DebugAction("Stellaris Tools", "Generate Arcaeological Site", false, false, allowedGameStates = AllowedGameStates.PlayingOnWorld)]
        public static void DebugGenerateSite()
        { 
            GenerateSite();
        }
        public static void GenerateSite()
        {
            PlanetTile tile;
            if (TryFindSiteTile(out tile))
            {
                var siteParts = new List<SitePartDefWithParams>
                {
                    new SitePartDefWithParams(StellarisDefOf.StellarisArchaeologicalSite, new SitePartParams
                    {
                            
                    })
                };
                Site site = SiteMaker.MakeSite(siteParts, tile,Faction.OfAncients);
                site.GetComponent<TimeoutComp>().StartTimeout(60000 * 180); 
                Find.WorldObjects.Add(site);
            }   
            else
            {
                Log.Error("Cannot find any valid site tile");
            }
        }
        private static bool TryFindSiteTile(out PlanetTile tile)
        {
            return TileFinder.TryFindNewSiteTile(out tile);
        }
    }
}
