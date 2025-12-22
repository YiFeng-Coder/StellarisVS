using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
            PlanetLayer spaceLayer = __instance.RegisterPlanetLayer(StellarisDefOf.StellarisSpaceLayer,StellarisDefOf.StellarisSpaceLayerSetting.settings);
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
}
