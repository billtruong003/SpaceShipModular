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

        // Add listeners to Toggles
        metallicToggle.onValueChanged.AddListener(OnMetallicToggleChanged);
        roughnessToggle.onValueChanged.AddListener(OnRoughnessToggleChanged);

        // Add listeners to Sliders
        metallicSlider.onValueChanged.AddListener(OnMetallicSliderChanged);
        roughnessSlider.onValueChanged.AddListener(OnRoughnessSliderChanged);
    }

    private void Start()
    {
        // Initialize the maps as black textures
        InitializeTextures();
        InitMaterialPart();
    }

    private void GetTexture()
    {
        Debug.Log($"[InstanceID:{GetInstanceID()}] GetTexture() called. targetMaterial: {targetMaterial}"); // Debug log added
        if (targetMaterial != null)
        {
            metallicMap = targetMaterial.GetTexture("_MetallicMap") as Texture2D;
            roughnessMap = targetMaterial.GetTexture("_SmoothnessMap") as Texture2D; // Corrected to "_SmoothnessMap"
            Debug.Log($"[InstanceID:{GetInstanceID()}] metallicMap after GetTexture: {metallicMap}"); // Debug log added
            Debug.Log($"[InstanceID:{GetInstanceID()}] roughnessMap after GetTexture: {roughnessMap}"); // Debug log added
        }
        else
        {
            Debug.LogError($"[InstanceID:{GetInstanceID()}] targetMaterial is NULL in GetTexture()!"); // Error log if targetMaterial is null
        }
    }

    private void InitMaterialPart()
    {
        foreach (var item in materialParts)
        {
            item.Init();
            Debug.Log($"[InstanceID:{GetInstanceID()}] InitMaterialPart: Initialized material part for {item.Part} with material {item.Material.name}"); // Debug log
        }
    }

    private void InitializeTextures()
    {
        Debug.Log($"[InstanceID:{GetInstanceID()}] InitializeTextures() called. metallicMap before GetTexture: {metallicMap}, roughnessMap before GetTexture: {roughnessMap}"); // Debug log
        if (metallicMap == null && roughnessMap == null)
        {
            GetTexture();
        }
        Debug.Log($"[InstanceID:{GetInstanceID()}] metallicMap before InitializeTexture call: {metallicMap}"); // Debug log
        Debug.Log($"[InstanceID:{GetInstanceID()}] roughnessMap before InitializeTexture call: {roughnessMap}"); // Debug log
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
        if (texture == null) // Null check added
        {
            Debug.LogError($"[InstanceID:{GetInstanceID()}] InitializeTexture received NULL texture!");
            return;
        }
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
        Debug.Log($"[InstanceID:{GetInstanceID()}] CheckPixelColor called. metallicMap: {metallicMap}, roughnessMap: {roughnessMap}"); // Debug log added
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
        if (texture == null) // Null check added
        {
            Debug.LogError($"[InstanceID:{GetInstanceID()}] GetPixelColorAtPosition received NULL texture!");
            return Color.black; // Return a default color to avoid further errors
        }
        int x = Mathf.Clamp((int)pixelPosition.x, 0, texture.width - 1);
        int y = Mathf.Clamp((int)pixelPosition.y, 0, texture.height - 1);
        return texture.GetPixel(x, y); // Line 119 - No longer directly throws NullReferenceException due to null check
    }

    private void OnMetallicToggleChanged(bool isOn)
    {
        if (metallicToggle == null) return; // Thêm check null cho toggle
        Debug.Log($"[InstanceID:{GetInstanceID()}] OnMetallicToggleChanged: isOn = {isOn}"); // Debug log
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
        if (roughnessToggle == null) return; // Thêm check null cho toggle
        Debug.Log($"[InstanceID:{GetInstanceID()}] OnRoughnessToggleChanged: isOn = {isOn}"); // Debug log
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
        if (texture == null) return; // Null check added, exit if texture is null
        int x = Mathf.Clamp((int)pixelPosition.x, 0, texture.width - 1);
        int y = Mathf.Clamp((int)pixelPosition.y, 0, texture.height - 1);

        texture.SetPixel(x, y, Color.black);
        texture.Apply();
    }

    public void SetPixelPick()
    {
        pixelPick = changeTextureColor.GetCurrentPixel;
        Debug.Log($"[InstanceID:{GetInstanceID()}] SetPixelPick called. targetMaterial: {targetMaterial}, metallicMap: {metallicMap}, roughnessMap: {roughnessMap}"); // Debug log added

        CheckPixelColor();
    }

    // Đặt pixel thành màu trắng tại vị trí pixelPick
    public void ApplyPixelColor(Texture2D texture, Vector2 pixelPosition)
    {
        if (texture == null) return; // Null check added, exit if texture is null
        int x = Mathf.Clamp((int)pixelPosition.x, 0, texture.width - 1);
        int y = Mathf.Clamp((int)pixelPosition.y, 0, texture.height - 1);

        texture.SetPixel(x, y, Color.white);
        texture.Apply();
        UpdateMaterial();
    }

    // Update material based on the active map (metallic or roughness)
    private void UpdateMaterial()
    {
        Debug.Log($"[InstanceID:{GetInstanceID()}] UpdateMaterial() được gọi"); // Debug log
        if (metallicToggle.isOn)
        {
            if (materialParts.Count > (int)pixelPick.y && materialParts[(int)pixelPick.y] != null && materialParts[(int)pixelPick.y].Material != null)
            {
                Debug.Log($"[InstanceID:{GetInstanceID()}] Set _Metallic thành: {metallicSlider.value} cho material part {materialParts[(int)pixelPick.y].Material.name}"); // Debug log
                materialParts[(int)pixelPick.y].Material.SetFloat("_Metallic", metallicSlider.value);
            }
            if (targetMaterial != null)
            {
                targetMaterial.SetTexture("_MetallicMap", metallicMap);
                Debug.Log($"[InstanceID:{GetInstanceID()}] Set targetMaterial _MetallicMap texture"); // Debug log
            }

        }
        else if (roughnessToggle.isOn)
        {
            if (materialParts.Count > (int)pixelPick.y && materialParts[(int)pixelPick.y] != null && materialParts[(int)pixelPick.y].Material != null)
            {
                Debug.Log($"[InstanceID:{GetInstanceID()}] Set _Smoothness thành: {roughnessSlider.value} cho material part {materialParts[(int)pixelPick.y].Material.name}"); // Debug log
                materialParts[(int)pixelPick.y].Material.SetFloat("_Smoothness", roughnessSlider.value); // Đã sửa thành _Smoothness
            }
            if (targetMaterial != null)
            {
                targetMaterial.SetTexture("_RoughnessMap", roughnessMap);
                Debug.Log($"[InstanceID:{GetInstanceID()}] Set targetMaterial _RoughnessMap texture"); // Debug log
            }
        }
    }

    // Cập nhật khi giá trị slider metallic thay đổi
    private void OnMetallicSliderChanged(float value)
    {
        Debug.Log($"[InstanceID:{GetInstanceID()}] OnMetallicSliderChanged: giá trị = {value}"); // Debug log
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
        Debug.Log($"[InstanceID:{GetInstanceID()}] OnRoughnessSliderChanged: giá trị = {value}"); // Debug log
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