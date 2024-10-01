using System;
using BillUtils.ColorTextureUtils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace BillUtils.SpaceShipData
{
    [Serializable]
    public enum E_SpaceShipPart
    {
        NONE,
        HEAD,
        WING,
        WEAP,
        ENGINE
    }

    [Serializable]
    public class MaterialPart
    {
        public E_SpaceShipPart Part;
        public MeshRenderer MeshRenderer;
        public Material Material;

        public void Init()
        {
            Material = MeshRenderer.material;
        }
    }
    [Serializable]
    public class ButtonChangeColor
    {
        public E_SpaceShipPart SpaceShipPart;
        public Button Button;
        public Image ColorImage;
        public Vector2Int PixelPick;
        private Color currentColor;

        public void Init(Action<Vector2Int> openColorPicker, Action<ButtonChangeColor> addCurrentButton)
        {
            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(() => addCurrentButton(this));
            Button.onClick.AddListener(() => openColorPicker(PixelPick));

        }
        public void SetColor(Color colorSet)
        {
            ColorImage.color = colorSet;
        }
        public void SetButtonColor(Texture2D texture2D)
        {
            if (Button == null)
            {
                Debug.LogWarning("Button reference is missing.");
                return;
            }
            currentColor = TextureColorUtils.GetPixelColor(texture2D, PixelPick);
            ColorImage.color = currentColor;
        }

    }

    public class SpaceShipPartBase
    {
        public string Name;
        public E_SpaceShipPart Part;
        public string Path;
        public string IconPath;

        public Sprite GetSprite()
        {
            return Resources.Load<Sprite>(IconPath);
        }
        public Mesh GetMesh()
        {
            return Resources.Load<Mesh>(Path);
        }
    }
    [Serializable]
    public class SpaceShipHead : SpaceShipPartBase { }

    [Serializable]
    public class SpaceShipWing : SpaceShipPartBase { }
    [Serializable]
    public class SpaceShipWeap : SpaceShipPartBase { }

    [Serializable]
    public class SpaceShipEngine : SpaceShipPartBase { }
}