using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Stellaris.PlanetTravel
{
    // 静态全局状态，用于跨越不同世界（World）保留数据
    public static class StellarisGlobalState
    {
        // 1. 恒星系数据的全局唯一实例
        public static GalaxyCluster GlobalGalaxyCluster;
        
        // 标记是否正在进行星际穿越（用于判断是加载存档还是生成新星球）
        public static bool IsSwitchingPlanets = false;
        public static bool HasPendingShipTransfer = false;
        public static PlanetType SwitchingPlanetType = PlanetType.Terrestrial;
        // 新增：缓存旧世界的时间
        public static int SavedGameAbsTick = 0;
        public static int SavedGameTicksInt = 0;
        // 新增：缓存活体生物（Pawn）
        public static List<Pawn> SavedTravelers = new List<Pawn>();
        // 新增：保存地图尺寸
        public static IntVec3 SavedMapSize = new IntVec3(250, 1, 250);

        // 重置方法（用于彻底退出到主菜单时清理）
        public static void Reset()
        {
            GlobalGalaxyCluster = null;
            IsSwitchingPlanets = false;
            HasPendingShipTransfer = false;
            SavedTravelers.Clear(); // 清空缓存
            SavedGameAbsTick = 0;
            SavedMapSize = new IntVec3(250, 1, 250);
        }
    }

    // 飞船蓝图数据结构
    [Obsolete]
    public class ShipSnapshot
    {
        public int SizeX;
        public int SizeZ;
        public TerrainDef[] TerrainGrid; // 地面数据
        public List<BuildingSnapshot> Buildings = new List<BuildingSnapshot>(); // 建筑数据
        // 注意：Pawn（单位）依然建议使用上一轮提到的 HyperspaceCache 单独处理
    }
    [Obsolete]
    public class BuildingSnapshot : IExposable
    {
        public ThingDef Def;
        public ThingDef Stuff;
        public IntVec3 Pos;
        public Rot4 Rot;
        public int HitPoints;
        public string ExtraDataXml; // 用于存储箱子里的物品、工作台设置等深层数据

        public void ExposeData()
        {
            Scribe_Defs.Look(ref Def, "def");
            Scribe_Defs.Look(ref Stuff, "stuff");
            Scribe_Values.Look(ref Pos, "pos");
            Scribe_Values.Look(ref Rot, "rot");
            Scribe_Values.Look(ref HitPoints, "hp");
            Scribe_Values.Look(ref ExtraDataXml, "extraData");
        }
    }
}