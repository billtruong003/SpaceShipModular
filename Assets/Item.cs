using System.Collections;
using System.Collections.Generic;
using BillUtils.SpaceShipData;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemController itemController;
    public SpaceShipPartBase spaceShipPartBase;
    public SpaceShipData shipData;
    public E_SpaceShipPart e_SpaceShipPart;
    public Image image;

    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => OnClickButton());
    }
    public void Init(E_SpaceShipPart e_SpaceShipPart, int index)
    {
        switch (e_SpaceShipPart)
        {
            case E_SpaceShipPart.HEAD:
                this.e_SpaceShipPart = E_SpaceShipPart.HEAD;
                SetSpaceShipPart(shipData.spaceShipHeads[index]);
                image.sprite = spaceShipPartBase.GetSprite();
                break;
            case E_SpaceShipPart.WING:
                this.e_SpaceShipPart = E_SpaceShipPart.WING;
                SetSpaceShipPart(shipData.spaceShipWings[index]);
                image.sprite = spaceShipPartBase.GetSprite();
                break;
            case E_SpaceShipPart.WEAP:
                this.e_SpaceShipPart = E_SpaceShipPart.WEAP;
                SetSpaceShipPart(shipData.spaceShipWeaps[index]);
                image.sprite = spaceShipPartBase.GetSprite();
                break;
            case E_SpaceShipPart.ENGINE:
                this.e_SpaceShipPart = E_SpaceShipPart.ENGINE;
                SetSpaceShipPart(shipData.spaceShipEngines[index]);
                image.sprite = spaceShipPartBase.GetSprite();
                break;
            default:
                Debug.LogWarning("Không tìm thấy phần bộ phận tàu vũ trụ.");
                break;
        }
    }
    public void OnClickButton()
    {
        itemController.ChangeModular(spaceShipPartBase.GetMesh(), e_SpaceShipPart);
    }
    public void SetSpaceShipPart(SpaceShipPartBase spaceShipPartBase)
    {
        this.spaceShipPartBase = spaceShipPartBase;
    }
}
