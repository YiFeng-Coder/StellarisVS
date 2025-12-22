using LudeonTK;
using RimWorld;
using Stellaris.PlanetTravel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Verse;

namespace Stellaris.DevTools
{
    public class AreaSaver
    {
        [DebugAction("Stellaris Tools", "Save Map Data", false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void DebugSaveMap()
        {
            /*
            Map map = Find.CurrentMap;
            HashSet<IntVec3> area = map.AllCells.ToHashSet<IntVec3>();
            SaveAreaToXml(area, map, "TestSite");
            */
            ShipTransporter.CaptureAndSerializeMap(Find.CurrentMap, ShipTransporter.SITE_SNAPSHOT);
        }
        public static void SaveAreaToXml(HashSet<IntVec3> area, Map map, string filePath)
        {
            AreaSaveData saveData = new AreaSaveData();

            // 收集区域内的所有东西
            foreach (var cell in area)
            {
                if (!cell.InBounds(map)) continue;

                var thingList = map.thingGrid.ThingsListAt(cell);
                foreach (var thing in thingList)
                {
                    if (thing is Pawn pawn)
                    {
                        SavePawnData(pawn, saveData);
                    }
                    else if (thing is Building building)
                    {
                        SaveBuildingData(building, saveData);
                    }
                    else
                    {
                        SaveThingData(thing, saveData);
                    }
                }
            }

            // 序列化为XML
            var xmlDoc = SerializeToXml(saveData);
            xmlDoc.Save(filePath);
        }

        private static void SavePawnData(Pawn pawn, AreaSaveData saveData)
        {
            var pawnData = new PawnData
            {
                pawnKindDef = pawn.kindDef.defName,
                position = pawn.Position,
                faction = pawn.Faction?.def,
                name = pawn.Name?.ToStringFull ?? "Unnamed",
                posture = pawn.pather.Moving ? PawnPosture.Standing : pawn.GetPosture()
            };
            saveData.pawns.Add(pawnData);
        }

        private static void SaveBuildingData(Building building, AreaSaveData saveData)
        {
            var buildingData = new BuildingData
            {
                defName = building.def.defName,
                position = building.Position,
                rotation = building.Rotation,
                faction = building.Faction?.def,
                hitPoints = building.HitPoints,
                
            };
            if (building.Stuff != null) 
            {
                buildingData.stuffsDefName = building.Stuff.defName;
            }
            saveData.buildings.Add(buildingData);
        }

        private static void SaveThingData(Thing thing, AreaSaveData saveData)
        {
            var thingData = new ThingData
            {
                defName = thing.def.defName,
                position = thing.Position,
                stackCount = thing.stackCount,
                hitPoints = thing.HitPoints,
            };
            if (thing.Stuff != null)
            {
                thingData.stuffsDefName = thing.Stuff.defName;
            }
            saveData.things.Add(thingData);
        }

        private static XDocument SerializeToXml(AreaSaveData saveData)
        {
            var root = new XElement("AreaSaveData");

            // 保存物品
            var thingsElement = new XElement("Things");
            foreach (var thing in saveData.things)
            {
                XElement thingElement;
                if (thing.stuffsDefName != null)
                {
                        thingElement =  new XElement("Thing",
                        new XAttribute("defName", thing.defName),
                        new XAttribute("x", thing.position.x),
                        new XAttribute("z", thing.position.z),
                        new XAttribute("stackCount", thing.stackCount),
                        new XAttribute("hitPoints", thing.hitPoints),
                        new XAttribute("stuffsDefName", thing.stuffsDefName)
                    );
                }
                else
                {
                        thingElement =  new XElement("Thing",
                        new XAttribute("defName", thing.defName),
                        new XAttribute("x", thing.position.x),
                        new XAttribute("z", thing.position.z),
                        new XAttribute("stackCount", thing.stackCount),
                        new XAttribute("hitPoints", thing.hitPoints)
                        );
                }
                thingsElement.Add(thingElement);
            }
            root.Add(thingsElement);

            // 保存建筑
            var buildingsElement = new XElement("Buildings");
            foreach (var building in saveData.buildings)
            {
                XElement buildingElement;
                if (building.stuffsDefName != null)
                {
                    buildingElement = new XElement("Building",
                    new XAttribute("defName", building.defName),
                    new XAttribute("x", building.position.x),
                    new XAttribute("z", building.position.z),
                    new XAttribute("rotation", building.rotation.AsInt),
                    new XAttribute("hitPoints", building.hitPoints),
                    new XAttribute("faction", building.faction?.defName ?? "None"),
                    new XAttribute("stuffsDefName", building.stuffsDefName)
                );

                }
                else
                {
                    buildingElement = new XElement("Building",
                    new XAttribute("defName", building.defName),
                    new XAttribute("x", building.position.x),
                    new XAttribute("z", building.position.z),
                    new XAttribute("rotation", building.rotation.AsInt),
                    new XAttribute("hitPoints", building.hitPoints),
                    new XAttribute("faction", building.faction?.defName ?? "None")
                    );
                }
                    buildingsElement.Add(buildingElement);
            }
            root.Add(buildingsElement);

            // 保存殖民者和生物
            var pawnsElement = new XElement("Pawns");
            foreach (var pawn in saveData.pawns)
            {
                var pawnElement = new XElement("Pawn",
                    new XAttribute("pawnKindDef", pawn.pawnKindDef),
                    new XAttribute("x", pawn.position.x),
                    new XAttribute("z", pawn.position.z),
                    new XAttribute("faction", pawn.faction?.defName ?? "None"),
                    new XAttribute("name", pawn.name),
                    new XAttribute("posture", pawn.posture.ToString())
                );
                pawnsElement.Add(pawnElement);
            }
            root.Add(pawnsElement);

            return new XDocument(root);
        }
    }
}