using RimWorld.Planet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Stellaris
{
    public class WorldDrawLayer_LavaPlanetUngeneratedPlanetParts : WorldDrawLayer
    {       // Token: 0x0601526A RID: 86634 RVA: 0x00654A03 File Offset: 0x00652C03
        public override IEnumerable Regenerate()
        {
            foreach (object obj in base.Regenerate())
            {
                yield return obj;
            }
            IEnumerator enumerator = null;
            Vector3 surfaceViewCenter = Find.WorldGrid.SurfaceViewCenter;
            float surfaceViewAngle = Find.WorldGrid.SurfaceViewAngle;
            if (surfaceViewAngle < 180f)
            {
                List<Vector3> collection;
                List<int> collection2;
                SphereGenerator.Generate(4, this.planetLayer.Radius + -0.16f, -surfaceViewCenter, 180f - Mathf.Min(surfaceViewAngle, 180f) + 10f, out collection, out collection2);
                LayerSubMesh subMesh = base.GetSubMesh(StellarisMaterials.GetLavaPlanetUngeneratedParts());
                subMesh.verts.AddRange(collection);
                subMesh.tris.AddRange(collection2);
            }
            base.FinalizeMesh(MeshParts.All);
            yield break;
        }

        // Token: 0x0400EC13 RID: 60435
        private const int SubdivisionsCount = 4;

        // Token: 0x0400EC14 RID: 60436
        private const float ViewAngleOffset = 10f;
    }
}
