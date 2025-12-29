using LudeonTK;
using RimWorld;
using RimWorld.Planet;
using Stellaris.PlanetTravel;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Profile;

namespace Stellaris.PlanetTravel
{
    public static class PlanetSwitchService
    {
        //[DebugAction("Stellaris Tools", "Wrap To Original Planet", false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void DebugWrapToOriginPlanet()
        {
            WarpToKnownPlanet("Origin_Planet", WorldShip.playerShip.Map.mapPawns.AllPawns);
        }
        [DebugAction("Stellaris Tools", "Wrap To New Planet", false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void DebugWrapToNewPlanet()
        {
            WarpToNewPlanet(WorldShip.playerShip.Map.mapPawns.AllPawns,GalaxyCluster.initialSystem.TryRamdonPlanet());
        }
        /// <summary>
        /// 前往一个全新的未知行星
        /// </summary>
        public static void WarpToNewPlanet(List<Pawn> travelers , Planet planet)
        {
            StellarisGlobalState.IsSwitchingPlanets = true;
            StellarisGlobalState.SwitchingPlanetType = planet.type;
            // 1. 保存当前行星的状态

            ShipTransporter.CaptureAndSerializeMap(WorldShip.playerShip.Map);
            string currentSaveName = $"Stellaris_Planet_{HyperspaceCache.CurrentPlanetId}";
            SaveCurrentGame(currentSaveName);
            // 2. 将旅行者存入静态缓存区（进入超空间） 
            HyperspaceCache.StoreTravelers(travelers);

            // 3. 生成新的行星ID
            string newPlanetId = System.Guid.NewGuid().ToString().Substring(0, 8);
            HyperspaceCache.CurrentPlanetId = newPlanetId;

            // 4. 将旅行者从当前地图彻底移除（避免ID冲突，虽然后面会销毁世界，但这是好习惯）
            
            for (int i = travelers.Count-1; i >= 0; i--)
            {
                Pawn p = travelers[i];
                if (p.Spawned) p.DeSpawn();
            }
            
            // 5. 强制生成新世界
            // 警告：这将清除当前游戏内存
            GenerateNewWorldAndLand(planet);
        }

        /// <summary>
        /// 回到已知的行星
        /// </summary>
        public static void WarpToKnownPlanet(string targetPlanetId, List<Pawn> travelers)
        {
            // 1. 保存离开时的行星
            string currentSaveName = $"Stellaris_Planet_{HyperspaceCache.CurrentPlanetId}";
            SaveCurrentGame(currentSaveName);

            // 2. 缓存旅行者和地图
            HyperspaceCache.StoreTravelers(travelers);
            for (int i = travelers.Count - 1; i >= 0; i--)
            {
                Pawn p = travelers[i];
                if (p.Spawned) p.DeSpawn();
            }
            ShipTransporter.CaptureAndSerializeMap(Find.CurrentMap);

            // 3. 切换ID
            HyperspaceCache.CurrentPlanetId = targetPlanetId;

            // 4. 加载目标行星的存档
            string targetSaveName = $"Stellaris_Planet_{targetPlanetId}";
            LoadPlanetSave(targetSaveName);
        }

        private static void SaveCurrentGame(string fileName)
        {
            GameDataSaveLoader.SaveGame(fileName);
        }
        private static void GenerateNewWorldAndLand(Planet planet)
        {
            ExplorationManager.planetPlayerAt.universeObjects.Remove(WorldShip.playerShip);
            ExplorationManager.planetPlayerAt = planet;
            planet.universeObjects.Add(WorldShip.playerShip);
            // 如果读取到的尺寸太小（说明未正确保存），则强制使用标准大地图尺寸
            if (StellarisGlobalState.SavedMapSize.x < 50 || StellarisGlobalState.SavedMapSize.z < 50)
            {
                Log.Warning("[Stellaris] Map size not found, defaulting to 250x250.");
                StellarisGlobalState.SavedMapSize = new IntVec3(250, 1, 250);
            }
            // --- 阶段 1：数据备份 ---

            // 1. 备份并清理剧本 (解决报错二)
            // 我们不需要"着陆时的初始物资/宠物"，因为我们是带着飞船来的 
            //通过切换剧本实现旅行至指定星球。

            if (GalaxyCluster.initialScenario == null)
            {
                GalaxyCluster.initialScenario = Current.Game.Scenario;
            }
            Scenario savedScenario;
            if (planet != GalaxyCluster.initialPlanet)
            {
                savedScenario = ScenarioLister.AllScenarios().Where(x => x.name == "StellarisPlanetTravel").First();
            }
            else
            {
                savedScenario = GalaxyCluster.initialScenario;
            }
            StorytellerDef savedStorytellerDef = Find.Storyteller.def;
            DifficultyDef savedDifficulty = Find.Storyteller.difficultyDef;
            Difficulty savedDifficultySettings = Find.Storyteller.difficulty;

            // 2. 保存关键状态
            StellarisGlobalState.SavedGameAbsTick = Find.TickManager.gameStartAbsTick;
            StellarisGlobalState.SavedMapSize = Find.CurrentMap.Size;
            StellarisGlobalState.SavedGameTicksInt = Find.TickManager.TicksGame;
            // 4. 序列化地图物体
            ShipTransporter.CaptureAndSerializeMap(Find.CurrentMap);

            // --- 阶段 2：执行跳跃 (耗时操作) ---
            LongEventHandler.QueueLongEvent(() =>
            {
                // 1. 初始化游戏核心
                Current.Game = new Game();
                Current.Game.InitData = new GameInitData();
                Current.Game.Scenario = savedScenario;
                Current.Game.storyteller = new Storyteller(savedStorytellerDef, savedDifficulty);
                Current.Game.storyteller.difficulty = savedDifficultySettings;
                // 在 new Game() 之后立即调用：
                if (Current.Game != null && Current.Game.tutor != null)
                {
                    // 强制关闭教学模式，这是Mod整合包常见的稳定手段
                    Prefs.AdaptiveTrainingEnabled = false;
                }
                // 恢复时间

                Current.Game.tickManager.gameStartAbsTick = StellarisGlobalState.SavedGameAbsTick;
                //Current.Game.tickManager.DebugSetTicksGame(StellarisGlobalState.SavedGameTicksInt);
                // 2. 生成世界
                Current.Game.World = WorldGenerator.GenerateWorld(0.3f, planet.name, OverallRainfall.Normal, OverallTemperature.Normal, OverallPopulation.Normal, LandmarkDensity.Normal);
                Current.Game.World.FinalizeInit(false);

                //Current.Game.tickManager.gameStartAbsTick = StellarisGlobalState.SavedGameAbsTick;
                //Current.Game.tickManager.DebugSetTicksGame(StellarisGlobalState.SavedGameTicksInt);
                // 创建地图父对象
                WorldShip worldShip = ShipUtility.MakeWorldShip(Faction.OfPlayer);
                Find.WorldObjects.Add(worldShip);
                /*
                MapParent newSettlement = (MapParent)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
                newSettlement.Tile = tile;
                newSettlement.SetFaction(Faction.OfPlayer);
                Find.WorldObjects.Add(newSettlement);
                */
                // 4. 生成新地图
                Map newMap = MapGenerator.GenerateMap(
                    StellarisGlobalState.SavedMapSize,
                    worldShip,
                    StellarisDefOf.StellarisSpace,
                    worldShip.ExtraGenStepDefs,
                    null
                );
                if (!Find.Maps.Contains(newMap))
                {
                    Find.Maps.Add(newMap);
                }
                // 5. [优化] 快速清理地形
                // 批量获取需要销毁的物体，避免一边遍历一边销毁导致性能问题
                // 只销毁阻挡飞船的特定层级（比如植物、废墟），保留地形本身
                List<Thing> thingsToDestroy = new List<Thing>();
                foreach (Thing t in newMap.listerThings.AllThings)
                {
                    // 只有可被保存的非Pawn物体才需要被清理（给飞船腾位置）
                    if (t.def.isSaveable && !t.Destroyed)
                    {
                        thingsToDestroy.Add(t);
                    }
                }
                foreach (var t in thingsToDestroy)
                {
                    if (!t.Destroyed) t.Destroy(DestroyMode.Vanish);
                }

                // --- 阶段 3：重建 (性能瓶颈修复) ---
                /*
                // [核心性能修复] 暂时禁用所有自动更新，防止每放置一块墙就计算一次房间
                bool oldRoomUpdates = newMap.regionAndRoomUpdater.Enabled;
                // 还可以通过反射禁用 GlowGrid 等，但 RoomUpdater 是最大的卡顿源
                newMap.regionAndRoomUpdater.Enabled = false;
                */
                try
                {
                    // A. 重建飞船
                    ShipTransporter.DeserializeAndReconstruct(newMap);
                    /*
                    // B. 放置人员 (使用快速放置)
                    IntVec3 center = newMap.Center;
                    Faction newPlayerFaction = Faction.OfPlayer;

                    foreach (var p in StellarisGlobalState.SavedTravelers)
                    {
                        try
                        {
                            if (p == null)
                            {
                                Log.Message("NULL pawn in saved travelers.");
                                continue;
                            }
                            if (!p.DestroyedOrNull())
                            {
                                if (p.Faction != null && p.Faction.IsPlayer) p.SetFaction(newPlayerFaction);

                                IntVec3 safePos = p.Position;
                                if (!safePos.InBounds(newMap)) safePos = center;

                                // 直接生成，内部会自动处理
                                GenSpawn.Spawn(p, safePos, newMap);
                            }
                        }
                        catch (System.Exception ex) { Log.Warning($"Warp pawn issue: {ex.Message}"); }
                    }
                    */
                }
                finally
                {
                    /*
                    // [恢复] 重新启用并强制刷新一次
                    newMap.regionAndRoomUpdater.Enabled = oldRoomUpdates;
                    newMap.regionAndRoomUpdater.RebuildAllRegionsAndRooms();

                    // 强制刷新电力网和温度
                    newMap.powerNetManager.UpdatePowerNetsAndConnections_First();
                    newMap.mapTemperature.TemperatureUpdate();
                    */
                }

                // 清理缓存
                StellarisGlobalState.SavedTravelers.Clear();
                //Log.Message("Wrap Jump Completed NODE A");
                // --- 阶段 4：启动 ---
                Current.ProgramState = ProgramState.Playing;
                Current.Game.CurrentMap = newMap;

                Find.CameraDriver.SetRootPosAndSize(newMap.Center.ToVector3Shifted(), Find.CameraDriver.RootSize);
                //Find.WindowStack.Add(new MainTabWindow_Inspect());
                Find.Scenario.PostGameStart();

                // 最后重新生成迷雾，保证视觉正常
                //FloodFillerFog.DebugRefogMap(newMap);

                Log.Message($"[Stellaris] Warp Jump Complete in {(Find.TickManager.TicksGame - StellarisGlobalState.SavedGameAbsTick)} ticks.");
                StellarisGlobalState.IsSwitchingPlanets = false;
            }, "GeneratingNewPlanet", true, null);
        }

        private static void LoadPlanetSave(string saveName)
        {
            // 使用原版的加载逻辑，但在加载完成后注入我们的回调
            GameDataSaveLoader.LoadGame(saveName);

            LongEventHandler.QueueLongEvent(() =>
            {
                // 存档加载完毕后的回调
                WorldShip worldShip;
                List<WorldObject> objects = Find.WorldObjects.AllWorldObjectsOnLayer(Find.WorldGrid.FirstLayerOfDef(StellarisDefOf.StellarisSpaceLayer)).Where(x => x is WorldShip && x.Faction.IsPlayer).ToList();
                if (objects.Any())
                {
                    worldShip = (WorldShip)objects.First();
                    WorldShip.playerShip = worldShip;
                }
                else
                {
                    worldShip = ShipUtility.MakeWorldShip(Faction.OfPlayer);
                }
                Map map = MapGenerator.GenerateMap(
                    StellarisGlobalState.SavedMapSize,
                    worldShip,
                    StellarisDefOf.StellarisSpace,
                    worldShip.ExtraGenStepDefs,
                    null
                );
                List<Pawn> returningColonists = HyperspaceCache.RetrieveTravelers(map);
                ShipTransporter.DeserializeAndReconstruct(map);
                // 清理缓存
                StellarisGlobalState.SavedTravelers.Clear();
                //Log.Message("Wrap Jump Completed NODE A");
                // --- 阶段 4：启动 ---
                Current.ProgramState = ProgramState.Playing;
                Current.Game.CurrentMap = map;

                Find.CameraDriver.SetRootPosAndSize(map.Center.ToVector3Shifted(), Find.CameraDriver.RootSize);
                //Find.WindowStack.Add(new MainTabWindow_Inspect());
                Find.Scenario.PostGameStart();

                // 最后重新生成迷雾，保证视觉正常
                //FloodFillerFog.DebugRefogMap(newMap);

                Log.Message($"[Stellaris] Warp Jump Complete in {(Find.TickManager.TicksGame - StellarisGlobalState.SavedGameAbsTick)} ticks.");
                Messages.Message("飞船已返回该行星轨道。", MessageTypeDefOf.PositiveEvent);
            }, "LandingProcess", false, null);
        }
    }
}