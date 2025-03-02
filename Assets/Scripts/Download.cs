using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Collections;
using TMPro;
using DG.Tweening;
using UnityGLTF;  // Đảm bảo bạn đã cài đặt UnityGLTF
using UnityGLTF.Loader;
using System;
using NaughtyAttributes;
using SimpleFileBrowser;  // Nhập khẩu các lớp cần thiết
using Assimp;
using System.Linq;
using AsciiFBXExporter;


public class GLBExportTool : MonoBehaviour
{
    public MeshFilter headMesh;
    public MeshFilter engineMesh;
    public MeshFilter wingMesh;
    public MeshFilter weaponMesh;

    public UnityEngine.Material material; // Material cần export, bao gồm shader và các texture
    [SerializeField] private TextMeshProUGUI messageText;

    private string exportFolderPath = "Assets/ExportedGLB/";

    [Button, ContextMenu("Export GLB and Zip")]
    public void ExportAndZip()
    {


        // Kiểm tra nền tảng
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            // Bắt đầu coroutine để thực hiện các bước xuất file và zip tuần tự
            StartCoroutine(ExportProcess());
        }
        else
        {
            // Thay đổi văn bản trong TextMeshPro
            ShowMessage("Please download the Windows version to export.");
        }
    }

    private IEnumerator ExportProcess()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Folders, false, null, null, "Select Folder", "Select");

        if (FileBrowser.Success)
        {
            // Lưu đường dẫn người dùng chọn
            exportFolderPath = FileBrowser.Result[0];
            Debug.Log("Selected folder: " + exportFolderPath);
        }
        else
        {
            Debug.Log("No folder selected.");
        }

        // Tạo đối tượng tạm thời để gộp các phần mesh lại
        GameObject combinedObject = new GameObject("Spaceship");

        // Copy các mesh vào đối tượng mới
        CombineMeshes(combinedObject);

        // Lưu texture từ material
        yield return StartCoroutine(SaveTexturesAndMaterials(combinedObject));

        yield return StartCoroutine(ExportToGlb(combinedObject, exportFolderPath));

        string glbPath = Path.Combine(exportFolderPath, "SpaceshipGLBWithTextures.glb");
        string objPath = Path.Combine(exportFolderPath, "SpaceshipOBJWithTextures.obj");
        yield return StartCoroutine(ConvertGlbToObj(glbPath, objPath));

        // Xuất FBX
        string fbxFileName = "SpaceshipFBX.fbx";
        string fbxPath = Path.Combine(exportFolderPath, fbxFileName);
        bool exportSuccess = AsciiFBXExporter.FBXExporter.ExportGameObjToFBX(combinedObject, fbxPath, false, true);

        // Xóa đối tượng tạm thời sau khi xuất
        Destroy(combinedObject);

        // Đảm bảo tất cả các tài nguyên đã được sao chép trước khi zip
        yield return new WaitForSeconds(1);

        // Tạo file zip
        string zipFilePath = Path.Combine(exportFolderPath, "ExportedFiles.zip");
        yield return StartCoroutine(ZipExportFolder(zipFilePath));
        

    }

    private IEnumerator ConvertGlbToObj(string glbPath, string objOutputPath)
    {

        AssimpContext context = new AssimpContext();

        try
        {
            // Import GLB file
            Scene model = context.ImportFile(glbPath, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs | PostProcessSteps.GenerateNormals);

            if (model == null)
            {
                Debug.LogError("Failed to load GLB file. Assimp error:");
                yield break;
            }

            // Export to OBJ
            bool exportSuccess = context.ExportFile(model, objOutputPath, "obj");

            if (exportSuccess && File.Exists(objOutputPath))
            {
                Debug.Log($"Successfully exported OBJ to: {objOutputPath}");
            }
            else
            {
                Debug.LogError("Failed to export OBJ file. Assimp error: ");
            }
        }
        catch (AssimpException ex)
        {
            Debug.LogError($"AssimpException: {ex.Message}");
        }
        catch (DllNotFoundException ex)
        {
            Debug.LogError($"DLL Not Found: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"General Exception: {ex.Message}");
        }
        finally
        {
            context.Dispose();
        }

        yield return null;
    }





    private void CombineMeshes(GameObject combinedObject)
    {
        GameObject headMeshObject = new GameObject("HeadMesh");
        GameObject engineMeshObject = new GameObject("EngineMesh");
        GameObject wingMeshObject = new GameObject("WingMesh");
        GameObject weaponMeshObject = new GameObject("WeaponMesh");

        headMeshObject.transform.parent = combinedObject.transform;
        engineMeshObject.transform.parent = combinedObject.transform;
        wingMeshObject.transform.parent = combinedObject.transform;
        weaponMeshObject.transform.parent = combinedObject.transform;

        if (headMesh != null)
        {
            headMeshObject.AddComponent<MeshFilter>().mesh = headMesh.sharedMesh;
            headMeshObject.AddComponent<MeshRenderer>().material = material;
        }

        if (engineMesh != null)
        {
            engineMeshObject.AddComponent<MeshFilter>().mesh = engineMesh.sharedMesh;
            engineMeshObject.AddComponent<MeshRenderer>().material = material;
        }

        if (wingMesh != null)
        {
            wingMeshObject.AddComponent<MeshFilter>().mesh = wingMesh.sharedMesh;
            wingMeshObject.AddComponent<MeshRenderer>().material = material;
        }

        if (weaponMesh != null)
        {
            weaponMeshObject.AddComponent<MeshFilter>().mesh = weaponMesh.sharedMesh;
            weaponMeshObject.AddComponent<MeshRenderer>().material = material;
        }
    }

    private IEnumerator ExportToGlb(GameObject combinedObject, string filePath)
    {
        // Kiểm tra và xóa tệp cũ nếu tồn tại
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch (IOException ex)
            {
                Debug.LogError($"Could not delete existing file: {ex.Message}");
                yield break;
            }
        }

        ExportContext exportContext = new ExportContext();
        var gltfExporter = new GLTFSceneExporter(new[] { combinedObject.transform }, exportContext);

        Debug.Log($"Exporting to path: {filePath}");

        try
        {
            gltfExporter.SaveGLB(filePath, "SpaceshipWithTextures.glb");

            if (File.Exists(filePath))
            {
                Debug.Log($"GLB file saved to: {filePath}");
            }
            else
            {
                Debug.LogError("Failed to export GLB file: File not created.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during export: {ex.Message}");
        }

        yield break;
    }

    private IEnumerator SaveTexturesAndMaterials(GameObject combinedObject)
    {
        // Lấy tất cả các material từ combinedObject
        Renderer[] renderers = combinedObject.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.sharedMaterials)
            {
                if (mat != null)
                {
                    // Lưu các texture từ material
                    SaveTexture(mat, "_BaseMap");
                    SaveTexture(mat, "_MetallicMap");
                    SaveTexture(mat, "_SmoothnessMap");
                }
            }
        }
        yield return null;
    }

    private void SaveTexture(UnityEngine.Material material, string textureProperty)
    {
        if (material.HasProperty(textureProperty))
        {
            Texture texture = material.GetTexture(textureProperty);
            if (texture != null)
            {
                string texturePath = Path.Combine(exportFolderPath, $"{texture.name}.png");
                // Chuyển texture thành PNG và lưu
                var texture2D = texture as Texture2D;
                if (texture2D != null)
                {
                    byte[] bytes = texture2D.EncodeToPNG();
                    File.WriteAllBytes(texturePath, bytes);
                    Debug.Log($"Saved texture to: {texturePath}");
                }
            }
        }
    }

    private IEnumerator ZipExportFolder(string zipFilePath)
    {
        bool fileDeleted = false;

        // Xóa file zip cũ nếu có
        if (File.Exists(zipFilePath))
        {
            try
            {
                File.Delete(zipFilePath);
                fileDeleted = true;
                Debug.Log("Deleted existing zip file: " + zipFilePath);
            }
            catch (IOException ex)
            {
                Debug.LogError("Failed to delete zip file: " + ex.Message);
                yield break;
            }
        }

        // Chờ cho đến khi file zip đã bị xóa
        if (fileDeleted)
        {
            yield return new WaitUntil(() => !File.Exists(zipFilePath));
        }

        // Kiểm tra xem thư mục xuất đã tồn tại chưa
        if (Directory.Exists(exportFolderPath))
        {
            try
            {
                // Kiểm tra xem có file nào trong thư mục đang bị khóa hay không
                string[] files = Directory.GetFiles(exportFolderPath);
                foreach (var file in files)
                {
                    try
                    {
                        using (FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None))
                        {
                            stream.Close();
                        }
                    }
                    catch (IOException)
                    {
                        Debug.LogError($"File is locked and cannot be accessed: {file}");
                        yield break;
                    }
                }

                // Tạo file zip, bỏ qua các file .meta nếu không cần
                using (ZipArchive zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                {
                    foreach (string file in files)
                    {
                        if (!file.EndsWith(".meta"))
                        {
                            zip.CreateEntryFromFile(file, Path.GetFileName(file));
                            Debug.Log($"Added {file} to zip.");
                        }
                    }
                }

                Debug.Log("Created zip: " + zipFilePath);

                // Sau khi zip thành công, hiển thị thông báo đến người dùng
                ShowMessage($"Download successful! Files saved in: {exportFolderPath}");
            }
            catch (IOException ex)
            {
                Debug.LogError("Failed to create zip: " + ex.Message);
                yield break;
            }
        }
        else
        {
            Debug.LogError("Export folder does not exist. Cannot create zip file.");
        }

        yield return null;
    }

    private void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.DOKill();
            messageText.text = message;
            messageText.DOFade(1f, 0.5f).From(0f).OnComplete(() =>
            {
                messageText.DOFade(0f, 3).SetEase(Ease.Linear).OnComplete(() => { messageText.text = ""; });
            });
        }
        else
        {
            Debug.LogWarning("Message TextMeshPro is not assigned.");
        }
    }
}
