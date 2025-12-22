using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellaris
{
    public class UniverseMapParent : SpaceMapParent, IUniversable
    {
        
        public StarSystem starSystem;
        public Planet planet;
        public int drawPrority = 0;
        StarSystem IUniversable.StarSystem { get => starSystem; set => starSystem = value; }
        Planet IUniversable.Planet { get => planet; set => planet = value; }
        public int DrawPrority { get => drawPrority; set => drawPrority = value; }

        public override void PostRemove()
        {
            base.PostRemove();
            starSystem.universeObjects.Remove(this);
            planet.universeObjects.Remove(this);
        }

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            if (this.def == StellarisDefOf.UniverseMapParent_Ship)
            {
                drawPrority = (int)DrawPriorityUniverseMapParent.Ship;
            }
        }

        public virtual void DrawOnGalaxyGUI()
        {

        }
    }
    enum DrawPriorityUniverseMapParent
    { 
        Ship = 9
    }
}
