using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Stellaris
{
    public class CompProperties_OxygenAllRoomPusher : CompProperties
    {
        public CompProperties_OxygenAllRoomPusher()
        {
            this.compClass = typeof(CompOxygenAllRoomPusher);
        }

        public bool requiresPower = true;

        public float airPerSecondPerHundredCells = 0.1f;
    }
    public class CompOxygenAllRoomPusher : ThingComp
    {
        private CompProperties_OxygenAllRoomPusher Props
        {
            get
            {
                return (CompProperties_OxygenAllRoomPusher)this.props;
            }
        }

        public CompPowerTrader PowerTrader
        {
            get
            {
                CompPowerTrader result;
                if ((result = this.intPowerTrader) == null)
                {
                    result = (this.intPowerTrader = this.parent.GetComp<CompPowerTrader>());
                }
                return result;
            }
        }

        public override void CompTickRare() //什么指定的转换无效？
        {
            if (this.Props.requiresPower && this.PowerTrader.Off) //现在听懂了
            {
                return;
            }
            foreach (Room room in this.parent.Map.regionGrid.AllRooms)
            {
                if (room == null || room.Fogged)
                {
                    continue;
                }
                if (!room.UsesOutdoorTemperature)
                {
                    float num = 100f / (float)room.CellCount * this.Props.airPerSecondPerHundredCells * 4.1666665f;
                    room.Vacuum = room.Vacuum - num;
                }
            }
        }

        // Token: 0x0400ABCE RID: 43982
        private const float IntervalToPerSecond = 4.1666665f;

        // Token: 0x0400ABCF RID: 43983
        private CompPowerTrader intPowerTrader;
    }
}
