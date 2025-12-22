using System.Collections.Generic;
using RimWorld;
using Verse;
using System.IO;

namespace Stellaris.PlanetTravel
{
    // 静态类，数据在加载新存档/新世界时不会丢失
    public static class HyperspaceCache
    {
        // 存储旅行中的单位数据的XML字符串
        public static string CachedTravelersXml;

        // 存储行星注册表：行星ID -> 存档文件名
        public static Dictionary<string, string> PlanetSaveRegistry = new Dictionary<string, string>();

        // 当前所在的行星ID
        public static string CurrentPlanetId = "Origin_Planet";

        // 用于将Pawn列表序列化为字符串暂存
        public static void StoreTravelers(List<Pawn> pawns)
        {
            
            // 使用Scribe将Pawn列表保存到内存中的XML
            // 注意：这是一个简化概念，实际操作中你可能需要自定义ScribeHandler来处理DeepSave
            // 这里为了演示，我们假设将数据写入临时文件更稳妥
            string tempPath = Path.Combine(GenFilePaths.ConfigFolderPath, "Stellaris_Buffer.xml");

            Scribe.saver.InitSaving(tempPath, "Hyperspace");
            Scribe_Collections.Look(ref pawns, "Travelers", LookMode.Deep);
            Scribe.saver.FinalizeSaving();

            CachedTravelersXml = File.ReadAllText(tempPath);
            
        }

        // 从缓存中恢复单位
        public static List<Pawn> RetrieveTravelers()
        {
            if (string.IsNullOrEmpty(CachedTravelersXml)) return new List<Pawn>();

            string tempPath = Path.Combine(GenFilePaths.ConfigFolderPath, "Stellaris_Buffer.xml");
            File.WriteAllText(tempPath, CachedTravelersXml);

            List<Pawn> travelers = new List<Pawn>();

            // 欺骗游戏我们在加载数据
            Scribe.loader.InitLoading(tempPath);
            Scribe_Collections.Look(ref travelers, "Travelers", LookMode.Deep);
            Scribe.loader.FinalizeLoading();

            // 清理缓存
            CachedTravelersXml = null;
            return travelers;
        }
    }
}