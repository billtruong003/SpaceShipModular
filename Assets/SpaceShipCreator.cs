using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipCreator : MonoBehaviour
{
    [SerializeField] private SpaceShipPart Head;
    [SerializeField] private SpaceShipPart Wing;
    [SerializeField] private SpaceShipPart Weap;
    [SerializeField] private SpaceShipPart Engine;
}

[Serializable]
public class SpaceShipPart
{
    public GameObject partObject;
    public MeshRenderer mesh;
    public Material material;
}