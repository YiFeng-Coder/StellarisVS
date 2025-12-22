using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace Stellaris.Transfer
{
    public static class MapObjectTransfer
    {
        public static void TransferObjectsFromArea(HashSet<IntVec3> area, Map sourceMap, Map targetMap, IntVec3 targetBasePos)
        {
            if (area == null || sourceMap == null || targetMap == null)
                return;

            // 3. 转移建筑
            TransferBuildings(area, sourceMap, targetMap, targetBasePos);

            // 1. 转移物品
            TransferItems(area, sourceMap, targetMap, targetBasePos);

            // 2. 转移殖民者和动物
            TransferPawns(area, sourceMap, targetMap, targetBasePos);

            // 4. 清理源地图
            foreach (IntVec3 cell in area)
            {
                sourceMap.roofGrid.SetRoof(cell,RoofDefOf.RoofConstructed);
                sourceMap.roofGrid.RoofGridUpdate();
                sourceMap.mapDrawer.MapMeshDirty(cell, MapMeshFlagDefOf.Things);
            }
        }


        private static void TransferItems(HashSet<IntVec3> area, Map sourceMap, Map targetMap, IntVec3 targetBasePos)
        {
            List<Thing> thingsToTransfer = new List<Thing>();

            // 收集所有在区域内的物品
            foreach (IntVec3 cell in area)
            {
                if (!cell.InBounds(sourceMap)) continue;

                List<Thing> thingsInCell = sourceMap.thingGrid.ThingsListAt(cell);

                foreach (Thing thing in thingsInCell)
                {
                    if (!thingsToTransfer.Contains(thing) && !thing.def.IsFilth && !(thing is Building_SteamGeyser))
                    {
                        thingsToTransfer.Add(thing);
                    }
                }
            }

            // 转移物品
            foreach (Thing thing in thingsToTransfer)
            {
                IntVec3 relativePos = thing.Position - area.First();
                IntVec3 targetPos = targetBasePos + relativePos;

                if (targetPos.InBounds(targetMap))
                {
                    thing.DeSpawn();
                    GenSpawn.Spawn(thing, targetPos, targetMap,thing.Rotation);
                }
            }
        }

        private static void TransferPawns(HashSet<IntVec3> area, Map sourceMap, Map targetMap, IntVec3 targetBasePos)
        {
            List<Pawn> pawnsToTransfer = new List<Pawn>();

            // 收集所有在区域内的生物
            foreach (IntVec3 cell in area)
            {
                if (!cell.InBounds(sourceMap)) continue;

                List<Thing> thingsInCell = sourceMap.thingGrid.ThingsListAt(cell);
                foreach (Thing thing in thingsInCell)
                {
                    if (thing is Pawn pawn && !pawnsToTransfer.Contains(pawn))
                    {
                        pawnsToTransfer.Add(pawn);
                    }
                }
            }

            // 转移生物
            foreach (Pawn pawn in pawnsToTransfer)
            {
                IntVec3 relativePos = pawn.Position - area.First();
                IntVec3 targetPos = targetBasePos + relativePos;
                // 将pawn从源地图移除
                pawn.DeSpawn();
                // 添加到目标地图\

                GenSpawn.Spawn(pawn, targetPos, targetMap);
            }
        }

        private static void TransferBuildings(HashSet<IntVec3> area, Map sourceMap, Map targetMap, IntVec3 targetBasePos)
        {
            List<Building> buildingsToTransfer = new List<Building>();

            // 收集所有在区域内的建筑
            foreach (IntVec3 cell in area)
            {
                if (!cell.InBounds(sourceMap)) continue;

                Building building = cell.GetEdifice(sourceMap);
                if (building != null && !buildingsToTransfer.Contains(building))
                {
                    buildingsToTransfer.Add(building);
                }
            }
            Log.Message("Building Count: " + buildingsToTransfer.Count);
            // 转移建筑
            foreach (Building building in buildingsToTransfer)
            {
                IntVec3 relativePos = building.Position - area.First();
                IntVec3 targetPos = targetBasePos + relativePos;

                if (CanPlaceBuildingAt(building, targetPos, targetMap))
                {
                    building.DeSpawn();
                    GenSpawn.Spawn(building,targetPos,targetMap,building.Rotation);
                }
            }
        }

        private static bool CanPlaceBuildingAt(Building building, IntVec3 pos, Map map)
        {
            return true;
            //return GenConstruct.CanPlaceBlueprintAt(building.def, pos, Rot4.North, map, false);
        }

        private static void TransferBuildingContents(Building source, Building destination, Map targetMap)
        {
            // 处理特殊建筑类型的内容物转移


            // 可以添加其他特殊建筑类型的处理逻辑
        }
    }
}
