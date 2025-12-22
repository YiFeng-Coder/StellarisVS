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
        private CompProperties_OxygenPusher Props
        {
            get
            {
                return (CompProperties_OxygenPusher)this.props;
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

        public override void CompTickRare()
        {
            if (this.Props.requiresPower && this.PowerTrader.Off)
            {
                return;
            }
            List<Room> allRooms = this.parent.Map.regionGrid.AllRooms.ToList();
            foreach (Room room in allRooms)
            {
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
