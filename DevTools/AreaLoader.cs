using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Verse;

namespace Stellaris.DevTools
{
    public class AreaLoader
    {
        public static string pathToLoadShipFile = @"..\ShipsData\";
        public static string pathToLoadSiteFile = @"..\SiteMapData\";
        public static void LoadAreaFromXml(string fileName, Map map, IntVec3 offset,bool isSite = true)
        {
            string filePath;
            if (isSite)
            {
                filePath = pathToLoadSiteFile + fileName;
            }
            else
            {
                filePath = pathToLoadShipFile + fileName;
            }
            if (!File.Exists(filePath))
            {
                Log.Error($"Area save file not found: {filePath}");
                return;
            }

            var xmlDoc = XDocument.Load(filePath);
            var root = xmlDoc.Element("AreaSaveData");

            if (root == null)
            {
                Log.Error("Invalid area save file format");
                return;
            }

            // 加载物品
            var thingsElement = root.Element("Things");
            if (thingsElement != null)
            {
                foreach (var thingElement in thingsElement.Elements("Thing"))
                {
                    LoadThing(thingElement, map, offset);
                }
            }

            // 加载建筑
            var buildingsElement = root.Element("Buildings");
            if (buildingsElement != null)
            {
                foreach (var buildingElement in buildingsElement.Elements("Building"))
                {
                    LoadBuilding(buildingElement, map, offset);
                }
            }

            // 加载生物
            var pawnsElement = root.Element("Pawns");
            if (pawnsElement != null)
            {
                foreach (var pawnElement in pawnsElement.Elements("Pawn"))
                {
                    LoadPawn(pawnElement, map, offset);
                }
            }
        }

        private static void LoadThing(XElement thingElement, Map map, IntVec3 offset)
        {
            var defName = thingElement.Attribute("defName")?.Value;
            if (defName == null) return;

            var thingDef = DefDatabase<ThingDef>.GetNamedSilentFail(defName);
            if (thingDef == null) return;

            var x = int.Parse(thingElement.Attribute("x").Value) + offset.x;
            var z = int.Parse(thingElement.Attribute("z").Value) + offset.z;
            var position = new IntVec3(x, 0, z);
            string stuffsDefName = null;
            if (thingElement.Attribute("stuffsDefName") != null)
            {
                stuffsDefName = thingElement.Attribute("stuffsDefName").Value;
            }
            ThingDef stuff = null;
            if (stuffsDefName != null)
            {
                stuff = DefDatabase<ThingDef>.GetNamed(stuffsDefName);
            }
            Thing thing;
            if (stuff != null)
            {
                thing = ThingMaker.MakeThing(thingDef,stuff);
            }
            else
            {
                thing = ThingMaker.MakeThing(thingDef);
            }
            thing.stackCount = int.Parse(thingElement.Attribute("stackCount")?.Value ?? "1");

            GenSpawn.Spawn(thing, position, map);
        }

        private static void LoadBuilding(XElement buildingElement, Map map, IntVec3 offset)
        {
            var defName = buildingElement.Attribute("defName")?.Value;
            if (defName == null) return;

            var buildingDef = DefDatabase<ThingDef>.GetNamedSilentFail(defName);
            if (buildingDef == null || !buildingDef.IsBuildingArtificial) return;

            var x = int.Parse(buildingElement.Attribute("x").Value) + offset.x;
            var z = int.Parse(buildingElement.Attribute("z").Value) + offset.z;
            var position = new IntVec3(x, 0, z);
            var rotation = new Rot4(int.Parse(buildingElement.Attribute("rotation")?.Value ?? "0"));
            
            var factionDefName = buildingElement.Attribute("faction")?.Value;
            Faction faction = null;
            if (factionDefName != "None")
            {
                var factionDef = DefDatabase<FactionDef>.GetNamedSilentFail(factionDefName);
                if (factionDef != null)
                {
                    faction = Find.FactionManager.FirstFactionOfDef(factionDef);
                }
            }
            string stuffsDefName= null;
            if (buildingElement.Attribute("stuffsDefName") != null)
            {

                stuffsDefName = buildingElement.Attribute("stuffsDefName").Value;
            }
            ThingDef stuff = null;
            if (stuffsDefName != null)
            {
                stuff = DefDatabase<ThingDef>.GetNamed(stuffsDefName);
            }
            Building building;
            if (stuff != null)
            {
                building = (Building)ThingMaker.MakeThing(buildingDef, stuff);
            }
            else
            {
                building = (Building)ThingMaker.MakeThing(buildingDef);
            }
            building.SetFactionDirect(faction);

            GenSpawn.Spawn(building, position, map, rotation);
        }

        private static void LoadPawn(XElement pawnElement, Map map, IntVec3 offset)
        {
            var pawnKindDefName = pawnElement.Attribute("pawnKindDef")?.Value;
            if (pawnKindDefName == null) return;

            var pawnKindDef = DefDatabase<PawnKindDef>.GetNamedSilentFail(pawnKindDefName);
            if (pawnKindDef == null) return;

            var x = int.Parse(pawnElement.Attribute("x").Value) + offset.x;
            var z = int.Parse(pawnElement.Attribute("z").Value) + offset.z;
            var position = new IntVec3(x, 0, z);

            var factionDefName = pawnElement.Attribute("faction")?.Value;
            Faction faction = null;
            if (factionDefName != "None")
            {
                var factionDef = DefDatabase<FactionDef>.GetNamedSilentFail(factionDefName);
                if (factionDef != null)
                {
                    faction = Find.FactionManager.FirstFactionOfDef(factionDef);
                }
            }

            var pawn = PawnGenerator.GeneratePawn(pawnKindDef, faction);
            GenSpawn.Spawn(pawn, position, map);
        }
    }
}
