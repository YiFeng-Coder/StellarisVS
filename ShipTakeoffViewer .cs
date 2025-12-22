using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;

namespace Stellaris
{
    // 这个类负责处理起飞的视觉效果和逻辑
    public class ShipTakeoffViewer : Thing
    {
        private ShipRegion shipRegion;
        private List<Building> payload = new List<Building>(); // 存储被带走的飞船建筑
        private List<ThingComp> payloadComps = new List<ThingComp>(); // 缓存组件用于绘制（如炮塔旋转等）

        private int tickCounter = 0;
        private const int TotalTakeoffTicks = 300; // 起飞总时长（5秒）

        // 动画参数
        private float currentAltitude = 0f;
        private float startSpeed = 0.05f;
        private float acceleration = 0.002f;

        // 阴影材质
        private static readonly Material ShadowMat = MaterialPool.MatFrom("Things/Skyfaller/SkyfallerShadow", ShaderDatabase.Transparent);

        public void Setup(ShipRegion region, Map map)
        {
            this.shipRegion = region.DeepCopy(); // 保存一份副本以防万一

            // 1. 收集区域内的建筑物
            HashSet<Thing> thingsToTake = new HashSet<Thing>();
            foreach (IntVec3 cell in region.allCells)
            {
                List<Thing> thingList = cell.GetThingList(map);
                foreach (var t in thingList)
                {
                    if (t is Building b && t.def.category == ThingCategory.Building)
                    {
                        thingsToTake.Add(b);
                    }
                }
            }

            // 2. 将它们从地图上移除，并保存引用
            foreach (var t in thingsToTake)
            {
                Building b = t as Building;
                if (b != null)
                {
                    payload.Add(b);
                    // 保存需要特殊绘制逻辑的组件
                    payloadComps.AddRange(b.AllComps);
                }
            }

            // 设置Viewer的位置为飞船中心，以便摄像机追踪（可选）
            this.Position = region.centerCell;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref shipRegion, "shipRegion");
            Scribe_Collections.Look(ref payload, "payload", LookMode.Deep);
            Scribe_Values.Look(ref tickCounter, "tickCounter", 0);
            Scribe_Values.Look(ref currentAltitude, "currentAltitude", 0f);
        }

        protected override void Tick()
        {
            base.Tick();
            tickCounter++;

            // 1. 计算当前高度 (简单的物理公式 s = ut + 0.5at^2)
            float speedCurrent = startSpeed + (acceleration * tickCounter);
            currentAltitude += speedCurrent;

            // 2. 特效处理：生成尾焰与尘土
            if (tickCounter < TotalTakeoffTicks - 60) // 起飞后期停止尘土
            {
                DoDustEffects();
            }

            // 3. 摄像机震动
            if (tickCounter % 5 == 0 && tickCounter < 150)
            {
                Find.CameraDriver.shaker.DoShake(1.0f);
            }

            // 4. 完成起飞逻辑
            if (tickCounter >= TotalTakeoffTicks)
            {
                FinishTakeoff();
            }
        }

        private void DoDustEffects()
        {
            // 在飞船随机位置生成烟尘
            if (shipRegion.allCells.Count > 0 && this.Map != null)
            {
                // 随机取样几个点
                for (int i = 0; i < 3; i++)
                {
                    IntVec3 cell = shipRegion.allCells.RandomElement();
                    if (cell.InBounds(Map))
                    {
                        FleckMaker.ThrowDustPuff(cell, Map, 2.0f);
                        if (tickCounter > 30) // 延迟一点喷火
                            FleckMaker.ThrowFireGlow(cell.ToVector3(), Map, 1.0f);
                    }
                }
            }
        }

        public override void DrawGUIOverlay()
        {
            // 此时不仅要画自己，还要画带着的飞船
            // 重点：计算绘图向量
            Vector3 drawOffset = new Vector3(0, currentAltitude, currentAltitude * 1.1f); // Y轴是图层高度，Z轴是视觉高度(RimWorld是2.5D)

            foreach (var building in payload)
            {
                // 获取建筑物原本在地面的中心坐标
                Vector3 baseDrawPos = building.TrueCenter();
                // 加上起飞偏移
                Vector3 flyingPos = baseDrawPos + drawOffset;

                // 调整Layer (AltitudeLayer)，确保飞船飞起来后遮挡地面物体
                // Item是较高的层级，或者你可以使用特定的 FlyingItem 层级
                flyingPos.y = AltitudeLayer.Skyfaller.AltitudeFor();

                // 调用原本的Graphic进行绘制
                // 注意：这里可能需要处理复杂的Graphic（如带有多层贴图的建筑）
                // 简单的 DrawWorker 调用：
                building.Graphic.DrawWorker(flyingPos, building.Rotation, building.def, this, 0);

                // 绘制建筑物的“幽灵”阴影（可选，随高度变小）
                DrawShadow(baseDrawPos, currentAltitude);
            }
        }

        private void DrawShadow(Vector3 center, float height)
        {
            // 简单的阴影逻辑：越高阴影越淡/越小
            if (height > 50f) return; // 太高就不画了

            float alpha = Mathf.Clamp01(1f - (height / 50f));
            Color shadowColor = new Color(0, 0, 0, alpha);
            Vector3 shadowPos = center;
            shadowPos.y = AltitudeLayer.Shadows.AltitudeFor();

            Matrix4x4 matrix = Matrix4x4.TRS(shadowPos, Quaternion.identity, new Vector3(1f, 1f, 1f));
            Graphics.DrawMesh(MeshPool.plane10, matrix, ShadowMat,0,null,0,new MaterialPropertyBlock());
        }

        private void FinishTakeoff()
        {
            // 在这里处理飞船离开后的逻辑
            // 比如转移到新地图，或者彻底销毁

            // 示例：显示一条消息并销毁动画器
            Messages.Message("ShipLaunched".Translate(), MessageTypeDefOf.PositiveEvent);
            this.Destroy();

            // 下方代码取决于你是否要永久删除这些建筑
            // 如果要转移数据，这时候 payload 列表已经包含了所有数据，可以直接序列化传输
        }
    }
}