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
    public class WorkGiver_OperatePlanetScanner : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest =>
            ThingRequest.ForDef(StellarisDefOf.StellarisPlanetScanner);

        public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t is Building building && building.Spawned && !t.IsForbidden(pawn))
            {
                CompPlanetScanner scanner = building.GetComp<CompPlanetScanner>();
                CompPowerTrader power = building.GetComp<CompPowerTrader>();
                if (scanner != null && scanner.CanScanWith(pawn) && scanner.CanScanNow() && power.PowerOn)
                {
                    if (pawn.CanReserve(t))
                        return true;
                }
            }
            return false;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(StellarisDefOf.StellarisPlanetScanJob, t);
        }
    }
}
