using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Stellaris
{
    [StaticConstructorOnStartup]
    public class StellarisMaterials
    {
        public static Material GetLavaPlanetUngeneratedParts()
        {
            if (LavaPlanetUngeneratedPlanetParts != null)
            {
                return LavaPlanetUngeneratedPlanetParts;
            }
            else
            {
                Log.Message("Try generate material LavaPlanetUngeneratedPlanetParts...");
                Texture2D texture = ContentFinder<Texture2D>.Get("Biome/LavaPlanetBiome");//"World/Biomes/LavaFields");
                LavaPlanetUngeneratedPlanetParts = new Material(ShaderDatabase.WorldTerrain);
                LavaPlanetUngeneratedPlanetParts.mainTexture = texture;
            }
            return LavaPlanetUngeneratedPlanetParts;
        }
        public static Material LavaPlanetUngeneratedPlanetParts;// = MatLoader.LoadMat("World/UngeneratedPlanetParts", 3500);
        public static Material ShieldMat = MaterialPool.MatFrom(
                "UI/Combat/ShieldUI",
                ShaderDatabase.MoteGlow,
                new Color(0.2f, 0.6f, 1f, 0.25f) // 颜色：浅蓝，Alpha 0.25 (半透明)
            );
    }
}
