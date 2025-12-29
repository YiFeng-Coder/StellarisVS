using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Stellaris
{
    public static class PowerUtility
    {
        public static bool TryComsumeBatteryEnergy(this CompPower compPower, float energyToComsume)
        {
            float energySum = 0;
            int batteryCount = 0;
            foreach (var battery in compPower.PowerNet.batteryComps)
            {
                energySum += battery.StoredEnergy;
                batteryCount++;
            }
            if (energySum < energyToComsume)
            {
                return false;
            }
            float remainingToDrain = energyToComsume;
            foreach (var battery in compPower.PowerNet.batteryComps)
            {
                if (remainingToDrain <= 0f) break;

                float currentLevel = battery.StoredEnergy;

                if (currentLevel > 0f)
                {
                    float drainAmount = Mathf.Min(remainingToDrain, currentLevel);

                    battery.DrawPower(drainAmount);

                    remainingToDrain -= drainAmount;
                }
            }
            return true;
        }
    }
}
