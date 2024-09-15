using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using BillUtils.SpaceShipData;
public class SpaceShipCreator : MonoBehaviour
{
    [SerializeField] private SpaceShipData spaceShipData;

    [Foldout("Space Ship Part")]
    [SerializeField] private SpaceShipPart Head;
    [Foldout("Space Ship Part")]
    [SerializeField] private SpaceShipPart Wing;
    [Foldout("Space Ship Part")]
    [SerializeField] private SpaceShipPart Weap;
    [Foldout("Space Ship Part")]
    [SerializeField] private SpaceShipPart Engine;

    [Button]
    public void InitSpaceShip()
    {
        Head.Init();
        Wing.Init();
        Weap.Init();
        Engine.Init();
    }

    [Button]
    public void GenerateSpaceShip()
    {
        Head.meshFilter.mesh = spaceShipData.GetPartMesh(E_SpaceShipPart.HEAD);
        Wing.meshFilter.mesh = spaceShipData.GetPartMesh(E_SpaceShipPart.WING);
        Weap.meshFilter.mesh = spaceShipData.GetPartMesh(E_SpaceShipPart.WEAP);
        Engine.meshFilter.mesh = spaceShipData.GetPartMesh(E_SpaceShipPart.ENGINE);
    }
}

[Serializable]
public class SpaceShipPart
{
    [ShowAssetPreview]
    public GameObject partObject;
    public MeshFilter meshFilter;
    public MeshRenderer mesh;
    public Material material;

    public void Init()
    {
        mesh = partObject.GetComponent<MeshRenderer>();
        meshFilter = partObject.GetComponent<MeshFilter>();
    }
}