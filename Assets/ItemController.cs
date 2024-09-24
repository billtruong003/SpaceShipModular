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
    private E_SpaceShipPart e_SpaceShipPart;

    void Start()
    {
        items = new();
        Init();
        UIAnim.gameObject.SetActive(false);
    }
    private GameObject obj;
    private Item item;
    private void Init()
    {
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

    public void SetUpItem(E_SpaceShipPart e_SpaceShipPart)
    {
        if (UIAnim.gameObject.activeSelf == false)
        {
            UIAnim.gameObject.SetActive(true);
        }
        if (this.e_SpaceShipPart == e_SpaceShipPart)
        {
            UIAnim.SetTrigger("OnDeactive");
            this.e_SpaceShipPart = E_SpaceShipPart.NONE;
        }
        else
        {
            UIAnim.SetTrigger("OnActive");
            this.e_SpaceShipPart = e_SpaceShipPart;
        }

        for (int i = 0; i < spaceShipData.Limit + 1; i++)
        {
            items[i].Init(e_SpaceShipPart, i);
        }
    }
}