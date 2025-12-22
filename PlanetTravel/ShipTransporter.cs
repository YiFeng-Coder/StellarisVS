using RimWorld;
using Stellaris.PlanetTravel;
using System.Collections.Generic;
using System.IO;
using Verse;

namespace Stellaris.PlanetTravel
{
    public static class ShipTransporter
    {
        public const string SITE_SNAPSHOT = "SiteSnapshot";
        // 临时缓冲文件路径
        private static string BufferFilePath => Path.Combine(GenFilePaths.ConfigFolderPath, "Stellaris_Hyperspace_Buffer.xml");

        /// <summary>
        /// 1. 将当前地图打包进入超空间（保存到临时文件）
        /// </summary>
        public static void CaptureAndSerializeMap(Map map,string elementName = "snapshot")
        {
            Log.Message("[Stellaris] Initiating Hyderspace Serialization...");

            try
            {
                // 创建容器
                MapSnapshot snapshot = new MapSnapshot(map);

                // 启动 Scribe 保存模式
                // 这里的 "StellarisSnapshot" 是根节点名称
                Scribe.saver.InitSaving(BufferFilePath, "StellarisSnapshot");

                // 写入数据
                // 这里的 "snapshot" 是XML标签名
                Scribe_Deep.Look(ref snapshot, elementName);

                // 结束保存
                Scribe.saver.FinalizeSaving();

                StellarisGlobalState.HasPendingShipTransfer = true;
                Log.Message($"[Stellaris] Ship serialized successfully. Objects count: {snapshot.allThings.Count}");
            }
            catch (System.Exception ex)
            {
                Log.Error($"[Stellaris] Serialization Failed: {ex}");
                // 紧急处理：如果保存失败，取消传送标记，避免坏档
                StellarisGlobalState.HasPendingShipTransfer = false;
            }
        }

        /// <summary>
        /// 2. 在新世界从超空间重建地图（从临时文件读取）
        /// </summary>
        public static void DeserializeAndReconstruct(Map newMap , bool isSpaceTravel = true,string elementName = "snapshot")
        {
            if (!StellarisGlobalState.HasPendingShipTransfer || !File.Exists(BufferFilePath))
            {
                Log.Warning("[Stellaris] No ship data in hyperspace buffer.");
                return;
            }

            Log.Message("[Stellaris] Reconstructing ship from Hyperspace...");

            MapSnapshot snapshot = null;

            try
            {
                // 1. 启动 Scribe 加载模式
                Scribe.loader.InitLoading(BufferFilePath);

                // 2. 读取数据 (这会实例化所有 Thing，但不会 Spawn 它们)
                // 这一步非常神奇，它会恢复物体的 ID、属性、Component 等所有数据
                Scribe_Deep.Look(ref snapshot, elementName);

                // 3. 解决引用 (处理物体间的相互指向，比如桌子和椅子的链接)
                Scribe.loader.FinalizeLoading();
            }
            catch (System.Exception ex)
            {
                Log.Error($"[Stellaris] Deserialization Failed: {ex}");
                return; // 无法继续
            }

            // 4. 将数据应用到新地图
            if (snapshot != null)
            {
                ApplySnapshotToMap(snapshot, newMap,isSpaceTravel);
            }

            // 5. 清理临时文件 (可选，保留方便调试)
            // File.Delete(BufferFilePath);
            StellarisGlobalState.HasPendingShipTransfer = false;
        }
        public static void ApplySnapshotToMap(MapSnapshot snapshot, Map map, bool isSpaceTravel = true)
        {
            // ... 前置代码保持不变 ...

            // 定义安全边界：留出 2 格的缓冲带，防止阴影计算越界崩溃
            int safeMinX = 2;
            int safeMinZ = 2;
            int safeMaxX = map.Size.x - 2;
            int safeMaxZ = map.Size.z - 2;
            /*
            // A. 恢复地形和屋顶
            // 注意：如果新地图尺寸和旧地图不一样，这里会越界报错。假设尺寸一致。
            for (int i = 0; i < snapshot.terrainGrid.Count; i++)
            {
                IntVec3 c = map.cellIndices.IndexToCell(i);
                if (!c.InBounds(map))
                {
                    continue;
                }
                map.roofGrid.SetRoof(c, null);
            }*/

            // --- 恢复物体 ---
            if (snapshot.allThings != null)
            {
                foreach (var thingData in snapshot.allThings)
                {
                    try
                    {
                        //Log.Message("allThings"+thingData.Label);


                        IntVec3 pos = thingData.Position;

                        // [修复核心崩溃] 严格的边界检查
                        if (pos.x < safeMinX || pos.z < safeMinZ || pos.x >= safeMaxX || pos.z >= safeMaxZ)
                        {
                            // 如果飞船部件超出了安全区，为了防止崩溃，必须丢弃（或移动）
                            // 这里我们选择丢弃，保证游戏能运行
                            continue;
                        }
                        /*
                        // [修复喷泉报错] 检查目标位置是否有"不可销毁"的天然物体（如喷泉、山脉）
                        List<Thing> Obstacles = map.thingGrid.ThingsListAt(pos);
                        for (int i = Obstacles.Count - 1; i >= 0; i--)
                        {
                            Thing obs = Obstacles[i];
                            // 强制移除挡路的东西
                            if (obs.def.category == ThingCategory.Building || obs.def.category == ThingCategory.Plant)
                            {
                                // 使用 DeSpawn 并丢弃，比 Destroy 更强力，可以移除喷泉
                                if (!obs.Destroyed) obs.DeSpawn(DestroyMode.Vanish);
                            }
                        }
                        */
                        // ... 原有的生成逻辑 ...
                        if (thingData.def.category == ThingCategory.Item)
                        {
                            GenSpawn.Spawn(thingData, pos, map, thingData.Rotation);
                        }
                        else if (thingData.def.category == ThingCategory.Building)
                        {
                            Building building = thingData as Building;
                            //Log.Message("spawn building " + building.Label);
                            GenSpawn.Spawn(building,pos,map);
                        }
                        else if (thingData.def.category  == ThingCategory.Pawn)
                        {
                            Pawn pawn = thingData as Pawn;
                            //Log.Message("spawn pawn " + pawn.Label);
                            GenSpawn.Spawn(pawn, pos,map);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        // 忽略单个错误
                    }
                }
            }
            //恢复pawn
            if (isSpaceTravel)
            {
                List<Pawn> pawns = HyperspaceCache.RetrieveTravelers();
                foreach (Pawn pawn in pawns)
                {
                    if (pawn != null)
                    {
                        GenSpawn.Spawn(pawn, pawn.Position, map);
                    }
                }
            }
        }
    }
}