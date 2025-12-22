using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Stellaris
{
    public static class DrawUtility
    {
        private static Dictionary<string, Texture2D> hollowCircleTextureCache = new Dictionary<string, Texture2D>();

        /// <summary>
        /// 在指定位置绘制一个空心圆
        /// </summary>
        /// <param name="center">圆心位置</param>
        /// <param name="radius">圆的半径</param>
        /// <param name="color">圆的颜色</param>
        /// <param name="borderWidth">边框宽度</param>
        public static void DrawHollowCircle(Vector2 center, float radius, Color color, float borderWidth = 2f)
        {
            int diameter = Mathf.RoundToInt(radius * 2);

            // 获取或创建空心圆纹理
            Texture2D circleTexture = GetHollowCircleTexture(diameter, color, borderWidth);

            // 计算绘制矩形（以圆心为中心）
            Rect drawRect = new Rect(
                center.x - radius,
                center.y - radius,
                diameter,
                diameter
            );

            // 绘制空心圆
            GUI.DrawTexture(drawRect, circleTexture);
        }

        /// <summary>
        /// 获取空心圆纹理（带缓存）
        /// </summary>
        private static Texture2D GetHollowCircleTexture(int diameter, Color color, float borderWidth)
        {
            string cacheKey = $"{diameter}_{color}_{borderWidth}";

            if (!hollowCircleTextureCache.TryGetValue(cacheKey, out Texture2D texture))
            {
                texture = CreateHollowCircleTexture(diameter, color, borderWidth);
                hollowCircleTextureCache[cacheKey] = texture;
            }

            return texture;
        }

        /// <summary>
        /// 创建空心圆纹理
        /// </summary>
        private static Texture2D CreateHollowCircleTexture(int diameter, Color color, float borderWidth)
        {
            if (diameter <= 0) return null;

            Texture2D texture = new Texture2D(diameter, diameter);
            float radius = diameter / 2f;
            float outerRadiusSquared = radius * radius;
            float innerRadius = radius - borderWidth;
            float innerRadiusSquared = innerRadius * innerRadius;

            // 遍历所有像素点
            for (int x = 0; x < diameter; x++)
            {
                for (int y = 0; y < diameter; y++)
                {
                    // 计算当前像素到圆心的距离平方
                    float distanceSquared = (x - radius) * (x - radius) + (y - radius) * (y - radius);

                    // 如果在圆环内（外圆内且内圆外），设置颜色；否则设置为透明
                    if (distanceSquared <= outerRadiusSquared && distanceSquared >= innerRadiusSquared)
                    {
                        texture.SetPixel(x, y, color);
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                }
            }

            texture.Apply();
            return texture;
        }

    }
}
