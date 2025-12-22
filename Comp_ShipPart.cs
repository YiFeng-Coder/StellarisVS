using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;
namespace Stellaris
{
    public class CompProperties_ShipPart : CompProperties
    {
        public CompProperties_ShipPart()
        {
            compClass = typeof(Comp_ShipPart);
        }

        public bool roof;
        public bool hermetic;
    }

    public class Comp_ShipPart : ThingComp
    {
        private HashSet<IntVec3> cellsUnder;

        public CompProperties_ShipPart Props => (CompProperties_ShipPart)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.cellsUnder = this.parent.OccupiedRect().ToHashSet<IntVec3>();
            bool roof = this.Props.roof;
            if (roof)
            {
                foreach (IntVec3 c in this.cellsUnder)
                {
                    RoofDef roof2 = this.parent.Map.roofGrid.RoofAt(c);
                    if (!ShipUtility.IsRoofDefAirtight(roof2) && this.Props.hermetic)
                    {
                        this.parent.Map.roofGrid.SetRoof(c, StellarisDefOf.StellarisShipRoof);
                    }
                    else
                    {
                        //this.parent.Map.roofGrid.SetRoof(c, null);
                    }
                }
            }
            if(Props.hermetic)
            {
                foreach (IntVec3 c in this.cellsUnder)
                {
                    if (this.parent.Map.terrainGrid.UnderTerrainAt(c) != null)
                    {
                        this.parent.Map.terrainGrid.RemoveTopLayer(c);
                    }
                    if (this.parent.Map.terrainGrid.FoundationAt(c) != StellarisDefOf.StellarisShipFakeFloorInsideShip)
                    {
                        this.parent.Map.terrainGrid.SetFoundation(c, StellarisDefOf.StellarisShipFakeFloorInsideShip);
                    }
                }
            }
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            RemoveRoofAndFoundation(previousMap);
        }

        public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
        {
            base.PostDeSpawn(map, mode);
            RemoveRoofAndFoundation(map);


        }
        public void RemoveRoofAndFoundation(Map map)
        {
            if (cellsUnder == null)
            {
                Log.Error("cellsUnder == null");
            }
            if (Props.roof)
            {
                foreach (IntVec3 c in this.cellsUnder)
                {
                     map.roofGrid.SetRoof(c, null);
                     map.roofGrid.RemoveRoofUnsafe(map.cellIndices.CellToIndex(c));
                }
            }
            if (Props.hermetic)
            {
                foreach (IntVec3 c in this.cellsUnder)
                {

                    if (map.terrainGrid.FoundationAt(c) == StellarisDefOf.StellarisShipFakeFloorInsideShip)
                    {
                        if (map.terrainGrid.CanRemoveFoundationAt(c))
                        {
                            map.terrainGrid.RemoveFoundation(c);
                        }
                        else
                        {
                            Log.Error("Can't remove foundation at" + c.x + " " + c.y + " " + c.z);
                        }
                    }
                }
            }
        }
    }
}
