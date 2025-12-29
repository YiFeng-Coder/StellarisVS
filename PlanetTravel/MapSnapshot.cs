using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Stellaris.PlanetTravel
{
    // 这是一个实现了IExposable的容器类，专门用于打包一切
    public class MapSnapshot : IExposable
    {
        // 存储地图尺寸，防止新地图大小不一致出错
        public int sizeX;
        public int sizeZ;

        // 地形和屋顶数据（用字节数组或字符串存压缩数据更高效，这里为了简单用列表）
        public List<TerrainDef> terrainGrid = new List<TerrainDef>();
        public List<RoofDef> roofGrid = new List<RoofDef>();
        // 核心：所有物体的列表
        public List<Thing> allThings = new List<Thing>();

        // 构造函数用于读取
        public MapSnapshot() { }

        // 构造函数用于保存
        public MapSnapshot(Map map)
        {
            this.sizeX = map.Size.x;
            this.sizeZ = map.Size.z;
            
            // 1. 抓取地形
            foreach (IntVec3 c in map.AllCells)
            {
                this.terrainGrid.Add(map.terrainGrid.TerrainAt(c));
                this.roofGrid.Add(map.roofGrid.RoofAt(c));
            }

            // 2. 抓取物体
            // 我们不能保存所有东西（比如 Mote 粒子效果、光照、迷雾等），需要过滤
            for (int i = map.listerThings.AllThings.Count - 1; i >= 0; i--)
            {
                Thing t = map.listerThings.AllThings[i];
                if (ShouldSaveThing(t))
                {
                    allThings.Add(t);
                }
            }
            for (int i = map.listerBuildings.allBuildingsColonist.Count - 1; i >= 0; i--)
            {
                Thing t = map.listerBuildings.allBuildingsColonist[i];
                if (ShouldSaveThing(t) && !allThings.Contains(t))
                {
                    allThings.Add(t);
                }
            }
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref sizeX, "sizeX");
            Scribe_Values.Look(ref sizeZ, "sizeZ");

            // 保存简单的列表
            Scribe_Collections.Look(ref terrainGrid, "terrainGrid", LookMode.Def);
            Scribe_Collections.Look(ref roofGrid, "roofGrid", LookMode.Def);
            // 核心：Deep Save 所有物体
            // LookMode.Deep 会保存物体内部的所有状态（comp、health、art、inventory等）
            Scribe_Collections.Look(ref allThings, "allThings", LookMode.Deep);
        }

        // 过滤不需要保存的物体
        private bool ShouldSaveThing(Thing t)
        {
            if (!t.Spawned) return false;
            if (!t.def.isSaveable) return false;

            if (t is Pawn) return true;

            if (t is Mote) return false;
            if (t is Projectile) return false;

            return true;
        }
    }
}
