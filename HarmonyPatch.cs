using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Stellaris.PlanetTravel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
namespace Stellaris
{
    [StaticConstructorOnStartup]
    public class StellarisPatch
    {
        static StellarisPatch()
        {
            var harmony = new Harmony("com.stellaris.patch");
            Harmony.DEBUG = true;
            harmony.PatchAll();
            //Log.Message("Harmony patches has been added");
        }
    }

    [HarmonyPatch(typeof(WorldGrid), "CreateRequiredLayers")]
    public class PlanetLayerPatch
    {
        public static void Postfix(WorldGrid __instance)
        {
            PlanetLayer spaceLayer = __instance.RegisterPlanetLayer(StellarisDefOf.StellarisSpaceLayer, StellarisDefOf.StellarisSpaceLayerSetting.settings);
            PlanetLayer obritLayer = Find.WorldGrid.PlanetLayers.Where(x => x.Value.Def.isSpace).First().Value;
            spaceLayer.AddConnection(obritLayer, 200f);
            spaceLayer.zoomInToLayer = obritLayer;
            obritLayer.zoomOutToLayer = spaceLayer;
            //Log.Message("Harmony patch PostfixPlanetLayer has ran");
        }
    }
    // （延续之前的代码：自定义电池组件标记，用于识别目标电池）
    public class CompProperties_SafeBatteryProtector : CompProperties
    {
        public CompProperties_SafeBatteryProtector() => compClass = typeof(Comp_SafeBatteryProtector);
    }
    public class Comp_SafeBatteryProtector : ThingComp { }

    // 关键补丁：拦截短路事件生成逻辑
    [HarmonyPatch(typeof(IncidentWorker_ShortCircuit), "CanFireNowSub")]
    public static class Patch_IncidentWorker_ShortCircuit_CanFireNowSub
    {
        public static bool Prefix(IncidentParms parms, ref bool __result)
        {
            // 获取事件发生的地图
            Map map = parms.target as Map;
            if (map == null) return true; // 无地图，允许事件（通常不会发生）

            // 检查地图上是否存在“安全高容电池”
            bool hasSafeBattery = CheckMapForSafeBatteries(map);

            if (hasSafeBattery)
            {
                // 如果存在安全电池，阻止事件触发（__result = false）
                __result = false;
                return false; // 不再执行原方法
            }

            // 否则允许事件正常判断
            return true;
        }

        // 辅助方法：检查地图上是否存在“安全高容电池”
        private static bool CheckMapForSafeBatteries(Map map)
        {
            // 遍历地图上所有已安装的电池
            foreach (var battery in map.listerBuildings.AllBuildingsColonistOfClass<Building_Battery>())
            {
                // 检查电池是否带有我们的自定义保护组件
                if (battery.TryGetComp<Comp_SafeBatteryProtector>() != null)
                {
                    return true; // 找到安全电池，返回 true
                }
            }
            return false; // 未找到安全电池
        }
    }
    [HarmonyPatch(typeof(GenStep_ScenParts), nameof(GenStep_ScenParts.Generate))]
    public static class Patch_PreventScenGenerate 
    {
        public static bool Prefix(Map map, GenStepParams parms)
        {
            //return true;
            if (StellarisGlobalState.IsSwitchingPlanets)
            {
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Map), "get_IsPlayerHome")]
    public static class Patch_ShipIsPlayerHome
    {
        public static void Postfix(Map __instance, ref bool __result)
        {
            if (__instance.Parent is WorldShip)
            {
                __result = true;
            }
        }
    }
    [HarmonyPatch(typeof(Projectile), "Tick")]
    public static class Patch_Projectile_Tick
    {
        // =========================================================
        // 关键修复：创建一个“字段引用访问器”
        // =========================================================
        // 这行代码会生成一个极其快速的委托（Delegate），专门用来读取 Projectile.origin。
        // 它只会在类加载时运行一次，并在 Tick 中被重复调用，性能损耗几乎为零。
        private static readonly AccessTools.FieldRef<Projectile, Vector3> OriginAccessor =
            AccessTools.FieldRefAccess<Projectile, Vector3>("origin");  

        public static void Postfix(Projectile __instance)
        {
            // 1. 快速剔除
            if (__instance.Destroyed || __instance.Map == null) return;

            // 2. 获取缓存组件
            var cache = __instance.Map.GetComponent<ShipMapComp>();
            if (cache == null) return; // 安全检查

            var shields = cache.shields;
            int count = shields.Count;
            if (count == 0) return;

            Vector3 projPos = __instance.ExactPosition;

            // 3. 使用访问器读取 protected 字段 'origin'
            Vector3 originPos = OriginAccessor(__instance);

            // 4. 遍历缓存列表
            for (int i = 0; i < count; i++)
            {
                var shield = shields[i];

                if (!shield.IsActive()) continue;

                // 距离平方计算
                float distStrCurrent = (projPos - shield.DrawPos).sqrMagnitude;
                float distSqrOrigin = (originPos - shield.DrawPos).sqrMagnitude;
                float rSqr = shield.shieldRadiusSqr;

                // 逻辑：起点在外，当前在内
                if (distSqrOrigin > rSqr && distStrCurrent <= rSqr)
                {
                    InterceptProjectile(__instance, shield);
                    return;
                }
            }
        }

        private static void InterceptProjectile(Projectile bullet, Building_StellarisShield shield)
        {
            GenExplosion.DoExplosion(bullet.ExactPosition.ToIntVec3(),bullet.Map, 1.9f ,DamageDefOf.EMP, shield);
            MoteMaker.ThrowExplosionCell(bullet.ExactPosition.ToIntVec3(), bullet.Map,ThingDefOf.Mote_LightBallLights,Color.blue);
            bullet.Destroy();
        }
    }
    [HarmonyPatch(typeof(RoofCollapseBufferResolver), "CollapseRoofsMarkedToCollapse")]
    public static class NoShipHullCollapse
    {
        private static readonly AccessTools.FieldRef<RoofCollapseBufferResolver, Map> mapAccessor =
            AccessTools.FieldRefAccess<RoofCollapseBufferResolver, Map>("map");
        public static void Prefix(RoofCollapseBufferResolver __instance)
        {
            if (__instance == null) { return; }
            RoofCollapseBuffer roofCollapseBuffer = mapAccessor(__instance).roofCollapseBuffer;
            if (roofCollapseBuffer == null)
            {
                return;
            }
            if (roofCollapseBuffer.CellsMarkedToCollapse == null || roofCollapseBuffer.CellsMarkedToCollapse .Empty())
            {
                return;
            }
            for (int i = roofCollapseBuffer.CellsMarkedToCollapse.Count - 1; i >= 0; i--)
            {
                if (mapAccessor(__instance).roofGrid.RoofAt(roofCollapseBuffer.CellsMarkedToCollapse[i]) == null)
                {
                    continue;
                }
                bool flag = !mapAccessor(__instance).roofGrid.RoofAt(roofCollapseBuffer.CellsMarkedToCollapse[i]).canCollapse;
                if (flag)
                {
                    roofCollapseBuffer.CellsMarkedToCollapse.RemoveAt(i);
                }
            }
        }
    }
}
