using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using BillUtils.SpaceShipData;

public class SpaceShipCreator : MonoBehaviour
{
    [SerializeField] private SpaceShipData spaceShipData;

    [Foldout("Space Ship Parts")]
    [SerializeField] private SpaceShipPart Head;
    [Foldout("Space Ship Parts")]
    [SerializeField] private SpaceShipPart Wing;
    [Foldout("Space Ship Parts")]
    [SerializeField] private SpaceShipPart Weap;
    [Foldout("Space Ship Parts")]
    [SerializeField] private SpaceShipPart Engine;

    [BoxGroup("Settings")]
    [SerializeField] private int PickModelMark;
    [BoxGroup("Settings")]
    [SerializeField] private bool PickCorrectModelMark;

    private void Start()
    {
        InitSpaceShip();
    }

    [Button("Initialize Space Ship")]
    public void InitSpaceShip()
    {
        InitIcon();
        InitPart(Head);
        InitPart(Wing);
        InitPart(Weap);
        InitPart(Engine);
    }

    [Button("Generate Space Ship")]
    public void GenerateSpaceShip()
    {
        if (!PickCorrectModelMark)
        {
            SetPartMesh(Head, E_SpaceShipPart.HEAD);
            SetPartMesh(Wing, E_SpaceShipPart.WING);
            SetPartMesh(Weap, E_SpaceShipPart.WEAP);
            SetPartMesh(Engine, E_SpaceShipPart.ENGINE);
        }
        else
        {
            SetCorrectPartMesh(Head, E_SpaceShipPart.HEAD, PickModelMark);
            SetCorrectPartMesh(Wing, E_SpaceShipPart.WING, PickModelMark);
            SetCorrectPartMesh(Weap, E_SpaceShipPart.WEAP, PickModelMark);
            SetCorrectPartMesh(Engine, E_SpaceShipPart.ENGINE, PickModelMark);
        }
    }

    [Button("Reset Colliders")]
    public void ResetCollider()
    {
        ResetPartCollider(Head);
        ResetPartCollider(Wing);
        ResetPartCollider(Weap);
        ResetPartCollider(Engine);
    }

    private void InitIcon()
    {
        Head.icon = spaceShipData.GetIconFromMesh(Head.MeshName, E_SpaceShipPart.HEAD);

        Wing.icon = spaceShipData.GetIconFromMesh(Wing.MeshName, E_SpaceShipPart.WING);
        Weap.icon = spaceShipData.GetIconFromMesh(Weap.MeshName, E_SpaceShipPart.WEAP);
        Engine.icon = spaceShipData.GetIconFromMesh(Engine.MeshName, E_SpaceShipPart.ENGINE);
    }

    private void InitPart(SpaceShipPart part)
    {
        part.Init();
    }

    private void SetPartMesh(SpaceShipPart part, E_SpaceShipPart partType)
    {
        part.SetUpMesh(spaceShipData.GetPartMesh(partType));
    }

    private void SetCorrectPartMesh(SpaceShipPart part, E_SpaceShipPart partType, int modelMark)
    {
        part.SetUpMesh(spaceShipData.GetCorrectMesh(partType, modelMark));
    }

    private void ResetPartCollider(SpaceShipPart part)
    {
        part.ResetCollider();
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
                Debug.LogWarning($"Không tìm thấy bộ phận tàu vũ trụ: {e_SpaceShipPart}");
                break;
        }
    }
}

[Serializable]
public class SpaceShipPart
{
    public string MeshName;

    [ShowAssetPreview] public GameObject partObject;
    [ShowAssetPreview] public Sprite icon;

    [SerializeField, ReadOnly] private MeshFilter meshFilter;
    [SerializeField, ReadOnly] private MeshRenderer mesh;
    [SerializeField, ReadOnly] private Material material;
    [SerializeField, ReadOnly] private MeshCollider meshCollider;


    public void Init()
    {
        mesh = partObject.GetComponent<MeshRenderer>();
        meshFilter = partObject.GetComponent<MeshFilter>();
        meshCollider = partObject.GetComponent<MeshCollider>();
        MeshName = mesh.name;
        material = mesh.material;

    }

    public void SetUpMesh(Mesh mesh)
    {
        MeshName = mesh.name;
        meshFilter.mesh = mesh;
        ResetCollider();
    }

    public void SetLayer()
    {
        partObject.layer = LayerMask.NameToLayer("CaptureLayer");
    }

    public void ResetLayer()
    {
        partObject.layer = LayerMask.NameToLayer("Default");
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
