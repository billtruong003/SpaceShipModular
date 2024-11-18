using UnityEngine;

namespace BillUtils.ColorTextureUtils
{
    public static class TextureColorUtils
    {
        public static Color GetPixelColor(Texture2D texture, Vector2Int pixelPick)
        {
            int x = pixelPick.x;
            int y = pixelPick.y;
            if (x < 0 || x >= texture.width || y < 0 || y >= texture.height)
            {
                Debug.LogWarning("Tọa độ pixel không hợp lệ!");
                return Color.clear;
            }

            Color color = texture.GetPixel(x, y);
            Debug.Log($"Màu sắc tại pixel ({x}, {y}): {color}");
            return color;
        }


        public static void SetPixelColor(Texture2D texture, int x, int y, Color color)
        {
            if (x >= 0 && x < texture.width && y >= 0 && y < texture.height)
            {
                texture.SetPixel(x, y, color);
            }
            else
            {
                Debug.LogWarning("Tọa độ pixel không hợp lệ!");
            }
            texture.Apply();
        }
    }
}
