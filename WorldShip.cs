using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace Stellaris
{

    public class WorldShip : UniverseMapParent
    {
        public static bool isFirstLaunch = true;
        public static WorldShip playerShip;
        public ShipRegion shipRegion;
        public string label = "Ship";
        public override string Label => label;
        public void UpdateShipRegion(IntVec3 startPosition)
        {
            shipRegion = ShipUtility.CalculateShipRegion(startPosition, Map);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            // 序列化舰船区域数据
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                // 保存时，将HashSet转换为列表
                List<IntVec3> cellList = shipRegion?.allCells?.ToList() ?? new List<IntVec3>();
                Scribe_Collections.Look(ref cellList, "shipCells", LookMode.Value);

                // 保存边界矩形
                CellRect bounds = shipRegion?.boundingRect ?? new CellRect();
                Scribe_Values.Look(ref bounds, "shipBounds");
                    
                // 保存中心点
                IntVec3 center = shipRegion?.centerCell ?? IntVec3.Zero;
                Scribe_Values.Look(ref center, "shipCenter");
            }
            else if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                // 加载时，从列表重建HashSet
                List<IntVec3> cellList = new List<IntVec3>();
                Scribe_Collections.Look(ref cellList, "shipCells", LookMode.Value);

                CellRect bounds = new CellRect();
                Scribe_Values.Look(ref bounds, "shipBounds");

                IntVec3 center = IntVec3.Zero;
                Scribe_Values.Look(ref center, "shipCenter");

                // 重建ShipRegion
                shipRegion = new ShipRegion();
                if (cellList != null)
                {
                    shipRegion.allCells = new HashSet<IntVec3>(cellList);
                    shipRegion.boundingRect = bounds;
                    shipRegion.centerCell = center;
                }
            }

        }

        public override void DrawOnGalaxyGUI()
        {
            /*
            if (shipRegion == null) return;

            // 在世界地图上绘制舰船图标，使用舰船区域的中心点
            Vector2 drawPos = Find.WorldGrid.GetTileCenter(Tile);
            drawPos.y += 0.1f; // 在陆地上空

            Rect iconRect = new Rect(drawPos.x - 12f, drawPos.y - 12f, 24f, 24f);
            GUI.DrawTexture(iconRect, ContentFinder<Texture2D>.Get("World/Objects/ShipIcon"));
            */
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var item in base.GetGizmos())
            {
                yield return item;
            }
            if (Prefs.DevMode)
            {
                yield return new Command_Action()
                {
                    defaultLabel = "destroy map",
                    defaultDesc = "destroy",
                    action = delegate 
                    {
                        Current.Game.DeinitAndRemoveMap(Map,true);
                    }
                };
            }

        }

        public override string GetInspectString()
        {
            string baseStr = base.GetInspectString();
            int partCount = shipRegion?.allCells?.Count ?? 0;
            return $"{baseStr}\n太空舰船\n部件数量: {partCount}\n状态: {(Map != null ? "运行中" : "离线")}";
        }
    }
}
