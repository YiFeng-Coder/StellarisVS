using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace Stellaris
{
    public class Planet : IExposable
    {
        public Planet()
        { 
            
        }
        public string name;
        public PlanetType type;
        public float orbitRadius;
        public float orbitPeriod;
        public float mass;
        public float temperature;
        public bool habitable;
        public List<ThingDef> resources;
        public bool surveyed = false;
        public List<IUniversable> universeObjects = new List<IUniversable>();
        public void ExposeData()
        {
            Scribe_Values.Look(ref name, "name");
            Scribe_Values.Look(ref type, "type");
            Scribe_Values.Look(ref orbitRadius, "orbitRadius");
            Scribe_Values.Look(ref orbitPeriod, "orbitPeriod");
            Scribe_Values.Look(ref mass, "mass");
            Scribe_Values.Look(ref temperature, "temperature");
            Scribe_Values.Look(ref habitable, "habitable");
            Scribe_Values.Look(ref surveyed, "surveyed");
            Scribe_Collections.Look(ref resources, "resources", LookMode.Def);
            Scribe_Collections.Look(ref universeObjects, "universeObjects", LookMode.Deep);
        }
    }
}