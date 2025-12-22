using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;
using static System.Collections.Specialized.BitVector32;

namespace Stellaris
{
    public class SpaceMiningPad : Building
    {
        public CompPowerTrader powerTrader;
        public AutonomousMiner installedMiner;
        private int nextCargoTick;
        private const int CargoInterval = 60000; // 每1天投放一次
        public static List<UniverseObjectAutonomousMiner> universeObjectAutonomousMiners = new List<UniverseObjectAutonomousMiner>();
        public WorldShip worldShip;
        public bool HasMinerInstalled => installedMiner != null;
        public AutonomousMiner InstalledMiner => installedMiner;
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            // 初始化下一次运输仓到达时间
            powerTrader = GetComp<CompPowerTrader>();
            nextCargoTick = Find.TickManager.TicksGame + Rand.Range(30000, 60000);
            if (this.Map.Parent is WorldShip)
            {
                worldShip = this.Map.Parent as WorldShip;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref installedMiner, "installedMiner");
            Scribe_Values.Look(ref nextCargoTick, "nextCargoTick");
            Scribe_References.Look(ref worldShip, "worldShip");
            Scribe_Collections.Look(ref universeObjectAutonomousMiners, "universeObjectAutonomousMiners", LookMode.Deep);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }

            // 如果有安装采矿机，显示发射按钮
            var launch = new Command_Action
            {
                defaultLabel = "StellarisLaunchMinerLabel".Translate(),
                defaultDesc = "StellarisLaunchMinerDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/LaunchShip"),
                action = LaunchMiner
            };
            launch.disabledReason = " ";
            bool flag = false;
            if (!HasMinerInstalled)
            {
                launch.disabledReason += "StellarisNoMinerInstalled".Translate();
                flag = true;
            }
            if (!powerTrader.PowerOn)
            {
                launch.disabledReason += "StellarisPowerOff".Translate();
                flag = true;
            }
            if (worldShip == null)
            {
                launch.disabledReason += "StellarisMustBeOnShipLaunch".Translate();
                flag = true;
            }
            else
            {
                if (!worldShip.planet.surveyed)
                {
                    launch.disabledReason += "StellarisUnsurveyedPlanet".Translate();
                    flag = true;
                }
            }

            if (flag)
            {
                launch.Disable(launch.disabledReason);
            }
            yield return launch;
            if (Prefs.DevMode)
            {
                yield return new Command_Action
                {
                    defaultDesc = "get resource immediately",
                    defaultLabel = "Get Resource Immediately",
                    action = DeliverCargo
                };
            }
        }

        private void LaunchMiner()
        {
            if (this.Map.Parent is WorldShip)
            {
                worldShip = this.Map.Parent as WorldShip;
            }
            else
            {
                Log.Error("Not WorldShip When Launching");
                return;
            }
                // 播放发射动画和音效
            PlayLaunchEffects();
            // 设置采矿机为已发射状态

            installedMiner = Position.GetThingList(Map).Find(x => x is AutonomousMiner) as AutonomousMiner;
            if (installedMiner != null)
            {
                installedMiner.Launch();
                return;
            }
            else
            {
                Log.Message("installedMiner is NULL");
                installedMiner = Position.GetThingList(Map).Find(x => x is AutonomousMiner) as AutonomousMiner;
                installedMiner.Launch();
            }
            if (installedMiner == null)
            {
                Log.Error("installedMiner is NULL After Fixed");
            }

            RemoveMiner();
        }

        private void PlayLaunchEffects()
        {
            // 播放发射音效
            SoundDefOf.ShipTakeoff.PlayOneShot(new TargetInfo(Position, Map));
            // 创建发射粒子效果
            MoteMaker.MakeStaticMote(Position.ToVector3Shifted(), Map, ThingDefOf.Mote_ResurrectFlash, 2f);

            // 烟雾效果

        }

        public void InstallMiner(AutonomousMiner miner)
        {
            installedMiner = miner;
        }

        public void RemoveMiner()
        {
            installedMiner = null;
        }

        protected override void Tick()
        {
            base.Tick();

            // 检查是否应该投放运输仓
            if (Find.TickManager.TicksGame >= nextCargoTick)
            {
                DeliverCargo();
                nextCargoTick = Find.TickManager.TicksGame + CargoInterval;
            }
        }

        private void DeliverCargo()
        {
            if (Map == null) return;

            // 生成矿物
            List<Thing> resources = GenerateMinedResources();

            // 投放运输仓
            //DropPodUtility.DropThingsNear(Position,Map,resources,110,false,false,false,false,false,Faction.OfPlayer);
            ActiveTransporterInfo activeTransporterInfo = new ActiveTransporterInfo();
            foreach (Thing item2 in resources)
            {
                activeTransporterInfo.innerContainer.TryAdd(item2);
            }
            activeTransporterInfo.openDelay = 110;
            activeTransporterInfo.leaveSlag = false;
            DropSpacePodUtility.MakeSpaceDropPodAt(Position, Map, activeTransporterInfo, Faction.OfPlayer);
            // 播放到达效果
            Messages.Message("StellarisMinerPodArrival".Translate(), new TargetInfo(Position, Map), MessageTypeDefOf.PositiveEvent);
        }

        private List<Thing> GenerateMinedResources()
        {
            var resources = new List<Thing>();
            foreach(UniverseObjectAutonomousMiner miner in universeObjectAutonomousMiners)
            {
                resources.AddRange(miner.planet.resources.ConvertAll<Thing>(x => ThingMaker.MakeThing(x)));
            }
            foreach (var item in resources)
            {
                item.stackCount = Rand.Range(10,100);
            }
            return resources;
        }

        public override string GetInspectString()
        {
            string baseString = base.GetInspectString();
            if (Prefs.DevMode)
            {
                baseString += "\nticksToDeliverResource: "+ (-Find.TickManager.TicksGame + nextCargoTick);
            }

            string status = HasMinerInstalled
                ? (installedMiner.Launched
                    ? "状态: 采矿机运行中"
                    : "状态: 采矿机待发射")
                : "状态: 等待安装采矿机";

            return baseString + $"\n{status}";
        }

    }
}
