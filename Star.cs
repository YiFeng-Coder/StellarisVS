using UnityEngine;
using Verse;

namespace Stellaris
{
    public class Star : IExposable
    {
        public Star()
        { 
        }
        public string name;
        public StarType type;
        public float mass;
        public float temperature;
        public Color color;
        public float luminosity;
        public bool surveyed = false;
        public void ExposeData()
        {
            Scribe_Values.Look(ref name, "name");
            Scribe_Values.Look(ref type, "type");
            Scribe_Values.Look(ref mass, "mass");
            Scribe_Values.Look(ref temperature, "temperature");
            Scribe_Values.Look(ref color, "color");
            Scribe_Values.Look(ref luminosity, "luminosity");
            Scribe_Values.Look(ref surveyed, "surveyed");
        }
    }
}