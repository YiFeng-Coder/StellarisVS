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
    public static class CellHighlighter
    {
        // 定义材质：这里使用的是MetaOverlay着色器，支持半透明
        // 你可以将 null 替换为你自己的 Texture2D (ContentFinder<Texture2D>.Get("Your/Texture/Path"))
        private static readonly Material HighlightMat = MaterialPool.MatFrom(
            Texture2D.whiteTexture, // RimWorld内置的一张白色高亮图，也可以换成你自己的图片路径
            ShaderDatabase.MetaOverlay,
            new Color(1f, 1f, 1f, 0.5f) // 颜色：白色，0.5f 代表 50% 透明度
        );

        /// <summary>
        /// 在当前帧绘制高亮格子
        /// </summary>
        /// <param name="allCells">需要高亮的格子集合</param>
        public static void DrawHelpers(HashSet<IntVec3> allCells)
        {
            if (allCells == null || allCells.Count == 0) return;

            foreach (var cell in allCells)
            {
                // 检查格子是否在相机可视范围内（优化性能）
                // 这一步是可选的，但在大量格子时推荐加上
                if (!cell.InBounds(Find.CurrentMap) || !Find.CameraDriver.CurrentViewRect.Contains(cell))
                    continue;

                // 获取格子的渲染中心点
                // AltitudeLayer.MetaOverlays 确保它渲染在地面物体之上，但在迷雾之下
                Vector3 pos = cell.ToVector3ShiftedWithAltitude(AltitudeLayer.MetaOverlays);

                // 绘制 1x1 的平面
                Graphics.DrawMesh(
                    MeshPool.plane10,
                    pos,
                    Quaternion.identity,
                    HighlightMat,
                    0
                );
            }
        }
    }
}
