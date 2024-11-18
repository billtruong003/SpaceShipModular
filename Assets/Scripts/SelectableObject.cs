using System.Collections;
using System.Collections.Generic;
using BillUtils.SpaceShipData;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    [SerializeField] private ChangeTextureColor changeTextureColor;
    [SerializeField] private E_SpaceShipPart partType;
    [SerializeField] private Transform pivot;
    [SerializeField] private GameObject ColorPalette;
    public Transform GetPivot => pivot;
    public E_SpaceShipPart GetPartType => partType;
}
