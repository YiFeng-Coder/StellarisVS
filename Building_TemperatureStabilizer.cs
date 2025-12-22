using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace Stellaris
{
    // 继承 Building_TempControl 以获得设定目标温度的 UI 功能
    public class Building_TemperatureStabilizer : Building_TempControl
    {
        // 定义性能参数
        private const float EnergyPerSecond = 64f; // 每秒的热量交换能力 (类似于加热器或空调的功率)
        private const float PowerConsumptionHigh = 600f; // 全功率运行时的耗电量
        private const float PowerConsumptionLow = 20f;   // 待机时的耗电量 (温度已达标)
        private const float TemperatureThreshold = 0.5f; // 温差在这个范围内视为达标

        public override void TickRare()
        {
            base.TickRare();

            // 检查电源组件是否存在且通电
            if (this.compPowerTrader == null || !this.compPowerTrader.PowerOn)
            {
                return;
            }

            // 获取当前所在的房间
            Room room = this.GetRoom(RegionType.Set_All);

            // 如果房间不存在或者是室外，则不进行操作并进入低功耗模式
            if (room == null || room.UsesOutdoorTemperature)
            {
                this.compPowerTrader.PowerOutput = -PowerConsumptionLow;
                return;
            }

            // 获取当前温度和目标温度
            float currentTemp = room.Temperature;
            float targetTemp = this.compTempControl.targetTemperature;
            float diff = targetTemp - currentTemp;

            // 如果温差很小，进入待机模式
            if (Mathf.Abs(diff) < TemperatureThreshold)
            {
                this.compPowerTrader.PowerOutput = -PowerConsumptionLow;
            }
            else
            {
                // 全功率运行
                this.compPowerTrader.PowerOutput = -PowerConsumptionHigh;

                // 计算热量推力
                // TickRare 每 250 tick 运行一次，所以我们需要乘以 4.16f (250 / 60) 来匹配每秒的效果，或者直接处理能量值
                float energyPush = EnergyPerSecond * 4.1666667f;

                if (diff > 0)
                {
                    // 房间太冷 -> 加热 (Push 正热量)
                    // 为了防止过热，如果我们需要的热量少于全功率，可以限制一下（可选）
                    GenTemperature.PushHeat(this, energyPush);
                }
                else
                {
                    // 房间太热 -> 制冷 (Push 负热量)
                    GenTemperature.PushHeat(this, -energyPush);
                }
            }
        }
    }
}