using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Stellaris
{
    public class JobDriver_Console : JobDriver
    {
        private CompShipControl Console
        {
            get 
            {
                return TargetThingA.TryGetComp<CompShipControl>();
            }
        }
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.GetTarget(TargetIndex.A), this.job, 1, -1, null, errorOnFailed, false);
        }

        // Token: 0x060076C5 RID: 30405 RVA: 0x00242AF3 File Offset: 0x00240CF3
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell, false);
            Toil toil = ToilMaker.MakeToil("MakeNewToils");
            toil.initAction = delegate ()
            {
                Thing thing = this.job.GetTarget(TargetIndex.A).Thing;
                pawn.rotationTracker.FaceTarget(TargetThingA);
                pawn.GainComfortFromCellIfPossible(1, true);
                if (thing == null)
                {
                    return;
                }
                Console.hasDriver = true;
            };
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.handlingFacing = true;
            toil.AddFinishAction(delegate { Console.hasDriver = false; });
            yield return toil;
            yield break;
        }

        // Token: 0x0400514A RID: 20810
        private const TargetIndex ConsoleInd = TargetIndex.A;
    }
}
