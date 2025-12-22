using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Stellaris
{
    public class PlaceWorker_AutonomousMiner : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            List<Thing> things = loc.GetThingList(map);
            foreach (Thing t in things)
            {
                if (t is SpaceMiningPad && t.Position == loc)
                {
                    return AcceptanceReport.WasAccepted;
                }
            }
            return "StellarisMustPlaceOnSpaceMiningPad".Translate();
        }
    }
}
