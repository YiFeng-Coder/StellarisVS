using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stellaris
{
    public static class NameGenerator
    {
        // 命名组件（与之前相同）
        private static readonly string[] constellationNames =
        {
        "Andromeda", "Antlia", "Apus", "Aquarius", "Aquila", "Ara", "Aries", "Auriga",
        "Boötes", "Caelum", "Camelopardalis", "Cancer", "Canes Venatici", "Canis Major", "Canis Minor",
        "Capricornus", "Carina", "Cassiopeia", "Centaurus", "Cepheus", "Cetus", "Chamaeleon",
        "Circinus", "Columba", "Coma Berenices", "Corona Australis", "Corona Borealis", "Corvus",
        "Crater", "Crux", "Cygnus", "Delphinus", "Dorado", "Draco", "Equuleus", "Eridanus",
        "Fornax", "Gemini", "Grus", "Hercules", "Horologium", "Hydra", "Hydrus", "Indus",
        "Lacerta", "Leo", "Leo Minor", "Lepus", "Libra", "Lupus", "Lynx", "Lyra",
        "Mensa", "Microscopium", "Monoceros", "Musca", "Norma", "Octans", "Ophiuchus",
        "Orion", "Pavo", "Pegasus", "Perseus", "Phoenix", "Pictor", "Pisces", "Piscis Austrinus",
        "Puppis", "Pyxis", "Reticulum", "Sagitta", "Sagittarius", "Scorpius", "Sculptor",
        "Scutum", "Serpens", "Sextans", "Taurus", "Telescopium", "Triangulum", "Triangulum Australe",
        "Tucana", "Ursa Major", "Ursa Minor", "Vela", "Virgo", "Volans", "Vulpecula"
    };

        private static readonly string[] catalogPrefixes =
        {
        "HD", "HIP", "HR", "GJ", "Gliese", "Luyten", "Ross", "Wolf", "Barnard", "Kapteyn",
        "Lalande", "WISE", "2MASS", "TYC", "UCAC", "SDSS", "Gaia", "Kepler", "TESS"
    };

        private static readonly string[] scientistNames =
        {
        "Einstein", "Newton", "Galileo", "Copernicus", "Kepler", "Hubble", "Sagan", "Hawking",
        "Curie", "Herschel", "Brahe", "Ptolemy", "Aristarchus", "Hipparchus", "Alhazen",
        "Fermi", "Planck", "Heisenberg", "Schrödinger", "Dirac", "Feynman", "Bohr",
        "Eddington", "Chandrasekhar", "Hoyle", "Gamow", "Lemaitre", "Oort", "Shapley"
    };

        private static readonly string[] mythologicalNames =
        {
        "Avalon", "Camelot", "Olympus", "Asgard", "Valhalla", "Elysium", "Tartarus", "Niflheim",
        "Muspelheim", "Alfheim", "Svartalfheim", "Jotunheim", "Vanaheim", "Helheim", "Midgard",
        "Atlantis", "El Dorado", "Shambhala", "Agartha", "Lemuria", "Mu", "Hyperborea",
        "Arcadia", "Utopia", "Shangri-La", "Xanadu", "Eden", "Purgatory", "Nirvana"
    };

        private static readonly string[] descriptiveTerms =
        {
        "Abyssal", "Ancient", "Azure", "Binary", "Blazing", "Celestial", "Crimson", "Crystalline",
        "Dark", "Dawn", "Diamond", "Distant", "Divine", "Echo", "Eternal", "Fading", "Frozen",
        "Golden", "Harmonic", "Hidden", "Infinite", "Luminous", "Majestic", "Nebulous", "Obsidian",
        "Primal", "Quantum", "Radiant", "Sapphire", "Scarlet", "Serene", "Shadow", "Silent",
        "Solar", "Stellar", "Timeless", "Twilight", "Ultimate", "Void", "Whispering"
    };

        private static readonly string[] numericalSuffixes =
        {
        "Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Zeta", "Eta", "Theta",
        "Iota", "Kappa", "Lambda", "Mu", "Nu", "Xi", "Omicron", "Pi",
        "Rho", "Sigma", "Tau", "Upsilon", "Phi", "Chi", "Psi", "Omega",
        "Prima", "Secunda", "Tertia", "Quarta", "Quinta", "Sexta", "Septima", "Octava",
        "Nona", "Decima", "Major", "Minor", "Superior", "Inferior", "Borealis", "Australis"
    };

        private static readonly string[] discoveryCodes =
        {
        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P",
        "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD"
    };

        // 命名生成器委托和权重
        private delegate string NameGeneratorDelegate(Vector2? position);

        private static readonly List<(NameGeneratorDelegate generator, float weight)> namingGenerators = new List<(NameGeneratorDelegate, float)>
    {
        (GenerateConstellationName, 25f),
        (GenerateCatalogName, 20f),
        (GenerateScientistName, 15f),
        (GenerateMythologicalName, 15f),
        (GenerateDescriptiveName, 12f),
        (GenerateBinaryMarkerName, 8f),
        (GenerateCoordinateName, 5f)
    };

        public static string GenerateSystemName(Vector2? position = null)
        {
            // 选择生成器
            var generator = SelectWeightedGenerator(namingGenerators);
            return generator(position ?? Vector2.zero);
        }

        private static NameGeneratorDelegate SelectWeightedGenerator(List<(NameGeneratorDelegate generator, float weight)> generators)
        {
            float totalWeight = generators.Sum(g => g.weight);
            float randomValue = Rand.Range(0f, totalWeight);

            float currentWeight = 0f;
            foreach (var (generator, weight) in generators)
            {
                currentWeight += weight;
                if (randomValue <= currentWeight)
                {
                    return generator;
                }
            }

            return generators[0].generator; // 默认返回第一个
        }

        private static string GenerateConstellationName(Vector2? position = null)
        {
            string constellation = constellationNames.RandomElement();

            // 添加编号或后缀
            if (Rand.Value < 0.7f)
            {
                if (Rand.Value < 0.6f)
                {
                    // 数字编号
                    return $"{constellation} {Rand.Range(1, 999)}";
                }
                else
                {
                    // 希腊字母后缀
                    return $"{constellation} {numericalSuffixes[Rand.Range(0, 24)]}";
                }
            }

            return constellation;
        }

        private static string GenerateCatalogName(Vector2? position = null)
        {
            string prefix = catalogPrefixes.RandomElement();

            // 生成编号
            string number;
            if (prefix == "HD" || prefix == "HIP")
            {
                // 大型目录使用大数字
                number = Rand.Range(1, 300000).ToString();
            }
            else if (prefix == "GJ" || prefix == "Gliese")
            {
                // Gliese目录格式
                number = Rand.Range(1, 9000).ToString();
            }
            else if (prefix == "2MASS" || prefix == "WISE")
            {
                // 巡天目录使用坐标式编号
                int raHours = Rand.Range(0, 24);
                int raMinutes = Rand.Range(0, 60);
                int decDegrees = Rand.Range(-90, 91);
                int decMinutes = Rand.Range(0, 60);

                string decSign = decDegrees >= 0 ? "+" : "-";
                number = $"{raHours:D2}{raMinutes:D2}{decSign}{Math.Abs(decDegrees):D2}{decMinutes:D2}";
            }
            else
            {
                // 其他目录
                number = Rand.Range(1, 99999).ToString();
            }

            // 有时添加发现代码
            if (Rand.Value < 0.3f)
            {
                string discoveryCode = discoveryCodes[Rand.Range(0, discoveryCodes.Length)];
                return $"{prefix} {number}{discoveryCode}";
            }

            return $"{prefix} {number}";
        }

        private static string GenerateScientistName(Vector2? position = null)
        {
            string scientist = scientistNames.RandomElement();

            // 有时添加荣誉后缀
            if (Rand.Value < 0.4f)
            {
                string[] honorifics = { "'s Star", "'s World", "'s Legacy", "'s Discovery", " Station", " Base" };
                return scientist + honorifics.RandomElement();
            }

            // 有时添加编号
            if (Rand.Value < 0.3f)
            {
                return $"{scientist} {Rand.Range(1, 10).ToRoman()}";
            }

            return scientist;
        }

        private static string GenerateMythologicalName(Vector2? position = null)
        {
            string mythological = mythologicalNames.RandomElement();

            // 有时添加描述性前缀
            if (Rand.Value < 0.4f)
            {
                string prefix = descriptiveTerms.RandomElement();
                return $"{prefix} {mythological}";
            }

            // 有时添加编号
            if (Rand.Value < 0.25f)
            {
                return $"{mythological} {Rand.Range(1, 5).ToRoman()}";
            }

            return mythological;
        }

        private static string GenerateDescriptiveName(Vector2? position = null)
        {
            string term1 = descriptiveTerms.RandomElement();
            string term2;

            do
            {
                term2 = descriptiveTerms.RandomElement();
            } while (term2 == term1 && descriptiveTerms.Length > 1);

            // 两种组合方式
            if (Rand.Value < 0.6f)
            {
                // 直接组合
                return $"{term1} {term2}";
            }
            else
            {
                // 添加系统后缀
                string[] systemSuffixes = { "System", "Nexus", "Realm", "Domain", "Sector", "Cluster" };
                return $"{term1} {systemSuffixes.RandomElement()}";
            }
        }

        private static string GenerateBinaryMarkerName(Vector2? position = null)
        {
            // 创建一个不包含BinaryMarkerName的生成器列表
            var filteredGenerators = namingGenerators
                .Where(g => g.generator != GenerateBinaryMarkerName)
                .ToList();

            // 选择基础名称
            var baseGenerator = SelectWeightedGenerator(filteredGenerators);
            string baseName = baseGenerator(position ?? Vector2.zero);

            // 添加双星标记
            string[] binaryMarkers = { "Binary", "AB", "A-B", "Double", "Twin" };

            if (Rand.Value < 0.5f)
            {
                // 前置标记
                return $"{binaryMarkers.RandomElement()} {baseName}";
            }
            else
            {
                // 后置标记
                return $"{baseName} {binaryMarkers.RandomElement()}";
            }
        }

        private static string GenerateCoordinateName(Vector2? position = null)
        {
            Vector2 actualPosition = position ?? Vector2.zero;

            // 基于星系坐标生成名称
            int x = Mathf.RoundToInt(actualPosition.x);
            int y = Mathf.RoundToInt(actualPosition.y);

            string coordinate;

            if (Rand.Value < 0.6f)
            {
                // 简单坐标格式
                coordinate = $"Sector {x}-{y}";
            }
            else
            {
                // 十六进制坐标格式
                coordinate = $"0x{x:X2}{y:X2}";
            }

            // 有时添加区域前缀
            if (Rand.Value < 0.4f)
            {
                string[] regions = { "Outer", "Inner", "Core", "Rim", "Frontier", "Border", "Deep" };
                return $"{regions.RandomElement()} {coordinate}";
            }

            return coordinate;
        }

        // 特殊名称生成方法，用于初始系统
        public static string GenerateInitialSystemName()
        {
            // 初始系统有特殊的命名选项
            string[] initialNames =
            {
            "Home", "Sanctuary", "Genesis", "Cradle", "Beginning", "Origin",
            "Hope", "Promise", "Destiny", "Sanctum", "Refuge", "Haven"
        };

            if (Rand.Value < 0.7f)
            {
                return initialNames.RandomElement();
            }
            else
            {
                // 有时使用常规名称
                return GenerateSystemName(Vector2.zero);
            }
        }

        // 为特定位置生成名称（考虑周围系统）
        public static string GenerateSystemNameForPosition(Vector2 position, GalaxyCluster galaxy)
        {
            // 检查附近是否有已命名的系统，如果有的话可能会影响命名
            StarSystem nearestNamed = galaxy.starSystems
                .Where(s => s.position != position)
                .OrderBy(s => Vector2.Distance(s.position, position))
                .FirstOrDefault();

            // 如果附近有系统且距离很近，可能会使用相关命名
            if (nearestNamed != null && Vector2.Distance(nearestNamed.position, position) < 2f)
            {
                if (Rand.Value < 0.3f)
                {
                    // 使用相关命名（同一星座或目录）
                    string baseName = nearestNamed.name;

                    // 尝试提取基础名称部分
                    if (baseName.Contains(' '))
                    {
                        string[] parts = baseName.Split(' ');
                        string root = parts[0];

                        // 如果是星座名，添加不同编号
                        if (constellationNames.Contains(root))
                        {
                            int existingNumber = 1;
                            if (parts.Length > 1 && int.TryParse(parts[1], out int num))
                            {
                                existingNumber = num;
                            }

                            return $"{root} {existingNumber + Rand.Range(1, 5)}";
                        }

                        // 如果是目录名，添加不同发现代码
                        if (catalogPrefixes.Contains(root))
                        {
                            string newDiscoveryCode = discoveryCodes[Rand.Range(0, discoveryCodes.Length)];
                            return $"{root} {parts[1]}{newDiscoveryCode}";
                        }
                    }
                }
            }

            // 否则使用常规命名
            return GenerateSystemName(position);
        }
    }

    // 罗马数字转换扩展（用于行星命名）
    public static class IntExtensions
    {
        private static readonly string[] romanNumerals =
        {
        "O", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X",
        "XI", "XII", "XIII", "XIV", "XV", "XVI", "XVII", "XVIII", "XIX", "XX"
    };

        public static string ToRoman(this int number)
        {
            if (number >= 0 && number < romanNumerals.Length)
                return romanNumerals[number];
            return number.ToString();
        }
    }

    // 扩展方法，用于生成特定类型的名称
    public static class SpecialNameGenerator
    {
        public static string GenerateNebulaName()
        {
            string[] nebulaNames =
            {
            "Crab", "Orion", "Eagle", "Horsehead", "Ring", "Helix", "Dumbbell", "Cats Eye",
            "Butterfly", "Lagoon", "Trifid", "Carina", "Keyhole", "Rosette", "Cone",
            "Ghost", "Veil", "Witch Head", "Tarantula", "Flame", "Pillars of Creation"
        };

            string[] descriptors =
            {
            "Nebula", "Cloud", "Molecular Cloud", "Nebular Complex", "Gaseous Formation"
        };

            return $"{nebulaNames.RandomElement()} {descriptors.RandomElement()}";
        }

        public static string GenerateBlackHoleName()
        {
            string[] blackHoleNames =
            {
            "Cygnus X-1", "Sagittarius A*", "M87*", "Gargantua", "Abyss", "Void Maw",
            "Singularity", "Event Horizon", "Gravitational Anomaly"
        };

            return blackHoleNames.RandomElement();
        }
    }
}