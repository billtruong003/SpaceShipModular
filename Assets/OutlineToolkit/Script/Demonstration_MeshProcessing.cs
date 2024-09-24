using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demonstration_MeshProcessing : MonoBehaviour
{

    public GameObject[] g; // Danh sách các GameObject cần xử lý
    public Material mat; // Vật liệu để hiển thị outline

    void Start()
    {
        foreach (GameObject o in g)
        {
            Mesh originalMesh = o.GetComponent<MeshFilter>().mesh;
            if (originalMesh == null)
            {
                Debug.LogWarning($"GameObject {o.name} does not have a valid Mesh.");
                continue;
            }

            // Tạo một bản sao của Mesh và thiết lập isReadable
            Mesh meshCopy = Instantiate(originalMesh);
            meshCopy.name = originalMesh.name + " Copy";
            meshCopy.UploadMeshData(false); // Cho phép truy cập và chỉnh sửa Mesh

            // Kiểm tra thông tin Mesh trước khi xử lý
            if (meshCopy.vertexCount == 0 || meshCopy.triangles.Length == 0)
            {
                Debug.LogWarning($"Mesh {meshCopy.name} has invalid vertex or triangle data.");
                continue;
            }

            // Xử lý Mesh để tạo outline
            Mesh r = MeshProcessor.processForOutlineMesh(meshCopy);
            if (r == null)
            {
                Debug.LogError($"Failed to process mesh {meshCopy.name} for outline.");
                continue;
            }

            // Tạo GameObject mới để hiển thị Mesh đã xử lý
            GameObject f = new GameObject();
            f.transform.position = o.transform.position;
            f.transform.rotation = o.transform.rotation;
            f.transform.localScale = o.transform.localScale;
            f.name = o.name + " processed outline";

            f.AddComponent<MeshFilter>().mesh = r;
            f.AddComponent<MeshRenderer>().material = mat;
            f.GetComponent<MeshRenderer>().material.SetFloat("_Width", 0.05f / o.transform.localScale.x);

            // In thông tin chi tiết về Mesh đã xử lý
            Debug.Log($"Created processed mesh with {r.vertexCount} vertices and {r.triangles.Length / 3} triangles.");
        }
    }

}
