using System.IO;
using UnityEngine;
using NaughtyAttributes;
using BillUtils.SpaceShipData;
using System.Collections.Generic;
using System;
using Unity.Mathematics;
using System.Linq;
using System.Collections;

public class ToolMeshToPNG : MonoBehaviour
{
    [Foldout("Space Ship Part")]
    [SerializeField] private SpaceShipData spaceShipData;
    [Foldout("Space Ship Part")]
    [SerializeField] private SpaceShipPart Head;
    [Foldout("Space Ship Part")]
    [SerializeField] private SpaceShipPart Wing;
    [Foldout("Space Ship Part")]
    [SerializeField] private SpaceShipPart Weap;
    [Foldout("Space Ship Part")]
    [SerializeField] private SpaceShipPart Engine;

    [SerializeField] private List<AngleCapture> AngleCaptures;
    [SerializeField] private Camera captureCamera;
    [SerializeField] private int textureWidth = 1024;
    [SerializeField] private int textureHeight = 1024;
    [SerializeField] private string savePath;

    private RenderTexture renderTexture;
    private Texture2D texture;


    [Button]
    public void InitSpaceShip()
    {
        if (Head != null) Head.Init();
        if (Wing != null) Wing.Init();
        if (Weap != null) Weap.Init();
        if (Engine != null) Engine.Init();
    }
    [Button]
    public void InitAngle()
    {
        foreach (var item in AngleCaptures)
        {
            item.Init();
        }
    }
    [Button]
    public void SaveAngle()
    {
        foreach (var item in AngleCaptures)
        {
            item.Save();
        }
    }
    public void CapturePartToPNG(SpaceShipPart part, string savePath)
    {
        if (part == null || part.partObject == null)
        {
            Debug.LogError("Part or partObject is null");
            return;
        }

        SetInActive(part);
        renderTexture = captureCamera.targetTexture;
        texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);

        captureCamera.gameObject.SetActive(true);
        SetAngleCam(part.MeshName);

        part.SetLayer();

        captureCamera.Render();

        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(savePath, bytes);

        Debug.Log($"Saved PNG at: {savePath}");

        part.ResetLayer();
        renderTexture.Release();
    }
    int meshOrder = 0;
    [Button]
    public void CaptureAllParts()
    {
        StartCoroutine(CaptureAllPartsCoroutine());
    }

    private IEnumerator CaptureAllPartsCoroutine()
    {
        for (int i = 1; i < spaceShipData.Limit + 2; i++)
        {
            meshOrder = i;
            Head.SetUpMesh(spaceShipData.GetCorrectMesh(E_SpaceShipPart.HEAD, meshOrder));
            Debug.Log("Head: " + Head.MeshName);
            Wing.SetUpMesh(spaceShipData.GetCorrectMesh(E_SpaceShipPart.WING, meshOrder));
            Debug.Log("Wing: " + Wing.MeshName);
            Weap.SetUpMesh(spaceShipData.GetCorrectMesh(E_SpaceShipPart.WEAP, meshOrder));
            Debug.Log("Weap: " + Weap.MeshName);
            Engine.SetUpMesh(spaceShipData.GetCorrectMesh(E_SpaceShipPart.ENGINE, meshOrder));
            Debug.Log("Engine: " + Engine.MeshName);

            SetInActive(Head);
            yield return new WaitForEndOfFrame();
            CapturePartToPNG(Head, $"{savePath}/Head_{i}.png");

            SetInActive(Wing);
            yield return new WaitForEndOfFrame();
            CapturePartToPNG(Wing, $"{savePath}/Wing_{i}.png");

            SetInActive(Weap);
            yield return new WaitForEndOfFrame();
            CapturePartToPNG(Weap, $"{savePath}/Weap_{i}.png");

            SetInActive(Engine);
            yield return new WaitForEndOfFrame();
            CapturePartToPNG(Engine, $"{savePath}/Engine_{i}.png");
        }
    }

    private void SetInActive(SpaceShipPart activePart)
    {
        List<SpaceShipPart> allParts = new List<SpaceShipPart> { Head, Wing, Weap, Engine };

        foreach (var part in allParts)
        {
            if (part != activePart)
            {
                part.partObject.SetActive(false);
            }
            else
            {
                part.partObject.SetActive(true);
            }
        }
    }


    private void OnDestroy()
    {
        if (renderTexture != null)
        {
            RenderTexture.ReleaseTemporary(renderTexture);
        }
    }

    private string ProcessName(string name)
    {
        string processedName = new string(name.Where(char.IsLetter).ToArray());
        return processedName.ToUpper();
    }

    private void ProcessAngleCam(string processedName)
    {
        foreach (var item in AngleCaptures)
        {

            if (processedName == item.e_SpaceShipPart.ToString())
            {
                Debug.Log(processedName + " == " + item.e_SpaceShipPart.ToString());
                captureCamera.transform.localPosition = item.position;
                captureCamera.transform.localEulerAngles = item.eulerAngle;
            }
        }
    }
    private void SetAngleCam(string name)
    {
        string processedName = ProcessName(name);
        ProcessAngleCam(processedName);
    }

    [Foldout("Cheat ManualCapture")]
    [SerializeField] private E_SpaceShipPart manualCheck;
    [Foldout("Cheat ManualCapture")]
    [SerializeField] private string manualCaptureName; // Trường tên cho capture thủ công
    [Foldout("Cheat ManualCapture")]
    [SerializeField] private bool skipChangePos;
    [Foldout("Cheat ManualCapture")]
    [SerializeField] private bool manualCapture;
    [Button]
    public void ManualCapture()
    {
        if (manualCapture && !string.IsNullOrEmpty(manualCaptureName))
        {
            // Kiểm tra camera và render texture
            if (captureCamera != null && captureCamera.targetTexture != null)
            {
                renderTexture = captureCamera.targetTexture;
                renderTexture.Release();
                texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
                if (!skipChangePos)
                    SetAngleCam(manualCheck.ToString());
                captureCamera.gameObject.SetActive(true);
                captureCamera.Render();

                RenderTexture.active = renderTexture;
                texture.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
                texture.Apply();

                byte[] bytes = texture.EncodeToPNG();
                File.WriteAllBytes($"{savePath}/{manualCaptureName}.png", bytes); // Thêm .png vào tên

                Debug.Log($"Saved PNG at: {savePath}/{manualCaptureName}.png");
            }
            else
            {
                Debug.LogError("Camera or render texture is not set correctly.");
            }
        }
        else
        {
            Debug.LogWarning("Manual capture is not enabled or name is empty.");
        }
    }

}
[Serializable]
public class AngleCapture
{
    public Transform angleObject;
    public E_SpaceShipPart e_SpaceShipPart;
    public Vector3 position;
    public Vector3 eulerAngle;
    public void Init()
    {
        position = angleObject.localPosition;
        eulerAngle = angleObject.localEulerAngles;
    }

    public void Save()
    {
        angleObject.localPosition = position;
        angleObject.localEulerAngles = eulerAngle;
    }
}