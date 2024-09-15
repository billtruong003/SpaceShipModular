using System.Collections.Generic;
using UnityEngine;
using BillUtils.SpaceShipData;
using NaughtyAttributes;
using System.Linq.Expressions;

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
                Path = $"HEAD/Head.{indexStr}"
            };
            spaceShipHeads.Add(head);

            // Tạo Wing
            SpaceShipWing wing = new SpaceShipWing
            {
                Name = $"Wing.{indexStr}",
                Part = E_SpaceShipPart.WING,
                Path = $"WING/Wing.{indexStr}"
            };
            spaceShipWings.Add(wing);

            // Tạo Weap
            SpaceShipWeap weap = new SpaceShipWeap
            {
                Name = $"Weap.{indexStr}",
                Part = E_SpaceShipPart.WEAP,
                Path = $"WEAP/Weap.{indexStr}"
            };
            spaceShipWeaps.Add(weap);

            // Tạo Engine
            SpaceShipEngine engine = new SpaceShipEngine
            {
                Name = $"Engine.{indexStr}",
                Part = E_SpaceShipPart.ENGINE,
                Path = $"ENGINE/Engine.{indexStr}"
            };
            spaceShipEngines.Add(engine);
        }
    }
}
