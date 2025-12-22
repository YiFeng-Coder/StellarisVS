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
    public class JobDriver_OperatePlanetScanner : JobDriver
    {
        private CompPlanetScanner Scanner => TargetThingA.TryGetComp<CompPlanetScanner>();

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            this.FailOn(delegate
            {
                CompPowerTrader power = TargetThingA.TryGetComp<CompPowerTrader>();
                return power != null && !power.PowerOn;
            });

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);

            Toil operate = ToilMaker.MakeToil(); 
            
            operate.tickAction = delegate
            {
                pawn.rotationTracker.FaceTarget(TargetThingA);
                Scanner.DoScanTick(pawn);
                pawn.skills.Learn(SkillDefOf.Intellectual, 1f);
                pawn.GainComfortFromCellIfPossible(1,true);
            };
            operate.defaultCompleteMode = ToilCompleteMode.Never;
            operate.handlingFacing = true;
            operate.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
            operate.AddFailCondition(() => !Scanner.CanScanNow());
            operate.activeSkill = () => SkillDefOf.Intellectual;
            yield return operate;
        }

    }
}
