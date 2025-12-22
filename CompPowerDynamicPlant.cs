using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Sound;

namespace Stellaris
{
    public class CompPowerDynamicPlant : CompPowerTrader
    {
        protected virtual float DesiredPowerOutput
        {
            get
            {
                return -powerConsumption;
            }
        }

        // Token: 0x0600BA20 RID: 47648 RVA: 0x0035B5A0 File Offset: 0x003597A0
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.refuelableComp = this.parent.GetComp<CompRefuelable>();
            this.breakdownableComp = this.parent.GetComp<CompBreakdownable>();
            this.autoPoweredComp = this.parent.GetComp<CompAutoPowered>();
            this.toxifier = this.parent.GetComp<CompToxifier>();
            if (base.Props.PowerConsumption < 0f && !this.parent.IsBrokenDown() && FlickUtility.WantsToBeOn(this.parent))
            {
                base.PowerOn = true;
            }
            powerConsumption = Props.PowerConsumption;
        }

        // Token: 0x0600BA21 RID: 47649 RVA: 0x0035B62B File Offset: 0x0035982B
        public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
        {
            base.PostDeSpawn(map, mode);
            if (this.sustainerProducingPower != null && !this.sustainerProducingPower.Ended)
            {
                this.sustainerProducingPower.End();
            }
        }

        // Token: 0x0600BA22 RID: 47650 RVA: 0x0035B658 File Offset: 0x00359858
        public override void CompTick()
        {
            base.CompTick();
            this.UpdateDesiredPowerOutput();
            if (base.Props.soundAmbientProducingPower != null)
            {
                if (base.PowerOutput > 0.01f)
                {
                    if (this.sustainerProducingPower == null || this.sustainerProducingPower.Ended)
                    {
                        this.sustainerProducingPower = base.Props.soundAmbientProducingPower.TrySpawnSustainer(SoundInfo.InMap(this.parent, MaintenanceType.None));
                    }
                    this.sustainerProducingPower.Maintain();
                    return;
                }
                if (this.sustainerProducingPower != null)
                {
                    this.sustainerProducingPower.End();
                    this.sustainerProducingPower = null;
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref powerConsumption, "powerConsumption");
        }

        // Token: 0x0600BA23 RID: 47651 RVA: 0x0035B6F0 File Offset: 0x003598F0
        public virtual void UpdateDesiredPowerOutput()
        {
            if ((this.breakdownableComp != null && this.breakdownableComp.BrokenDown) || (this.refuelableComp != null && !this.refuelableComp.HasFuel) || (this.flickableComp != null && !this.flickableComp.SwitchIsOn) || (this.autoPoweredComp != null && !this.autoPoweredComp.WantsToBeOn) || (this.toxifier != null && !this.toxifier.CanPolluteNow) || !base.PowerOn)
            {
                base.PowerOutput = 0f;
                return;
            }
            base.PowerOutput = this.DesiredPowerOutput;
        }
        public float powerConsumption = 0f;
        // Token: 0x04008068 RID: 32872
        protected CompRefuelable refuelableComp;

        // Token: 0x04008069 RID: 32873
        protected CompBreakdownable breakdownableComp;

        // Token: 0x0400806A RID: 32874
        protected CompAutoPowered autoPoweredComp;

        // Token: 0x0400806B RID: 32875
        protected CompToxifier toxifier;

        // Token: 0x0400806C RID: 32876
        private Sustainer sustainerProducingPower;
    }
}
