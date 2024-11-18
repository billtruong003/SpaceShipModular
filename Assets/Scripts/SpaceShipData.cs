using System.Collections.Generic;
using UnityEngine;
using BillUtils.SpaceShipData;
using NaughtyAttributes;
using System.Linq.Expressions;
using System.Linq;

[CreateAssetMenu(fileName = "SpaceShipData", menuName = "SpaceShipData", order = 0)]
public class SpaceShipData : ScriptableObject
{
    public List<SpaceShipHead> spaceShipHeads;
    public List<SpaceShipWing> spaceShipWings;
    public List<SpaceShipWeap> spaceShipWeaps;
    public List<SpaceShipEngine> spaceShipEngines;

    [Range(0, 100)]
    public int Limit = 3; // Giới hạn số lượng phần tử muốn tạo

    public Mesh GetPartMesh(E_SpaceShipPart e_SpaceShipPart)
    {
        switch (e_SpaceShipPart)
        {
            case E_SpaceShipPart.HEAD:
                return LoadMesh(spaceShipHeads, "HEAD");
            case E_SpaceShipPart.WING:
                return LoadMesh(spaceShipWings, "WING");
            case E_SpaceShipPart.WEAP:
                return LoadMesh(spaceShipWeaps, "WEAP");
            case E_SpaceShipPart.ENGINE:
                return LoadMesh(spaceShipEngines, "ENGINE");
            default:
                Debug.LogWarning("Không tìm thấy phần bộ phận tàu vũ trụ.");
                return null;
        }
    }
    public Sprite GetIconFromMesh(string meshName, E_SpaceShipPart e_SpaceShipPart)
    {
        switch (e_SpaceShipPart)
        {
            case E_SpaceShipPart.HEAD:
                return GetIcon(meshName, spaceShipHeads.Cast<SpaceShipPartBase>().ToList());
            case E_SpaceShipPart.WING:
                return GetIcon(meshName, spaceShipWings.Cast<SpaceShipPartBase>().ToList());
            case E_SpaceShipPart.WEAP:
                return GetIcon(meshName, spaceShipWeaps.Cast<SpaceShipPartBase>().ToList());
            case E_SpaceShipPart.ENGINE:
                return GetIcon(meshName, spaceShipEngines.Cast<SpaceShipPartBase>().ToList());
            default:
                Debug.LogWarning("Không tìm thấy phần bộ phận tàu vũ trụ.");
                return null;
        }
    }


    public Sprite GetIcon(string meshName, List<SpaceShipPartBase> spaceShipPart)
    {
        foreach (var item in spaceShipPart)
        {
            if (item.Name == meshName)
            {
                return item.GetSprite();
            }
        }
        return spaceShipPart[0].GetSprite();
    }
    public Mesh GetCorrectMesh(E_SpaceShipPart e_SpaceShipPart, int num)
    {
        if (num <= 0 || num > Limit + 2)
        {
            Debug.LogWarning("Số lượng không hợp lệ.");
            return null;
        }

        switch (e_SpaceShipPart)
        {
            case E_SpaceShipPart.HEAD:
                return spaceShipHeads[num - 1].GetMesh();
            case E_SpaceShipPart.WING:
                return spaceShipWings[num - 1].GetMesh();
            case E_SpaceShipPart.WEAP:
                return spaceShipWeaps[num - 1].GetMesh();
            case E_SpaceShipPart.ENGINE:
                return spaceShipEngines[num - 1].GetMesh();
            default:
                Debug.LogWarning("Không tìm thấy phần bộ phận tàu vũ trụ.");
                return null;
        }
    }

    private Mesh LoadMesh<T>(List<T> parts, string partType) where T : SpaceShipPartBase
    {
        if (parts == null || parts.Count == 0)
        {
            Debug.LogWarning($"{partType} không có phần tử nào.");
            return null;
        }

        int index = Random.Range(0, Mathf.Min(Limit + 1, parts.Count));
        T part = parts[index];
        Mesh mesh = Resources.Load<Mesh>(part.Path);

        if (mesh == null)
        {
            Debug.LogWarning($"Không tìm thấy Mesh tại đường dẫn: {part.Path}");
        }

        return mesh;
    }

    [Button]
    private void GenerateSpaceShipPart()
    {
        // Clear danh sách cũ trước khi tạo mới
        spaceShipHeads.Clear();
        spaceShipWings.Clear();
        spaceShipWeaps.Clear();
        spaceShipEngines.Clear();

        // Generate các bộ phận tàu vũ trụ
        for (int i = 0; i <= Limit; i++)
        {
            string indexStr = i.ToString("D3"); // Tạo chuỗi số với định dạng 000

            // Tạo Head
            SpaceShipHead head = new SpaceShipHead
            {
                Name = $"Head.{indexStr}",
                Part = E_SpaceShipPart.HEAD,
                Path = $"HEAD/Head.{indexStr}",
                IconPath = $"IconPNG/Head_{i}"
            };
            spaceShipHeads.Add(head);

            // Tạo Wing
            SpaceShipWing wing = new SpaceShipWing
            {
                Name = $"Wing.{indexStr}",
                Part = E_SpaceShipPart.WING,
                Path = $"WING/Wing.{indexStr}",
                IconPath = $"IconPNG/Wing_{i}"
            };
            spaceShipWings.Add(wing);

            // Tạo Weap
            SpaceShipWeap weap = new SpaceShipWeap
            {
                Name = $"Weap.{indexStr}",
                Part = E_SpaceShipPart.WEAP,
                Path = $"WEAP/Weap.{indexStr}",
                IconPath = $"IconPNG/Weap_{i}"
            };
            spaceShipWeaps.Add(weap);

            // Tạo Engine
            SpaceShipEngine engine = new SpaceShipEngine
            {
                Name = $"Engine.{indexStr}",
                Part = E_SpaceShipPart.ENGINE,
                Path = $"ENGINE/Engine.{indexStr}",
                IconPath = $"IconPNG/Engine_{i}"
            };
            spaceShipEngines.Add(engine);
        }
    }
}
