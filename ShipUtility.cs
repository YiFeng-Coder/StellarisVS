using RimWorld;
using RimWorld.Planet;
using Stellaris.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stellaris
{
    // 舰船工具类
    public static class ShipUtility
    {
        // 修改后的舰船边界计算函数
        public static ShipRegion CalculateShipRegion(IntVec3 startPos, Map map)
        {
            ShipRegion region = new ShipRegion();

            // 使用洪水填充算法找到所有相连的舰船部件
            var visited = new HashSet<IntVec3>();
            var toVisit = new Queue<IntVec3>();
            toVisit.Enqueue(startPos);

            while (toVisit.Count > 0)
            {
                IntVec3 current = toVisit.Dequeue();
                if (!visited.Add(current)) continue;

                // 添加到舰船区域
                region.allCells.Add(current);

                // 检查所有方向的相邻单元格
                foreach (IntVec3 direction in GenAdj.CardinalDirections)
                {
                    IntVec3 neighbor = current + direction;

                    if (neighbor.InBounds(map) &&
                        !visited.Contains(neighbor) &&
                        IsConnectedShipPart(map, neighbor, current))
                    {
                        toVisit.Enqueue(neighbor);
                    }
                }

                // 检查对角线方向（确保复杂的舰船形状也能正确识别）
                foreach (IntVec3 direction in GenAdj.DiagonalDirections)
                {
                    IntVec3 neighbor = current + direction;

                    if (neighbor.InBounds(map) &&
                        !visited.Contains(neighbor) &&
                        IsDiagonalConnected(map, current, neighbor))
                    {
                        toVisit.Enqueue(neighbor);
                    }
                }
            }

            region.CalculateBounds();
            return region;
        }

        // 检查是否是相连的舰船部件
        private static bool IsConnectedShipPart(Map map, IntVec3 pos, IntVec3 fromPos)
        {
            // 检查目标位置是否有舰船部件
            if (!HasShipPart(map, pos))
                return false;

            // 检查从原位置到目标位置是否可以通过（不是被墙完全隔开）
            return true;
        }

        // 检查对角线连接
        private static bool IsDiagonalConnected(Map map, IntVec3 fromPos, IntVec3 toPos)
        {
            if (!HasShipPart(map, toPos))
                return false;

            // 检查两个相邻的直角方向是否都有通道
            IntVec3 horizontal = new IntVec3(toPos.x, 0, fromPos.z);
            IntVec3 vertical = new IntVec3(fromPos.x, 0, toPos.z);

            return (HasShipPart(map, horizontal)&&
                   (HasShipPart(map, vertical)));
        }

        // 检查是否可以穿越两个位置之间
        private static bool CanTraverseBetween(Map map, IntVec3 fromPos, IntVec3 toPos)
        {
            // 如果目标位置有真空气闸，需要特殊处理
            Thing thingAtTo = map.edificeGrid[toPos];
            if (thingAtTo != null && thingAtTo.def.defName == "ShipAirlock")
            {
                // 真空气闸可以作为连接点
                return true;
            }

            // 检查原位置和目标位置之间的建筑是否会阻挡连接
            // 这里简化处理，实际可能需要更复杂的路径检查
            return true;
        }

        // 检查位置是否有舰船部件
        private static bool HasShipPart(Map map, IntVec3 pos)
        {
            if (!pos.InBounds(map))
                return false;

            //Thing thing = map.edificeGrid[pos];
            List<Thing> things = map.thingGrid.ThingsListAt(pos);
            bool flag = false;
            if (things != null && !things.Empty())
            {
                foreach (var item in things)
                {
                    if (IsShipBuilding(item) || item.def == StellarisDefOf.StellarisShipHullTile)
                    {
                        flag = true;
                    }
                }
            }

            return flag;
        }

        // 检查位置是否有阻挡性的舰船部件（如完整的墙）
        private static bool IsBlockingShipPart(Map map, IntVec3 pos)
        {
            if (!pos.InBounds(map))
                return true; // 地图边界视为阻挡

            Thing thing = map.edificeGrid[pos];
            if (thing == null)
                return false; // 空位置不阻挡

            // 完整的船体墙会阻挡连接
            return thing.def.defName == "ShipHullWall" && thing.HitPoints == thing.MaxHitPoints;
        }

        // 修改发射舰船函数
        public static bool  TryLaunchShip(Map map, IntVec3 controllerPos , bool force = false)
        {
            // 计算舰船区域
            ShipRegion shipRegion = CalculateShipRegion(controllerPos, map);
            
            if (shipRegion.allCells.Count == 0)
            {
                Messages.Message("StellarisInvalidShipRegion".Translate(), MessageTypeDefOf.RejectInput);
                return false;
            }
            if (!force)
            {
                // 检查舰船是否完整
                if (!IsShipComplete(map, shipRegion))
                {
                    Messages.Message("StellarisShipIsNotComplete".Translate(), MessageTypeDefOf.RejectInput);
                    return false;
                }
            }

            LaunchShip(map,controllerPos,shipRegion);

            return true;
        }
        public static WorldShip MakeWorldShip(Faction shipFaction, ShipRegion shipRegion = null)
        {
            WorldShip worldShip = (WorldShip)WorldObjectMaker.MakeWorldObject(StellarisDefOf.UniverseMapParent_Ship);

            if (Find.WorldGrid.FirstLayerOfDef(StellarisDefOf.StellarisSpaceLayer) == null)
            {
                Log.Error("StellarisSpaceLayer is null");
            }
            PlanetTile planetTile = Find.WorldGrid.FirstLayerOfDef(StellarisDefOf.StellarisSpaceLayer)[0].tile;
            planetTile.Tile.PrimaryBiome = BiomeDefOf.Orbit;
            planetTile.Tile.temperature = -270f;
            planetTile.Tile.rainfall = 0f;
            planetTile.Tile.swampiness = 0f;
            planetTile.Tile.pollution = 0f;
            
            worldShip.Tile = planetTile;


            worldShip.SetFaction(shipFaction);
            worldShip.shipRegion = shipRegion;
            WorldShip.playerShip = worldShip;
            if (shipFaction == Faction.OfPlayer)
            {
                WorldShip.playerShip = worldShip;
                if (WorldShip.isFirstLaunch)
                {
                    worldShip.starSystem = GalaxyCluster.initialSystem;
                    worldShip.planet = GalaxyCluster.initialPlanet;
                    Current.Game.GetComponent<GalaxyComponent>().ClusterData.universeObjects.Add(worldShip);
                    worldShip.starSystem.universeObjects.Add(worldShip);
                    ExplorationManager.planetPlayerAt.universeObjects.Add(worldShip);
                }
            }

            return worldShip;
        }
        public static void LaunchShip(Map map, IntVec3 controllerPos,ShipRegion shipRegion)
        {
            // 创建太空世界对象
            WorldShip worldShip = MakeWorldShip(Faction.OfPlayer,shipRegion);
            // 转移到太空
            TransferToSpace(map, worldShip, shipRegion);

            // 添加世界对象
            Find.WorldObjects.Add(worldShip);
            //$"舰船成功发射到太空，包含 {shipRegion.allCells.Count} 个部件"
            Messages.Message("StellarisShipLaunchSuccessfully".Translate(shipRegion.allCells.Count), MessageTypeDefOf.PositiveEvent);
        }
        // 修改完整性检查函数
        private static bool IsShipComplete(Map map, ShipRegion shipRegion)
        {
            bool hasController = false;
            bool hasThruster = false;

            foreach (IntVec3 cell in shipRegion.allCells)
            {
                Thing thing = map.edificeGrid[cell];
                //Log.Message(cell.x+"  "+cell.y+ "  " + cell.z);
                
                if (thing != null)
                {
                    if (thing.HasComp<CompShipControl>())
                    {
                        //Log.Message("hasController");
                        hasController = true;
                    }
                    if (thing.HasComp<CompShipThruster>())
                    {                        
                        //Log.Message("hasThruster");
                        hasThruster = true;
                    }

                }

                if (hasController && hasThruster)
                    return true;
            }

            return false;
        }

        // 太空转移函数
        public static void TransferToSpace(Map sourceMap, WorldShip worldShip, ShipRegion shipRegion)
        {
            if (worldShip.Map == null)
            {
                ArriveNewMap(sourceMap,worldShip,shipRegion);
            }
        }

        public static void ArriveNewMap(Map sourceMap, WorldShip worldShip, ShipRegion shipRegion, bool isLanding = false, Tile TargetTile = null)
        {
            // 想要真空环境，必须要把Biome改成真空的，就要改Tile的 他妈的
            // 成了 加了个Layer 老子真聪明
            if (!isLanding)
            {
                Map mapGenerated = MapGenerator.GenerateMap(new IntVec3(250, 1, 250), worldShip, StellarisDefOf.StellarisSpace);
                MapObjectTransfer.TransferObjectsFromArea(shipRegion.allCells, sourceMap, mapGenerated, shipRegion.allCells.First());

                if (Prefs.PauseOnLoad)
                {
                    Find.TickManager.DoSingleTick();
                    Find.TickManager.CurTimeSpeed = TimeSpeed.Paused;
                }
                CameraJumper.TryJump(worldShip, CameraJumper.MovementMode.Cut);
                Find.CameraDriver.shaker.DoShake(0.1f, 180);
            }
            else if (TargetTile != null)
            {
                MapParent mapParent = Find.WorldObjects.MapParentAt(TargetTile.tile);
                if (mapParent == null)
                {
                    if (TargetTile.tile.LayerDef.DefaultWorldObject == TargetTile.tile.LayerDef.SettlementWorldObjectDef)
                    {
                        mapParent = SettleUtility.AddNewHome(TargetTile.tile, Faction.OfPlayer);
                    }
                    else
                    {
                        mapParent = (MapParent)WorldObjectMaker.MakeWorldObject(TargetTile.tile.LayerDef.DefaultWorldObject);
                        mapParent.Tile = TargetTile.tile;
                        Find.WorldObjects.Add(mapParent);
                    }
                    if (mapParent.def.canHaveFaction && mapParent.Faction == null)
                    {
                        mapParent.SetFaction(Faction.OfPlayer);
                    }
                    mapParent.Tile = TargetTile.tile;
                    Settlement settlement = mapParent as Settlement;
                    if (settlement != null)
                    {
                        settlement.Name = "GravshipLandingSite".Translate("Ship").CapitalizeFirst();
                        settlement.namedByPlayer = true;
                    }
                }
                else if (mapParent.def.canHaveFaction && mapParent.Faction == null)
                {
                    mapParent.SetFaction(Faction.OfPlayer);
                }
                IntVec3 intVec = Find.World.info.initialMapSize;
                if (mapParent.def.overrideMapSize != null)
                {
                    intVec = mapParent.def.overrideMapSize.Value;
                }
                Site site = mapParent as Site;
                if (site != null)
                {
                    IntVec3 preferredMapSize = site.PreferredMapSize;
                    intVec = new IntVec3(Mathf.Max(intVec.x, preferredMapSize.x), Mathf.Max(intVec.y, preferredMapSize.y), Mathf.Max(intVec.z, preferredMapSize.z));
                    if (site.MainSitePartDef.minMapSize != null)
                    {
                        IntVec3 value = site.MainSitePartDef.minMapSize.Value;
                        intVec = new IntVec3(Mathf.Max(intVec.x, value.x), Mathf.Max(intVec.y, value.y), Mathf.Max(intVec.z, value.z));
                    }
                }
                Map targetMap = GetOrGenerateMapUtility.GetOrGenerateMap(TargetTile.tile, intVec, mapParent.def);
                ShipRegion copyRegion = shipRegion.DeepCopy();
                ShipMapComp shipMapComp = targetMap.GetComponent<ShipMapComp>();
                shipMapComp.isLanding = true;
                shipMapComp.cachedShipRegion = copyRegion;
                shipMapComp.landingAction = delegate { MapObjectTransfer.TransferObjectsFromArea(shipRegion.allCells, sourceMap, targetMap, copyRegion.allCells.First()); };
                CameraJumper.TryJump(targetMap.spawnedThings.First());
            }
            else
            {
                Log.Error("TargetTile is null");
            }
        }

        // 计算目标偏移量
        private static IntVec3 CalculateTargetOffset(Map sourceMap, Map targetMap, ShipRegion shipRegion)
        {
            // 将舰船区域放置在目标地图中心
            return new IntVec3(
                (targetMap.Size.x - shipRegion.boundingRect.Width) / 2 - shipRegion.boundingRect.minX,
                0,
                (targetMap.Size.z - shipRegion.boundingRect.Height) / 2 - shipRegion.boundingRect.minZ
            );
        }

        // 检查是否是舰船建筑
        private static bool IsShipBuilding(Thing thing)
        {
            if (thing.def.tradeTags != null)
            {
                //Log.Message("IsShipBuilding: thing.def.tradeTags != null");
                if (thing.HasComp<Comp_ShipPart>())
                {
                    //Log.Message("IsShipBuilding: thing.def.tradeTags.Contains(\"StellarisShip\")");
                    return true;
                }
                //Log.Message("IsShipBuilding: " + thing.def.tradeTags.First());
            }
            return false;
        }

        public static bool IsRoofDefAirtight(RoofDef roof)
        {
            if (roof == StellarisDefOf.StellarisShipRoof)
            {
                return true;
            }
            return false;
        }

        public static void LandShip(WorldShip worldShip , Thing console)
        {
            worldShip.UpdateShipRegion(console.Position);
            CameraJumper.TryJump(CameraJumper.GetWorldTarget(console), CameraJumper.MovementMode.Pan);
            Find.WorldSelector.ClearSelection();
            PlanetTile curTile = console.Map.Tile;
            PlanetLayer curLayer = console.Map.Tile.Layer;
            PlanetTile cachedClosestLayerTile = PlanetTile.Invalid;
            TilePicker tilePicker = Find.TilePicker;
            CompShipControl compConsole = console.TryGetComp<CompShipControl>();
            float fuelToCost = CalculateShipRegion(console.Position,console.Map).allCells.Count;
            Func<PlanetTile, bool> validator = delegate (PlanetTile tile)
            {
                if (!compConsole.HasEnoughFuel(fuelToCost))
                {
                    Messages.Message("CannotLaunchNotEnoughFuel".Translate().CapitalizeFirst(), MessageTypeDefOf.RejectInput, false);
                    return false;
                }
                if (!TileFinder.IsValidTileForNewSettlement(tile, null, true))
                {
                    Messages.Message("CannotLandOnInvalidTile".Translate(), MessageTypeDefOf.RejectInput, false);
                    return false;
                }
                return true;
            };
            Action<PlanetTile> tileChosen = delegate (PlanetTile tile)
            {
                LongEventHandler.QueueLongEvent(delegate
                {
                    Log.Message("Tile Chosen");
                    ShipRegion shipRegion = CalculateShipRegion(console.Position, console.Map);
                    PlanetTile tile2 = tile;
                    ArriveNewMap(console.Map, WorldShip.playerShip, shipRegion, true, tile.Tile);
                    Action settleAction = delegate ()
                    {
                        Find.World.renderer.wantedMode = WorldRenderMode.None;
                        compConsole.ConsumeFuelDirectly(fuelToCost);
                        SoundDefOf.Gravship_Launch.PlayOneShotOnCamera(null);
                    };
                }, "GeneratingMap", true, null);
            };
            Action onGuiAction = delegate ()
            {
                WorldObject singleSelectedObject = Find.WorldSelector.SingleSelectedObject;
                PlanetTile planetTile = GenWorld.MouseTile(false);
                PlanetTile planetTile2 = (!planetTile.Valid && singleSelectedObject != null) ? singleSelectedObject.Tile : planetTile;
                Vector2 mousePosition = Event.current.mousePosition;
                GUI.DrawTexture(new Rect(mousePosition.x + 8f, mousePosition.y + 8f, 32f, 32f), ContentFinder<Texture2D>.Get("UI/Overlays/LaunchableMouseAttachment", true));
                if (planetTile2.Valid)
                {
                    bool flag = false; 
                    PlanetTile tileA = (curTile.Layer == planetTile2.Layer) ? curTile : planetTile2.Layer.GetClosestTile_NewTemp(curTile, false);
                    string text =  "StellarisLaunch".Translate();
                    if (singleSelectedObject != null && !planetTile.Valid)
                    {
                        Widgets.WorldAttachedLabel(singleSelectedObject.DrawPos, text, 0f, 0f, new Color?(flag ? Color.white : ColorLibrary.White));
                        return;
                    }
                    Widgets.MouseAttachedLabel(text, 0f, 0f, new Color?(flag ? Color.white : ColorLibrary.White));
                }
            };
            Action onUpdateAction = delegate ()
            {
            };
            Action noTileChosen = delegate ()
            {
                CameraJumper.TryJump(console, CameraJumper.MovementMode.Cut);
            };
            string title = "ChooseWhereToLand".Translate();
            bool showRandomButton = false;
            bool selectTileBehindObject = true;
            bool hideFormCaravanGizmo = true;
            string noTileChosenMessage = "MessageNoLandingSiteSelected".Translate();
            tilePicker.StartTargeting_NewTemp(validator, tileChosen, onGuiAction, onUpdateAction, true, noTileChosen, title, showRandomButton, selectTileBehindObject, hideFormCaravanGizmo, true, true, noTileChosenMessage);
        }
    }
}
