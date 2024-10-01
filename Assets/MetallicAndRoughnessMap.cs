using System.Collections;
using System.Collections.Generic;
using BillUtils.SpaceShipData;
using UnityEngine;
using UnityEngine.UI;

public class MetallicAndRoughnessMap : MonoBehaviour
{
    [SerializeField] private ChangeTextureColor changeTextureColor;
    [SerializeField] private Animator panelAnim;
    [SerializeField] private Toggle metallicToggle;
    [SerializeField] private Toggle roughnessToggle;
    [SerializeField] private Slider metallicSlider;
    [SerializeField] private Slider roughnessSlider;
    [SerializeField] private Material targetMaterial;
    [SerializeField] private Texture2D metallicMap;
    [SerializeField] private Texture2D roughnessMap;
    [SerializeField] private Vector2 pixelPick;
    [SerializeField] private List<MaterialPart> materialParts;
    private bool isOpen;

    private void OnEnable()
    {
        isOpen = false;
    }

    private void Awake()
    {
        changeTextureColor.OnSetPixelPick = SetPixelPick;
        // Initialize the maps as black textures
        InitializeTextures();
        InitMaterialPart();
        // Add listeners to Toggles
        metallicToggle.onValueChanged.AddListener(OnMetallicToggleChanged);
        roughnessToggle.onValueChanged.AddListener(OnRoughnessToggleChanged);

        // Add listeners to Sliders
        metallicSlider.onValueChanged.AddListener(OnMetallicSliderChanged);
        roughnessSlider.onValueChanged.AddListener(OnRoughnessSliderChanged);
    }

    private void GetTexture()
    {
        metallicMap = targetMaterial.GetTexture("_MetallicMap") as Texture2D;
        roughnessMap = targetMaterial.GetTexture("_RoughnessMap") as Texture2D;
    }

    private void InitMaterialPart()
    {
        foreach (var item in materialParts)
        {
            item.Init();
        }
    }
    private void InitializeTextures()
    {
        if (metallicMap == null && roughnessMap == null)
        {
            GetTexture();
        }
        InitializeTexture(metallicMap);
        InitializeTexture(roughnessMap);
    }

    public void OnClickOpenPanel()
    {
        if (!isOpen)
        {
            panelAnim.SetTrigger("Open");
            isOpen = true;
            return;
        }
        panelAnim.SetTrigger("Close");
        isOpen = false;
    }

    private void InitializeTexture(Texture2D texture)
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, Color.black);
            }
        }
        texture.Apply();
    }

    private void CheckPixelColor()
    {
        Color metallicPixelColor = GetPixelColorAtPosition(pixelPick, metallicMap);
        if (metallicPixelColor == Color.white)
        {
            metallicToggle.isOn = true;
        }
        else if (metallicPixelColor == Color.black)
        {
            metallicToggle.isOn = false;
        }

        Color roughnessPixelColor = GetPixelColorAtPosition(pixelPick, roughnessMap);
        if (roughnessPixelColor == Color.white)
        {
            roughnessToggle.isOn = true;
        }
        else if (roughnessPixelColor == Color.black)
        {
            roughnessToggle.isOn = false;
        }
    }

    private Color GetPixelColorAtPosition(Vector2 pixelPosition, Texture2D texture)
    {
        int x = Mathf.Clamp((int)pixelPosition.x, 0, texture.width - 1);
        int y = Mathf.Clamp((int)pixelPosition.y, 0, texture.height - 1);
        return texture.GetPixel(x, y);
    }

    private void OnMetallicToggleChanged(bool isOn)
    {
        if (isOn)
        {
            ApplyPixelColor(metallicMap, pixelPick); // Đặt pixel thành màu trắng khi bật toggle
            UpdateMaterial();
        }
        else
        {
            SetPixelToBlack(pixelPick, metallicMap); // Set pixel thành màu đen khi tắt toggle
            UpdateMaterial();
        }
    }
    private void OnRoughnessToggleChanged(bool isOn)
    {
        if (isOn)
        {
            ApplyPixelColor(roughnessMap, pixelPick); // Đặt pixel thành màu trắng khi bật toggle
            UpdateMaterial();
        }
        else
        {
            SetPixelToBlack(pixelPick, roughnessMap); // Set pixel thành màu đen khi tắt toggle
            UpdateMaterial();
        }
    }

    private void SetPixelToBlack(Vector2 pixelPosition, Texture2D texture)
    {
        int x = Mathf.Clamp((int)pixelPosition.x, 0, texture.width - 1);
        int y = Mathf.Clamp((int)pixelPosition.y, 0, texture.height - 1);

        texture.SetPixel(x, y, Color.black);
        texture.Apply();
    }

    public void SetPixelPick()
    {
        pixelPick = changeTextureColor.GetCurrentPixel;

        CheckPixelColor();
    }

    // Đặt pixel thành màu trắng tại vị trí pixelPick
    public void ApplyPixelColor(Texture2D texture, Vector2 pixelPosition)
    {
        int x = Mathf.Clamp((int)pixelPosition.x, 0, texture.width - 1);
        int y = Mathf.Clamp((int)pixelPosition.y, 0, texture.height - 1);

        texture.SetPixel(x, y, Color.white);
        texture.Apply();
        UpdateMaterial();
    }

    // Update material based on the active map (metallic or roughness)
    private void UpdateMaterial()
    {
        if (metallicToggle.isOn)
        {
            materialParts[(int)pixelPick.y].Material.SetFloat("_Metallic", metallicSlider.value);
            targetMaterial.SetTexture("_MetallicMap", metallicMap);
        }
        else if (roughnessToggle.isOn)
        {
            materialParts[(int)pixelPick.y].Material.SetFloat("_Metallic", metallicSlider.value);
            targetMaterial.SetTexture("_RoughnessMap", roughnessMap);
        }
    }

    // Cập nhật khi giá trị slider metallic thay đổi
    private void OnMetallicSliderChanged(float value)
    {
        if (metallicToggle.isOn)
        {
            UpdateMaterial();
        }
        else
        {
            SetPixelToBlack(pixelPick, metallicMap);
        }
    }

    // Cập nhật khi giá trị slider roughness thay đổi
    private void OnRoughnessSliderChanged(float value)
    {
        if (roughnessToggle.isOn)
        {
            UpdateMaterial();
        }
        else
        {
            SetPixelToBlack(pixelPick, roughnessMap);
        }
    }
}
