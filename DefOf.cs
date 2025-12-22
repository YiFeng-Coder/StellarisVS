using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Stellaris
{
    [DefOf]
    public static class StellarisDefOf
    {        
        static StellarisDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(StellarisDefOf));
        }

        public static MainButtonDef GalaxyMap;
        public static WorldObjectDef UniverseMapParent_Ship;
        public static WorldObjectDef UniverseObject_AutonomousMiner;
        public static RoofDef StellarisShipRoof;
        public static TerrainDef StellarisShipFakeFloorInsideShip;
        public static ThingDef StellarisShipHullTile;
        public static ThingDef StellarisResourceOrganic;
        public static ThingDef ActiveSpaceDropPod;
        public static ThingDef SpaceDropPodIncoming;
        public static ThingDef StellarisResourceDeuterium;
        public static ThingDef StellarisResourceRareMetal;
        public static ThingDef StellarisResourceHelium;
        public static ThingDef StellarisResourceExoticCrystals;
        public static ThingDef StellarisPlanetScanner;
        public static JobDef StellarisPlanetScanJob;
        public static PlanetLayerSettingsDef StellarisSpaceLayerSetting;
        public static PlanetLayerDef StellarisSpaceLayer;
        public static MapGeneratorDef StellarisSpace;
        public static ThingDef StellarisShipConsole;
        public static JobDef  StellarisShipConsoleJob;
        public static SitePartDef StellarisArchaeologicalSite;
        public static GenStepDef StellarisGenStepArchaeologicalSite;
    }
}
