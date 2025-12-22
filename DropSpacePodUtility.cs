using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Stellaris
{
    public static class DropSpacePodUtility
    {
        public static void MakeSpaceDropPodAt(IntVec3 c, Map map, ActiveTransporterInfo info, Faction faction = null)
        {
            ActiveTransporter activeTransporter = (ActiveTransporter)ThingMaker.MakeThing(StellarisDefOf.ActiveSpaceDropPod);
            activeTransporter.Contents = info;
            SkyfallerMaker.SpawnSkyfaller(StellarisDefOf.SpaceDropPodIncoming, activeTransporter, c, map);
            foreach (Thing item in (IEnumerable<Thing>)activeTransporter.Contents.innerContainer)
            {
                if (item is Pawn pawn && pawn.IsWorldPawn())
                {
                    Find.WorldPawns.RemovePawn(pawn);
                    pawn.psychicEntropy?.SetInitialPsyfocusLevel();
                }
            }
        }
    }
}
