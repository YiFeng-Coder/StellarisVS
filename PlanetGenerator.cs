using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Stellaris
{
    public static class PlanetGenerator
    {
        private static readonly string[] planetNamePrefixes = new[]
        {
        "Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Zeta", "Eta", "Theta",
        "Iota", "Kappa", "Lambda", "Mu", "Nu", "Xi", "Omicron", "Pi",
        "Rho", "Sigma", "Tau", "Upsilon", "Phi", "Chi", "Psi", "Omega"
    };

        private static readonly string[] planetNameSuffixes = new[]
        {
        "Prime", "Secunda", "Tertius", "Quarta", "Quinta", "Sexta", "Septima", "Octava",
        "Nona", "Decima", "Major", "Minor", "Superior", "Inferior", "Borealis", "Australis"
    };

        private static readonly string[] mythologicalNames = new[]
        {
        "Aegir", "Baldur", "Freyr", "Heimdall", "Loki", "Odin", "Thor", "Tyr",
        "Aphrodite", "Apollo", "Ares", "Artemis", "Athena", "Hades", "Hephaestus", "Hera",
        "Hermes", "Hestia", "Poseidon", "Zeus", "Anubis", "Horus", "Isis", "Osiris",
        "Ra", "Seth", "Thoth", "Vulcan", "Juno", "Jupiter", "Mars", "Mercury",
        "Minerva", "Neptune", "Saturn", "Venus", "Vesta"
    };

        public static List<Planet> GeneratePlanets(Star star)
        {
            List<Planet> planets = new List<Planet>();

            // 根据恒星类型决定行星数量
            int planetCount = CalculatePlanetCount(star);

            // 生成轨道参数
            List<OrbitSlot> orbitSlots = GenerateOrbitSlots(planetCount, star);

            // 为每个轨道生成行星
            foreach (var slot in orbitSlots)
            {
                Planet planet = GeneratePlanetForSlot(slot, star, planets.Count);
                if (planet != null)
                {
                    planets.Add(planet);
                }
            }

            return planets;
        }
        private static int CalculatePlanetCount(Star star)
        {
            // 根据恒星类型决定行星数量范围
            switch (star.type)
            {
                case StarType.RedDwarf:
                    return Rand.RangeInclusive(1, 4); // 红矮星通常行星较少
                case StarType.OrangeDwarf:
                    return Rand.RangeInclusive(2, 6);
                case StarType.YellowDwarf:
                    return Rand.RangeInclusive(3, 8); // 类似太阳系
                case StarType.WhiteDwarf:
                    return Rand.RangeInclusive(0, 3); // 白矮星可能没有行星
                case StarType.RedGiant:
                    return Rand.RangeInclusive(0, 5); // 红巨星可能吞噬了内行星
                case StarType.BlueGiant:
                    return Rand.RangeInclusive(1, 4); // 蓝巨星星系不稳定
                case StarType.NeutronStar:
                    return Rand.RangeInclusive(0, 2); // 中子星很少有行星
                case StarType.BinaryStar:
                    return Rand.RangeInclusive(2, 7); // 双星系统可能有复杂轨道
                case StarType.Protostar:
                    return Rand.RangeInclusive(0, 6); // 原恒星周围可能还在形成行星
                default:
                    return Rand.RangeInclusive(2, 6);
            }
        }

        public static List<OrbitSlot> GenerateOrbitSlots(int planetCount, Star star)
        {
            List<OrbitSlot> slots = new List<OrbitSlot>();

            // 使用提丢斯-波得定则的变体来生成轨道半径
            float innerLimit = CalculateInnerLimit(star);
            float outerLimit = CalculateOuterLimit(star);

            for (int i = 0; i < planetCount; i++)
            {
                OrbitSlot slot = new OrbitSlot();

                // 使用几何序列分布轨道
                float progression = (float)(i + 1) / (planetCount + 1);
                slot.orbitRadius = Mathf.Lerp(innerLimit, outerLimit, progression);

                // 添加一些随机扰动
                slot.orbitRadius *= Rand.Range(0.8f, 1.2f);

                // 确保轨道不重叠
                slot.orbitRadius = EnsureMinOrbitalSeparation(slot.orbitRadius, slots);

                slot.temperature = CalculatePlanetTemperature(star, slot.orbitRadius);
                slot.typeWeights = CalculatePlanetTypeWeights(slot, star);

                slots.Add(slot);
            }

            return slots.OrderBy(s => s.orbitRadius).ToList();
        }

        private static Planet GeneratePlanetForSlot(OrbitSlot slot, Star star, int index)
        {
            Planet planet = new Planet();

            // 选择行星类型
            planet.type = SelectPlanetType(slot.typeWeights);

            // 生成名称
            planet.name = GeneratePlanetName(star.name, index, planet.type);

            // 设置轨道参数
            planet.orbitRadius = slot.orbitRadius;
            planet.orbitPeriod = CalculateOrbitPeriod(planet.orbitRadius, star.mass);

            // 生成物理属性
            planet.mass = GeneratePlanetMass(planet.type, slot.orbitRadius);
            planet.temperature = slot.temperature + Rand.Range(-50f, 50f);
            planet.habitable = CheckHabitable(planet, slot, star);

            // 生成资源
            planet.resources = GeneratePlanetResources(planet.type);

            return planet;
        }
        public static Planet GeneratePlanetWithType(OrbitSlot slot, Star star, int index,PlanetType type)
        {
            Planet planet = new Planet();

            // 选择行星类型
            planet.type = type;

            // 生成名称
            planet.name = GeneratePlanetName(star.name, index, planet.type);

            // 设置轨道参数
            planet.orbitRadius = slot.orbitRadius;
            planet.orbitPeriod = CalculateOrbitPeriod(planet.orbitRadius, star.mass);

            // 生成物理属性
            planet.mass = GeneratePlanetMass(planet.type, slot.orbitRadius);
            planet.temperature = slot.temperature + Rand.Range(-50f, 50f);
            planet.habitable = CheckHabitable(planet, slot, star);

            // 生成资源
            planet.resources = GeneratePlanetResources(planet.type);

            return planet;
        }
        private static PlanetType SelectPlanetType(Dictionary<PlanetType, float> weights)
        {
            float totalWeight = weights.Values.Sum();
            float randomValue = Rand.Range(0f, totalWeight);

            float currentWeight = 0f;
            foreach (var kvp in weights)
            {
                currentWeight += kvp.Value;
                if (randomValue <= currentWeight)
                {
                    return kvp.Key;
                }
            }

            return PlanetType.Terrestrial; // 默认类型
        }

        private static string GeneratePlanetName(string starName, int index, PlanetType type)
        {
            // 有多种命名方案
            if (Rand.Value < 0.3f && mythologicalNames.Length > 0)
            {
                // 使用神话名称
                return mythologicalNames.RandomElement();
            }
            else if (index < planetNamePrefixes.Length)
            {
                // 使用希腊字母
                string suffix = "";
                if (Rand.Value < 0.4f && planetNameSuffixes.Length > 0)
                {
                    suffix = " " + planetNameSuffixes.RandomElement();
                }
                return planetNamePrefixes[index] + suffix;
            }
            else
            {
                // 使用数字编号
                return starName + " " + (index + 1).ToRoman() + (Rand.Value < 0.3f ? "b" : "");
            }
        }

        private static float CalculateInnerLimit(Star star)
        {
            // 内限基于恒星类型和温度
            float baseLimit = 0.1f; // 最小0.1AU

            // 高温恒星有更大的内限
            if (star.temperature > 10000f)
                baseLimit = 0.5f;
            else if (star.temperature > 6000f)
                baseLimit = 0.2f;

            return baseLimit;
        }

        private static float CalculateOuterLimit(Star star)
        {
            // 外限基于恒星质量
            float baseLimit = 30f; // 最小30AU

            // 更大质量的恒星有更大的外限
            if (star.mass > 2f)
                baseLimit = 100f;
            else if (star.mass > 1f)
                baseLimit = 50f;

            return baseLimit;
        }

        private static float EnsureMinOrbitalSeparation(float proposedRadius, List<OrbitSlot> existingSlots)
        {
            const float minSeparationFactor = 1.4f; // 相邻轨道至少相差40%

            foreach (var slot in existingSlots)
            {
                float ratio = proposedRadius / slot.orbitRadius;
                if (ratio > 1f / minSeparationFactor && ratio < minSeparationFactor)
                {
                    // 轨道太近，调整
                    proposedRadius = slot.orbitRadius * minSeparationFactor * Rand.Range(1f, 1.2f);
                }
            }

            return proposedRadius;
        }

        private static float CalculatePlanetTemperature(Star star, float orbitRadius)
        {
            // 使用斯蒂芬-玻尔兹曼定律的简化版本
            // T_planet = T_star * sqrt(R_star / (2 * D)) * (1 - A)^0.25
            // 其中A是反照率，这里使用平均值

            float albedo = 0.3f; // 平均反照率
            float temperature = star.temperature * Mathf.Sqrt(1f / (2f * orbitRadius)) * Mathf.Pow(1f - albedo, 0.25f);

            return temperature;
        }

        private static Dictionary<PlanetType, float> CalculatePlanetTypeWeights(OrbitSlot slot, Star star)
        {
            var weights = new Dictionary<PlanetType, float>();

            // 基础权重
            float distanceFromStar = slot.orbitRadius;
            float temperature = slot.temperature;

            // 雪线位置（水结冰的距离）
            float snowLine = 2.7f * Mathf.Pow(star.luminosity, 0.5f);

            // 宜居带范围（水以液态存在的距离）
            float habitableInner = 0.95f * Mathf.Sqrt(star.luminosity);
            float habitableOuter = 1.37f * Mathf.Sqrt(star.luminosity);

            // 根据距离和温度设置不同类型行星的权重
            if (distanceFromStar < 0.4f)
            {
                // 内行星带
                weights[PlanetType.Lava] = 5f;
                weights[PlanetType.Terrestrial] = 3f;
                weights[PlanetType.Barren] = 2f;
                weights[PlanetType.GasDwarf] = 1f;
            }
            else if (distanceFromStar < habitableInner)
            {
                // 热行星带
                weights[PlanetType.Terrestrial] = 4f;
                weights[PlanetType.Desert] = 3f;
                weights[PlanetType.Barren] = 2f;
                weights[PlanetType.GasDwarf] = 1f;
            }
            else if (distanceFromStar >= habitableInner && distanceFromStar <= habitableOuter)
            {
                // 宜居带
                weights[PlanetType.Terrestrial] = 5f;
                weights[PlanetType.Oceanic] = 4f;
                weights[PlanetType.Desert] = 2f;
                weights[PlanetType.GasDwarf] = 1f;
            }
            else if (distanceFromStar > habitableOuter && distanceFromStar < snowLine)
            {
                // 外行星带（雪线内）
                weights[PlanetType.Terrestrial] = 3f;
                weights[PlanetType.GasDwarf] = 2f;
                weights[PlanetType.Barren] = 2f;
                weights[PlanetType.Toxic] = 1f;
            }
            else
            {
                // 雪线外
                weights[PlanetType.IceGiant] = 4f;
                weights[PlanetType.GasGiant] = 3f;
                weights[PlanetType.Ice] = 3f;
                weights[PlanetType.Barren] = 1f;
            }

            return weights;
        }

        private static float CalculateOrbitPeriod(float orbitRadius, float starMass)
        {
            // 开普勒第三定律：T² ∝ a³/M
            // 简化计算，以地球年为单位
            float period = Mathf.Sqrt(Mathf.Pow(orbitRadius, 3f) / starMass);
            return period;
        }

        private static float GeneratePlanetMass(PlanetType type, float orbitRadius)
        {
            // 行星质量范围（以地球质量为单位）
            FloatRange massRange;

            switch (type)
            {
                case PlanetType.Terrestrial:
                case PlanetType.Lava:
                case PlanetType.Desert:
                case PlanetType.Barren:
                    massRange = new FloatRange(0.1f, 5f);
                    break;
                case PlanetType.Oceanic:
                    massRange = new FloatRange(0.5f, 3f);
                    break;
                case PlanetType.Ice:
                    massRange = new FloatRange(0.2f, 3f);
                    break;
                case PlanetType.Toxic:
                    massRange = new FloatRange(0.3f, 4f);
                    break;
                case PlanetType.GasDwarf:
                    massRange = new FloatRange(5f, 20f);
                    break;
                case PlanetType.IceGiant:
                    massRange = new FloatRange(10f, 50f);
                    break;
                case PlanetType.GasGiant:
                    massRange = new FloatRange(50f, 2000f); // 木星质量约318地球质量
                    break;
                default:
                    massRange = new FloatRange(0.1f, 10f);
                    break;
            }

            // 考虑轨道半径的影响：内轨道通常有较小的行星
            float radiusFactor = Mathf.Clamp01(orbitRadius / 5f);
            float mass = massRange.LerpThroughRange(Rand.Range(0.2f, 0.8f) * radiusFactor);

            return mass;
        }

        private static bool CheckHabitable(Planet planet, OrbitSlot slot, Star star)
        {
            // 检查是否在宜居带内
            float habitableInner = 0.95f * Mathf.Sqrt(star.luminosity);
            float habitableOuter = 1.37f * Mathf.Sqrt(star.luminosity);

            bool inHabitableZone = planet.orbitRadius >= habitableInner && planet.orbitRadius <= habitableOuter;

            // 检查温度范围（水以液态存在）
            bool suitableTemperature = planet.temperature >= 200f && planet.temperature <= 400f;

            // 检查行星类型
            bool suitableType = planet.type == PlanetType.Terrestrial || planet.type == PlanetType.Oceanic;

            // 检查质量范围（能够保持大气层）
            bool suitableMass = planet.mass >= 0.5f && planet.mass <= 5f;

            return inHabitableZone && suitableTemperature && suitableType && suitableMass && Rand.Value < 0.3f;
        }

        private static List<ThingDef> GeneratePlanetResources(PlanetType type)
        {
            List<ThingDef> resources = new List<ThingDef>();

            // 基础资源
            switch (type)
            {
                case PlanetType.Terrestrial:
                    resources.AddRange(new[] { ThingDefOf.Steel });
                    if (Rand.Value < 0.7f) resources.Add(StellarisDefOf.StellarisResourceOrganic);
                    if (Rand.Value < 0.4f) resources.Add(ThingDefOf.Uranium);
                    break;

                case PlanetType.Oceanic:
                    resources.AddRange(new[] { StellarisDefOf.StellarisResourceDeuterium, StellarisDefOf.StellarisResourceOrganic });
                    if (Rand.Value < 0.5f) resources.Add(StellarisDefOf.StellarisResourceRareMetal);
                    break;

                case PlanetType.Desert:
                    resources.AddRange(new[] {  ThingDefOf.Steel});
                    if (Rand.Value < 0.3f) resources.Add(ThingDefOf.Chemfuel);
                    break;

                case PlanetType.Lava:
                    resources.AddRange(new[] { ThingDefOf.Steel });
                    if (Rand.Value < 0.6f) resources.Add(StellarisDefOf.StellarisResourceRareMetal);
                    break;

                case PlanetType.Ice:
                    resources.AddRange(new[] { StellarisDefOf.StellarisResourceOrganic });
                    break;

                case PlanetType.GasGiant:
                case PlanetType.IceGiant:
                    resources.AddRange(new[] { StellarisDefOf.StellarisResourceHelium, StellarisDefOf.StellarisResourceOrganic});
                    if (Rand.Value < 0.3f) resources.Add(StellarisDefOf.StellarisResourceDeuterium);
                    break;

                case PlanetType.Toxic:
                    resources.AddRange(new[] { StellarisDefOf.StellarisResourceOrganic });
                    break;

                case PlanetType.Barren:
                    resources.AddRange(new[] { ThingDefOf.Steel });
                    if (Rand.Value < 0.2f) resources.Add(StellarisDefOf.StellarisResourceRareMetal);
                    break;
            }

            // 随机稀有资源
            if (Rand.Value < 0.2f)
            {
                ThingDef[] rareResources = { ThingDefOf.Gold, StellarisDefOf.StellarisResourceExoticCrystals };
                resources.Add(rareResources.RandomElement());
            }

            return resources;
        }

        // 轨道槽位类（用于行星生成）
        public class OrbitSlot
        {
            public float orbitRadius;
            public float temperature;
            public Dictionary<PlanetType, float> typeWeights;
        }
    }
}
