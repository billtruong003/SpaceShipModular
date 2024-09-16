using UnityEngine;
using System.IO;

public class ChangeTextureColor : MonoBehaviour
{
    public Material material; // Tấm material bạn muốn thay đổi
    private Texture2D texture; // Texture 2x2

    [SerializeField]
    private Color[] colors = new Color[4]; // Mảng chứa 4 màu, 1 màu cho mỗi pixel

    void Start()
    {
        if (material == null)
            material = gameObject.GetComponent<MeshRenderer>().material;

        texture = material.mainTexture as Texture2D;

        // Kiểm tra xem texture có hợp lệ và có kích thước 2x2 hay không
        if (texture == null || texture.width != 2 || texture.height != 2)
        {
            Debug.LogError("Texture không hợp lệ hoặc không có kích thước 2x2!");
            return;
        }

        // Kiểm tra nếu texture có thể đọc/ghi
        if (!texture.isReadable)
        {
            Debug.LogError("Texture không thể đọc/ghi. Đảm bảo đã bật Read/Write trong Import Settings.");
            return;
        }

        // Áp dụng màu sắc ban đầu cho texture
        ApplyColorsToTexture();
    }

    void OnValidate()
    {
        if (colors.Length != 4)
        {
            Debug.LogError("Phải có đúng 4 màu trong mảng!");
            return;
        }

        // Kiểm tra xem Material và Texture có hợp lệ không
        if (material != null && material.mainTexture is Texture2D mainTexture)
        {
            texture = mainTexture;

            // Kiểm tra kích thước của texture
            if (texture.width == 2 && texture.height == 2)
            {
                ApplyColorsToTexture();
            }
            else
            {
                Debug.LogWarning("Texture không có kích thước 2x2.");
            }
        }
    }

    // Hàm để áp dụng màu sắc vào texture
    void ApplyColorsToTexture()
    {
        // Đặt màu cho từng pixel trong texture
        SetPixelColor(0, 0, colors[0]); // Pixel [0,0]
        SetPixelColor(0, 1, colors[1]); // Pixel [0,1]
        SetPixelColor(1, 0, colors[2]); // Pixel [1,0]
        SetPixelColor(1, 1, colors[3]); // Pixel [1,1]

        // Cập nhật texture sau khi thay đổi
        texture.Apply();
    }

    // Hàm để thay đổi màu pixel trong texture
    void SetPixelColor(int x, int y, Color color)
    {
        if (x >= 0 && x < texture.width && y >= 0 && y < texture.height)
        {
            texture.SetPixel(x, y, color);
        }
        else
        {
            Debug.LogWarning("Tọa độ pixel không hợp lệ!");
        }
    }

    // Mở trình chọn màu cho pixel
    public void OpenColorPicker(int pixelIndex)
    {
        // Kiểm tra pixelIndex hợp lệ
        if (pixelIndex < 0 || pixelIndex >= colors.Length)
        {
            Debug.LogWarning("Pixel index không hợp lệ!");
            return;
        }

        // Tạo trình chọn màu và truyền index pixel
        ColorPicker.Create(colors[pixelIndex], "Chọn màu cho pixel " + pixelIndex, (c) => OnColorChanged(c, pixelIndex), (c) => OnColorSelected(c, pixelIndex), false);
    }

    // Sự kiện khi màu được thay đổi trong ColorPicker
    private void OnColorChanged(Color c, int pixelIndex)
    {
        // Cập nhật màu của pixel tương ứng
        colors[pixelIndex] = c;
        ApplyColorsToTexture();
    }

    // Sự kiện khi màu được chọn trong ColorPicker
    private void OnColorSelected(Color c, int pixelIndex)
    {
        // Cập nhật màu của pixel khi người dùng chọn
        colors[pixelIndex] = c;
        ApplyColorsToTexture();
    }

    // Lưu texture ra file PNG
    public void SaveTextureToFile(string filePath)
    {
        if (texture == null)
        {
            Debug.LogError("Texture không hợp lệ!");
            return;
        }

        // Tạo một mảng byte từ texture
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("Texture đã được lưu tại: " + filePath);
    }
}
