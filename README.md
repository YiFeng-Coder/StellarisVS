[README.md](https://github.com/user-attachments/files/27570331/README.md)
# Stellaris 模组 Def 与 Source 概览

本文档基于剔除贴图后的 `StellarisAIGC_v2` 压缩包静态扫描生成，主要记录两部分：`Defs` 中所有带 `defName` 的 XML Def，以及 `Source/Stellaris` 中所有 C# 类型的大概作用。

## 扫描统计

- Def XML 文件：65 个
- 带 `defName` 的 Def：221 个
- C# 源码文件：124 个
- C# 类型（class / struct / interface / enum）：174 个

> 说明：`Languages`、`Sounds`、`Assemblies`、`SiteMapData`、`.git`、`bin/obj/packages` 不属于本 README 的主体扫描范围。

## 命名约定检查

项目约定：新增 DefName 应统一使用 `Stellaris_` 前缀以减少与其他模组冲突。

当前扫描到 74 个 DefName 未使用严格的 `Stellaris_` 前缀。部分名称虽然以 `Stellaris` 开头，但没有下划线；如果这些 Def 已被存档或 C# 引用，重命名前需要同步迁移引用。

- **Stellaris.PlanetDef**：`StellarisLavaPlanet`, `StellarisTerrestrialPlanet`, `StellarisToxicPlanet`
- **PlanetLayerDef**：`StellarisSpaceLayer`, `StellarisLavaPlanetSurface`, `StellarisToxicPlanetSurface`
- **PlanetLayerSettingsDef**：`StellarisSpaceLayerSetting`, `StellarisLavaPlanetLayerSetting`, `StellarisToxicPlanetLayerSetting`
- **BiomeDef**：`StellarisLavaPlanetBiome`, `StellarisPressurizedAcidicBarrens`, `StellarisLavaFieldBackGround`
- **TerrainDef**：`StellarisMeltTerrain`, `StellarisToxicSoil`, `StellarisShipFakeFloorInsideShip`
- **MapGeneratorDef**：`StellarisCommonPlanetGenerator`, `StellarisLavaPlanetGenerator`, `StellarisAcidBarrensGenerator`, `StellarisSpace`
- **GenStepDef**：`StellarisGenStep_Acidification`
- **WorldGenStepDef**：`StellarisWorldGenStepLavaPlanet`, `StellarisWorldGenStepToxicPlanet`
- **WeatherDef**：`StellarisNoOxygen`, `StellarisAcidSmog`
- **ThingDef**：`StellarisVacuumBarrier`, `StellarisShipWall`, `StellarisShipConsole`, `StellarisShipEngine`, `StellarisShipHullTile`, `StellarisPlanetScanner`, `StellarisSpaceMiningPad`, `StellarisAutonomousMiner`, `StellarisLifeSupportDevice`, `StellarisNuclearFissionReactor`, `StellarisLargeChemfuelTank`, `StellarisTemperatureStabilizer`, `StellarisSafeSuperBattery`, `StellarisResourceOrganic`, `StellarisResourceDeuterium`, `StellarisResourceRareMetal`, `StellarisResourceHelium`, `StellarisResourceExoticCrystals`, `StellarisResourceUraniumFuelCapsule`, `ActiveSpaceDropPod`, `SpaceDropPodIncoming`, `StellarisToxMycelium`, `StellarisToxSporePod`
- **RecipeDef**：`Make_StellarisResourceUraniumFuelCapsule`
- **ResearchTabDef**：`StellarisResearchTab`
- **ResearchProjectDef**：`StellarisResearch_ExtremeEnvironmentSurvival`, `StellarisResearch_ShipConsole`, `StellarisResearch_ShipEngine`, `StellarisResearch_PlanetScanner`, `StellarisResearch_SpaceMining`, `StellarisResearch_LifeSupport`, `StellarisResearch_NuclearFissionReactor`, `StellarisResearch_TemperatureStabilizer`, `StellarisResearch_SafeSuperBattery`, `StellarisResearch_NuclearFusionReactor`, `StellarisResearch_OrbitalRailgun`, `StellarisResearch_Sheild`, `StellarisResearch_BioDigester`
- **JobDef**：`StellarisPlanetScanJob`, `StellarisShipConsoleJob`
- **WorkGiverDef**：`StellarisPlanetScannerWorkGiver`
- **MainButtonDef**：`GalaxyMap`
- **WorldObjectDef**：`UniverseMapParent_Ship`, `UniverseObject_AutonomousMiner`, `WorldObject_EnemyShip`
- **ScenarioDef**：`StellarisLavaPlanetTravel`, `StellarisToxicPlanetTravel`
- **ScenPartDef**：`StellarisPlanetLayerFixed`
- **RoofDef**：`StellarisShipRoof`
- **Stellaris.EnemyShipDef**：`StellarisEnemyShipTest`

## Def 实现总览

### Def 类型数量

| Def 类型 | 数量 |
|---|---:|
| `Stellaris.PlanetDef` | 6 |
| `PlanetLayerDef` | 6 |
| `PlanetLayerSettingsDef` | 6 |
| `BiomeDef` | 14 |
| `TerrainDef` | 25 |
| `MapGeneratorDef` | 7 |
| `GenStepDef` | 6 |
| `WorldGenStepDef` | 5 |
| `WeatherDef` | 9 |
| `ThingDef` | 95 |
| `RecipeDef` | 1 |
| `ResearchTabDef` | 1 |
| `ResearchProjectDef` | 13 |
| `JobDef` | 4 |
| `WorkGiverDef` | 3 |
| `MainButtonDef` | 1 |
| `Stellaris.ArchaeologicalSiteDef` | 2 |
| `SitePartDef` | 2 |
| `WorldObjectDef` | 3 |
| `ScenarioDef` | 5 |
| `ScenPartDef` | 1 |
| `RoofDef` | 1 |
| `SoundDef` | 2 |
| `ThoughtDef` | 2 |
| `Stellaris.EnemyShipDef` | 1 |

### Stellaris.PlanetDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `StellarisLavaPlanet` | 熔融行星 | 一颗地质活动极度剧烈的行星，表面大部分被熔岩海洋、半凝固的玄武岩壳和持续喷发的火山覆盖。大气极其稠密，主要由硫化物、二氧化碳和少量金属蒸气组成。天际呈暗红色或橙黄色，闪电与火山灰风暴频繁。从轨道俯视，可见行星表面遍布发光的熔岩裂纹，如同破碎的燃烧玻璃球。 |  | `Defs/PlanetDefs/Planet.xml` |
| `StellarisTerrestrialPlanet` | 陆地行星 | 岩石表面覆盖着稀薄大气的行星，可能存在液态水。 |  | `Defs/PlanetDefs/Planet.xml` |
| `StellarisToxicPlanet` | 剧毒行星 | 一颗大气中充满高浓度硫化物、甲烷、氯气或其他致命化合物的行星。地表被腐蚀性湖泊、毒雾笼罩的荒原和化学沉积岩层覆盖。植物与动物多为极端嗜酸或化学合成生物，甚至存在完全由重金属或晶体构成的“生命”形式。从轨道观察，行星呈现出黄绿色、紫黑色或橙红色的诡异色调，厚重的毒云层中偶尔闪烁出酸雨闪电。 |  | `Defs/PlanetDefs/Planet.xml` |
| `Stellaris_DeadPlanet` | 死寂行星 | 一颗完全没有大气层、磁场或地质活动的岩石星球。它并非因文明衰亡而死去，而是从未真正活过：地表覆盖远古撞击留下的环形山、干涸玄武岩平原与昼夜温差撕裂出的碎石海。 |  | `Defs/PlanetDefs/Planet.xml` |
| `Stellaris_FrozenPlanet` | 冰封行星 | 一颗表面被永久冰川、雪原或冻土覆盖的寒冷世界。大气稀薄或中等，常见冰晶云、极地涡旋和极光环；液态水几乎只存在于冰下深层湖泊、冰晶洞穴或地热活跃区。 |  | `Defs/PlanetDefs/Planet_Frozen.xml` |
| `Stellaris_LostPlanet` | 遗落行星 | 一颗曾经存在过辉煌文明、但如今已彻底衰亡的行星。轨道上可见破碎的太空设施残骸、废弃星港骨架与绵延大陆的城市废墟；地表覆盖锈蚀巨构、沉默工业区和被灾变熔融后凝固的玻璃平原。这里没有活着的文明，但废墟之下潜藏着自动防御系统、纳米污染、古代机械与被遗忘的知识。 |  | `Defs/PlanetDefs/Planet_Lost.xml` |

### PlanetLayerDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `Stellaris_FrozenPlanetSurface` | 冰封行星 | 自定义星球/宇宙图层 Def。 |  | `Defs/PlanetLayerDefs/PlanetLayer_FrozenPlanet.xml` |
| `Stellaris_LostPlanetSurface` | 遗落行星 | 自定义星球/宇宙图层 Def。 |  | `Defs/PlanetLayerDefs/PlanetLayer_LostPlanet.xml` |
| `StellarisLavaPlanetSurface` | 熔融行星 | 自定义星球/宇宙图层 Def。 |  | `Defs/PlanetLayerDefs/PlanetLayers.xml` |
| `StellarisSpaceLayer` | 星际空间 | 自定义星球/宇宙图层 Def。 | Parent=SpaceLayer | `Defs/PlanetLayerDefs/PlanetLayers.xml` |
| `StellarisToxicPlanetSurface` | 剧毒行星 | 自定义星球/宇宙图层 Def。 |  | `Defs/PlanetLayerDefs/PlanetLayers.xml` |
| `Stellaris_DeadPlanetSurface` | 死寂行星 | 自定义星球/宇宙图层 Def。 |  | `Defs/PlanetLayerDefs/PlanetLayers.xml` |

### PlanetLayerSettingsDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `StellarisLavaPlanetLayerSetting` | 熔融行星 | 自定义星球/宇宙图层绘制设置 Def。 |  | `Defs/WorldLayerSettingsDefs/PlanetLayerSettings.xml` |
| `StellarisSpaceLayerSetting` | 星际空间 | 自定义星球/宇宙图层绘制设置 Def。 |  | `Defs/WorldLayerSettingsDefs/PlanetLayerSettings.xml` |
| `StellarisToxicPlanetLayerSetting` | 剧毒行星 | 自定义星球/宇宙图层绘制设置 Def。 |  | `Defs/WorldLayerSettingsDefs/PlanetLayerSettings.xml` |
| `Stellaris_DeadPlanetLayerSetting` | 死寂行星 | 自定义星球/宇宙图层绘制设置 Def。 |  | `Defs/WorldLayerSettingsDefs/PlanetLayerSettings.xml` |
| `Stellaris_FrozenPlanetLayerSetting` | 冰封行星 | 自定义星球/宇宙图层绘制设置 Def。 |  | `Defs/WorldLayerSettingsDefs/PlanetLayerSettings_FrozenPlanet.xml` |
| `Stellaris_LostPlanetLayerSetting` | 遗落行星 | 自定义星球/宇宙图层绘制设置 Def。 |  | `Defs/WorldLayerSettingsDefs/PlanetLayerSettings_LostPlanet.xml` |

### BiomeDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `Stellaris_FrozenGlacialPlain` | 冰川平原 | 广阔平坦的冰川覆盖区域，冰面光滑，少有障碍物。强风会把细雪吹成贴地漂移的雪雾，冰层深处偶尔夹杂可开采的金属沉积物。 | Worker=BiomeWorker_IceSheet | `Defs/BiomeDefs/Biome_FrozenPlanet.xml` |
| `Stellaris_FrozenIceCrystalCave` | 冰晶洞穴 | 冰川下方的洞穴系统，墙壁与穹顶覆盖着发光冰晶，光线像极光一样在冰面中折射。部分低洼区域有冰下湖泊或地热热泉。 | Worker=BiomeWorker_IceSheet | `Defs/BiomeDefs/Biome_FrozenPlanet.xml` |
| `Stellaris_FrozenPolarWasteland` | 极地荒原 | 行星极点附近的破碎冻土与冰蚀岩原。这里几乎没有降水，强风持续剥蚀冰层，裸露基岩、冰砾和深层矿脉暴露在极昼与极夜下。 | Worker=BiomeWorker_IceSheet | `Defs/BiomeDefs/Biome_FrozenPlanet.xml` |
| `Stellaris_LostGlassPlain` | 玻璃平原 | 极端热能瞬间冲击形成的熔融凝固地貌。城市与生命在一瞬间消失，只留下光滑扭曲的硅酸盐玻璃、熔铸金属雕塑和地面上的永恒阴影。 | Worker=BiomeWorker_Desert | `Defs/BiomeDefs/Biome_LostPlanet.xml` |
| `Stellaris_LostRustedPlain` | 锈蚀平原 | 广阔的废弃工业区与物流枢纽，被氧化铁染成深红褐色。精炼塔、管道网络和巨型运输载具残骸散落其间，低洼处常有酸性泥坑与泄漏的化学残余。 | Worker=BiomeWorker_Desert | `Defs/BiomeDefs/Biome_LostPlanet.xml` |
| `Stellaris_LostSilentCity` | 寂静城区 | 曾经繁华的城市中心，如今只剩摩天楼残骸、坍塌高架与遍布弹坑的广场。部分建筑内部仍可通行，废弃终端、家具残骸和数据核心散落在静默街区中。 | Worker=BiomeWorker_Desert | `Defs/BiomeDefs/Biome_LostPlanet.xml` |
| `StellarisLavaPlanetBiome` | 熔融荒原 | 充满高温熔融物的生命禁区。熔融物中含有丰富的稀有矿物，但取不取得到，可就看你本事了。 | Worker=BiomeWorker_LavaField | `Defs/BiomeDefs/Biome_Planets.xml` |
| `StellarisPressurizedAcidicBarrens` | 高压酸性荒地 | 这是一个死寂的世界，大气压得令人窒息，酸湖沸腾。空气中弥漫着硫磺和二氧化碳，形成了失控的温室效应。地表无法获得阳光，而气压之高足以压垮脆弱的结构。在这里生存需要完全密封大气层。 | Worker=BiomeWorker_Desert | `Defs/BiomeDefs/Biome_Planets.xml` |
| `Stellaris_DeadCraterWastes` | 陨坑荒原 | 死寂行星最典型的地貌。一望无际的灰色平原上密布大小不一的撞击坑，地表覆盖由撞击粉碎的岩石风化层，夹杂陨铁碎片与冲击玻璃颗粒。 | Worker=BiomeWorker_Desert | `Defs/BiomeDefs/Biome_Planets.xml` |
| `Stellaris_DeadRadiationDesert` | 辐射沙漠 | 这里并非因水或风形成的沙漠，而是因恒星辐射与宇宙射线长年轰击而“烤干”的高原区。硅酸盐粉尘像灰一样堆积，放射性矿物偶尔在黑暗中泛出微弱荧光。 | Worker=BiomeWorker_Desert | `Defs/BiomeDefs/Biome_Planets.xml` |
| `StellarisLavaFieldBackGround` | 熔融地块 | 完全熔融的地块。一般情况下，多数机械体和生物都无法在这里行动。 | Worker=BiomeWorker_Ocean | `Defs/BiomeDefs/Biomes_BackGround.xml` |
| `Stellaris_DeadPlanetBackGround` | 死寂地块 | 无法在当前比例下细分的死寂岩石地块。 | Worker=BiomeWorker_Desert | `Defs/BiomeDefs/Biomes_BackGround.xml` |
| `Stellaris_FrozenPlanetBackGround` | 冰封行星 | 世界地图生态群系/行星表面区域 Def。 | Worker=BiomeWorker_IceSheet | `Defs/BiomeDefs/Biomes_BackGround_FrozenPlanet.xml` |
| `Stellaris_LostPlanetBackGround` | 遗落行星背景 | 从轨道俯瞰，遗落行星呈现灰、褐与锈红交织的斑驳色调，夜侧偶尔闪烁残余电网与反应堆衰变辉光。 | Worker=BiomeWorker_Desert | `Defs/BiomeDefs/Biomes_BackGround_LostPlanet.xml` |

### TerrainDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `Stellaris_FrozenBlueIce` | 蓝晶冰 | 被巨大压力压实的深层蓝冰，半透明且极其滑溜，表面可见被封入其中的暗色裂纹。 |  | `Defs/TerrainDefs/Terrain_FrozenPlanet.xml` |
| `Stellaris_FrozenCaveIce` | 洞穴折光冰 | 冰下洞穴中的古老冰壁崩落后压成的地面，表层布满微小冰晶，能反射蓝绿色幽光。 |  | `Defs/TerrainDefs/Terrain_FrozenPlanet.xml` |
| `Stellaris_FrozenDriftSnow` | 漂移积雪 | 被强风推挤成波纹状的细雪层，足以掩住裂缝和小型冰砾，行动速度较慢。 |  | `Defs/TerrainDefs/Terrain_FrozenPlanet.xml` |
| `Stellaris_FrozenErodedRock` | 冰蚀岩石 | 被冰川长期研磨出的裸露基岩，表面有浅色擦痕与冻结裂隙，可支撑重型结构。 |  | `Defs/TerrainDefs/Terrain_FrozenPlanet.xml` |
| `Stellaris_FrozenGlacialIce` | 冰川冰面 | 厚重、光滑的永久冰川表面，反射淡蓝色冷光。冰面可行走，但容易打滑。 |  | `Defs/TerrainDefs/Terrain_FrozenPlanet.xml` |
| `Stellaris_FrozenIcyGravel` | 冰砾 | 冰蚀作用留下的岩屑与碎冰混合层，尖锐、松散且行走困难。 |  | `Defs/TerrainDefs/Terrain_FrozenPlanet.xml` |
| `Stellaris_FrozenPermafrost` | 干裂冻土 | 几乎没有液态水循环的古老冻土，被低温收缩撕裂成多边形裂纹。 |  | `Defs/TerrainDefs/Terrain_FrozenPlanet.xml` |
| `Stellaris_FrozenSubglacialLake` | 冰下湖薄冰 | 覆盖在冰下湖泊上方的薄冰与暗水，承载力极差，不适合通行。 |  | `Defs/TerrainDefs/Terrain_FrozenPlanet.xml` |
| `Stellaris_FrozenThermalSpring` | 地热温泉冰缘 | 地热把冰下水加热后形成的潮湿薄冰和矿物沉积边缘。它比周围略暖，但湿滑且危险。 |  | `Defs/TerrainDefs/Terrain_FrozenPlanet.xml` |
| `Stellaris_VitrifiedReactorFloor` | 玻璃化反应堆地面 | 灰蓝色玻璃化混凝土和陶瓷地面，裂缝中嵌着熔凝金属颗粒。 |  | `Defs/TerrainDefs/Terrain_FusionReactorSite.xml` |
| `Stellaris_LostCorrosiveMud` | 腐蚀性泥坑 | 由泄漏化学物、酸雨与工业粉尘混合形成的黏稠泥坑。它会拖慢移动并污染暴露装备。 |  | `Defs/TerrainDefs/Terrain_LostPlanet.xml` |
| `Stellaris_LostCrackedAsphalt` | 龟裂道路 | 古代城市道路的残余，沥青或复合铺装层被热胀冷缩撕开，裂缝中积满灰尘和碎玻璃。 |  | `Defs/TerrainDefs/Terrain_LostPlanet.xml` |
| `Stellaris_LostFusedGlass` | 熔凝玻璃地表 | 灾变热冲击将硅酸盐与建筑材料熔成光滑玻璃层，表面布满尖锐裂纹。 |  | `Defs/TerrainDefs/Terrain_LostPlanet.xml` |
| `Stellaris_LostIndustrialSlag` | 工业废渣 | 压实的矿渣、金属碎屑与氧化铁粉末构成的粗糙地面，红褐色锈层覆盖一切。 |  | `Defs/TerrainDefs/Terrain_LostPlanet.xml` |
| `Stellaris_LostShadowImprint` | 灾变阴影 | 极端闪光把建筑与生命的剪影永久烙在玻璃化地表上，暗色痕迹中仍有微弱衰变辉光。 |  | `Defs/TerrainDefs/Terrain_LostPlanet.xml` |
| `Stellaris_LostUrbanRubble` | 城市碎石 | 由混凝土、陶瓷、玻璃与腐朽建筑材料混合而成的废墟地表，偶尔露出管线和破损地砖。 |  | `Defs/TerrainDefs/Terrain_LostPlanet.xml` |
| `StellarisMeltTerrain` | 高温熔融物 | 地图地形 Def。 | Parent=LavaBase | `Defs/TerrainDefs/Terrain_Planets.xml` |
| `StellarisToxicSoil` | 酸性土壤 | 被重金属化合物和腐蚀性雨水浸透的地面。它几乎无法维持生命。 |  | `Defs/TerrainDefs/Terrain_Planets.xml` |
| `Stellaris_DeadBasaltRough` | 裸露玄武岩 | 没有大气侵蚀的冷硬基岩，呈暗灰到铁黑色，表面布满热胀冷缩造成的裂纹。 |  | `Defs/TerrainDefs/Terrain_Planets.xml` |
| `Stellaris_DeadMeteoricGravel` | 陨铁碎砾 | 混杂着镍铁陨石碎片的撞击砾层，行走不便，但常能从中回收钢铁和稀有金属。 |  | `Defs/TerrainDefs/Terrain_Planets.xml` |
| `Stellaris_DeadRadiationDust` | 辐射粉尘 | 被恒星辐射与宇宙射线长期轰击后形成的细腻硅酸盐粉尘。它会陷住脚步，并携带危险的放射性颗粒。 |  | `Defs/TerrainDefs/Terrain_Planets.xml` |
| `Stellaris_DeadRegolith` | 松散风化层 | 由亿万年微陨石撞击粉碎出的灰色岩屑层，松散、干燥且没有任何有机质。 |  | `Defs/TerrainDefs/Terrain_Planets.xml` |
| `Stellaris_DeadStaticGlass` | 静电玻璃沉积层 | 被辐射固结的玻璃状沉积层，表面有细碎裂纹并积累危险静电。 |  | `Defs/TerrainDefs/Terrain_Planets.xml` |
| `Stellaris_StoneCorroded_Rough` | 腐蚀岩 | 在酸性大气压下形成蜂窝状的古老岩层。它锋利、不平坦，行走起来很危险，但足够坚固，可以支撑重型结构。 |  | `Defs/TerrainDefs/Terrain_Planets.xml` |
| `StellarisShipFakeFloorInsideShip` | ship | 地图地形 Def。 | Parent=StellarisShipFloorBase | `Defs/TerrainDefs/Terrain_Space.xml` |

### MapGeneratorDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `Stellaris_FrozenPlanetGenerator` | frozen planet generation | 地图生成器 Def。 |  | `Defs/MapGeneratorDefs/MapGenerator_FrozenPlanet.xml` |
| `Stellaris_LostPlanetGenerator` | lost planet generation | 地图生成器 Def。 |  | `Defs/MapGeneratorDefs/MapGenerator_LostPlanet.xml` |
| `StellarisAcidBarrensGenerator` | acid barrens generation | 地图生成器 Def。 | Parent=StellarisPlanetMapBase | `Defs/MapGeneratorDefs/PlanetsMapGenerator.xml` |
| `StellarisCommonPlanetGenerator` |  | 地图生成器 Def。 | Parent=StellarisPlanetMapBase | `Defs/MapGeneratorDefs/PlanetsMapGenerator.xml` |
| `StellarisLavaPlanetGenerator` |  | 地图生成器 Def。 | Parent=StellarisPlanetMapBase | `Defs/MapGeneratorDefs/PlanetsMapGenerator.xml` |
| `Stellaris_DeadPlanetGenerator` | dead planet generation | 地图生成器 Def。 |  | `Defs/MapGeneratorDefs/PlanetsMapGenerator.xml` |
| `StellarisSpace` | space | 地图生成器 Def。 |  | `Defs/MapGeneratorDefs/SpaceMapGenerator.xml` |

### GenStepDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `Stellaris_GenStep_FrozenPlanetTerrain` |  | 地图生成步骤 Def。 |  | `Defs/GenStepDefs/GenSteps_FrozenPlanet.xml` |
| `Stellaris_GenStep_LostPlanetTerrain` |  | 地图生成步骤 Def。 |  | `Defs/GenStepDefs/GenSteps_LostPlanet.xml` |
| `StellarisGenStep_Acidification` |  | 地图生成步骤 Def。 |  | `Defs/GenStepDefs/PlanetGenSteps.xml` |
| `Stellaris_GenStep_DeadPlanetTerrain` |  | 地图生成步骤 Def。 |  | `Defs/GenStepDefs/PlanetGenSteps.xml` |
| `Stellaris_GenStep_FusionReactorArchaeologicalSite` |  | 地图生成步骤 Def。 |  | `Defs/SitePartDefs/Stellaris_ArchaeologicalSites.xml` |
| `Stellaris_GenStep_ArchaeologicalSite` |  | 地图生成步骤 Def。 |  | `Defs/Sites/Outpost.xml` |

### WorldGenStepDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `StellarisWorldGenStepLavaPlanet` |  | 世界地块生成步骤 Def。 |  | `Defs/WorldGeneration/WorldGenerator.xml` |
| `StellarisWorldGenStepToxicPlanet` |  | 世界地块生成步骤 Def。 |  | `Defs/WorldGeneration/WorldGenerator.xml` |
| `Stellaris_WorldGenStep_DeadPlanet` |  | 世界地块生成步骤 Def。 |  | `Defs/WorldGeneration/WorldGenerator.xml` |
| `Stellaris_WorldGenStep_FrozenPlanet` |  | 世界地块生成步骤 Def。 |  | `Defs/WorldGeneration/WorldGenerator_FrozenPlanet.xml` |
| `Stellaris_WorldGenStep_LostPlanet` |  | 世界地块生成步骤 Def。 |  | `Defs/WorldGeneration/WorldGenerator_LostPlanet.xml` |

### WeatherDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `StellarisAcidSmog` | 硫酸烟雾 | 一层厚重且令人窒息的硫磺云层和高压雾霾。会对绝大多数衣物造成腐蚀。 |  | `Defs/WeatherDefs/Weathers.xml` |
| `StellarisNoOxygen` | 缺氧环境 | 这是缺氧环境。通常出现在非宜居行星上。 |  | `Defs/WeatherDefs/Weathers.xml` |
| `Stellaris_DeadStillness` | 绝对死寂 | 没有风、没有云、没有任何声音传播介质。恒星光线直接照射地表，阴影区域则迅速坠入极寒。 |  | `Defs/WeatherDefs/Weathers.xml` |
| `Stellaris_ElectrostaticDust` | 静电尘暴 | 没有大气层中的风，却有静电荷失衡引发的粉尘喷涌。细粉尘会遮蔽视线、干扰设备，并让所有户外行动变得困难。 |  | `Defs/WeatherDefs/Weathers.xml` |
| `Stellaris_FrozenBlizzard` | 极寒暴风雪 | 强风卷起细碎冰晶和雪雾，能见度降低，温度急剧下降。户外工作和远程射击都会受到明显影响。 |  | `Defs/WeatherDefs/Weathers_FrozenPlanet.xml` |
| `Stellaris_FrozenStillness` | 冰封寂静 | 冰封行星少有的平稳天气。天空冷白，空气清澈，温度依然极低。 |  | `Defs/WeatherDefs/Weathers_FrozenPlanet.xml` |
| `Stellaris_LostAshHaze` | 废墟灰霾 | 腐朽城市与工业尘埃形成的低能见度灰霾。空气勉强存在，却夹杂细粉尘、惰性气体和毒性残留物。 |  | `Defs/WeatherDefs/Weathers_LostPlanet.xml` |
| `Stellaris_LostCorrosiveDrizzle` | 腐蚀性薄雨 | 从残余工业云层中凝结出的弱酸性降水。雨量不大，但足以腐蚀裸露金属与劣质防护服。 |  | `Defs/WeatherDefs/Weathers_LostPlanet.xml` |
| `Stellaris_LostReactorGlow` | 反应堆余辉 | 地下衰变反应堆与熔封数据核心释放出病态蓝绿辉光，照亮玻璃化地表并携带持续辐射风险。 |  | `Defs/WeatherDefs/Weathers_LostPlanet.xml` |

### ThingDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `Stellaris_AshFilterMask` | 火山灰过滤面罩 | 过滤火山灰、硫化物与金属蒸气的呼吸面罩，本身并不足以应对熔融行星的极端高温。 | Parent=HatMakeableBase | `Defs/ThingDefs_Apparel/Apparel_ExtremePlanet.xml` |
| `Stellaris_CryoHelmet` | 深寒头盔 | 为冰封行星设计的工业级深寒头盔，拥有优异的保温层，但并非完整真空装备。 | Parent=HatMakeableBase | `Defs/ThingDefs_Apparel/Apparel_ExtremePlanet.xml` |
| `Stellaris_CryoParka` | 深寒大衣 | 用于冰封行星的厚重深寒大衣，可在接近-120°C的环境中提供稳定保温，但真空适应能力有限。 | Parent=ApparelBase | `Defs/ThingDefs_Apparel/Apparel_ExtremePlanet.xml` |
| `Stellaris_DeadWorldEVAHelmet` | 死寂EVA头盔 | 为死寂行星与无大气环境设计的密封头盔，能够抵御真空、强辐射与剧烈昼夜温差。 | Parent=HatMakeableBase | `Defs/ThingDefs_Apparel/Apparel_ExtremePlanet.xml` |
| `Stellaris_DeadWorldEVASuit` | 死寂EVA服 | 为死寂行星设计的重型密封环境服，穿齐套装后能够完全适应真空，并显著降低环境性中毒累积。 | Parent=ApparelBase | `Defs/ThingDefs_Apparel/Apparel_ExtremePlanet.xml` |
| `Stellaris_HeatedCryoHelmet` | 主动加热深寒头盔 | 具备主动加热与密封结构的深寒头盔，用于冰封行星上的长期重度作业与真空环境登陆。 | Parent=HatMakeableBase | `Defs/ThingDefs_Apparel/Apparel_ExtremePlanet.xml` |
| `Stellaris_HeatedCryoSuit` | 主动加热深寒服 | 为冰封行星重度作业设计的高级密封深寒服，兼容室温飞船环境，并可在穿齐套装时提供完整真空适应性。 | Parent=ApparelBase | `Defs/ThingDefs_Apparel/Apparel_ExtremePlanet.xml` |
| `Stellaris_MagmaVisorHelmet` | 熔岩环境观察头盔 | 高温密封头盔，能够反射强辐射热并过滤火山灰与金属蒸气，是熔融行星重型作业套装的一部分。 | Parent=HatMakeableBase | `Defs/ThingDefs_Apparel/Apparel_ExtremePlanet.xml` |
| `Stellaris_MagmaticHeatSuit` | 岩浆隔热服 | 为熔融行星设计的重型隔热密封服，可抵御接近900°C的极端高温，并在穿齐套装时提供完整真空适应性。 | Parent=ApparelBase | `Defs/ThingDefs_Apparel/Apparel_ExtremePlanet.xml` |
| `Stellaris_AncientFusionReactor` | 远古聚变反应堆 | 一个古老的聚变反应堆。看上去已经损坏了，但我们依旧能从中得到不少关于聚变反应堆的科技应用。 | Comps=Stellaris.CompProperties_ReverseEngineer; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Buildings_ArchaeologicalSite.xml` |
| `Stellaris_PaperInfo` | 写满字的散乱纸堆 | 一堆乱七八糟的草稿纸，上面写满了字。 | Comps=Stellaris.CompProperties_TextData; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Buildings_ArchaeologicalSite.xml` |
| `Stellaris_StoneInfo` | 刻字石碑 | 刻有文字的石碑。 | Comps=Stellaris.CompProperties_TextData; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Buildings_ArchaeologicalSite.xml` |
| `Stellaris_FrozenLuminousIceCrystal` | 发光冰晶簇 | 在冰下洞穴中缓慢生长的半透明冰晶簇，内部封存的微量矿物会折射出淡蓝绿色光芒。它主要作为环境装饰，也能在黑暗洞穴中提供少量光照。 | ThingClass=Building; Comps=CompProperties_Glower; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Buildings_FrozenPlanet.xml` |
| `StellarisAutonomousMiner` | 无人采矿机 | 自动化的太空采矿设备，发射后会自动开采矿物并返回运输仓。 | ThingClass=Stellaris.AutonomousMiner; Comps=CompPowerTrader, CompProperties_Flickable; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Buildings_Ship.xml` |
| `StellarisLargeChemfuelTank` | 化学燃油存储箱 | 用于存储化学燃料推进器所需要的化学燃料的电控存储箱。 | Comps=CompProperties_Refuelable, Stellaris.CompShipPowerPlant; Parent=StellarisFuelTankBase | `Defs/ThingDefs_Buildings/Buildings_Ship.xml` |
| `StellarisLifeSupportDevice` | 维生装置 | 通过装置内小型生态系统循环而不断释放出氧气，吸收二氧化碳的维生装置。同时也能负责水循环等维持生命的必要循环。\n相较于氧气泵而言更高效。 | Comps=CompPowerTrader, CompProperties_LowPowerUnlessVacuum, CompProperties_Flickable 等 5 个; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Buildings_Ship.xml` |
| `StellarisNuclearFissionReactor` | 核裂变反应堆 | 基于核裂变原理的核反应堆。能够产出可观的热量并转化为电力。可以通过调节中子通量率来调节热量产生速率，进而改变发电量。但也会改变燃料仓的消耗速率。 | ThingClass=Building; Comps=Stellaris.CompPowerDynamicPlant, CompProperties_Flickable, CompProperties_Refuelable 等 8 个; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Buildings_Ship.xml` |
| `StellarisPlanetScanner` | 天体扫描仪 | 能够通过特定手段远距离扫描舰船所处行星或恒星的扫描器。\n可以从扫描数据中分析出天体的各种信息，例如温度，亮度，宜居性，蕴含资源等。 | Comps=CompProperties_Forbiddable, CompPowerTrader, CompProperties_Breakdownable 等 5 个; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Buildings_Ship.xml` |
| `StellarisSafeSuperBattery` | 电容阵列 | 一种采用了先进绝缘技术的高容量电池。它能够储存大量电力，转换效率极高，并且经过特殊处理，即不会发生短路。 | ThingClass=Building_Battery; Comps=CompProperties_Battery, Stellaris.CompProperties_SafeBatteryProtector; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Buildings_Ship.xml` |
| `StellarisShipConsole` | 舰桥 | 舰船的控制中心。需要驾驶员驾驶。能够操控舰船起飞与降落，武器的开火，舰船移动等。 | ThingClass=Building; Comps=Stellaris.CompShipPowerPlant, CompProperties_Flickable, Stellaris.CompProperties_ShipControl 等 5 个; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Buildings_Ship.xml` |
| `StellarisShipEngine` | 化学燃料推进器 | 给舰船提供充足动力的化学燃料推进器。对燃料的利用效率较低，但至少能够将舰船送上太空。 | ThingClass=Building; Comps=Stellaris.CompShipPowerPlant, Stellaris.CompProperties_ShipThruster, Stellaris.CompProperties_ShipPart; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Buildings_Ship.xml` |
| `StellarisShipHullTile` | 船体甲板 | 铺设在舰船结构上下的甲板，防止气体逸散在真空中的同时为船体提供结构支撑。 | Comps=Stellaris.CompProperties_ShipPart; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Buildings_Ship.xml` |
| `StellarisShipWall` | 船体 | 气密性极高的墙壁。内部铺设了各种各样的线缆，管道。可以阻止房间内气体逸散到真空中。 | ThingClass=Building; Comps=CompProperties_MeditationFocus, Stellaris.CompProperties_ShipPart, CompPowerTransmitter; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Buildings_Ship.xml` |
| `StellarisSpaceMiningPad` | 太空采矿站基座 | 用于建造和发射无人采矿机的基座设施。受发射功率限制，只能在星际空间内飞船上发射。 | ThingClass=Stellaris.SpaceMiningPad; Comps=CompPowerTrader, CompProperties_Flickable; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Buildings_Ship.xml` |
| `StellarisTemperatureStabilizer` | 温度调控器 | 一种高科技装置，将室内温度稳定在特定目标值。依靠电加热。冷却时通过舰船将能量导向外界。 | ThingClass=Stellaris.Building_TemperatureStabilizer; Comps=CompPowerTrader, CompProperties_TempControl; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Buildings_Ship.xml` |
| `StellarisVacuumBarrier` | 真空屏障 | 将气体分隔的屏障。可以阻止房间内气体逸散到真空中。 | Comps=CompPowerPlant, CompProperties_Breakdownable, CompProperties_Styleable 等 4 个; Parent=DoorBase | `Defs/ThingDefs_Buildings/Buildings_Ship.xml` |
| `Stellaris_ShipLandingController` | ship landing sequence | 舰船、太空设施或船体结构建筑 Def。 | ThingClass=Stellaris.ShipLandingController | `Defs/ThingDefs_Buildings/Buildings_Ship.xml` |
| `Stellaris_ShipTakeoffController` | ship takeoff sequence | 舰船、太空设施或船体结构建筑 Def。 | ThingClass=Stellaris.ShipTakeoffController | `Defs/ThingDefs_Buildings/Buildings_Ship.xml` |
| `Stellaris_ShieldGen` | 护盾发生器 | 创建一个防护圆顶，可以阻挡来袭的射弹，但允许传出的火力。 | ThingClass=Stellaris.Building_StellarisShield; Comps=Stellaris.CompShipPowerPlant; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Buildings_ShipCombats.xml` |
| `Stellaris_ARGOSCamera` | 阿尔戈斯监控节点 | 墙角的古代监控节点，镜头外壳布满灰尘。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_ARGOSDroneDock` | 阿尔戈斯无人机停泊槽 | 地面停泊槽里还有断电的巡逻机接口。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_ARGOSSecurityCore` | 阿尔戈斯安保核心 | 独立供电的安保主机仍保持整洁，服务器柱之间有稳定的冷光。 | Comps=Stellaris.CompProperties_StellarisArchaeologyWork; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_ARGOSWallTurret` | 阿尔戈斯墙嵌火力点 | 嵌在墙体里的古代自动火力点。它使用原版炮塔逻辑，由 ARGOS 机房的集中电网供能。 | ThingClass=Building_TurretGun; Comps=CompPowerTrader, CompProperties_Flickable; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_ActivatedMetalDebris` | 活化金属残骸 | 被高能中子与长年热冲击活化的反应堆残骸。它主要作为危险遗址中的装饰性废墟。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_AncientCapacitorBank` | 古代点火电容阵列 | 两排厚重电容柱之间布满电弧烧痕，绝缘陶瓷已经开裂。 | Comps=Stellaris.CompProperties_StellarisArchaeologyWork; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_AncientFusionFuelCanister` | 古代聚变燃料罐 | 封存在厚墙燃料仓里的古代聚变燃料罐。谨慎开启能回收氘氚燃料和氦-3，粗暴破坏会造成氚泄漏。 | Comps=Stellaris.CompProperties_StellarisArchaeologyWork; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_AncientFusionReactorCore` | 古代聚变反应堆核心 | 城市级聚变反应堆的残存核心。它不会像裂变堆一样链式核爆，但残余等离子体、高温活化金属、旧电容与安保 AI 仍足以杀死粗心的探索者。 | Comps=Stellaris.CompProperties_StellarisArchaeologyWork, Stellaris.CompProperties_StellarisArchaeologyWork, Stellaris.CompProperties_StellarisArchaeologyWork; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_AncientSecurityTerminal` | 古代安保终端 | 安保检查站的装甲访问终端。骇入它能关闭部分入口防御，并降低核心区安保 AI 的反制强度。 | Comps=Stellaris.CompProperties_StellarisArchaeologyWork; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_BrokenPowerPylon` | 倒塌输电塔残骸 | 地表输能塔坍塌后的扭曲钢架，断开的电缆埋进尘土。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_BunkLocker` | 驻站储物柜 | 生活区的个人储物柜，有些柜门半开。 | Comps=Stellaris.CompProperties_StellarisArchaeologyWork; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_CargoPalletDebris` | 货运托盘残骸 | 物流托盘、箱体和绑带散在地上。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_CollapsedCatwalk` | 坍塌检修栈桥 | 从高处摔落的检修栈桥，金属格栅和护栏挤成一团。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_CoolantPumpColumn` | 巨型冷却泵柱 | 高大的冷却泵柱，管壁凝着白霜与灰尘。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_CrackedMonitorWall` | 破裂监控屏墙 | 多联监控屏幕，有些完全熄灭，有些只剩下雪花。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_DamagedCoolantManifold` | 损坏冷却歧管 | 断裂的冷却与排放管线。它无法完全修复，但可以确认排放井和残余电荷状态。 | Comps=Stellaris.CompProperties_StellarisArchaeologyWork; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_DamagedMagneticCoil` | 损毁磁约束线圈 | 围绕核心的超导线圈残段，部分外壳扭曲撕裂。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_DestroyedSecurityDrone` | 损毁安保无人机 | 被近距离打穿的古代安保无人机残骸。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_EmergencyOxygenBottles` | 空氧气瓶堆 | 一堆用尽的便携氧气瓶。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_EmergencyQuenchNozzle` | 紧急喷淬喷头 | 指向核心腔的粗大喷淬装置，喷口处有烧蚀痕。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_EmergencySupplyCrate` | 应急补给箱 | 没来得及搬走的应急补给箱，封条已经破裂。 | Comps=Stellaris.CompProperties_StellarisArchaeologyWork; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_FieldMedicalStation` | 站医急救台 | 临时急救台、药柜和断电监护器挤在一起。 | Comps=Stellaris.CompProperties_StellarisArchaeologyWork; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_FuelInjectionBench` | 燃料注入预处理台 | 注入器、压力管线和机械臂停在半动作状态。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_FuelInjectionPort` | 核心燃料注入口 | 连接核心的燃料注入口，管线在事故后被强行切断。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_FusionControlConsole` | 聚变反应堆控制台 | 烧蚀严重的反应堆控制台，仍保存着事故最后十七分钟的运行记录。 | Comps=Stellaris.CompProperties_StellarisArchaeologyWork; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_FusionFuelRack` | 聚变燃料货架 | 加固货架上固定着几只冷冻燃料罐。 | Comps=Stellaris.CompProperties_StellarisArchaeologyWork; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_FusionSitePersonalLog` | 员工个人日志终端 | 生活区角落里仍接着应急电池的小型终端，外壳上有手指擦出的灰痕。 | Comps=Stellaris.CompProperties_StellarisArchaeologyWork; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_LastStandBarricade` | 临时掩体 | 用桌子、箱体和金属板匆忙堆出的掩体。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_MagneticPulseEmitter` | 不稳定电磁脉冲源 | 破裂线圈旁的电磁节点，附近散落物被吸向同一方向。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_ManualBreakerPanel` | 手动断路面板 | 手动断路面板旁有烧焦手印和切割痕。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_MessTableClutter` | 配餐桌残骸 | 配餐桌、餐盘和饮料包散乱地停在事故发生时的状态。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_MobileRepairRig` | 移动抢修架 | 推到现场的抢修架，接线还挂在半空。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_ObservationWindow` | 强化观察窗 | 面向核心区的厚重观察窗，玻璃布满冲击裂纹。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_OfficeDebris` | 调度室办公残骸 | 翻倒的座椅、碎屏和文件盒混在一起。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_PipeDiagramBoard` | 管线示意板 | 维护区墙上的管线示意板，部分线路被手工圈出。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_PowerConduitBundle` | 裸露电缆束 | 从墙体和地板里拖出的粗电缆束。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_PowerRoutingWallMap` | 供能调度墙图 | 占据墙面的旧供能示意图，线路指示灯停在不同颜色。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_ReactorBurnthroughPit` | 反应堆熔穿井 | 核心下方的熔穿井，边缘是凝固金属和玻璃化地面。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_ReactorDispatchConsole` | 反应堆调度终端 | 行政调度室残留的能源分配终端。屏幕在断续刷新，几个负载区域始终停在红色。 | Comps=Stellaris.CompProperties_StellarisArchaeologyWork; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_ReactorHallSealConsole` | 核心大厅密封控制台 | 控制核心大厅隔离墙和停堆联锁的密封控制台。它不保存最终数据，但决定你们是按流程进入核心大厅，还是强行破封。 | Comps=Stellaris.CompProperties_StellarisArchaeologyWork, Stellaris.CompProperties_StellarisArchaeologyWork; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_RupturedCoolantVent` | 破裂冷却蒸汽口 | 破裂喷口周围结着冷却剂白霜，偶尔有残余蒸汽冒出。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_ShelterEmergencyLog` | 避难区紧急日志 | 地下避难区遗留的数据终端。外壳接着最后一块应急电池，屏幕停在工程主管授权序列上。 | Comps=Stellaris.CompProperties_StellarisArchaeologyWork; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_ToolCrateDebris` | 工具箱残骸 | 翻开的工具箱与散落零件。 | Comps=Stellaris.CompProperties_StellarisArchaeologyWork; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_WeaponLockerDebris` | 武器柜残骸 | 被撬开或撞裂的安保武器柜。 | Comps=Stellaris.CompProperties_StellarisArchaeologyWork; Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_WorkbenchClutter` | 工程值班台 | 值班台上摊着工具、接线盒和未收起的维修记录。 | Parent=BuildingBase | `Defs/ThingDefs_Buildings/Stellaris_ArchaeologyBuildings.xml` |
| `Stellaris_AncientTechFragment` | 远古科技碎片 | 从遗落文明设备、数据板或损坏智库核心中回收的高密度科技残片。它无法直接理解，但可作为逆向工程、稀有科技解锁或遗落科技制造的核心材料。 | Parent=ResourceBase | `Defs/ThingDefs_Item/Items_LostPlanet.xml` |
| `StellarisResourceDeuterium` | 氘氚燃料(DT燃料) | 氘氚混合物。\n氘是氢的同位素，又称重氢，化学符号为D或2H，常温下氘气是一种无色、无味的可燃性气体，在地球上的丰度为0.015%，它在普氢中的含量很少，且大多以重水D2O即氧化氘形式存在于海水与普通水中。\n氚（Tritium）是氢的一种放射性同位素，原子核内含一个质子和两个中子。由于其放射性，氚在自然界中极为稀少，主要通过人工方式生产，如核反应堆中的锂核反应。 | Parent=ResourceBase | `Defs/ThingDefs_Item/Items_Resource_Stuff.xml` |
| `StellarisResourceExoticCrystals` | 稀有水晶 | 一块散发着非自然内部光芒的晶体碎片。它的形成需要极端压力、高温以及微量奇异粒子——这种条件极为罕见。晶体的晶格以近乎不可察觉的频率持续振动，缓慢释放储存的能量。它可用作武器的聚焦透镜，或仅仅作为一颗美丽的异星宝石被珍藏。 | Parent=ResourceBase | `Defs/ThingDefs_Item/Items_Resource_Stuff.xml` |
| `StellarisResourceHelium` | 氦3 | 氦-3是氦的稳定同位素，化学符号³He，原子核由两个质子和一个中子组成，常温下为无色无味无臭气体，通常以高压气瓶储存。 | Parent=ResourceBase | `Defs/ThingDefs_Item/Items_Resource_Stuff.xml` |
| `StellarisResourceOrganic` | 简单有机物 | 简单有机物聚合体，含有各种烃。 | Parent=ResourceBase | `Defs/ThingDefs_Item/Items_Resource_Stuff.xml` |
| `StellarisResourceRareMetal` | 稀土 | 稀土元素（rare earth element [48]），又称“工业维生素”、稀土金属， [48] [49]是元素周期表ⅢB族中原子序数为21、39和57~71的17种化学元素的总称，化学符号用RE代表。 [48]包括镧、铈、镨、钕、钷、钐、铕、钆、铽、镝、钬、铒、铥、镱、镥、钪、钇等元素。 [32]其中57~71号元素称为镧系元素。 [48]稀土属于不可再生资源， [56]为银白色或灰色金属，但镨、钕是略带浅黄色金属。大部分金属呈密集六方晶格或面心立方晶格。常温下，稀土金属是顺磁性的。 [50]此外，稀土金属化学性质活泼，在空气中能迅速被氧化，失去光泽而变暗。稀土金属具有仅次于碱金属和碱土金属的强还原性。 | Parent=ResourceBase | `Defs/ThingDefs_Item/Items_Resource_Stuff.xml` |
| `StellarisResourceUraniumFuelCapsule` | 铀燃料仓 | 用高密度金属包裹的放射性铀同位素的燃料仓。 | Parent=ResourceBase | `Defs/ThingDefs_Item/Items_Resource_Stuff.xml` |
| `Stellaris_AncientFuelInjector` | 古代燃料注入器 | 聚变燃料系统中的高精度注入器，接口处有未清除的冷冻燃料残留。 | Parent=ResourceBase | `Defs/ThingDefs_Items/Stellaris_ArchaeologyItems.xml` |
| `Stellaris_AncientFusionCoreModule` | 古代聚变核心模块 | 从核心内层完整拆出的高价值模块，仍被多层屏蔽壳包住。 | Parent=ResourceBase | `Defs/ThingDefs_Items/Stellaris_ArchaeologyItems.xml` |
| `Stellaris_ChiefEngineerOverrideKey` | 工程主管授权钥 | 一枚烧蚀严重的古代授权数据钥。它是那名工程主管最后覆盖 AI 指令的证据，也是一件罕见纪念品。 | Parent=ResourceBase | `Defs/ThingDefs_Items/Stellaris_ArchaeologyItems.xml` |
| `Stellaris_DamagedFusionBlueprint` | 破损聚变反应堆蓝图 | 强行提取数据后得到的残缺聚变蓝图。它包含有价值的设计片段，但许多关键安全序列已经永久丢失。 | Parent=ResourceBase | `Defs/ThingDefs_Items/Stellaris_ArchaeologyItems.xml` |
| `Stellaris_DeuteriumTritiumFuel` | 氘氚燃料 | 封存在低温燃料芯中的氘氚混合燃料。它可用于聚变相关科技、舰船能源设备和未来的高能反应堆配方。 | Parent=ResourceBase | `Defs/ThingDefs_Items/Stellaris_ArchaeologyItems.xml` |
| `Stellaris_FusedReactorGlass` | 反应堆熔凝玻璃 | 熔穿井边缘回收的玻璃化碎片，里面夹着细小金属颗粒。 | Parent=ResourceBase | `Defs/ThingDefs_Items/Stellaris_ArchaeologyItems.xml` |
| `Stellaris_FusionControlDataCore` | 聚变控制数据核心 | 从控制系统中拆出的数据核心，外壳有高温熏黑和重新焊接痕迹。 | Parent=ResourceBase | `Defs/ThingDefs_Items/Stellaris_ArchaeologyItems.xml` |
| `Stellaris_FusionReactorBlueprint` | 完整聚变反应堆蓝图 | 从古代聚变核心中完整恢复的反应堆蓝图。它可作为聚变科技研究奖励，并能直接完成 StellarisResearch_NuclearFusionReactor 研究。 | Parent=ResourceBase | `Defs/ThingDefs_Items/Stellaris_ArchaeologyItems.xml` |
| `Stellaris_Helium3Canister` | 氦-3燃料罐 | 稀有的氦-3燃料罐，可作为高级聚变、舰船能源和深空工程材料。 | Parent=ResourceBase | `Defs/ThingDefs_Items/Stellaris_ArchaeologyItems.xml` |
| `Stellaris_SuperconductiveCoilFragment` | 超导线圈残片 | 从磁约束环上回收的超导材料残片，断面仍有蓝绿色斑点。 | Parent=ResourceBase | `Defs/ThingDefs_Items/Stellaris_ArchaeologyItems.xml` |
| `ActiveSpaceDropPod` | 太空运输仓 | 特殊运输仓、投送物或内部机制 ThingDef。 | ThingClass=ActiveTransporter | `Defs/ThingDefs_Misc/Things_Special.xml` |
| `SpaceDropPodIncoming` |  | 特殊运输仓、投送物或内部机制 ThingDef。 | Parent=DropPodIncoming | `Defs/ThingDefs_Misc/Things_Special.xml` |
| `StellarisToxMycelium` | 嗜毒菌丝 | 这是一种生物工程真菌，以污染为主要能量来源，能主动从土壤中吸收污染物，从而为其快速繁殖提供能量。\n\n成熟的菌丝会不断向周围蔓延、生长。 | Comps=CompProperties_Glower, Stellaris.CompProperties_ToxMycelium; Parent=StellarisCavePlantBase | `Defs/ThingDefs_Plants/Plants_Cave.xml` |
| `StellarisToxSporePod` | 嗜毒菌营养聚合物 | 从嗜毒菌丝采集的一种密集且具有脉动的真菌荚果。它过滤掉了重金属，并将营养物质压缩成高热量包。但是看起来一点儿也不好吃，散发着神秘绿光。 | Comps=CompProperties_Rottable; Parent=OrganicProductBase | `Defs/ThingDefs_Plants/Plants_Cave.xml` |

### RecipeDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `Make_StellarisResourceUraniumFuelCapsule` | 制作铀燃料仓 | 制作铀燃料仓。 |  | `Defs/RecipeDefs/Recipes_Production.xml` |

### ResearchTabDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `StellarisResearchTab` | 星际科技 | Def 配置。 |  | `Defs/ResearchDefs/ResearchTabs.xml` |

### ResearchProjectDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `StellarisResearch_ExtremeEnvironmentSurvival` | 极端环境生存 | 系统研究极端行星登陆所需的密封、隔热、保温、过滤与生命维持接口技术。完成后，可在精密装配台制作死寂、冰封与熔融行星专用防护装备。 | Parent=StellarisResearchProjectBase | `Defs/ResearchDefs/ResearchProjects_ExtremeEnvironment.xml` |
| `StellarisResearch_BioDigester` | 污染消化仓 | 经过基因改造的细菌，专为高效分解有毒废物而设计。废物越积越多，菌群活性越强，处理速度呈指数级提升。 | Parent=StellarisResearchProjectBase | `Defs/ResearchDefs/ResearchProjects_Ship.xml` |
| `StellarisResearch_LifeSupport` | 大型维生装置 | 通过装置内小型生态系统循环而不断释放出氧气，吸收二氧化碳的维生装置。同时也能负责水循环等维持生命的必要循环。\n相较于氧气泵而言更高效。 | Parent=StellarisResearchProjectBase | `Defs/ResearchDefs/ResearchProjects_Ship.xml` |
| `StellarisResearch_NuclearFissionReactor` | 裂变反应堆 | 基于核裂变原理的核反应堆。能够产出可观的热量并转化为电力。可以通过调节中子通量率来调节热量产生速率，进而改变发电量。但也会改变燃料仓的消耗速率。 | Parent=StellarisResearchProjectBase | `Defs/ResearchDefs/ResearchProjects_Ship.xml` |
| `StellarisResearch_NuclearFusionReactor` | 聚变反应堆 | 氘核和氚核在一定条件下（如超高温、高压）可以聚变为氦核，在发生聚变反应的同时，会释放出巨大能量。核聚变反应所产生的能量比核裂变反应所产生的能量更大。恒星内部连续进行着氢聚变成氦的过程，它的光和热就是由这种不断的核聚变反应产生的。\n\n我们要做的，就是利用聚变反应产生的巨大能量，让它为我们所用。 | Parent=StellarisResearchProjectBase | `Defs/ResearchDefs/ResearchProjects_Ship.xml` |
| `StellarisResearch_OrbitalRailgun` | 轨道炮 | 轨道炮（Rail Gun）， 也被称作电磁感应炮、电磁投射炮或电磁加速炮，是电磁炮最常见的式样，是在有电位差的两条轨道间放置可导电式炮弹，当两轨接入电源时，产生的电磁场将炮弹加速射出的一种高能武器。轨道炮速度快，比普通枪弹的速度快2—3倍。带有巨大动能的弹丸通过直接撞击目标将其摧毁，威力极大。同时极高的飞行速度可以减少炮弹的飞行时间，使炮弹不易受到干扰，保证了炮弹的精度。轨道炮的炮弹体积小，重量轻。炮弹几乎不使用推进剂，减少了装药量，所以炮弹的体积只是传统120毫米口径火炮炮弹的八分之一，重量是其十分之一，这样可显著提高武器系统的携弹量，减少后勤负担。 | Parent=StellarisResearchProjectBase | `Defs/ResearchDefs/ResearchProjects_Ship.xml` |
| `StellarisResearch_PlanetScanner` | 天体扫描仪 | 能够通过特定手段远距离扫描舰船所处行星或恒星的扫描器。\n可以从扫描数据中分析出天体的各种信息，例如温度，亮度，宜居性，蕴含资源等。 | Parent=StellarisResearchProjectBase | `Defs/ResearchDefs/ResearchProjects_Ship.xml` |
| `StellarisResearch_SafeSuperBattery` | 电容阵列 | 一种采用了先进绝缘技术的高容量电池。它能够储存大量电力，转换效率极高，并且经过特殊处理，即不会发生短路。 | Parent=StellarisResearchProjectBase | `Defs/ResearchDefs/ResearchProjects_Ship.xml` |
| `StellarisResearch_Sheild` | 偏射能量盾 | 能量偏射屏障可在过载前化解一定敌方火力，主要缺点是能耗高。 | Parent=StellarisResearchProjectBase | `Defs/ResearchDefs/ResearchProjects_Ship.xml` |
| `StellarisResearch_ShipConsole` | 舰船控制台 | 舰船的控制中心。需要驾驶员驾驶。能够操控舰船起飞与降落，武器的开火，舰船移动等。 | Parent=StellarisResearchProjectBase | `Defs/ResearchDefs/ResearchProjects_Ship.xml` |
| `StellarisResearch_ShipEngine` | 化学燃料推进器 | 给舰船提供充足动力的化学燃料推进器。对燃料的利用效率较低，但至少能够将舰船送上太空。 | Parent=StellarisResearchProjectBase | `Defs/ResearchDefs/ResearchProjects_Ship.xml` |
| `StellarisResearch_SpaceMining` | 太空采矿 | 自动化的太空采矿设备能够在已经探明的天体上进行开采作业，并且自动发射运输仓运回产物。 | Parent=StellarisResearchProjectBase | `Defs/ResearchDefs/ResearchProjects_Ship.xml` |
| `StellarisResearch_TemperatureStabilizer` | 舰船温度调控器 | 一种高科技装置，将室内温度稳定在特定目标值。依靠电加热。冷却时通过舰船将能量导向外界。 | Parent=StellarisResearchProjectBase | `Defs/ResearchDefs/ResearchProjects_Ship.xml` |

### JobDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `StellarisPlanetScanJob` |  | 工作/行为 JobDef。 | Driver=Stellaris.JobDriver_OperatePlanetScanner | `Defs/JobDefs/Jobs_Work.xml` |
| `StellarisShipConsoleJob` |  | 工作/行为 JobDef。 | Driver=Stellaris.JobDriver_Console | `Defs/JobDefs/Jobs_Work.xml` |
| `Stellaris_ReverseEngineerJob` |  | 工作/行为 JobDef。 | Driver=Stellaris.JobDriver_ReverseEngineer | `Defs/JobDefs/Jobs_Work.xml` |
| `Stellaris_ArchaeologyWorkJob` |  | 工作/行为 JobDef。 | Driver=Stellaris.JobDriver_StellarisArchaeologyWork | `Defs/JobDefs/Stellaris_ArchaeologyJobs.xml` |

### WorkGiverDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `Stellaris_DoArchaeologyWork` | 处理 Stellaris 考古目标 | 工作扫描器 WorkGiver Def。 | Giver=Stellaris.WorkGiver_StellarisArchaeologyWork | `Defs/WorkGiverDefs/Stellaris_ArchaeologyWorkGivers.xml` |
| `StellarisPlanetScannerWorkGiver` | 在天体扫描仪扫描 | 工作扫描器 WorkGiver Def。 | Giver=Stellaris.WorkGiver_OperatePlanetScanner | `Defs/WorkGiverDefs/WorkGivers.xml` |
| `Stellaris_DoReverseEngineer` | 逆向工程外星建筑 | 工作扫描器 WorkGiver Def。 | Giver=Stellaris.WorkGiver_ReverseEngineer | `Defs/WorkGiverDefs/WorkGivers.xml` |

### MainButtonDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `GalaxyMap` | Galaxy Map | View and explore the galactic cluster | Worker=Stellaris.MainButtonWorker_GalaxyMap | `Defs/MainButtonDefs/MainButtons.xml` |

### Stellaris.ArchaeologicalSiteDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `Stellaris_ArchaeologicalSiteA` | 坍塌的聚变反应堆遗址 | 一座远古聚变反应堆设施的残骸。外围结构已经坍塌，核心舱仍保留着可逆向分析的数据与部件。 |  | `Defs/ArchaeologicalSiteDefs/Outpost.xml` |
| `Stellaris_FusionReactorArchaeologicalSite` | 黎明核心聚变阵列遗址 | 一座深埋在废墟下的古代聚变阵列。地表只剩输电塔和物流平台残骸，地下仍保存着生活区、调度室、冷却泵站、安保主机和巨大的环形核心腔。 |  | `Defs/ArchaeologicalSiteDefs/Stellaris_FusionReactorArchaeologicalSite.xml` |

### SitePartDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `Stellaris_FusionReactorArchaeologicalSitePart` | 坍塌的聚变反应堆遗址 | 一处仍在周期性自检的古代聚变反应堆封存遗址。 | Worker=Stellaris.SitePartWorker_ArchaeologicalSite | `Defs/SitePartDefs/Stellaris_ArchaeologicalSites.xml` |
| `Stellaris_ArchaeologicalSite` | 考古站点 | 一处可发掘的远古遗址。 | Worker=Stellaris.SitePartWorker_ArchaeologicalSite | `Defs/Sites/Outpost.xml` |

### WorldObjectDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `UniverseMapParent_Ship` | 太空舰船 | 世界地图/宇宙地图对象 Def。 |  | `Defs/WorldObjectDefs/WorldObjectDefs.xml` |
| `UniverseObject_AutonomousMiner` | 空间采矿站 | 世界地图/宇宙地图对象 Def。 |  | `Defs/WorldObjectDefs/WorldObjectDefs.xml` |
| `WorldObject_EnemyShip` | 敌舰 | 世界地图/宇宙地图对象 Def。 |  | `Defs/WorldObjectDefs/WorldObjectDefs.xml` |

### ScenarioDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `Stellaris_FrozenPlanetTravel` | Stellaris_FrozenPlanetTravel | 冰封行星... | Parent=ScenarioBase | `Defs/Scenarios/Scenario_FrozenPlanet.xml` |
| `Stellaris_LostPlanetTravel` | Stellaris_LostPlanetTravel | 遗落行星... | Parent=ScenarioBase | `Defs/Scenarios/Scenario_LostPlanet.xml` |
| `StellarisLavaPlanetTravel` | StellarisLavaPlanetTravel | 熔融行星... | Parent=ScenarioBase | `Defs/Scenarios/Scenarios_PlanetTravel.xml` |
| `StellarisToxicPlanetTravel` | StellarisToxicPlanetTravel | 剧毒行星... | Parent=ScenarioBase | `Defs/Scenarios/Scenarios_PlanetTravel.xml` |
| `Stellaris_DeadPlanetTravel` | Stellaris_DeadPlanetTravel | 死寂行星... | Parent=ScenarioBase | `Defs/Scenarios/Scenarios_PlanetTravel.xml` |

### ScenPartDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `StellarisPlanetLayerFixed` | surface layer type | Def 配置。 |  | `Defs/Scenarios/ScenParts_Fixed.xml` |

### RoofDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `StellarisShipRoof` | 船舱顶部 | Def 配置。 |  | `Defs/RoofDefs/Roofs.xml` |

### SoundDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `Stellaris_ShipEngineLoop` |  | 音效 Def。 |  | `Defs/SoundDefs/Ship.xml` |
| `Stellaris_ShipIgnition` |  | 音效 Def。 |  | `Defs/SoundDefs/Ship.xml` |

### ThoughtDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `Stellaris_AteToxSpore` |  | 心情/思想 Def。 |  | `Defs/ThingDefs_Plants/Plants_Cave.xml` |
| `Stellaris_FrozenExposed` |  | 心情/思想 Def。 |  | `Defs/WeatherDefs/Weathers_FrozenPlanet.xml` |

### Stellaris.EnemyShipDef

| DefName | 名称 | 用途/描述 | 实现要点 | 文件 |
|---|---|---|---|---|
| `StellarisEnemyShipTest` | 测试敌舰 | 一个测试敌舰。 |  | `Defs/EnemyShipDefs/EnemyShips.xml` |

## Source C# 类型总览

下表按源码文件路径排序。`继承/接口` 列来自源码声明，描述为静态阅读后的概括。

| 文件 | 类型 | 名称 | 继承/接口 | 大概作用 |
|---|---|---|---|---|
| `Archaeology/ArchaeologySiteRecord.cs` | class | `ArchaeologySiteRecord` | IExposable | 可存档考古站点进度记录，保存阶段、标志位、完成结果等。 |
| `Archaeology/ArchaeologyUtility.cs` | class | `ArchaeologyUtility` |  | 考古站点工具类，负责奖励、完成状态、目标查找、消息和触发效果等。 |
| `Archaeology/ArchaeologyUtility.cs` | class | `StellarisThingDefCountRange` |  | Def+数量范围数据结构，用于考古奖励/掉落配置。 |
| `Archaeology/Building_ARGOSWallTurret.cs` | class | `Building_ARGOSWallTurret` | Building | 阿尔戈斯墙嵌炮塔建筑逻辑，支持安保系统激活/阵营设置。 |
| `Archaeology/CompStellarisArchaeologyWork.cs` | class | `CompProperties_StellarisArchaeologyWork` | CompProperties | 考古交互工作的 XML 配置类，定义工作类型、进度、奖励和完成键。 |
| `Archaeology/CompStellarisArchaeologyWork.cs` | class | `CompStellarisArchaeologyWork` | ThingComp | 通用考古交互 ThingComp，处理分析/修复/拆解等进度、Gizmo、完成回调。 |
| `Archaeology/CompStellarisArchaeologyWork.cs` | class | `JobDriver_StellarisArchaeologyWork` | JobDriver | 执行考古交互 Job，按 tick 推进目标 Comp。 |
| `Archaeology/CompStellarisArchaeologyWork.cs` | class | `WorkGiver_StellarisArchaeologyWork` | WorkGiver_Scanner | 扫描可考古交互目标，并向殖民者分配考古工作。 |
| `Archaeology/GenStep_FusionReactorArchaeologicalSite.cs` | class | `GenStep_FusionReactorArchaeologicalSite` | GenStep | 大型聚变反应堆考古站点的程序化地图生成器，生成分区、尸体、布景、安保与核心大厅。 |
| `Archaeology/GenStep_FusionReactorArchaeologicalSite.cs` | enum | `StellarisCorpseRole` |  | 考古站点尸体角色枚举，用于匹配背景、伤势和死亡场景。 |
| `Archaeology/MapComponent_FusionReactorSiteHazards.cs` | class | `MapComponent_FusionReactorSiteHazards` | MapComponent | 聚变站点危险地图组件，处理火灾、EMP、热害等延迟事件。 |
| `Archaeology/MapComponent_FusionSecurityResponseQueue.cs` | class | `MapComponent_FusionSecurityResponseQueue` | MapComponent | 聚变站点安保响应队列，延迟投放或激活机械体/安保反制。 |
| `Archaeology/MapComponent_FusionSecurityResponseQueue.cs` | class | `QueuedSecurityMechLaunch` | IExposable | 可存档安保机械体投放队列项。 |
| `Archaeology/StellarisArchaeologyManager.cs` | class | `StellarisArchaeologyManager` | GameComponent | 全局考古管理器，注册站点、记录进度、应用完成键与最终结算。 |
| `AutonomousMiner.cs` | class | `AutonomousMiner` | Building | 地图上的无人采矿机建筑；发射后转换为宇宙对象并参与行星资源产出。 |
| `Buildings/Building_StellarisShield.cs` | class | `Building_StellarisShield` | Building | 舰船护盾发生器建筑，依据供能绘制并阻挡/处理来袭威胁。 |
| `Buildings/Building_TemperatureStabilizer.cs` | class | `Building_TemperatureStabilizer` | Building_TempControl | 舰船温度调控器，扩展原版温控建筑逻辑。 |
| `Buildings/SpaceMiningPad.cs` | class | `SpaceMiningPad` | Building | 太空采矿站基座，可安装/发射无人采矿机，并周期性投送资源仓。 |
| `CellHighlighter.cs` | class | `CellHighlighter` |  | 地图格高亮绘制工具，用于在 UI 中提示船体/目标区域。 |
| `Command_OpenGalaxyMap.cs` | class | `Command_OpenGalaxyMap` | Command | 打开星系地图窗口的自定义命令按钮。 |
| `Comps/ThingComp/CompOxygenAllRoomPusher.cs` | class | `CompOxygenAllRoomPusher` | ThingComp | 向相邻封闭船舱房间推送氧气/维生状态的 ThingComp。 |
| `Comps/ThingComp/CompOxygenAllRoomPusher.cs` | class | `CompProperties_OxygenAllRoomPusher` | CompProperties | 维生装置供氧 Comp 的 XML 参数类。 |
| `Comps/ThingComp/CompPlanetScanner.cs` | class | `CompPlanetScanner` | ThingComp | 天体扫描仪 Comp，执行扫描进度并发现行星或考古站点。 |
| `Comps/ThingComp/CompPlanetScanner.cs` | class | `CompProperties_PlanetScanner` | CompProperties | 天体扫描仪 Comp 的 XML 参数类。 |
| `Comps/ThingComp/CompPlanetScanner.cs` | class | `Gizmo_PlanetScanProgress` | Gizmo | 天体扫描进度条 Gizmo。 |
| `Comps/ThingComp/CompPowerDynamicPlant.cs` | class | `CompPowerDynamicPlant` | CompPowerTrader | 动态发电 Comp，允许发电量随滑条或状态变化。 |
| `Comps/ThingComp/CompProperties_BioDigester .cs` | class | `CompBioDigester` | ThingComp | 污染消化仓逻辑，处理污染/有机物转化。 |
| `Comps/ThingComp/CompProperties_BioDigester .cs` | class | `CompProperties_BioDigester` | CompProperties | 污染消化仓 Comp 的 XML 参数类。 |
| `Comps/ThingComp/CompProperties_ReactorSlider.cs` | class | `CompProperties_ReactorSlider` | CompProperties | 反应堆功率滑条 Comp 参数类。 |
| `Comps/ThingComp/CompProperties_ToxMycelium.cs` | class | `CompProperties_ToxMycelium` | CompProperties | 嗜毒菌丝 Comp 参数类。 |
| `Comps/ThingComp/CompProperties_ToxMycelium.cs` | class | `CompToxMycelium` | ThingComp | 嗜毒菌丝生长/毒性/产物逻辑。 |
| `Comps/ThingComp/CompShipControl&&Thuster.cs` | class | `CompProperties_ShipControl` | CompProperties | 舰船控制台 Comp 参数类。 |
| `Comps/ThingComp/CompShipControl&&Thuster.cs` | class | `CompProperties_ShipThruster` | CompProperties | 推进器 Comp 参数类。 |
| `Comps/ThingComp/CompShipControl&&Thuster.cs` | class | `CompShipControl` | ThingComp | 舰船控制台 Comp，提供发射、降落、旅行等舰船控制入口。 |
| `Comps/ThingComp/CompShipControl&&Thuster.cs` | class | `CompShipThruster` | ThingComp | 推进器 Comp，用于船体完整性和起飞能力计算。 |
| `Comps/ThingComp/CompShipPowerPlant.cs` | class | `CompShipPowerPlant` | CompPowerPlant | 舰船设备用发电/供能 Comp。 |
| `Comps/ThingComp/CompTextData .cs` | class | `CompProperties_TextData` | CompProperties | 文本数据 Comp 参数类。 |
| `Comps/ThingComp/CompTextData .cs` | class | `CompTextData` | ThingComp | 为信息碑、纸堆、终端提供可阅读文本的 ThingComp。 |
| `Comps/ThingComp/CompTextData .cs` | class | `Dialog_TextConsole` | Window | 文本终端窗口，用于阅读碑文、日志或终端内容。 |
| `Comps/ThingComp/Comp_ReactorSlider.cs` | class | `Comp_ReactorSlider` | ThingComp | 反应堆功率滑条 ThingComp，驱动动态发电输出。 |
| `Comps/ThingComp/Comp_ShipPart.cs` | class | `CompProperties_ShipPart` | CompProperties | 船体部件参数类，用于标记建筑属于舰船结构。 |
| `Comps/ThingComp/Comp_ShipPart.cs` | class | `Comp_ShipPart` | ThingComp | 船体部件标记 Comp，参与船体连通、起飞校验和区域识别。 |
| `DefOf.cs` | class | `StellarisDefOf` |  | 集中声明 XML Def 引用，便于 C# 代码安全访问自定义 Def。 |
| `Defs/ArchaeologicalSiteDef.cs` | class | `ArchaeologicalSiteDef` | Def | 自定义考古站点 Def，定义发现条件、地图生成参数、目标与奖励。 |
| `Defs/ArchaeologicalSiteDef.cs` | class | `StellarisPlanetDiscoveryWeight` |  | 行星类型到考古发现权重的配置项。 |
| `Defs/EnemyShipDef.cs` | class | `EnemyShipDef` | Def | 敌舰 Def 数据结构，定义敌舰图标、威胁和生成参数。 |
| `Defs/PlanetDef.cs` | class | `PlanetDef` | Def | 自定义行星类型 Def 数据结构。 |
| `Defs/PlanetDef.cs` | enum | `PlanetTypeObsolete` |  | 旧版行星类型枚举，保留兼容。 |
| `Defs/PlanetDef.cs` | enum | `StarType` |  | 恒星类型枚举。 |
| `DevTools/AreaLoader.cs` | class | `AreaLoader` |  | 开发工具：从 XML 载入保存的地图区域。 |
| `DevTools/AreaSaveData.cs` | class | `AreaSaveData` |  | 开发工具：地图区域保存数据根对象。 |
| `DevTools/AreaSaveData.cs` | class | `BuildingData` |  | 开发工具：序列化建筑及其材料/血量等信息。 |
| `DevTools/AreaSaveData.cs` | class | `PawnData` |  | 开发工具：序列化 Pawn 位置与基础信息。 |
| `DevTools/AreaSaveData.cs` | class | `ThingData` |  | 开发工具：序列化普通 Thing 的位置、Def、旋转等。 |
| `DevTools/AreaSaver.cs` | class | `AreaSaver` |  | 开发工具：把地图指定区域导出为 XML。 |
| `Dialog_Land.cs` | class | `Dialog_Land` | Window | 选择降落目标/确认降落的窗口逻辑。 |
| `DrawUtility.cs` | class | `DrawUtility` |  | 封装材质、图标、线框等绘制辅助方法。 |
| `ExplorationManager.cs` | class | `ExplorationManager` |  | 管理当前探索位置、星系/行星状态与玩家所在天体。 |
| `GalaxyCluster.cs` | class | `GalaxyCluster` | IExposable | 可存档的星系团数据结构，保存多个恒星系统。 |
| `GalaxyComponent.cs` | class | `GalaxyComponent` | GameComponent | GameComponent，全局保存星系团、探索状态等跨地图数据。 |
| `GenSteps/GenStep_Acidification.cs` | class | `GenStep_Acidification` | GenStep | 地图生成步骤，把普通地形替换为酸性/剧毒行星地形。 |
| `GenSteps/GenStep_ArchaeologicalSite.cs` | class | `GenStep_ArchaeologicalSite` | GenStep | 较早版本的通用考古遗迹程序生成器。 |
| `GenSteps/GenStep_DeadPlanetTerrain.cs` | class | `GenStep_DeadPlanetTerrain` | GenStep | 死寂行星地图地形生成步骤。 |
| `GenSteps/GenStep_FrozenPlanetTerrain.cs` | class | `GenStep_FrozenPlanetTerrain` | GenStep | 冰封行星地图地形生成步骤。 |
| `GenSteps/GenStep_LostPlanetTerrain.cs` | class | `GenStep_LostPlanetTerrain` | GenStep | 遗落行星地图地形生成步骤。 |
| `HarmonyPatch.cs` | class | `CompProperties_SafeBatteryProtector` | CompProperties | 电容阵列保护 Comp 的 XML 参数类。 |
| `HarmonyPatch.cs` | class | `Comp_SafeBatteryProtector` | ThingComp | 限制电池短路等负面事件的 ThingComp。 |
| `HarmonyPatch.cs` | class | `MapGenerator_GenerateMap_Patch` |  | Harmony 补丁：接入自定义地图生成或行星场景生成。 |
| `HarmonyPatch.cs` | class | `NoShipHullCollapse` |  | Harmony 补丁：避免船体结构被屋顶坍塌等原版机制误伤。 |
| `HarmonyPatch.cs` | class | `Patch_IncidentWorker_ShortCircuit_CanFireNowSub` |  | Harmony 补丁：阻止安全电容阵列触发短路事件。 |
| `HarmonyPatch.cs` | class | `Patch_PreventScenGenerate` |  | Harmony 补丁：控制特定场景生成流程，避免重复或冲突生成。 |
| `HarmonyPatch.cs` | class | `Patch_Projectile_Tick` |  | Harmony 补丁：扩展或修正投射物 Tick 行为。 |
| `HarmonyPatch.cs` | class | `Patch_ShipIsPlayerHome` |  | Harmony 补丁：让舰船地图按玩家基地/家园逻辑处理。 |
| `HarmonyPatch.cs` | class | `PlanetLayerPatch` |  | 世界图层相关 Harmony 补丁，用于替换/扩展行星图层显示。 |
| `HarmonyPatch.cs` | class | `StellarisPatch` |  | Harmony 初始化入口，注册 Stellaris 的运行时补丁。 |
| `IncidentWorker_EnemyShipEncounter .cs` | class | `IncidentWorker_EnemyShipEncounter` | IncidentWorker | 敌舰遭遇事件 Worker，用于生成或触发太空敌舰。 |
| `Jobs/Driver/Job&Comp_ReverseEngineer .cs` | class | `CompProperties_ReverseEngineer` | CompProperties | 逆向工程 Comp 参数类。 |
| `Jobs/Driver/Job&Comp_ReverseEngineer .cs` | class | `CompReverseEngineer` | ThingComp | 可逆向工程目标的 ThingComp，处理进度和完成奖励。 |
| `Jobs/Driver/Job&Comp_ReverseEngineer .cs` | class | `JobDriver_ReverseEngineer` | JobDriver | 执行逆向工程工作的 JobDriver。 |
| `Jobs/Driver/Job&Comp_ReverseEngineer .cs` | class | `WorkGiver_ReverseEngineer` | WorkGiver_Scanner | 扫描可逆向工程建筑并分配工作。 |
| `Jobs/Driver/JobDriver_Console.cs` | class | `JobDriver_Console` | JobDriver | 操作舰桥/控制台的 JobDriver。 |
| `Jobs/Driver/JobDriver_OperatePlanetScanner.cs` | class | `JobDriver_OperatePlanetScanner` | JobDriver | 操作天体扫描仪的 JobDriver。 |
| `Jobs/Giver/WorkGiver_OperatePlanetScanner.cs` | class | `WorkGiver_OperatePlanetScanner` | WorkGiver_Scanner | 为天体扫描仪分配操作工作的 WorkGiver。 |
| `MainButtonWorker_GalaxyMap.cs` | class | `MainButtonWorker_GalaxyMap` | MainButtonWorker | 主界面按钮 Worker，点击后打开星系地图。 |
| `MapComponent_AcidEnvironment .cs` | class | `MapComponent_ToxicEnvironment` | MapComponent | 剧毒/酸性环境地图组件，周期性施加酸雾、毒性或环境效果。 |
| `MapComponent_DeadPlanetEnvironment.cs` | class | `MapComponent_DeadPlanetEnvironment` | MapComponent | 死寂行星地图组件，维持绝对死寂天气、缺氧/微陨石等暴露惩罚。 |
| `MapComponent_FrozenPlanetEnvironment.cs` | class | `MapComponent_FrozenPlanetEnvironment` | MapComponent | 冰封行星地图组件，处理极寒暴露、装备损耗与暴风雪脉冲。 |
| `MapComponent_LostPlanetEnvironment.cs` | class | `MapComponent_LostPlanetEnvironment` | MapComponent | 遗落行星地图组件，处理废墟环境暴露、灰霾/坍塌等威胁。 |
| `PlaceWorker_AutonomousMiner.cs` | class | `PlaceWorker_AutonomousMiner` | PlaceWorker | 无人采矿机放置校验，限制其必须放在采矿基座等合法位置。 |
| `Planet.cs` | class | `Planet` | IExposable | 可存档行星实例，保存行星 Def、资源、轨道、调查状态等。 |
| `PlanetCharacter.cs` | class | `PlanetCharacter` | IExposable | 行星性格/特征数据结构，用于描述行星附加属性。 |
| `PlanetGenerator.cs` | class | `OrbitSlot` |  | 轨道槽位数据，记录轨道距离、温度、周期等生成参数。 |
| `PlanetGenerator.cs` | class | `PlanetGenerator` |  | 按恒星系统轨道与温度生成行星列表、行星类型与资源。 |
| `PlanetResourceConfig.cs` | class | `PlanetResourceConfig` |  | 行星资源生成配置项。 |
| `PlanetTravel/HyperspaceCache.cs` | class | `HyperspaceCache` |  | 星际旅行临时缓存，保存切换世界前后的舰船、Pawn 与地图上下文。 |
| `PlanetTravel/InterstellarShipTravelStats.cs` | class | `InterstellarShipTravelStats` |  | 计算星际旅行能力、消耗、质量、推进器等统计结果。 |
| `PlanetTravel/InterstellarTravelReport.cs` | class | `InterstellarTravelReport` |  | 星际旅行校验结果，包含是否可飞行与失败原因。 |
| `PlanetTravel/MapSnapshot.cs` | class | `MapSnapshot` | IExposable | 可存档地图快照，用于保存船体区域、物品、Pawn、地形和屋顶。 |
| `PlanetTravel/MapSnapshot.cs` | class | `ThingSnapshotRecord` | IExposable | 单个 Thing 的快照记录。 |
| `PlanetTravel/PlanetIdentityUtility.cs` | class | `PlanetIdentityUtility` |  | 为世界/地图识别所属行星并生成稳定标识。 |
| `PlanetTravel/PlanetSwitchService.cs` | class | `PlanetSwitchService` |  | 执行当前世界到目标世界的切换流程。 |
| `PlanetTravel/PlanetWorldManifest.cs` | class | `PlanetWorldManifest` | IExposable | 保存行星世界槽位清单，记录已生成世界与回访信息。 |
| `PlanetTravel/PlanetWorldManifest.cs` | class | `PlanetWorldManifestManager` |  | Manifest 的全局访问和保存管理器。 |
| `PlanetTravel/PlanetWorldSlotRecord.cs` | class | `PlanetWorldSlotRecord` | IExposable | 单个行星世界槽位记录。 |
| `PlanetTravel/PlanetWorldTravelService.cs` | class | `PlanetWorldTravelService` |  | 独立 World 行星旅行核心服务，负责生成/加载目标世界、处理到达与返航。 |
| `PlanetTravel/ShipTransporter.cs` | class | `ShipTransporter` |  | 船体运输器，负责捕获、序列化、清除、重建和放置舰船地图区域。 |
| `PlanetTravel/StellarisGlobalState.cs` | class | `BuildingSnapshot` | IExposable | 旧式/轻量建筑快照结构。 |
| `PlanetTravel/StellarisGlobalState.cs` | class | `ShipSnapshot` |  | 旧式/轻量船体快照结构。 |
| `PlanetTravel/StellarisGlobalState.cs` | class | `StellarisGlobalState` |  | 星际旅行全局状态，保存待处理的快照、目标与返航状态。 |
| `PrefabMapLoader.cs` | class | `PrefabMapLoader` |  | 旧式预制地图加载占位/辅助类。 |
| `Projectile_Railgun.cs` | class | `Projectile_Railgun` | Projectile_Explosive | 轨道炮爆炸投射物逻辑。 |
| `Projectiles/Projectile_Departing .cs` | class | `Projectile_Departing` | Projectile | 离场/起飞视觉投射物。 |
| `Projectiles/Projectile_Reentry .cs` | class | `Projectile_Reentry` | Projectile | 再入/降落视觉投射物。 |
| `Render/ShipFlightAnimationController.cs` | enum | `FlightMode` |  | 舰船动画模式枚举：起飞或降落。 |
| `Render/ShipFlightAnimationController.cs` | class | `ShipFlightAnimationController` | Thing | 舰船起降动画控制器基类，管理飞行阶段、偏移和完成回调。 |
| `Render/ShipFlightAnimationDrawRegistry.cs` | class | `Patch_MapDrawer_Draw_StellarisShipFlightManualMeshes` |  | Harmony 补丁：在地图绘制阶段手动绘制飞行动画网格。 |
| `Render/ShipFlightAnimationDrawRegistry.cs` | class | `Patch_SectionLayer_TakePrintFrom_StellarisShipFlightOffset` |  | Harmony 补丁：让地形/结构绘制层参与船体飞行偏移。 |
| `Render/ShipFlightAnimationDrawRegistry.cs` | class | `Patch_Thing_DrawAt_StellarisShipFlightOffset` |  | Harmony 补丁：修正 Thing.DrawAt 的飞行偏移。 |
| `Render/ShipFlightAnimationDrawRegistry.cs` | class | `Patch_Thing_DrawPos_StellarisShipFlightOffset` |  | Harmony 补丁：为飞行动画中的 Thing 应用绘制偏移。 |
| `Render/ShipFlightAnimationDrawRegistry.cs` | class | `ShipFlightAnimationDrawRegistry` |  | 舰船起降动画绘制注册表，记录哪些 Thing 需要整体偏移绘制。 |
| `Render/ShipFlightAnimationUtility.cs` | class | `ShipFlightAnimationUtility` |  | 舰船起降动画辅助方法，寻找船体、计算偏移和目标位置。 |
| `Render/ShipFlightVisual.cs` | class | `BakedSubMesh` |  | 飞行视觉系统的烘焙子网格数据。 |
| `Render/ShipFlightVisual.cs` | class | `RealtimeRecord` |  | 飞行视觉系统中实时绘制对象记录。 |
| `Render/ShipFlightVisual.cs` | class | `ShipFlightBakedSectionLayer` | SectionLayer | 用于舰船飞行动画的烘焙 SectionLayer。 |
| `Render/ShipFlightVisual.cs` | class | `ShipFlightVisual` |  | 舰船飞行视觉系统，烘焙船体子网格并绘制移动中的整体船体。 |
| `Render/ShipLandingController .cs` | class | `ShipLandingController` | ShipFlightAnimationController | 降落动画控制器。 |
| `Render/ShipPartData.cs` | class | `ShipPartData` | IExposable | 可存档船体部件位置/旋转数据。 |
| `Render/ShipRenderRegistry.cs` | class | `ShipRenderRegistry` |  | 船体渲染注册表，保存需要特殊绘制的船体对象。 |
| `Render/ShipTakeoffController .cs` | class | `ShipTakeoffController` | ShipFlightAnimationController | 起飞动画控制器。 |
| `ShipMapComp.cs` | class | `ShipMapComp` | MapComponent | 舰船地图组件，保存船体区域、真空/房间状态等舰船地图信息。 |
| `ShipRegion.cs` | class | `ShipRegion` |  | 舰船连通区域数据，记录船体单元、边界与完整性。 |
| `ShipTakeoffViewer .cs` | class | `ShipTakeoffViewer` | Thing | 旧式起飞可视化 Thing，用于播放船体起飞视图。 |
| `SitePartWorker_ArchaeologicalSite.cs` | class | `SitePartWorker_ArchaeologicalSite` | SitePartWorker | 考古站点 SitePart Worker，连接世界对象与地图生成。 |
| `SpaceLayer.cs` | class | `SpaceLayer` | OrbitLayer | 宇宙空间图层类型。 |
| `Star.cs` | class | `Star` | IExposable | 可存档恒星数据，保存恒星类型、名称和所属系统信息。 |
| `StarGenerator.cs` | class | `StarGenerator` |  | 生成恒星、恒星模板与恒星系统基础数据。 |
| `StarGenerator.cs` | class | `StarTemplate` |  | 恒星生成模板，定义不同恒星类型的颜色/温度/概率等。 |
| `StarSystem.cs` | class | `StarSystem` | IExposable | 可存档恒星系统，保存恒星、行星、宇宙对象等。 |
| `StellarisMapGenerator.cs` | class | `StellarisMapGenerator` |  | 地图生成辅助入口，处理自定义地图生成时的公共逻辑。 |
| `StellarisMaterials.cs` | class | `StellarisMaterials` |  | 集中创建或缓存宇宙地图、行星图层等绘制材质。 |
| `StellarisMod.cs` | class | `StellarisMod` | Mod | Mod 主类，完成初始化、设置入口或 Harmony 引导。 |
| `SystemNameGenerator.cs` | class | `IntExtensions` |  | 整数扩展工具，服务于命名或格式化。 |
| `SystemNameGenerator.cs` | class | `NameGenerator` |  | 星系/恒星/行星名称生成器。 |
| `SystemNameGenerator.cs` | class | `SpecialNameGenerator` |  | 特殊名称生成器，用于生成带风格的天体名。 |
| `TimeManager.cs` | class | `TimeManager` |  | 时间换算/流逝辅助类，用于星际旅行或宇宙层时间。 |
| `Transfer/MapObjectTransfer.cs` | class | `MapObjectTransfer` |  | 地图对象迁移工具，把 Pawn/物品/建筑从一个地图转移到另一个地图。 |
| `UniverseObjectMaker.cs` | class | `UniverseObjectMaker` |  | 创建宇宙地图对象的工厂方法。 |
| `Utilities/DropSpacePodUtility.cs` | class | `DropSpacePodUtility` |  | 太空运输仓投送工具。 |
| `Utilities/PlanetGenerateUtility.cs` | class | `PlanetGenerateUtility` |  | 行星生成辅助方法，处理行星表面地图与场景。 |
| `Utilities/PowerUtility.cs` | class | `PowerUtility` |  | 电力网络/供电状态辅助方法。 |
| `Utilities/ShipUtility.cs` | class | `ShipUtility` |  | 舰船核心工具类，计算船体区域、完整性、起飞、降落与行星包裹逻辑。 |
| `Utilities/SiteUtility.cs` | class | `SiteUtility` |  | 站点生成/查找/生成位置辅助方法。 |
| `Windows/Window_GalaxyCluster.cs` | class | `Window_GalaxyCluster` | Window | 星系团总览窗口。 |
| `Windows/Window_StarSystem.cs` | class | `Window_StarSystem` | Window | 恒星系统窗口，显示恒星、行星和可交互对象。 |
| `Windows/Window_SystemInfo.cs` | class | `Window_SystemInfo` | Window | 系统/天体信息窗口。 |
| `WorldDrawLayer/WorldDrawLayer_AcidAtmosphere .cs` | class | `WorldDrawLayer_ToxicAtmosphere` | WorldDrawLayer | 剧毒行星世界图层大气效果绘制。 |
| `WorldDrawLayer/WorldDrawLayer_DeadPlanetUngeneratedPlanetParts.cs` | class | `WorldDrawLayer_DeadPlanetUngeneratedPlanetParts` | WorldDrawLayer | 死寂行星未生成区块世界绘制层。 |
| `WorldDrawLayer/WorldDrawLayer_FrozenPlanetUngeneratedPlanetParts.cs` | class | `WorldDrawLayer_FrozenPlanetUngeneratedPlanetParts` | WorldDrawLayer | 冰封行星未生成区块世界绘制层。 |
| `WorldDrawLayer/WorldDrawLayer_LavaPlanetUngeneratedPlanetParts.cs` | class | `WorldDrawLayer_LavaPlanetUngeneratedPlanetParts` | WorldDrawLayer | 熔融行星未生成区块世界绘制层。 |
| `WorldDrawLayer/WorldDrawLayer_LostPlanetUngeneratedPlanetParts.cs` | class | `WorldDrawLayer_LostPlanetUngeneratedPlanetParts` | WorldDrawLayer | 遗落行星未生成区块世界绘制层。 |
| `WorldDrawLayer/WorldDrawLayer_ToxicPlanetUngeneratedPlanetParts.cs` | class | `WorldDrawLayer_ToxicPlanetUngeneratedPlanetParts` | WorldDrawLayer | 剧毒行星未生成区块世界绘制层。 |
| `WorldGenSteps/WorldGenStep_DeadPlanet.cs` | class | `WorldGenStep_DeadPlanet` | WorldGenStep_Tiles | 死寂行星世界地块生成步骤。 |
| `WorldGenSteps/WorldGenStep_FrozenPlanet.cs` | class | `WorldGenStep_FrozenPlanet` | WorldGenStep_Tiles | 冰封行星世界地块生成步骤。 |
| `WorldGenSteps/WorldGenStep_LavaPlanet.cs` | class | `WorldGenStep_LavaPlanet` | WorldGenStep_Tiles | 熔融行星世界地块生成步骤。 |
| `WorldGenSteps/WorldGenStep_LostPlanet.cs` | class | `WorldGenStep_LostPlanet` | WorldGenStep_Tiles | 遗落行星世界地块生成步骤。 |
| `WorldGenSteps/WorldGenStep_ToxicTerrain .cs` | class | `WorldGenStep_ToxicTerrain` | WorldGenStep_Tiles | 剧毒行星世界地块生成步骤。 |
| `WorldObjects/IUniversable.cs` | interface | `IUniversable` |  | 宇宙地图对象接口，提供所属星系/行星等统一字段。 |
| `WorldObjects/TravelingShell .cs` | class | `TravelingShell` | WorldObject | 宇宙地图中的飞行壳/旅行中对象。 |
| `WorldObjects/UniverseMapParent.cs` | enum | `DrawPriorityUniverseMapParent` |  | 宇宙地图对象绘制优先级枚举。 |
| `WorldObjects/UniverseMapParent.cs` | class | `UniverseMapParent` | SpaceMapParent, IUniversable | 宇宙地图父对象基类，可持有地图并在星系层显示。 |
| `WorldObjects/UniverseObject.cs` | class | `UniverseObject` | WorldObject, IUniversable | 宇宙地图普通对象基类。 |
| `WorldObjects/UniverseObjectAutonomousMiner.cs` | class | `UniverseObjectAutonomousMiner` | UniverseObject | 宇宙地图上的无人采矿机对象。 |
| `WorldObjects/WorldObject_EnemyShip .cs` | class | `WorldObject_EnemyShip` | MapParent | 世界地图敌舰对象，承载敌舰地图或遭遇入口。 |
| `WorldObjects/WorldShip.cs` | class | `WorldShip` | UniverseMapParent | 玩家舰船世界对象，继承宇宙地图父对象并保存舰船所属行星/星系。 |

### 未声明 C# 类型的源码文件

- `PlanetTravel/HistoryTransferUtility.cs`
- `Properties/AssemblyInfo.cs`

## 功能模块速览

- **多行星与星系层**：`PlanetDef`、`PlanetLayerDef`、`WorldObjectDef`、`GalaxyComponent`、`PlanetGenerator`、`WorldShip` 等共同实现星系、恒星系统、行星、宇宙对象与星球表面图层。
- **独立 World 行星旅行**：`PlanetWorldTravelService`、`ShipTransporter`、`MapSnapshot`、`HyperspaceCache`、`StellarisGlobalState` 负责保存当前船体区域、生成或加载目标行星 World，并在抵达后重建舰船。
- **舰船建筑与生命支持**：船体、舰桥、推进器、真空屏障、维生装置、温控器、发电与电容阵列由 `ThingDef` + `Comp_ShipPart`、`CompShipControl`、`CompShipThruster`、`CompOxygenAllRoomPusher` 等组成。
- **特殊行星环境**：死寂、冰封、遗落、剧毒、熔融等行星通过 `BiomeDef`、`TerrainDef`、`WorldGenStepDef`、`GenStepDef` 和多个 `MapComponent_*Environment` 施加地形、天气和暴露惩罚。
- **聚变反应堆考古站点**：`Stellaris_FusionReactorArchaeologicalSite`、`GenStep_FusionReactorArchaeologicalSite`、`CompStellarisArchaeologyWork`、`StellarisArchaeologyManager` 实现 Def 驱动的考古站点、交互目标、阶段标志与最终奖励。
- **极端环境装备**：`Apparel_ExtremePlanet.xml` 定义死寂 EVA、深寒、主动加热、火山灰过滤、熔岩隔热等装备，并由单独科技 `StellarisResearch_ExtremeEnvironmentSurvival` 解锁。
- **太空采矿与敌舰**：`SpaceMiningPad`、`AutonomousMiner`、`UniverseObjectAutonomousMiner`、`EnemyShipDef`、`IncidentWorker_EnemyShipEncounter` 提供太空资源采集和敌舰遭遇基础。

