using System.Collections;
using System.Collections.Generic;
using BillUtils.SpaceShipData;
using UnityEngine;
using UnityEngine.UI;

public class ChangePart : MonoBehaviour
{
    [SerializeField] private E_SpaceShipPart e_SpaceShipPart;
    [SerializeField] private ItemController itemController;

    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => TriggerItemPicking());
    }
    private void TriggerItemPicking()
    {
        itemController.SetUpItem(e_SpaceShipPart);
    }
}
