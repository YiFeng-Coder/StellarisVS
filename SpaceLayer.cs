using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Stellaris
{
    public class SpaceLayer : OrbitLayer
    {
        public SpaceLayer()
        {
        }

        public SpaceLayer(int layerId, PlanetLayerDef def, float radius, Vector3 origin, Vector3 viewCenter, float viewAngle, int subdivisions, float extraCameraAltitude, float backgroundWorldCameraOffset, float backgroundWorldCameraParallaxDistancePer100Cells)
            : base(layerId, def, radius, origin, viewCenter, viewAngle, subdivisions, extraCameraAltitude, backgroundWorldCameraOffset, backgroundWorldCameraParallaxDistancePer100Cells)
        {
        }
    }
}
