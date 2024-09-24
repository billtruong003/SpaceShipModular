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
    [Button]
    public void ResetCollider()
    {
        Head.ResetCollider();
        Wing.ResetCollider();
        Weap.ResetCollider();
        Engine.ResetCollider();
    }

    public void SetModular(Mesh mesh, E_SpaceShipPart e_SpaceShipPart)
    {
        switch (e_SpaceShipPart)
        {
            case E_SpaceShipPart.HEAD:
                Head.SetUpMesh(mesh);
                break;
            case E_SpaceShipPart.WING:
                Wing.SetUpMesh(mesh);
                break;
            case E_SpaceShipPart.WEAP:
                Weap.SetUpMesh(mesh);
                break;
            case E_SpaceShipPart.ENGINE:
                Engine.SetUpMesh(mesh);
                break;
            default:
                Debug.LogWarning(e_SpaceShipPart);
                Debug.LogWarning("Không tìm thấy phần bộ phận tàu vũ trụ.");
                break;
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
    private MeshCollider meshCollider;
    public void Init()
    {
        mesh = partObject.GetComponent<MeshRenderer>();
        MeshName = mesh.name;
        material = mesh.material;
        meshFilter = partObject.GetComponent<MeshFilter>();
    }

    public void SetLayer()
    {
        partObject.layer = LayerMask.NameToLayer("CaptureLayer");
    }

    public void ResetLayer()
    {
        partObject.layer = LayerMask.NameToLayer("Default");
    }

    public void SetUpMesh(Mesh mesh)
    {
        MeshName = mesh.name;
        meshFilter.mesh = mesh;
        ResetCollider();
    }
    public void ResetCollider()
    {
        meshCollider = partObject.GetComponent<MeshCollider>();
        if (meshCollider != null && meshFilter != null)
        {
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = meshFilter.sharedMesh;
        }
    }
}