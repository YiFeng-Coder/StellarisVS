using RimWorld;
using RimWorld.Planet;
using Stellaris.PlanetTravel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace Stellaris.WorldGenSteps
{
    public class WorldGenStep_LavaPlanet : WorldGenStep_Tiles
    {
        public override int SeedPart => 25914728;
        private static float FreqMultiplier
        {
            get
            {
                return 10f;
            }
        }

        // Token: 0x0601550F RID: 87311 RVA: 0x006604B9 File Offset: 0x0065E6B9
        public override void GenerateFresh(string seed, PlanetLayer layer)
        {
            this.SetupElevationNoise(layer);
            this.SetupTemperatureOffsetNoise();
            this.SetupRainfallNoise();
            this.SetupHillinessNoise();
            this.SetupSwampinessNoise();
            base.GenerateFresh(seed, layer);
        }

        // Token: 0x06015510 RID: 87312 RVA: 0x006604E4 File Offset: 0x0065E6E4
        private void SetupElevationNoise(PlanetLayer layer)
        {
            float freqMultiplier = FreqMultiplier;
            ModuleBase lhs = new Perlin((double)(0.035f * freqMultiplier), 2.0, 0.4000000059604645, 6, Rand.Range(0, int.MaxValue), QualityMode.High);
            ModuleBase moduleBase = new RidgedMultifractal((double)(0.012f * freqMultiplier), 2.0, 6, Rand.Range(0, int.MaxValue), QualityMode.High);
            ModuleBase moduleBase2 = new Perlin((double)(0.12f * freqMultiplier), 2.0, 0.5, 5, Rand.Range(0, int.MaxValue), QualityMode.High);
            ModuleBase moduleBase3 = new Perlin((double)(0.01f * freqMultiplier), 2.0, 0.5, 5, Rand.Range(0, int.MaxValue), QualityMode.High);
            float num;
            if (Find.World.PlanetCoverage < 0.55f)
            {
                ModuleBase moduleBase4 = new DistanceFromPlanetViewCenter(layer.ViewCenter, Find.WorldGrid.SurfaceViewAngle, true);
                moduleBase4 = new ScaleBias(2.0, -1.0, moduleBase4);
                moduleBase3 = new Blend(moduleBase3, moduleBase4, new Const(0.4000000059604645));
                num = Rand.Range(-0.4f, -0.35f);
            }
            else
            {
                num = Rand.Range(0.15f, 0.25f);
            }
            NoiseDebugUI.StorePlanetNoise(moduleBase3, "elevContinents");
            moduleBase2 = new ScaleBias(0.5, 0.5, moduleBase2);
            moduleBase = new Multiply(moduleBase, moduleBase2);
            float num2 = Rand.Range(0.4f, 0.6f);
            this.noiseElevation = new Blend(lhs, moduleBase, new Const((double)num2));
            this.noiseElevation = new Blend(this.noiseElevation, moduleBase3, new Const((double)num));
            if (Find.World.PlanetCoverage < 0.9999f)
            {
                this.noiseElevation = new ConvertToIsland(Find.WorldGrid.SurfaceViewCenter, Find.WorldGrid.SurfaceViewAngle, this.noiseElevation);
            }
            this.noiseElevation = new ScaleBias(0.5, 0.5, this.noiseElevation);
            this.noiseElevation = new Power(this.noiseElevation, new Const(3.0));
            NoiseDebugUI.StorePlanetNoise(this.noiseElevation, "noiseElevation");
            this.noiseElevation = new ScaleBias((double)ElevationRange.Span, (double)ElevationRange.min, this.noiseElevation);
        }

        // Token: 0x06015511 RID: 87313 RVA: 0x00660748 File Offset: 0x0065E948
        private void SetupTemperatureOffsetNoise()
        {
            float freqMultiplier = FreqMultiplier;
            this.noiseTemperatureOffset = new Perlin((double)(0.018f * freqMultiplier), 2.0, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.High);
            this.noiseTemperatureOffset = new Multiply(this.noiseTemperatureOffset, new Const(4.0));
        }

        // Token: 0x06015512 RID: 87314 RVA: 0x006607AC File Offset: 0x0065E9AC
        private void SetupRainfallNoise()
        {
            float freqMultiplier = FreqMultiplier;
            ModuleBase moduleBase = new Perlin((double)(0.015f * freqMultiplier), 2.0, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.High);
            moduleBase = new ScaleBias(0.5, 0.5, moduleBase);
            NoiseDebugUI.StorePlanetNoise(moduleBase, "basePerlin");
            ModuleBase moduleBase2 = new AbsLatitudeCurve(new SimpleCurve
            {
                {
                    0f,
                    1.12f,
                    true
                },
                {
                    25f,
                    0.94f,
                    true
                },
                {
                    45f,
                    0.7f,
                    true
                },
                {
                    70f,
                    0.3f,
                    true
                },
                {
                    80f,
                    0.05f,
                    true
                },
                {
                    90f,
                    0.05f,
                    true
                }
            }, 100f);
            NoiseDebugUI.StorePlanetNoise(moduleBase2, "latCurve");
            this.noiseRainfall = new Multiply(moduleBase, moduleBase2);
            float num = 0.00022222222f;
            float num2 = -500f * num;
            ModuleBase moduleBase3 = new ScaleBias((double)num, (double)num2, this.noiseElevation);
            moduleBase3 = new ScaleBias(-1.0, 1.0, moduleBase3);
            moduleBase3 = new Clamp(0.0, 1.0, moduleBase3);
            NoiseDebugUI.StorePlanetNoise(moduleBase3, "elevationRainfallEffect");
            this.noiseRainfall = new Multiply(this.noiseRainfall, moduleBase3);
            Func<double, double> processor = delegate (double val)
            {
                if (val < 0.0)
                {
                    val = 0.0;
                }
                if (val < 0.12)
                {
                    val = (val + 0.12) / 2.0;
                    if (val < 0.03)
                    {
                        val = (val + 0.03) / 2.0;
                    }
                }
                return val;
            };
            this.noiseRainfall = new Arbitrary(this.noiseRainfall, processor);
            this.noiseRainfall = new Power(this.noiseRainfall, new Const(1.5));
            this.noiseRainfall = new Clamp(0.0, 999.0, this.noiseRainfall);
            NoiseDebugUI.StorePlanetNoise(this.noiseRainfall, "noiseRainfall before mm");
            this.noiseRainfall = new ScaleBias(4000.0, 0.0, this.noiseRainfall);
            SimpleCurve rainfallCurve = Find.World.info.overallRainfall.GetRainfallCurve();
            if (rainfallCurve != null)
            {
                this.noiseRainfall = new CurveSimple(this.noiseRainfall, rainfallCurve);
            }
        }

        // Token: 0x06015513 RID: 87315 RVA: 0x006609EC File Offset: 0x0065EBEC
        private void SetupHillinessNoise()
        {
            float freqMultiplier = FreqMultiplier;
            this.noiseMountainLines = new Perlin((double)(0.025f * freqMultiplier), 2.0, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.High);
            ModuleBase moduleBase = new Perlin((double)(0.06f * freqMultiplier), 2.0, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.High);
            this.noiseMountainLines = new Abs(this.noiseMountainLines);
            this.noiseMountainLines = new OneMinus(this.noiseMountainLines);
            moduleBase = new Filter(moduleBase, -0.3f, 1f);
            this.noiseMountainLines = new Multiply(this.noiseMountainLines, moduleBase);
            this.noiseMountainLines = new OneMinus(this.noiseMountainLines);
            NoiseDebugUI.StorePlanetNoise(this.noiseMountainLines, "noiseMountainLines");
            this.noiseHillsPatchesMacro = new Perlin((double)(0.032f * freqMultiplier), 2.0, 0.5, 5, Rand.Range(0, int.MaxValue), QualityMode.Medium);
            this.noiseHillsPatchesMicro = new Perlin((double)(0.19f * freqMultiplier), 2.0, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.High);
        }

        // Token: 0x06015514 RID: 87316 RVA: 0x00660B28 File Offset: 0x0065ED28
        private void SetupSwampinessNoise()
        {
            float freqMultiplier = FreqMultiplier;
            ModuleBase moduleBase = new Perlin((double)(0.09f * freqMultiplier), 2.0, 0.4000000059604645, 6, Rand.Range(0, int.MaxValue), QualityMode.High);
            ModuleBase moduleBase2 = new RidgedMultifractal((double)(0.025f * freqMultiplier), 2.0, 6, Rand.Range(0, int.MaxValue), QualityMode.High);
            moduleBase = new ScaleBias(0.5, 0.5, moduleBase);
            moduleBase2 = new ScaleBias(0.5, 0.5, moduleBase2);
            this.noiseSwampiness = new Multiply(moduleBase, moduleBase2);
            InverseLerp rhs = new InverseLerp(this.noiseElevation, SwampinessMaxElevation.max, SwampinessMaxElevation.min);
            this.noiseSwampiness = new Multiply(this.noiseSwampiness, rhs);
            InverseLerp rhs2 = new InverseLerp(this.noiseRainfall, SwampinessMinRainfall.min, SwampinessMinRainfall.max);
            this.noiseSwampiness = new Multiply(this.noiseSwampiness, rhs2);
            NoiseDebugUI.StorePlanetNoise(this.noiseSwampiness, "noiseSwampiness");
        }

        // Token: 0x06015515 RID: 87317 RVA: 0x00660C44 File Offset: 0x0065EE44
        protected override Tile GenerateTileFor(PlanetTile tile, PlanetLayer layer)
        {
            SurfaceTile surfaceTile = new SurfaceTile(tile);
            Vector3 tileCenter = layer.GetTileCenter(tile);
            surfaceTile.elevation = this.noiseElevation.GetValue(tileCenter);
            float value = this.noiseMountainLines.GetValue(tileCenter);
            if (value > 0.235f || surfaceTile.elevation <= 0f)
            {
                if (surfaceTile.elevation > 0f && this.noiseHillsPatchesMicro.GetValue(tileCenter) > 0.46f && this.noiseHillsPatchesMacro.GetValue(tileCenter) > -0.3f)
                {
                    if (Rand.Bool)
                    {
                        surfaceTile.hilliness = Hilliness.SmallHills;
                    }
                    else
                    {
                        surfaceTile.hilliness = Hilliness.LargeHills;
                    }
                }
                else
                {
                    surfaceTile.hilliness = Hilliness.Flat;
                }
            }
            else if (value > 0.12f)
            {
                switch (Rand.Range(0, 4))
                {
                    case 0:
                        surfaceTile.hilliness = Hilliness.Flat;
                        break;
                    case 1:
                        surfaceTile.hilliness = Hilliness.SmallHills;
                        break;
                    case 2:
                        surfaceTile.hilliness = Hilliness.LargeHills;
                        break;
                    case 3:
                        surfaceTile.hilliness = Hilliness.Mountainous;
                        break;
                }
            }
            else if (value > 0.0363f)
            {
                surfaceTile.hilliness = Hilliness.Mountainous;
            }
            else
            {
                surfaceTile.hilliness = Hilliness.Impassable;
            }
            float num = BaseTemperatureAtLatitude(layer.LongLatOf(tile).y);
            num -= TemperatureReductionAtElevation(surfaceTile.elevation);
            num += this.noiseTemperatureOffset.GetValue(tileCenter);
            SimpleCurve temperatureCurve = Find.World.info.overallTemperature.GetTemperatureCurve();
            if (temperatureCurve != null)
            {
                num = temperatureCurve.Evaluate(num);
            }
            surfaceTile.temperature = num;
            surfaceTile.rainfall = this.noiseRainfall.GetValue(tileCenter);
            if (float.IsNaN(surfaceTile.rainfall))
            {
                float value2 = this.noiseRainfall.GetValue(tileCenter);
                Log.ErrorOnce(string.Format("{0} rain bad at {1}", value2, tile), 694822);
            }
            if (surfaceTile.hilliness == Hilliness.Flat || surfaceTile.hilliness == Hilliness.SmallHills)
            {
                surfaceTile.swampiness = this.noiseSwampiness.GetValue(tileCenter);
            }
            surfaceTile.PrimaryBiome = this.BiomeFrom(surfaceTile, tile, layer);
            return surfaceTile;
        }

        // Token: 0x06015516 RID: 87318 RVA: 0x00660E2C File Offset: 0x0065F02C
        private BiomeDef BiomeFrom(Tile ws, PlanetTile tile, PlanetLayer layer)
        {
            List<BiomeDef> allDefsListForReading = PlanetGenerateUtility.GetLavePlanetBiomeDefs();
            BiomeDef biomeDef = null;
            float num = 0f;
            for (int i = 0; i < allDefsListForReading.Count; i++)
            {
                BiomeDef biomeDef2 = allDefsListForReading[i];
                if (biomeDef2.implemented && biomeDef2.generatesNaturally && biomeDef2.Worker.CanPlaceOnLayer(biomeDef2, layer))
                {
                    float score = biomeDef2.Worker.GetScore(biomeDef2, ws, tile);
                    if (score > num || biomeDef == null)
                    {
                        biomeDef = biomeDef2;
                        num = score;
                    }
                }
            }
            return biomeDef;
        }

        // Token: 0x06015517 RID: 87319 RVA: 0x00660EA8 File Offset: 0x0065F0A8
        private static float BaseTemperatureAtLatitude(float lat)
        {
            float x = Mathf.Abs(lat) / 90f;
            return AvgTempByLatitudeCurve.Evaluate(x);
        }

        // Token: 0x06015518 RID: 87320 RVA: 0x00660ED0 File Offset: 0x0065F0D0
        private static float TemperatureReductionAtElevation(float elev)
        {
            if (elev < 250f)
            {
                return 0f;
            }
            float t = (elev - 250f) / 4750f;
            return Mathf.Lerp(0f, 40f, t);
        }

        // Token: 0x0400EDCC RID: 60876
        private ModuleBase noiseElevation;

        // Token: 0x0400EDCD RID: 60877
        private ModuleBase noiseTemperatureOffset;

        // Token: 0x0400EDCE RID: 60878
        private ModuleBase noiseRainfall;

        // Token: 0x0400EDCF RID: 60879
        private ModuleBase noiseSwampiness;

        // Token: 0x0400EDD0 RID: 60880
        private ModuleBase noiseMountainLines;

        // Token: 0x0400EDD1 RID: 60881
        private ModuleBase noiseHillsPatchesMicro;

        // Token: 0x0400EDD2 RID: 60882
        private ModuleBase noiseHillsPatchesMacro;

        // Token: 0x0400EDD3 RID: 60883
        private const float ElevationFrequencyMicro = 0.035f;

        // Token: 0x0400EDD4 RID: 60884
        private const float ElevationFrequencyMacro = 0.012f;

        // Token: 0x0400EDD5 RID: 60885
        private const float ElevationMacroFactorFrequency = 0.12f;

        // Token: 0x0400EDD6 RID: 60886
        private const float ElevationContinentsFrequency = 0.01f;

        // Token: 0x0400EDD7 RID: 60887
        private const float MountainLinesFrequency = 0.025f;

        // Token: 0x0400EDD8 RID: 60888
        private const float MountainLinesHolesFrequency = 0.06f;

        // Token: 0x0400EDD9 RID: 60889
        private const float HillsPatchesFrequencyMicro = 0.19f;

        // Token: 0x0400EDDA RID: 60890
        private const float HillsPatchesFrequencyMacro = 0.032f;

        // Token: 0x0400EDDB RID: 60891
        private const float SwampinessFrequencyMacro = 0.025f;

        // Token: 0x0400EDDC RID: 60892
        private const float SwampinessFrequencyMicro = 0.09f;

        // Token: 0x0400EDDD RID: 60893
        private static readonly FloatRange SwampinessMaxElevation = new FloatRange(650f, 750f);

        // Token: 0x0400EDDE RID: 60894
        private static readonly FloatRange SwampinessMinRainfall = new FloatRange(725f, 900f);

        // Token: 0x0400EDDF RID: 60895
        private static readonly FloatRange ElevationRange = new FloatRange(-500f, 5000f);

        // Token: 0x0400EDE0 RID: 60896
        private const float TemperatureOffsetFrequency = 0.018f;

        // Token: 0x0400EDE1 RID: 60897
        private const float TemperatureOffsetFactor = 4f;

        // Token: 0x0400EDE2 RID: 60898
        private static readonly SimpleCurve AvgTempByLatitudeCurve = new SimpleCurve
        {
            {
                new CurvePoint(500f, 1000f),
                true
            },
            {
                new CurvePoint(500f, 1000f),
                true   
            },
            {
                new CurvePoint(500f, 1000f),
                true
            },
            {
                new CurvePoint(500f, 1000f),
                true
            }
        };

        // Token: 0x0400EDE3 RID: 60899
        private const float ElevationTempReductionStartAlt = 250f;

        // Token: 0x0400EDE4 RID: 60900
        private const float ElevationTempReductionEndAlt = 5000f;

        // Token: 0x0400EDE5 RID: 60901
        private const float MaxElevationTempReduction = 40f;

        // Token: 0x0400EDE6 RID: 60902
        private const float RainfallPower = 1.5f;

        // Token: 0x0400EDE7 RID: 60903
        private const float RainfallFactor = 4000f;

        // Token: 0x0400EDE8 RID: 60904
        private const float RainfallStartFallAltitude = 500f;

        // Token: 0x0400EDE9 RID: 60905
        private const float RainfallFinishFallAltitude = 5000f;
    }
}
