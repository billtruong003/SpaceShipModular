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
    [SerializeField] private int PickModelMark;
    [SerializeField] private bool PickCorrectModelMark;

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
        if (!PickCorrectModelMark)
        {
            Head.SetUpMesh(spaceShipData.GetPartMesh(E_SpaceShipPart.HEAD));
            Wing.SetUpMesh(spaceShipData.GetPartMesh(E_SpaceShipPart.WING));
            Weap.SetUpMesh(spaceShipData.GetPartMesh(E_SpaceShipPart.WEAP));
            Engine.SetUpMesh(spaceShipData.GetPartMesh(E_SpaceShipPart.ENGINE));
        }
        else
        {
            Head.SetUpMesh(spaceShipData.GetCorrectMesh(E_SpaceShipPart.HEAD, PickModelMark));
            Wing.SetUpMesh(spaceShipData.GetCorrectMesh(E_SpaceShipPart.WING, PickModelMark));
            Weap.SetUpMesh(spaceShipData.GetCorrectMesh(E_SpaceShipPart.WEAP, PickModelMark));
            Engine.SetUpMesh(spaceShipData.GetCorrectMesh(E_SpaceShipPart.ENGINE, PickModelMark));
        }
    }
}

[Serializable]
public class SpaceShipPart
{
    public string MeshName;
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

    public void SetUpMesh(Mesh mesh)
    {
        MeshName = mesh.name;
        meshFilter.mesh = mesh;
    }
}