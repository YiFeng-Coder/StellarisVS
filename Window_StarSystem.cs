using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Stellaris
{
    public class Window_StarSystem : Window
    {
        private StarSystem starSystem;
        private Vector2 scrollPosition;
        private const float ORBIT_SCALE = 20f;
        private const float STAR_SIZE = 80f;

        public Window_StarSystem(StarSystem system)
        {
            starSystem = system;
            forcePause = true;
            doCloseX = true;
            absorbInputAroundWindow = true;

            // 确保系统内容已生成
            system.GenerateSystemContent();
        }

        public override Vector2 InitialSize => new Vector2(1500f, 1000f);

        public override void DoWindowContents(Rect inRect)
        {
            // 标题
            Text.Font = GameFont.Medium;
            Rect titleRect = new Rect(0f, 0f, inRect.width, 35f);
            Widgets.Label(titleRect, starSystem.name + " Star System");
            Text.Font = GameFont.Small;

            // 分割为轨道图和详细信息
            Rect orbitRect = new Rect(0f, 40f, inRect.width * 0.6f, inRect.height - 40f); //480f 560f
            Rect infoRect = new Rect(inRect.width * 0.6f + 10f, 40f, inRect.width * 0.4f - 10f, inRect.height - 40f);

            DrawOrbitDiagram(orbitRect);
            DrawSystemDetails(infoRect);
        }

        private void DrawOrbitDiagram(Rect canvas)
        {
            Widgets.DrawMenuSection(canvas);

            Vector2 center = canvas.center;

            // 绘制恒星
            if (starSystem.star != null)
            {
                float starRadius = STAR_SIZE / 2f;
                GenDraw.DrawCircleOutline(center, starRadius, SimpleColor.Yellow);
                Rect systemRect = new Rect(center- new Vector2(STAR_SIZE/2, STAR_SIZE/2), new Vector2(STAR_SIZE, STAR_SIZE));
                Widgets.DrawAtlas(systemRect, ContentFinder<Texture2D>.Get("UI/Star/RedDwarf"));

                // 恒星光晕效果
                GUI.color = new Color(starSystem.star.color.r, starSystem.star.color.g, starSystem.star.color.b, 0.3f);
                DrawUtility.DrawHollowCircle(center, starRadius * 1.5f, GUI.color);
                GUI.color = Color.white;
            }

            // 绘制行星轨道和行星
            if (starSystem.planets != null)
            {
                foreach (var planet in starSystem.planets)
                {
                    DrawPlanetOrbit(center, planet, canvas);
                }
            }
        }

        private void DrawPlanetOrbit(Vector2 center, Planet planet, Rect canvas)
        {
            float orbitRadius = planet.orbitRadius * ORBIT_SCALE;

            // 限制轨道大小以适应画布
            float maxRadius = Math.Min(canvas.width, canvas.height) * 0.4f;
            orbitRadius = Math.Min(orbitRadius, maxRadius);

            // 绘制轨道
            GUI.color = new Color(1f, 1f, 1f, 0.3f);
            DrawUtility.DrawHollowCircle(center, orbitRadius, GUI.color);
            GUI.color = Color.white;

            // 计算行星位置（简化：使用固定角度）
            float angle = (float)(Time.realtimeSinceStartup * 0.5f / planet.orbitPeriod * Math.PI * 2);
            Vector2 planetPos = center + new Vector2(
                Mathf.Cos(angle) * orbitRadius,
                Mathf.Sin(angle) * orbitRadius
            );

            // 绘制行星
            Color planetColor = GetPlanetColor(planet.type);
            float planetSize = Mathf.Clamp(planet.mass * 7f, 10f, 40f);
            DrawUtility.DrawHollowCircle(planetPos, planetSize / 2f, planetColor);
            Rect planetDrawRect = new Rect(planetPos - new Vector2(planetSize / 2f, planetSize / 2f), new Vector2(planetSize, planetSize));
            Widgets.DrawAtlas(planetDrawRect, ContentFinder<Texture2D>.Get("UI/Planet/Terrestrial"));
            planetDrawRect.position += new Vector2(8f, 8f);
            DrawUniverseObjectsOnPlanet(planetDrawRect,planet);
            // 行星悬停交互   
            Rect planetRect = new Rect(planetPos.x - planetSize / 2, planetPos.y - planetSize / 2, planetSize, planetSize);
            if (Mouse.IsOver(planetRect))
            {
                Widgets.DrawHighlight(planetRect);
                TooltipHandler.TipRegion(planetRect, $"{planet.name}\nType: {planet.type}\nTemperature: {planet.temperature}K");
            }
        }

        private Color GetPlanetColor(PlanetType type)
        {
            switch (type)
            {
                case PlanetType.Terrestrial: return new Color(0.6f, 0.4f, 0.2f);
                case PlanetType.GasGiant: return new Color(0.8f, 0.7f, 0.3f);
                case PlanetType.IceGiant: return new Color(0.4f, 0.6f, 0.9f);
                case PlanetType.Lava: return new Color(0.9f, 0.3f, 0.1f);
                case PlanetType.Oceanic: return new Color(0.2f, 0.4f, 0.8f);
                default: return Color.gray;
            }                                                   
        }
        private void DrawUniverseObjectsOnPlanet(Rect drawRect, Planet planet)
        {
            if (!starSystem.universeObjects.Empty() && planet != null && !planet.universeObjects.Empty())
            {
                var mapParentToDraw = planet.universeObjects.Where(x => x != null).ToList().OrderByDescending(x => x.DrawPrority).First();
                Widgets.DrawAtlas(drawRect, ContentFinder<Texture2D>.Get(((UniverseMapParent)mapParentToDraw).def.texture));
            }
        }
        private void DrawSystemDetails(Rect infoRect)
        {
            Widgets.DrawMenuSection(infoRect);

            Rect innerRect = infoRect.ContractedBy(10f);
            Rect viewRect = new Rect(0f, 0f, innerRect.width - 20f, CalculateDetailsHeight());

            Widgets.BeginScrollView(innerRect, ref scrollPosition, viewRect);

            float y = 0f;

            // 恒星信息
            if (starSystem.star != null)
            {
                y = DrawStarInfo(viewRect, y);
                y += 10f;
            }

            // 行星列表
            if (starSystem.planets != null && starSystem.planets.Count > 0)
            {
                y = DrawPlanetsList(viewRect, y);
            }

            Widgets.EndScrollView();
        }

        private float DrawStarInfo(Rect rect, float y)
        {
            Widgets.Label(new Rect(0f, y, rect.width, 25f), "StellarisStarInfo".Translate());
            y += 25f;

            Widgets.Label(new Rect(10f, y, rect.width, 25f), "StellarisName".Translate(starSystem.star.name));
            y += 25f;
            Widgets.Label(new Rect(10f, y, rect.width, 25f), "StellarisType".Translate(starSystem.star.type));
            y += 25f;
            if (starSystem.star.surveyed)
            {
                Widgets.Label(new Rect(10f, y, rect.width, 25f), "StellarisStarMass".Translate(starSystem.star.mass));
                y += 25f;
                Widgets.Label(new Rect(10f, y, rect.width, 25f), "StellarisTemprature".Translate(starSystem.star.temperature));
                y += 25f;
                Widgets.Label(new Rect(10f, y, rect.width, 25f), "StellarisLuminosity".Translate(starSystem.star.luminosity));
                y += 25f;
            }
            else
            {
                Widgets.Label(new Rect(10f, y, rect.width, 25f), "StellarisUnsurveyedPlanet".Translate());
                y += 25f;
            }

                return y;
        }

        private float DrawPlanetsList(Rect rect, float y)
        {
            Widgets.Label(new Rect(0f, y, rect.width, 25f), "StellarisPlanetCount".Translate(starSystem.planets.Count));
            y += 30f;

            foreach (var planet in starSystem.planets)
            {
                Rect planetRect = new Rect(0f, y, rect.width, 100f);
                Widgets.DrawMenuSection(planetRect);

                Rect innerPlanetRect = planetRect.ContractedBy(5f);
                float planetY = innerPlanetRect.y;

                // 行星名称和类型
                Widgets.Label(new Rect(innerPlanetRect.x, planetY, innerPlanetRect.width, 25f),
                             $"{planet.name} - {planet.type}");
                planetY += 25f;
                if (planet.surveyed) 
                {
                    // 行星属性
                    Widgets.Label(new Rect(innerPlanetRect.x, planetY, innerPlanetRect.width, 20f),
                                 "StellarisOrbitRadius".Translate(
                        planet.orbitRadius) +" | " + "StellarisPlanetMass".Translate(planet.mass));
                    planetY += 20f;

                    Widgets.Label(new Rect(innerPlanetRect.x, planetY, innerPlanetRect.width, 20f),
                                 "StellarisTemprature".Translate(planet.temperature) +" | "+ "StellarisHabitable".Translate(planet.habitable ? "Yes" : "No"));
                    planetY += 20f;

                    // 资源
                    if (planet.resources != null && planet.resources.Count > 0)
                    {
                        string resourcesText = "StellarisResource".Translate(string.Join(", ", planet.resources.ConvertAll<string>(x => x.LabelCap)));
                        Widgets.Label(new Rect(innerPlanetRect.x, planetY, innerPlanetRect.width, 20f), resourcesText);
                    }

                }
                else
                {
                    Widgets.Label(new Rect(innerPlanetRect.x, planetY, innerPlanetRect.width, 20f), "StellarisUnsurveyedPlanet".Translate());
                }

                y += 85f;
            }

            return y;
        }

        private float CalculateDetailsHeight()
        {
            float height = 0f;

            // 恒星信息高度
            if (starSystem.star != null)
            {
                height += 25f + 25f * 5f + 10f; // 标题 + 5行信息 + 间距
            }

            // 行星列表高度
            if (starSystem.planets != null)
            {
                height += 30f; // 标题
                height += starSystem.planets.Count * 85f; // 每个行星项
            }

            return height + 20f; // 额外边距
        }
    }
}
