using UnityEngine;
using System.IO;
using System;
using BillUtils.ColorTextureUtils;
using System.Collections.Generic;
using BillUtils.SpaceShipData;
using NaughtyAttributes;
using UnityEngine.UI;
using TMPro;

public class ChangeTextureColor : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private Texture2D texture;
    [SerializeField] private Vector2Int currentPixel;
    [SerializeField] private Transform baseObject;
    [SerializeField] private TMP_Dropdown partDropdown;
    [SerializeField] private List<ButtonChangeColor> buttonChangeColors;
    [SerializeField] private ButtonChangeColor currentButton;

    [Foldout("Space Ship Part Toggles")]
    [SerializeField] private Toggle toggleHead;

    [Foldout("Space Ship Part Toggles")]
    [SerializeField] private Toggle toggleWing;

    [Foldout("Space Ship Part Toggles")]
    [SerializeField] private Toggle toggleWeap;

    [Foldout("Space Ship Part Toggles")]
    [SerializeField] private Toggle toggleEngine;

    public Vector2Int GetCurrentPixel => currentPixel;
    public Action OnSetPixelPick;
    private Color currentColorPick;
#if UnityEditor
    private Color[] colors = new Color[4];
#endif

    private void Start()
    {
        Init();
#if UnityEditor
        ApplyColorsToTexture();
#endif
    }

    private void Init()
    {
        InitMaterialAndTexture();
        InitButton();
    }

    private void InitMaterialAndTexture()
    {
        if (material == null)
            material = gameObject.GetComponent<MeshRenderer>().material;

        // Sử dụng đúng tên thuộc tính của Texture trong shader, ví dụ: "_BaseMap"
        string texturePropertyName = "_BaseMap"; // Thay bằng tên thuộc tính trong shader của bạn

        // Kiểm tra xem Material có thuộc tính này không
        if (material.HasProperty(texturePropertyName))
        {
            texture = material.GetTexture(texturePropertyName) as Texture2D;

            if (texture == null)
            {
                Debug.LogError($"Texture '{texturePropertyName}' không phải là Texture2D hoặc không tồn tại.");
                return;
            }

            if (!texture.isReadable)
            {
                Debug.LogError("Texture không thể đọc/ghi. Đảm bảo đã bật Read/Write trong Import Settings.");
                return;
            }
        }
        else
        {
            Debug.LogWarning($"Material không chứa thuộc tính Texture '{texturePropertyName}'.");
        }
    }


    private void InitButton()
    {
        foreach (ButtonChangeColor item in buttonChangeColors)
        {
            if (item.Button == null)
            {
                Debug.LogError($"Button component is missing for {item.SpaceShipPart}.");
                continue;
            }
            item.Init(OpenColorPicker, AddCurrentButtonColor);
            item.SetButtonColor(texture);
        }
    }


    int buttonIndex = 0;
    int pixelV = 0;
    int pixelH = 0;
    // [Button("SetupButtonLegacy")]
    private void SetupButtonOld()
    {
        pixelV = 0;
        pixelH = 0;
        buttonChangeColors.Clear();

        if (baseObject == null)
        {
            Debug.LogError("baseObject is not assigned!");
            return;
        }

        foreach (Transform baseTransform in baseObject)
        {
            foreach (Transform buttonTransform in baseTransform)
            {
                if (buttonTransform.name.Contains("Button"))
                {
                    Button buttonComponent = buttonTransform.GetComponent<Button>();

                    if (buttonComponent != null)
                    {
                        ButtonChangeColor buttonChangeColor = new ButtonChangeColor
                        {
                            SpaceShipPart = GetSpaceShipPart(pixelH),
                            Button = buttonComponent,
                            PixelPick = new Vector2Int(pixelV, pixelH)
                        };
                        pixelV++;
                        buttonChangeColors.Add(buttonChangeColor);
                        buttonChangeColor.Init(OpenColorPicker, AddCurrentButtonColor);
                        buttonIndex++;
                    }
                }

            }
            pixelV = 0;
            pixelH++;
        }
    }

    [Button("Set Up Button Standard")]
    private void SetupButtonNew()
    {
        pixelV = 0;
        pixelH = 0;
        buttonChangeColors.Clear();

        if (baseObject == null)
        {
            Debug.LogError("baseObject is not assigned!");
            return;
        }

        foreach (Transform baseTransform in baseObject)
        {
            Transform colorTransform = baseTransform.Find("Color");
            if (colorTransform == null)
            {
                Debug.LogWarning($"Color Transform not found in {baseTransform.name}");
                continue;
            }

            foreach (Transform buttonTransform in colorTransform)
            {
                if (buttonTransform.name.Contains("Button"))
                {
                    Button buttonComponent = buttonTransform.GetComponent<Button>();
                    Image colorImage = buttonTransform.GetChild(0).GetComponent<Image>();
                    if (buttonComponent != null)
                    {
                        ButtonChangeColor buttonChangeColor = new ButtonChangeColor
                        {
                            SpaceShipPart = GetSpaceShipPart(pixelH),
                            Button = buttonComponent,
                            ColorImage = colorImage,
                            PixelPick = new Vector2Int(pixelV, pixelH)
                        };
                        pixelV++;
                        buttonChangeColors.Add(buttonChangeColor);
                        buttonChangeColor.Init(OpenColorPicker, AddCurrentButtonColor);
                        buttonIndex++;
                    }
                }
            }

            pixelV = 0;
            pixelH++;
        }
    }

    public E_SpaceShipPart GetSpaceShipPart(int pixelY)
    {
        switch (pixelY)
        {
            case 0:
                Debug.Log("Head");
                return E_SpaceShipPart.HEAD;
            case 1:
                Debug.Log("Wing");
                return E_SpaceShipPart.WING;
            case 2:
                Debug.Log("Weap");
                return E_SpaceShipPart.WEAP;
            case 3:
                Debug.Log("Engine");
                return E_SpaceShipPart.ENGINE;
            default:
                Debug.Log("None");
                return E_SpaceShipPart.NONE;
        }
    }

    public void AddCurrentButtonColor(ButtonChangeColor buttonChangeColor)
    {
        currentButton = buttonChangeColor;
    }

    public void OpenColorPicker(Vector2Int pixelPick)
    {
        if (!ColorPicker.done)
        {
            ColorPicker.Done();
        }
        currentPixel = pixelPick;
        OnSetPixelPick?.Invoke();
        currentColorPick = TextureColorUtils.GetPixelColor(texture, pixelPick);

        ColorPicker.Create(currentColorPick, "Color Part Picking UV Position" + currentPixel.ToString(), (c) => OnColorChange(c, currentPixel), (c) => OnColorSelected(c, currentPixel), false);
    }

    private void OnColorChange(Color c, Vector2Int currentPixel)
    {
        if (currentButton != null)
        {
            currentButton.SetColor(c);
        }
        TextureColorUtils.SetPixelColor(texture, currentPixel.x, currentPixel.y, c);
    }

    private void OnColorSelected(Color c, Vector2Int currentPixel)
    {
        if (currentButton != null)
        {
            currentButton.SetColor(c);
        }
        TextureColorUtils.SetPixelColor(texture, currentPixel.x, currentPixel.y, c);
    }

    public void SaveTextureToFile(string filePath)
    {
        try
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            byte[] textureBytes = texture.EncodeToPNG();
            File.WriteAllBytes(filePath, textureBytes);
            Debug.Log("Texture saved successfully to " + filePath);
        }
        catch (UnauthorizedAccessException e)
        {
            Debug.LogError("Access denied: " + e.Message);
        }
        catch (Exception e)
        {
            Debug.LogError("An error occurred while saving the texture: " + e.Message);
        }
    }

    public void RandomizeColors()
    {
        RandomizeColorsQuadradic();
    }

    [Button("Randomize Colors Triad")] // Thêm button NaughtyAttributes để gọi hàm này trong Inspector
    public void RandomizeColorTriad()
    {
        // 1. Chọn màu gốc ngẫu nhiên (Hue)
        float baseHue = UnityEngine.Random.value; // Giá trị Hue từ 0.0 đến 1.0

        // 2. Tạo bộ ba màu triad (HSV)
        Color color1HSV = Color.HSVToRGB(baseHue, 0.7f, 0.9f); // Màu gốc (Saturation và Value có thể tùy chỉnh)
        Color color2HSV = Color.HSVToRGB((baseHue + 0.333f) % 1f, 0.7f, 0.9f); // Màu thứ hai (Hue + 120 độ = 1/3 vòng tròn)
        Color color3HSV = Color.HSVToRGB((baseHue + 0.666f) % 1f, 0.7f, 0.9f); // Màu thứ ba (Hue + 240 độ = 2/3 vòng tròn)

        Color[] triadColors = new Color[] { color1HSV, color2HSV, color3HSV }; // Mảng chứa bộ ba màu

        int colorIndex = 0; // Biến để theo dõi màu hiện tại trong triad

        foreach (ButtonChangeColor buttonChangeColor in buttonChangeColors)
        {
            bool isLocked = IsPartLocked(buttonChangeColor.SpaceShipPart);
            if (isLocked)
            {
                Debug.Log($"{buttonChangeColor.SpaceShipPart} is locked, skipping color change.");
                continue;
            }

            // 3. Lấy màu từ triad theo thứ tự vòng lặp
            Color colorToApply = triadColors[colorIndex % triadColors.Length]; // Lấy màu từ mảng triad và đảm bảo index không vượt quá giới hạn

            buttonChangeColor.SetColor(colorToApply);
            TextureColorUtils.SetPixelColor(texture, buttonChangeColor.PixelPick.x, buttonChangeColor.PixelPick.y, colorToApply);

            colorIndex++; // Chuyển sang màu tiếp theo trong triad cho nút tiếp theo
        }
        texture.Apply();
    }



    [Button("Randomize Colors Quadradic")] // Thêm button NaughtyAttributes để gọi hàm này trong Inspector
    public void RandomizeColorsQuadradic() // Đổi tên hàm thành RandomizeColorsQuadradic để phân biệt
    {
        // 1. Chọn màu gốc ngẫu nhiên (Hue)
        float baseHue = UnityEngine.Random.value;

        // 2. Tạo bộ tứ màu Quadradic (Rectangle - Hình chữ nhật)
        Color color1HSV = Color.HSVToRGB(baseHue, 0.6f, 0.9f);       // Màu gốc (Saturation và Value tùy chỉnh)
        Color color2HSV = Color.HSVToRGB((baseHue + 0.5f) % 1f, 0.6f, 0.9f);   // Màu bổ sung trực tiếp (180 độ)
        Color color3HSV = Color.HSVToRGB((baseHue + 0.25f) % 1f, 0.6f, 0.9f);  // Màu thứ ba (90 độ từ gốc)
        Color color4HSV = Color.HSVToRGB((baseHue + 0.75f) % 1f, 0.6f, 0.9f);  // Màu bổ sung của màu thứ ba (270 độ từ gốc, hoặc 180 độ từ màu thứ ba)

        Color[] quadColors = new Color[] { color1HSV, color2HSV, color3HSV, color4HSV }; // Mảng 4 màu Quadradic

        int colorIndex = 0;

        foreach (ButtonChangeColor buttonChangeColor in buttonChangeColors)
        {
            bool isLocked = IsPartLocked(buttonChangeColor.SpaceShipPart);
            if (isLocked)
            {
                Debug.Log($"{buttonChangeColor.SpaceShipPart} is locked, skipping color change.");
                continue;
            }

            Color colorToApply = quadColors[colorIndex % quadColors.Length]; // Lấy màu từ mảng quadColors

            buttonChangeColor.SetColor(colorToApply);
            TextureColorUtils.SetPixelColor(texture, buttonChangeColor.PixelPick.x, buttonChangeColor.PixelPick.y, colorToApply);

            colorIndex++;
        }
        texture.Apply();
    }


    private bool IsPartLocked(E_SpaceShipPart part)
    {
        switch (part)
        {
            case E_SpaceShipPart.HEAD:
                return toggleHead.isOn; // Nếu toggleHead bật thì part HEAD bị khóa
            case E_SpaceShipPart.WING:
                return toggleWing.isOn; // Tương tự với Wing
            case E_SpaceShipPart.WEAP:
                return toggleWeap.isOn; // Tương tự với Weap
            case E_SpaceShipPart.ENGINE:
                return toggleEngine.isOn; // Tương tự với Engine
            default:
                return false; // Nếu không phải các part trên thì không bị khóa
        }
    }


    public void ApplyColorFromSelectedPart()
    {
        E_SpaceShipPart selectedPart = (E_SpaceShipPart)partDropdown.value;
        ButtonChangeColor selectedButton = buttonChangeColors.Find(b => b.SpaceShipPart == selectedPart);

        if (selectedButton == null)
        {
            Debug.LogError("Không tìm thấy bộ phận được chọn.");
            return;
        }

        // Lấy màu hiện tại của các pixel của bộ phận được chọn theo trục X
        List<Color> selectedColors = new List<Color>();
        for (int x = 0; x < 4; x++)
        {
            Vector2Int pixelPick = new Vector2Int(x, selectedButton.PixelPick.y);
            selectedColors.Add(TextureColorUtils.GetPixelColor(texture, pixelPick));
        }

        foreach (ButtonChangeColor button in buttonChangeColors)
        {
            // Bỏ qua bộ phận đã chọn, chỉ thay đổi màu cho các phần khác
            if (button.SpaceShipPart != selectedPart)
            {

                Color colorToApply = selectedColors[button.PixelPick.x];
                button.SetColor(colorToApply);
                TextureColorUtils.SetPixelColor(texture, button.PixelPick.x, button.PixelPick.y, colorToApply);
            }
        }
        texture.Apply();
    }

    Color[] colors;
    int width;
    int height;
    int colorIndex;
    public void ResetTextureColors()
    {
        colors = new Color[]
        {
            HexToColor("#bba45a"),
            HexToColor("#672121"),
            HexToColor("#000000"),
            HexToColor("#ffffff")
        };

        width = texture.width;
        height = texture.height;
        colorIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color colorToApply = colors[colorIndex % colors.Length];
                TextureColorUtils.SetPixelColor(texture, x, y, colorToApply);
                ButtonChangeColor buttonChangeColor = buttonChangeColors.Find(b => b.PixelPick == new Vector2Int(x, y));
                if (buttonChangeColor != null)
                {
                    buttonChangeColor.SetColor(colorToApply);
                }

                colorIndex++;
            }
        }

        texture.Apply();
    }

    private Color HexToColor(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            return color;
        }
        else
        {
            Debug.LogError($"Mã màu không hợp lệ: {hex}");
            return Color.white;
        }
    }



#if UnityEditor
    void OnValidate()
    {
        if (colors.Length != 4)
        {
            Debug.LogError("Phải có đúng 4 màu trong mảng!");
            return;
        }

        if (material != null && material.mainTexture is Texture2D mainTexture)
        {
            texture = mainTexture;
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

    void ApplyColorsToTexture()
    {
        TextureColorUtils.SetPixelColor(texture, 0, 0, colors[0]);
        TextureColorUtils.SetPixelColor(texture, 0, 1, colors[1]);
        TextureColorUtils.SetPixelColor(texture, 1, 0, colors[2]);
        TextureColorUtils.SetPixelColor(texture, 1, 1, colors[3]);

        texture.Apply();
    }
#endif
}
