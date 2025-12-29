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
    public class Building_StellarisShield : Building 
    {
        // 1. 定义材质 (只加载一次，节省性能)
        // ==========================================
        // 我们复用 "Things/Projectile/Bullet_Big" 这个原版贴图，它是一个柔和的圆球
        // ShaderDatabase.MoteGlow 能让它产生"发光"的半透明效果
        private float shieldRadius = 10f;
        public float shieldRadiusSqr = 100f; // 缓存半径平方
        public CompShipPowerPlant power;
        public Graphic graphicActive;
        public Graphic graphicOff;
        private bool wasPowerOn = true;
        private int tmpTick = 0;


        public Building_StellarisShield() 
        {

        }

        public override Graphic Graphic
        {
            get
            {
                if (power == null)
                {
                    power = GetComp<CompShipPowerPlant>();
                }

                // 如果有电且开启，返回原本 XML 定义的默认贴图
                if (power != null && power.PowerOn)
                {
                    if (graphicActive == null)
                    {
                        graphicActive = GraphicDatabase.Get<Graphic_Single>(
                            def.graphicData.texPath,
                            def.graphicData.shaderType.Shader,
                            def.graphicData.drawSize,
                            def.graphicData.color
                        );
                    }
                    return graphicActive;
                }

                // 如果没电，尝试返回 graphicOff
                if (graphicOff == null)
                {
                    // 动态加载 _Off 贴图
                    // 注意：这里我们沿用了 XML 中定义的 drawSize 和 color，只改变了 texPath
                    graphicOff = GraphicDatabase.Get<Graphic_Single>(
                        def.graphicData.texPath + "_Off", // 自动寻找 _Off 后缀的文件
                        def.graphicData.shaderType.Shader,
                        def.graphicData.drawSize,
                        def.graphicData.color
                    );
                }
                return graphicOff;
            }
        }

        public float ShieldRadius { get => shieldRadius; 
            set 
            { 
                if (value >= 0 && value <= 60)
                { 
                    shieldRadius = value;
                    shieldRadiusSqr = shieldRadius * shieldRadius;
                }
                else if (value < 0 )
                {
                    shieldRadius = 0;
                    shieldRadiusSqr = shieldRadius * shieldRadius;
                }
                else
                {
                    shieldRadius = 60;
                    shieldRadiusSqr = shieldRadius * shieldRadius;
                }
            } 
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            shieldRadiusSqr = ShieldRadius * ShieldRadius;
            power = GetComp<CompShipPowerPlant>();
            if (power != null) wasPowerOn = power.PowerOn;
        }
        protected override void Tick()
        {
            base.Tick();
            if (graphicActive == null || graphicOff == null)
            {
                graphicActive = GraphicDatabase.Get<Graphic_Single>(
def.graphicData.texPath,
def.graphicData.shaderType.Shader,
def.graphicData.drawSize,
def.graphicData.color
);
                graphicOff = GraphicDatabase.Get<Graphic_Single>(
        def.graphicData.texPath + "_Off", // 自动寻找 _Off 后缀的文件
        def.graphicData.shaderType.Shader,
        def.graphicData.drawSize,
        def.graphicData.color
    );
            }
            /*
            tmpTick++;
            if (tmpTick >= 120)
            {
                Map.mapDrawer.MapMeshDirty(Position, MapMeshFlagDefOf.Buildings);
                tmpTick = 0;
            }*/
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);
            if (IsActive())
            {
                // 计算位置：以建筑为中心
                Vector3 center = Position.ToVector3Shifted();

                // 高度修正：让护盾浮在建筑上方，避免被地板遮挡
                // AltitudeLayer.MoteOverhead 通常用于特效，保证在最上层
                center.y = AltitudeLayer.MoteOverhead.AltitudeFor();

                // 呼吸灯特效：利用时间函数计算一个 0.95 ~ 1.05 的缩放比例
                // 让护盾看起来在微微脉动
                float pulse = 1f + Mathf.Sin(Time.realtimeSinceStartup * 2f) * 0.01f;
                float currentSize = (shieldRadius * 2f) * pulse;

                // 创建矩阵：定义位置、旋转(0)、缩放
                Matrix4x4 matrix = Matrix4x4.TRS(
                    center,
                    Quaternion.identity,
                    new Vector3(currentSize, 1f, currentSize)
                );

                // 核心绘制命令：绘制一个平面的网格
                // MeshPool.plane10 是原版提供的一个 10x10 的平面网格，很适合根据比例缩放
                Graphics.DrawMesh(MeshPool.plane10, matrix, StellarisMaterials.ShieldMat, 0);
            }
            if (Find.Selector.IsSelected(this) && !Destroyed)
            {
                GenDraw.DrawRadiusRing(Position, ShieldRadius);
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var item in base.GetGizmos())
            {
                yield return item;
            }
            int offset1 = -1;
            yield return new Command_Action
            {
                action = delegate ()
                {
                    ShieldRadius+=offset1;
                },
                defaultLabel = "StellarisShieldRadiusLowerLabel".Translate() + offset1,
                defaultDesc = "StellarisShieldRadiusLowerDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/TempLower", true)
            };
            int offset2 = -10;
            yield return new Command_Action
            {
                action = delegate ()
                {
                    ShieldRadius += offset2;
                },
                defaultLabel = "StellarisShieldRadiusLowerLabel".Translate() + offset2,
                defaultDesc = "StellarisShieldRadiusLowerDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/TempLower", true)
            };
            int offset3 = 1;
            yield return new Command_Action
            {
                action = delegate ()
                {
                    ShieldRadius += offset3;
                },
                defaultLabel = "StellarisShieldRadiusRaiseLabel".Translate() + offset3,
                defaultDesc = "StellarisShieldRadiusRaiseDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/TempRaise", true)
            };
            int offset4 = 10;
            yield return new Command_Action
            {
                action = delegate ()
                {
                    ShieldRadius += offset4;
                },
                defaultLabel = "StellarisShieldRadiusRaiseLabel".Translate() + offset4,
                defaultDesc = "StellarisShieldRadiusRaiseDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/TempRaise", true)
            };
        }

        public bool IsActive()
        {
            return (power.PowerOn) && !this.Destroyed;
        }
    }
}
