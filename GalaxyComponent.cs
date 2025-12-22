using Verse;
using Stellaris.PlanetTravel;
namespace Stellaris
{
    public class GalaxyComponent : GameComponent
    {
        private GalaxyCluster localGalaxyCluster;

        public GalaxyCluster ClusterData 
        {
            get
            {
                // 如果全局有数据，优先用全局的
                if (StellarisGlobalState.GlobalGalaxyCluster != null)
                {
                    return StellarisGlobalState.GlobalGalaxyCluster;
                }
                return localGalaxyCluster;
            }
        }

        public GalaxyComponent(Game game) : base()
        {
            if (game.CurrentMap != null)
            {
                InitializeGalaxy();
            }
        }

        public override void StartedNewGame()
        {
            InitializeGalaxy();
        }

        public override void LoadedGame()
        {
            InitializeGalaxy();
        }

        private void InitializeGalaxy()
        {
            // 情况A: 正在进行星际穿越 (从旧世界跳到新世界)
            if (StellarisGlobalState.IsSwitchingPlanets && StellarisGlobalState.GlobalGalaxyCluster != null)
            {
                // 直接继承上一个世界的星系数据，不需要ExposeData读取，也不需要生成
                localGalaxyCluster = StellarisGlobalState.GlobalGalaxyCluster;
                Log.Message("[Stellaris] Galaxy loaded from Hyperspace Memory.");
                return;
            }

            // 情况B: 正常读取存档 或 第一次新游戏
            if (localGalaxyCluster == null)
            {
                // 这里暂时创建一个空的，等待 ExposeData 填充或者 Generate 填充
                localGalaxyCluster = new GalaxyCluster();

                // 如果是新游戏且没有数据，生成初始星系
                if (localGalaxyCluster.starSystems == null || localGalaxyCluster.starSystems.Count == 0)
                {
                    localGalaxyCluster.GenerateInitialCluster();
                }
            }

            // 重要：将加载/生成的数据同步到全局静态变量
            StellarisGlobalState.GlobalGalaxyCluster = localGalaxyCluster;
        }

        public override void ExposeData()
        {            
            // 只有在存读档时才使用 Scribe
            // 如果是切换星球中间的临时状态，这一步不会执行
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                // 保存时，确保把最新的静态数据保存进去
                if (StellarisGlobalState.GlobalGalaxyCluster != null)
                {
                    localGalaxyCluster = StellarisGlobalState.GlobalGalaxyCluster;
                }
            }

            Scribe_Deep.Look(ref localGalaxyCluster, "galaxyCluster");

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                // 读档完成后，同步到全局
                if (localGalaxyCluster != null)
                {
                    StellarisGlobalState.GlobalGalaxyCluster = localGalaxyCluster;
                }
            }
        }
    }
}