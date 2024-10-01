using System.Collections;
using System.Collections.Generic;
using BillUtils.SpaceShipData;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private SpaceShipCreator spaceShipCreator;
    [SerializeField] private Animator UIAnim;
    [SerializeField] private SpaceShipData spaceShipData;
    [SerializeField] private GameObject TemplateObject;
    [SerializeField] private List<Item> items;
    [SerializeField] private bool isUpHover;
    private E_SpaceShipPart e_SpaceShipPart;
    private E_SpaceShipPart last_SpaceShipPartSet;

    void Start()
    {
        items = new();
        Init();
    }

    private GameObject obj;
    private Item item;

    private void Init()
    {
        last_SpaceShipPartSet = E_SpaceShipPart.HEAD;
        if (items.Count == 0)
        {
            for (int i = 0; i < spaceShipData.Limit + 1; i++)
            {
                obj = Instantiate(TemplateObject, transform);
                item = obj.GetComponent<Item>();
                item.itemController = this;
                item.shipData = spaceShipData;
                items.Add(item);
            }
        }
    }

    public void ChangeModular(Mesh mesh, E_SpaceShipPart e_SpaceShipPart)
    {
        spaceShipCreator.SetModular(mesh, e_SpaceShipPart);
    }
    public void OnClickTitleHover()
    {
        if (items[0].e_SpaceShipPart != E_SpaceShipPart.NONE && isUpHover)
        {
            UIAnim.SetTrigger("OnDeactive");
            this.e_SpaceShipPart = E_SpaceShipPart.NONE;
            isUpHover = false;
            return;
        }
        SetUpItem(last_SpaceShipPartSet);
    }
    public void SetUpItem(E_SpaceShipPart e_SpaceShipPart)
    {
        if (this.e_SpaceShipPart == e_SpaceShipPart)
        {
            UIAnim.SetTrigger("OnDeactive");
            this.e_SpaceShipPart = E_SpaceShipPart.NONE;
            isUpHover = false;
        }
        else
        {
            UIAnim.SetTrigger("OnActive");
            isUpHover = true;
            this.e_SpaceShipPart = e_SpaceShipPart;
            last_SpaceShipPartSet = e_SpaceShipPart;
        }


        for (int i = 0; i < spaceShipData.Limit + 1; i++)
        {
            items[i].Init(e_SpaceShipPart, i);
        }
    }
}