using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace Stellaris
{
    public class AutonomousMiner : Building
    {
        private bool launched = false;
        private SpaceMiningPad parentPad;
        private UniverseObjectAutonomousMiner universeObject;
        public bool Launched => launched;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            // 查找所在的采矿站基座
            parentPad = Position.GetThingList(map).Find(t => t is SpaceMiningPad) as SpaceMiningPad;
            if (parentPad != null)
            {
                parentPad.InstallMiner(this);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref launched, "launched");
            Scribe_References.Look(ref parentPad, "parentPad");
        }

        public void Launch()
        {
            launched = true;
            parentPad = Position.GetThingList(Map).Find(t => t is SpaceMiningPad) as SpaceMiningPad;
            if (parentPad != null)
            {
                parentPad.InstallMiner(this);
            }
            if (parentPad == null)
            {
                Log.Message("ParentPad is null");
            }
            var universeObjectTEMP = UniverseObjectMaker.MakeUniverseObject
                (StellarisDefOf.UniverseObject_AutonomousMiner, parentPad.worldShip.starSystem, parentPad.worldShip.planet);
            universeObject = (UniverseObjectAutonomousMiner)universeObjectTEMP;
            SpaceMiningPad.universeObjectAutonomousMiners.Add(universeObject);
            ExplorationManager.planetPlayerAt.universeObjects.Add(universeObject);
            // 发射后隐藏建筑图形
            Destroy(DestroyMode.Vanish);
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            parentPad?.RemoveMiner();
            base.Destroy(mode);
        }

    }
}
