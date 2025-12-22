using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Stellaris.DevTools
{
    public class AreaSaveData
    {
        public List<ThingData> things;
        public List<BuildingData> buildings;
        public List<PawnData> pawns;

        public AreaSaveData()
        {
            things = new List<ThingData>();
            buildings = new List<BuildingData>();
            pawns = new List<PawnData>();
        }
    }

    public class ThingData
    {
        public string defName;
        public IntVec3 position;
        public int stackCount;
        public float hitPoints;
        public string stuffsDefName;
    }

    public class BuildingData
    {
        public string defName;
        public IntVec3 position;
        public Rot4 rotation;
        public FactionDef faction;
        public float hitPoints;
        public string stuffsDefName;
    }

    public class PawnData
    {
        public string pawnKindDef;
        public IntVec3 position;
        public FactionDef faction;
        public string name;
        public PawnPosture posture = PawnPosture.Standing;
    }
}
