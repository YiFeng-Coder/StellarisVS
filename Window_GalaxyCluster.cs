using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace Stellaris
{
    public class Window_GalaxyCluster : Window
    {
        public GalaxyComponent comp;
        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(1000f, 1000f);
            }
        }
        private const float SYSTEM_SIZE = 50f;
        private const float SYSTEM_SPACING = 80f;
        private Vector2 scrollPosition = Vector2.zero;
        private StarSystem selectedSystem;
        private bool isDragging = false;
        private Vector2 dragStartPosition =  Vector2.zero;
        private float scrollVelocity = 0f;
        private float lastDragTime = 0f;
        private float scrollViewHeight = 2000f;

        public Window_GalaxyCluster()
        {
            forcePause = true;
            doCloseX = true;
            draggable = false;
            absorbInputAroundWindow = true;
            comp = Current.Game.GetComponent<GalaxyComponent>();
        }

        public override void DoWindowContents(Rect inRect)
        {
            // 标题
            Text.Font = GameFont.Medium;
            Rect titleRect = new Rect(0f, 0f, inRect.width, 35f);
            Widgets.Label(titleRect, "StellarisGalaxyClusterMapTitle".Translate());
            if (Prefs.DevMode)
            {
                titleRect.position = new Vector2(300f, 0);
                Widgets.Label(titleRect, scrollPosition.x + " " + scrollPosition.y);
                titleRect.position = new Vector2(600f, 0);
                Widgets.Label(titleRect, dragStartPosition.x + " " + dragStartPosition.y);
            }
            Text.Font = GameFont.Small;

            // 主内容区域
            Rect contentRect = new Rect(0f, 40f, inRect.width, inRect.height - 40f);

            // 绘制星系团地图
            DrawGalaxyMap(contentRect);
            // 选中的星系信息
            if (selectedSystem != null)
            {
                DrawSystemInfo(new Rect(inRect.width - 300f, 40f, 280f, 200f));
            }
        }
        private void DrawUniverseObjects(Rect inRect, StarSystem system)
        {
            if (!system.universeObjects.Empty())
            {
                UniverseMapParent mapParentToDraw = (UniverseMapParent)(system.universeObjects.Where(x => x is UniverseMapParent && x != null).ToList().OrderByDescending(x => ((UniverseMapParent)x).drawPrority).First());
                Widgets.DrawAtlas(inRect, ContentFinder<Texture2D>.Get(mapParentToDraw.def.texture));
            }
            else if (Prefs.DevMode)
            {
                //Log.Message(system.position.x + " " + system.position.y + " is Empty");
            }
        }
        private void DrawGalaxyMap(Rect canvas)
        {
            Widgets.DrawMenuSection(canvas);

            Rect viewRect = new Rect(0f, 0f, scrollViewHeight, scrollViewHeight);

            HandleMouseEvents();
            Widgets.BeginScrollView(canvas, ref scrollPosition, viewRect);

            var galaxy = Current.Game.GetComponent<GalaxyComponent>()?.ClusterData;
            if (galaxy != null)
            {
                foreach (var system in galaxy.starSystems)
                {
                    DrawStarSystem(system, viewRect);
                }
                // 绘制隐藏的星系位置
                foreach (var kvp in galaxy.exploredSystems)
                {
                    if (!kvp.Value)
                    {
                        DrawHiddenSystem(kvp.Key, viewRect);
                    }
                }
            }
            else
            {
                Log.Error("Galaxy Component is null");
            }
            Widgets.EndScrollView();
        }

        private void DrawStarSystem(StarSystem system, Rect canvas)
        {
            Vector2 screenPos = system.position * SYSTEM_SPACING + canvas.center;
            Rect systemRect = new Rect(screenPos.x - SYSTEM_SIZE / 2, screenPos.y - SYSTEM_SIZE / 2, SYSTEM_SIZE, SYSTEM_SIZE);

            // 绘制系统图标
            Color systemColor = system.star?.color ?? Color.yellow;
            DrawUtility.DrawHollowCircle(systemRect.center, SYSTEM_SIZE / 2, systemColor);
            Widgets.DrawAtlas(systemRect, ContentFinder<Texture2D>.Get("UI/Star/RedDwarf"));
            // 绘制UniverseObject
            systemRect.position -= new Vector2(8f, 8f);
            DrawUniverseObjects(systemRect, system);
            systemRect.position += new Vector2(8f, 8f);
            // 绘制系统名称
            if (Mouse.IsOver(systemRect))
            {
                Widgets.DrawHighlight(systemRect);
                TooltipHandler.TipRegion(systemRect, system.name);

                if (Widgets.ButtonInvisible(systemRect))
                {
                    selectedSystem = system;
                    Find.WindowStack.Add(new Window_StarSystem(system));
                }
            }

            // 绘制连接线到相邻系统
            DrawSystemConnections(system, canvas);
        }

        private void DrawHiddenSystem(Vector2 position, Rect canvas)
        {
            Vector2 screenPos = position * SYSTEM_SPACING + canvas.center;
            Rect systemRect = new Rect(screenPos.x - SYSTEM_SIZE / 2, screenPos.y - SYSTEM_SIZE / 2, SYSTEM_SIZE, SYSTEM_SIZE);

            // 绘制隐藏系统图标
            GUI.color = new Color(1f, 1f, 1f, 0.3f);
            DrawUtility.DrawHollowCircle(systemRect.center, SYSTEM_SIZE / 2, GUI.color);

            Widgets.DrawAtlas(systemRect, ContentFinder<Texture2D>.Get("UI/StarSystem/UnexpoloaredStarSystem"));

            GUI.color = Color.white;

            // 悬停提示
            if (Mouse.IsOver(systemRect))
            {
                TooltipHandler.TipRegion(systemRect, "StellarisUnexploredSystem".Translate());
            }
        }

        private void DrawSystemConnections(StarSystem system, Rect canvas)
        {
            var galaxy = Current.Game.GetComponent<GalaxyComponent>()?.ClusterData;
            if (galaxy == null) return;

            foreach (var otherSystem in galaxy.starSystems)
            {
                if (otherSystem != system && IsAdjacent(system.position, otherSystem.position))
                {
                    Vector2 startPos = system.position * SYSTEM_SPACING + canvas.center;
                    Vector2 endPos = otherSystem.position * SYSTEM_SPACING + canvas.center;

                    Widgets.DrawLine(startPos, endPos, Color.blue, 3f);
                }
            }
        }

        private bool IsAdjacent(Vector2 pos1, Vector2 pos2)
        {
            return Vector2.Distance(pos1, pos2) <= 1.5f;
        }

        private void DrawSystemInfo(Rect infoRect)
        {
            Widgets.DrawMenuSection(infoRect);

            Rect innerRect = infoRect.ContractedBy(10f);
            float y = innerRect.y;

            // 系统名称
            Widgets.Label(new Rect(innerRect.x, y, innerRect.width, 25f), selectedSystem.name);
            y += 30f;

            // 恒星信息
            if (selectedSystem.star != null )
            {
                Widgets.Label(new Rect(innerRect.x, y, innerRect.width, 25f), "StellarisStar".Translate(selectedSystem.star.name));
                y += 25f;
                Widgets.Label(new Rect(innerRect.x, y, innerRect.width, 25f), "StellarisType".Translate(selectedSystem.star.type));
                y += 25f;
            }

            // 行星数量
            Widgets.Label(new Rect(innerRect.x, y, innerRect.width, 25f), "StellarisPlanetCount".Translate(selectedSystem.planets?.Count ?? 0));
            y += 30f;

            // 探索按钮
            if (Widgets.ButtonText(new Rect(innerRect.x, y, innerRect.width, 30f), "StellarisOpenSystemWindow".Translate()))
            {
                Find.WindowStack.Add(new Window_StarSystem(selectedSystem));
            }
        }
        // 在 Window_GalaxyCluster 中添加舰船显示

        private void HandleMouseEvents()
        {
            Event current = Event.current;
            if (Prefs.DevMode) 
            {
                Widgets.Label(new Rect(200f,200f,400f,35f), current.mousePosition.x + " " + current.mousePosition.y);
            }
            
            switch (current.type)
            {
                case EventType.MouseDown:
                    if (current.button == 0) // 左键按下
                    {
                        isDragging = true;
                        dragStartPosition = current.mousePosition;
                        scrollVelocity = 0f;
                        current.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (isDragging)
                    {
                        Vector2 delta = dragStartPosition - current.mousePosition;
                        scrollPosition = scrollPosition + delta;
                        dragStartPosition = current.mousePosition;
                        // 限制滚动范围
                        scrollPosition.x = Mathf.Clamp(scrollPosition.x, 0f, scrollViewHeight);
                        scrollPosition.y = Mathf.Clamp(scrollPosition.y, 0f, scrollViewHeight);
                        current.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (current.button == 0 && isDragging)
                    {
                        isDragging = false;
                        // 可以在这里添加惯性滚动效果
                        current.Use();
                    }
                    break;
            }
        }
    }
}