using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Stellaris
{
    public static class StarGenerator
    {
        private static readonly List<StarTemplate> starTemplates = new List<StarTemplate>
    {
        // 红矮星 - 最常见
        new StarTemplate {
            type = StarType.RedDwarf,
            namePrefixes = new[] { "Gliese", "Luyten", "Lalande", "Ross", "Wolf" },
            massRange = new FloatRange(0.08f, 0.45f),
            temperatureRange = new FloatRange(2400f, 3700f),
            color = new Color(1f, 0.3f, 0.2f),
            luminosityRange = new FloatRange(0.0001f, 0.06f),
            weight = 76f
        },
        
        // 橙矮星
        new StarTemplate {
            type = StarType.OrangeDwarf,
            namePrefixes = new[] { "Epsilon", "Eridani", "Tau Ceti", "Groombridge", "GJ" },
            massRange = new FloatRange(0.45f, 0.8f),
            temperatureRange = new FloatRange(3700f, 5200f),
            color = new Color(1f, 0.6f, 0.3f),
            luminosityRange = new FloatRange(0.06f, 0.5f),
            weight = 12f
        },
        
        // 黄矮星（类似太阳）
        new StarTemplate {
            type = StarType.YellowDwarf,
            namePrefixes = new[] { "Sol", "Alpha Centauri", "Beta", "Gamma", "Delta" },
            massRange = new FloatRange(0.8f, 1.2f),
            temperatureRange = new FloatRange(5200f, 6000f),
            color = new Color(1f, 0.95f, 0.8f),
            luminosityRange = new FloatRange(0.5f, 1.5f),
            weight = 7.6f
        },
        
        // 白矮星
        new StarTemplate {
            type = StarType.WhiteDwarf,
            namePrefixes = new[] { "Sirius B", "Procyon B", "Van Maanen", "Stein" },
            massRange = new FloatRange(0.17f, 1.33f),
            temperatureRange = new FloatRange(8000f, 40000f),
            color = new Color(0.9f, 0.95f, 1f),
            luminosityRange = new FloatRange(0.0001f, 0.01f),
            weight = 1f
        },
        
        // 红巨星
        new StarTemplate {
            type = StarType.RedGiant,
            namePrefixes = new[] { "Arcturus", "Aldebaran", "Pollux", "Gacrux", "Mirach" },
            massRange = new FloatRange(0.8f, 8f),
            temperatureRange = new FloatRange(3000f, 5000f),
            color = new Color(1f, 0.4f, 0.2f),
            luminosityRange = new FloatRange(100f, 1000f),
            weight = 0.6f
        },
        
        // 蓝巨星
        new StarTemplate {
            type = StarType.BlueGiant,
            namePrefixes = new[] { "Rigel", "Bellatrix", "Spica", "Regor", "Alnilam" },
            massRange = new FloatRange(2f, 150f),
            temperatureRange = new FloatRange(10000f, 50000f),
            color = new Color(0.4f, 0.6f, 1f),
            luminosityRange = new FloatRange(1000f, 1000000f),
            weight = 0.13f
        },
        
        // 中子星
        new StarTemplate {
            type = StarType.NeutronStar,
            namePrefixes = new[] { "Pulsar", "Magnetar", "RX J", "PSR", "CXO" },
            massRange = new FloatRange(1.1f, 2.16f),
            temperatureRange = new FloatRange(100000f, 1000000f),
            color = new Color(0.7f, 0.8f, 1f),
            luminosityRange = new FloatRange(0.001f, 0.1f),
            weight = 0.01f
        },
        
        // 双星系统
        new StarTemplate {
            type = StarType.BinaryStar,
            namePrefixes = new[] { "Alpha", "Beta", "Gamma", "Eta", "Zeta" },
            massRange = new FloatRange(1.5f, 3f),
            temperatureRange = new FloatRange(4000f, 8000f),
            color = new Color(1f, 0.9f, 0.7f),
            luminosityRange = new FloatRange(2f, 10f),
            weight = 2.5f
        },
        
        // 原恒星
        new StarTemplate {
            type = StarType.Protostar,
            namePrefixes = new[] { "T Tauri", "FU Orionis", "YLW", "IRS", "VLA" },
            massRange = new FloatRange(0.1f, 3f),
            temperatureRange = new FloatRange(2000f, 5000f),
            color = new Color(1f, 0.7f, 0.3f),
            luminosityRange = new FloatRange(0.1f, 10f),
            weight = 0.5f
        }
    };

        public static Star GenerateStar()
        {
            // 根据权重随机选择恒星模板
            StarTemplate template = SelectWeightedTemplate();

            // 生成恒星
            Star star = new Star();
            star.type = template.type;
            star.name = GenerateStarName(template);
            star.mass = template.massRange.RandomInRange;
            star.temperature = template.temperatureRange.RandomInRange;
            star.color = AdjustColorForTemperature(template.color, star.temperature);
            star.luminosity = CalculateLuminosity(star.mass, star.temperature);

            return star;
        }

        private static StarTemplate SelectWeightedTemplate()
        {
            float totalWeight = starTemplates.Sum(t => t.weight);
            float randomValue = Rand.Range(0f, totalWeight);

            float currentWeight = 0f;
            foreach (var template in starTemplates)
            {
                currentWeight += template.weight;
                if (randomValue <= currentWeight)
                {
                    return template;
                }
            }

            return starTemplates[0]; // 默认返回第一个
        }

        private static string GenerateStarName(StarTemplate template)
        {
            string prefix = template.namePrefixes.RandomElement();

            // 生成编号或后缀
            string suffix = "";
            if (Rand.Value < 0.7f) // 70%几率有编号
            {
                if (Rand.Value < 0.5f)
                {
                    // 使用数字编号
                    suffix = " " + Rand.Range(1, 9999);
                }
                else
                {
                    // 使用字母编号
                    string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    suffix = " " + letters[Rand.Range(0, letters.Length)];
                }
            }

            return prefix + suffix;
        }

        private static Color AdjustColorForTemperature(Color baseColor, float temperature)
        {
            // 根据温度微调颜色
            float tempFactor = Mathf.InverseLerp(2000f, 50000f, temperature);

            // 高温偏蓝，低温偏红
            Color adjustedColor = baseColor;
            if (temperature > 10000f)
            {
                adjustedColor.b += tempFactor * 0.3f;
                adjustedColor.r -= tempFactor * 0.2f;
            }
            else if (temperature < 4000f)
            {
                adjustedColor.r += (1f - tempFactor) * 0.3f;
                adjustedColor.b -= (1f - tempFactor) * 0.2f;
            }

            return adjustedColor;
        }

        private static float CalculateLuminosity(float mass, float temperature)
        {
            // 简化版光度计算（质量-光度关系）
            // 对于主序星：L ∝ M^3.5
            float massLuminosity = Mathf.Pow(mass, 3.5f);

            // 温度修正（斯蒂芬-玻尔兹曼定律：L ∝ R²T⁴）
            float temperatureFactor = Mathf.Pow(temperature / 5778f, 4f); // 以太阳温度为标准

            return massLuminosity * temperatureFactor * Rand.Range(0.8f, 1.2f);
        }

        // 恒星模板类
        private class StarTemplate
        {
            public StarType type;
            public string[] namePrefixes;
            public FloatRange massRange; // 以太阳质量为单位
            public FloatRange temperatureRange; // 开尔文
            public Color color;
            public FloatRange luminosityRange; // 以太阳光度为单位
            public float weight; // 生成权重
        }
    }
}
