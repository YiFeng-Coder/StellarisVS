using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellaris
{
    public class UniverseObject : WorldObject, IUniversable
    {
        public StarSystem starSystem;
        public Planet planet;
        public int drawPrority = 0;
        StarSystem IUniversable.StarSystem { get => starSystem; set => starSystem = value; }
        Planet IUniversable.Planet { get => planet; set => planet = value; }
        public int DrawPrority { get => drawPrority; set => drawPrority = value; }
        public void DrawOnGalaxyGUI()
        {

        }
    }
}
