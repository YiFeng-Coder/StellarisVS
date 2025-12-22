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
    public static class UniverseObjectMaker
    {
        public static UniverseMapParent MakeUniverseMapParent(WorldObjectDef def,StarSystem system,Planet planet)
        { 
            UniverseMapParent universeMapParent = (UniverseMapParent)WorldObjectMaker.MakeWorldObject(def);
            universeMapParent.starSystem = system;
            universeMapParent.planet = planet;
            system.universeObjects.Add(universeMapParent);
            planet.universeObjects.Add(universeMapParent);
            Current.Game.GetComponent<GalaxyComponent>().ClusterData.universeObjects.Add(universeMapParent);
            return WorldObjectMaker.MakeWorldObject(def, universeMapParent) as UniverseMapParent;
        }
        public static UniverseObject MakeUniverseObject(WorldObjectDef def, StarSystem system, Planet planet)
        {
            UniverseObject universeObject = (UniverseObject)WorldObjectMaker.MakeWorldObject(def);
            universeObject.starSystem = system;
            universeObject.planet = planet;
            system.universeObjects.Add(universeObject);
            planet.universeObjects.Add(universeObject);
            Current.Game.GetComponent<GalaxyComponent>().ClusterData.universeObjects.Add(universeObject);
            //Log.Message("Finished Universe Object NODE 1");
            return WorldObjectMaker.MakeWorldObject(def, universeObject) as UniverseObject;
        }
    }
}
