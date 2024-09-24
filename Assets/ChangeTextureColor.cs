using UnityEngine;
using System.IO;
using System;
using BillUtils.ColorTextureUtils;
using System.Collections.Generic;
using BillUtils.SpaceShipData;
using NaughtyAttributes;
using UnityEngine.UI;
using Unity.VisualScripting;
public class ChangeTextureColor : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private Texture2D texture;
    [SerializeField] private Vector2Int currentPixel;
    [SerializeField] private Transform baseObject;
    [SerializeField] private List<ButtonChangeColor> buttonChangeColors;
    [SerializeField] private ButtonChangeColor currentButton;

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
        string texturePropertyName = "Texture2D_DB342F0C"; // Thay bằng tên thuộc tính trong shader của bạn

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
        currentColorPick = TextureColorUtils.GetPixelColor(texture, pixelPick);

        ColorPicker.Create(currentColorPick, "Color Part Picking UV Position" + currentPixel.ToString(), (c) => OnColorChange(c, currentPixel), (c) => OnColorSelecte(c, currentPixel), false);
    }

    private void OnColorChange(Color c, Vector2Int currentPixel)
    {
        if (currentButton != null)
        {
            currentButton.SetColor(c);
        }
        TextureColorUtils.SetPixelColor(texture, currentPixel.x, currentPixel.y, c);
    }

    private void OnColorSelecte(Color c, Vector2Int currentPixel)
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
