using UnityEngine;
using UnityEditor;
using System.IO;

public class FBXMeshCopier : EditorWindow
{
    [MenuItem("Tools/FBX Mesh Copier")]
    public static void ShowWindow()
    {
        GetWindow<FBXMeshCopier>("FBX Mesh Copier");
    }

    private Object fbxFile;
    private string outputFolder = "Assets/ExportedMeshes";

    private void OnGUI()
    {
        GUILayout.Label("FBX Mesh Copier Tool", EditorStyles.boldLabel);
        fbxFile = EditorGUILayout.ObjectField("FBX File", fbxFile, typeof(Object), false);

        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);

        if (GUILayout.Button("Copy Meshes"))
        {
            CopyMeshesFromFBX();
        }
    }

    private void CopyMeshesFromFBX()
    {
        if (fbxFile == null)
        {
            Debug.LogError("No FBX file selected!");
            return;
        }

        string path = AssetDatabase.GetAssetPath(fbxFile);
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);

        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        foreach (Object asset in assets)
        {
            if (asset is Mesh)
            {
                Mesh mesh = asset as Mesh;
                Mesh copiedMesh = Object.Instantiate(mesh);
                AssetDatabase.CreateAsset(copiedMesh, $"{outputFolder}/{mesh.name}.asset");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Meshes copied successfully!");
    }
}
