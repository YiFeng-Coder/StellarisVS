using RimWorld;
using System.Collections.Generic;
using System.IO;
using Verse;
using Verse.Noise;

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
            foreach (Pawn pawn in pawns)
            {
                pawn.relations.ClearAllRelations();
            }
            // 使用Scribe将Pawn列表保存到内存中的XML
            // 注意：这是一个简化概念，实际操作中你可能需要自定义ScribeHandler来处理DeepSave
            // 这里为了演示，我们假设将数据写入临时文件更稳妥

            string tempPath = Path.Combine(GenFilePaths.ConfigFolderPath, "Stellaris_Buffer.xml");
            List<Ideo> ideosToSave = new List<Ideo>();
            // 2. 方案 (Policies)
            List<ApparelPolicy> outfitsToSave = new List<ApparelPolicy>();
            List<DrugPolicy> drugsToSave = new List<DrugPolicy>();
            List<FoodPolicy> foodsToSave = new List<FoodPolicy>();
            List<ReadingPolicy> readingsToSave = new List<ReadingPolicy>();
            if (ModsConfig.IdeologyActive)
            {
                foreach (var p in pawns)
                {
                    if (ModsConfig.IdeologyActive && p.Ideo != null && !ideosToSave.Contains(p.Ideo))
                        ideosToSave.Add(p.Ideo);

                    // 收集着装方案
                    if (p.outfits?.CurrentApparelPolicy != null && !outfitsToSave.Contains(p.outfits.CurrentApparelPolicy))
                        outfitsToSave.Add(p.outfits.CurrentApparelPolicy);

                    // 收集用药方案
                    if (p.drugs?.CurrentPolicy != null && !drugsToSave.Contains(p.drugs.CurrentPolicy))
                        drugsToSave.Add(p.drugs.CurrentPolicy);

                    // 收集食物方案
                    if (p.foodRestriction?.CurrentFoodPolicy != null && !foodsToSave.Contains(p.foodRestriction.CurrentFoodPolicy))
                        foodsToSave.Add(p.foodRestriction.CurrentFoodPolicy);

                    // 收集阅读方案 (1.5 新增)
                    if (ModsConfig.IdeologyActive || ModsConfig.BiotechActive) // 阅读通常属于核心或DLC更新，加空判断即可
                    {
                        if (p.reading?.CurrentPolicy != null && !readingsToSave.Contains(p.reading.CurrentPolicy))
                            readingsToSave.Add(p.reading.CurrentPolicy);
                    }
                }
            }
            Scribe.saver.InitSaving(tempPath, "Hyperspace");
            // 1. 先保存文化 (Deep Save)
            Scribe_Collections.Look(ref ideosToSave, "warpIdeos", LookMode.Deep);
            // 2. 保存方案 (Deep) - 这样小人加载时引用的就是这些完整对象
            Scribe_Collections.Look(ref outfitsToSave, "warpOutfits", LookMode.Deep);
            Scribe_Collections.Look(ref drugsToSave, "warpDrugs", LookMode.Deep);
            Scribe_Collections.Look(ref foodsToSave, "warpFoods", LookMode.Deep);
            Scribe_Collections.Look(ref readingsToSave, "warpReadings", LookMode.Deep);
            // 3. 再保存人 (Deep Save)
            // 因为文化已经保存了，这里的人员对文化的引用(Reference)就能正确解析
            Scribe_Collections.Look(ref pawns, "Travelers", LookMode.Deep);
            Scribe.saver.FinalizeSaving();

            CachedTravelersXml = File.ReadAllText(tempPath);
            
        }

        // 从缓存中恢复单位
        public static List<Pawn> RetrieveTravelers(Map map)
        {
            if (string.IsNullOrEmpty(CachedTravelersXml)) return new List<Pawn>();

            string tempPath = Path.Combine(GenFilePaths.ConfigFolderPath, "Stellaris_Buffer.xml");
            File.WriteAllText(tempPath, CachedTravelersXml);

            List<Pawn> travelers = new List<Pawn>();
            List<Ideo> loadedIdeos = new List<Ideo>();
            // 方案列表
            List<ApparelPolicy> loadedOutfits = new List<ApparelPolicy>();
            List<DrugPolicy> loadedDrugs = new List<DrugPolicy>();
            List<FoodPolicy> loadedFoods = new List<FoodPolicy>();
            List<ReadingPolicy> loadedReadings = new List<ReadingPolicy>();
            Current.Game.outfitDatabase.AllOutfits.Clear();
            Current.Game.drugPolicyDatabase.AllPolicies.Clear();
            Current.Game.foodRestrictionDatabase.AllFoodRestrictions.Clear();
            Current.Game.readingPolicyDatabase.AllReadingPolicies.Clear();
            Log.Message("NODE 1");
            // 欺骗游戏我们在加载数据
            Scribe.loader.InitLoading(tempPath);
            Scribe_Collections.Look(ref loadedIdeos, "warpIdeos", LookMode.Deep);
            Scribe_Collections.Look(ref loadedOutfits, "warpOutfits", LookMode.Deep);
            Scribe_Collections.Look(ref loadedDrugs, "warpDrugs", LookMode.Deep);
            Scribe_Collections.Look(ref loadedFoods, "warpFoods", LookMode.Deep);
            Scribe_Collections.Look(ref loadedReadings, "warpReadings", LookMode.Deep);
            Scribe_Collections.Look(ref travelers, "Travelers", LookMode.Deep);
            Scribe.loader.FinalizeLoading();
            Log.Message("NODE 2");
            // 2. [关键修复] 将恢复的文化注册到新世界的管理器中
            // 如果不注册，人虽然有文化对象，但打开“文化面板”会看不到，且可能引发系统报错
            if (ModsConfig.IdeologyActive && loadedIdeos != null && !loadedIdeos.Empty())
            {
                Faction.OfPlayer.ideos.SetPrimary(loadedIdeos[0]);
                foreach (var ideo in loadedIdeos)
                {
                    // 检查新世界是否已经因为巧合生成了同样的 ID（极小概率），或者将其作为新文化加入
                    if (!Find.IdeoManager.IdeosListForReading.Contains(ideo))
                    {
                        Find.IdeoManager.Add(ideo);
                    }
                }
                /*
                foreach (Pawn p in travelers)
                {
                    if (p == null) continue;
                    // 确保文化被正确应用 (防止加载时的临时 null)
                    if (ModsConfig.IdeologyActive && p.Ideo == null)
                    {
                        // 如果万一丢了，试图分配回玩家默认文化，或者从我们加载的列表中找一个
                        if (loadedIdeos != null && loadedIdeos.Count > 0) p.ideo.SetIdeo(loadedIdeos[0]);
                    }
                    p.drafter.Drafted = false;
                }
                */
            }
            Log.Message("NODE 3");
            // B. 注册着装方案 (Outfit)
            if (loadedOutfits != null && !loadedOutfits.Empty())
            {
                foreach (var policy in loadedOutfits)
                {
                    // 只有当数据库里没有这个具体的对象实例时才添加
                    if (!Current.Game.outfitDatabase.AllOutfits.Contains(policy))
                    {
                        Current.Game.outfitDatabase.AllOutfits.Add(policy);
                    }
                }
            }

            // C. 注册用药方案 (DrugPolicy)
            if (loadedDrugs != null && !loadedDrugs.Empty())
            {
                foreach (var policy in loadedDrugs)
                {
                    if (!Current.Game.drugPolicyDatabase.AllPolicies.Contains(policy))
                    {
                        Current.Game.drugPolicyDatabase.AllPolicies.Add(policy);
                    }
                }
            }

            // D. 注册食物方案 (FoodPolicy)
            if (loadedFoods != null && !loadedFoods.Empty())
            {
                foreach (var policy in loadedFoods)
                {
                    if (!Current.Game.foodRestrictionDatabase.AllFoodRestrictions.Contains(policy))
                    {
                        Current.Game.foodRestrictionDatabase.AllFoodRestrictions.Add(policy);
                    }
                }
            }

            // E. 注册阅读方案 (ReadingPolicy - 1.5+)
            if (loadedReadings != null && Current.Game.readingPolicyDatabase != null && !loadedReadings.Empty())
            {
                foreach (var policy in loadedReadings)
                {
                    if (!Current.Game.readingPolicyDatabase.AllReadingPolicies.Contains(policy))
                    {
                        Current.Game.readingPolicyDatabase.AllReadingPolicies.Add(policy);
                    }
                }
            }
            
            Log.Message("NODE 4");

            foreach (Pawn pawn in travelers)
            {                                                                   
                pawn.SetFaction(Faction.OfPlayer);
                //GenSpawn.Spawn(pawn,pawn.Position,map);
            }
            // 清理缓存
            CachedTravelersXml = null;
            return travelers;
        }
    }
}