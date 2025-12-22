using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stellaris
{
    public class StarSystem : IExposable
    {
        public string name;
        public Vector2 position;
        public Star star;
        public List<Planet> planets = new List<Planet>();
        public bool explored;
        public List<IUniversable> universeObjects = new List<IUniversable>();
        public void ExposeData()
        {
            Scribe_Values.Look(ref name, "name");
            Scribe_Values.Look(ref position, "position");
            Scribe_Deep.Look(ref star, "star");
            Scribe_Collections.Look(ref planets, "planets", LookMode.Deep);
            Scribe_Values.Look(ref explored, "explored");
            Scribe_Collections.Look(ref universeObjects, "universeObjects",LookMode.Deep);
        }

        public void GenerateSystemContent()
        {
            if (star == null)
            {
                star = StarGenerator.GenerateStar();
                star.name = name + " " +star.name;
            }
            if (planets == null || planets.Empty())
            {
                planets = PlanetGenerator.GeneratePlanets(star);
            }
            
        }
    }
}