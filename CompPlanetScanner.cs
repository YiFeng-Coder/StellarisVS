using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Stellaris
{
    public class CompPlanetScanner : ThingComp
    {
        public float currentScanTick = 0;
        public CompPowerTrader power;
        public CompPlanetScanner() { }
        public CompProperties_PlanetScanner Props => (CompProperties_PlanetScanner)this.props;

        private float currentScanProgress = 0f;

        public float CurrentScanProgress => currentScanProgress;
        public float ProgressPercent => currentScanProgress /100f;

        public override void PostPostMake()
        {
            base.PostPostMake();
            power = this.parent.GetComp<CompPowerTrader>();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref currentScanProgress, "currentScanProgress", 0f);
            Scribe_Values.Look(ref power, "power");
        }

        public bool CanScanNow()
        {
            if (WorldShip.playerShip == null || !power.PowerOn)
            { 
                return false;
            }
            if (WorldShip.playerShip.planet != null && !WorldShip.playerShip.planet.surveyed)
            {
                return true;
            }
            if (WorldShip.playerShip.planet == null && !WorldShip.playerShip.starSystem.star.surveyed)
            {
                return true;
            }
            return false;
        }
        public void DoScanTick(Pawn pawn)
        {
            // 计算扫描速度（基于智识技能）
            float scanSpeed = CalculateScanSpeed(pawn);

            // 增加扫描进度
            currentScanTick += scanSpeed;
            if (currentScanTick > Props.scanIntervalTicks)
            {
                currentScanTick = 0;
                currentScanProgress++;
            }

            // 检查是否完成扫描
            if (currentScanProgress >= 100f)
            {
                CompleteScan(pawn);
            }
        }


        private float CalculateScanSpeed(Pawn worker)
        {
            float speed = Props.baseScanSpeed;

            // 智识技能加成
            if (worker.skills != null)
            {
                SkillRecord intellect = worker.skills.GetSkill(SkillDefOf.Intellectual);
                float skillFactor = 1f + (intellect.Level * 0.08f); // 每级智识技能增加8%速度
                speed *= skillFactor;
            }

            // 意识能力加成
            speed *= worker.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness);

            return speed;
        }

        private void CompleteScan(Pawn pawn)
        {
            // 生成远古建筑群
            if (WorldShip.playerShip == null)
            {
                return;
            }
            else if (WorldShip.playerShip.planet != null)
            {
                WorldShip.playerShip.planet.surveyed = true;
            }
            else
            {
                WorldShip.playerShip.starSystem.star.surveyed = true;
            }

                // 重置进度
            currentScanProgress = 0f;

            // 发送消息
            Messages.Message("StellarisPlanetScanComplete".Translate(pawn.LabelShort),
                           this.parent, MessageTypeDefOf.PositiveEvent);
        }
        private void CompleteScanDev()
        {
            if (WorldShip.playerShip == null)
            {
                return;
            }
            else if (WorldShip.playerShip.planet != null)
            {
                WorldShip.playerShip.planet.surveyed = true;
            }
            else
            {
                WorldShip.playerShip.starSystem.star.surveyed = true;
            }

            // 重置进度
            currentScanProgress = 0f;
        }

        public bool CanScanWith(Pawn worker)
        {
            return worker != null &&
                   worker.Spawned &&
                   worker.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) &&
                   worker.health.capacities.CapableOf(PawnCapacityDefOf.Moving) &&
                   !worker.Downed;
        }

        public override string CompInspectStringExtra()
        {
            string str = base.CompInspectStringExtra();
            str += "ScanProcess".Translate() + ": " + currentScanProgress;
            return str;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {            
            if (currentScanProgress > 0f)
            {
                yield return new Gizmo_PlanetScanProgress
                {
                    scanner = this
                };
            }
            if (Prefs.DevMode)
            {
                yield return new Command_Action()
                { 
                    action = delegate 
                    {
                        CompleteScanDev();
                    },
                    defaultDesc = "ImmediatelyFinishSurvey",
                    defaultLabel = "ImmediatelyFinishSurvey"
                };
            }
            foreach (Gizmo g in base.CompGetGizmosExtra())
                yield return g;
            
            // 显示扫描进度
            

            
        }
    }

    // 自定义Gizmo显示扫描进度
    public class Gizmo_PlanetScanProgress : Gizmo
    {
        public CompPlanetScanner scanner;

        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth,GizmoRenderParms gizmoRenderParms)
        {
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Widgets.DrawWindowBackground(rect);

            Rect progressRect = new Rect(rect.x + 10f, rect.y + 10f, rect.width - 20f, 30f);
            Widgets.FillableBar(progressRect, scanner.ProgressPercent);

            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(progressRect, $"{scanner.ProgressPercent:P1}");
            Text.Anchor = TextAnchor.UpperLeft;

            Rect labelRect = new Rect(rect.x, rect.y + 45f, rect.width, 30f);
            Widgets.Label(labelRect, "ScanProgress".Translate());

            return new GizmoResult(GizmoState.Clear);
        }
    }
    public class CompProperties_PlanetScanner : CompProperties
    {
        public float scanWorkAmount = 1000f; // 完成一次扫描所需的总工作量
        public float baseScanSpeed = 1f; // 基础扫描速度
        public int scanIntervalTicks = 60; // 扫描间隔（游戏刻）

        public CompProperties_PlanetScanner()
        {
            this.compClass = typeof(CompPlanetScanner);
        }
    }
}
