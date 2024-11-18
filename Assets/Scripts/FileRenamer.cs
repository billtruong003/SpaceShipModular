using System.IO;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class FileRenamer : MonoBehaviour
{
    [SerializeField] private string folderPath; // Đường dẫn đến thư mục chứa file

    [Button]
    public void RenameFiles()
    {
        // Lấy danh sách tất cả các file trong thư mục
        var files = Directory.GetFiles(folderPath);

        // Danh sách để lưu tên file cần đổi tên
        var filesToRename = new List<string>();

        // Duyệt qua từng file
        foreach (var file in files)
        {
            string fileName = Path.GetFileName(file);

            // Kiểm tra nếu file có đuôi "_0"
            if (fileName.EndsWith("_0.png"))
            {
                // Xóa file
                File.Delete(file);
                Debug.Log($"Đã xóa: {fileName}");
            }
            else if (fileName.Contains("_"))
            {
                // Thêm vào danh sách để đổi tên
                filesToRename.Add(file);
            }
        }

        // Đổi tên các file còn lại
        foreach (var file in filesToRename)
        {
            string fileName = Path.GetFileName(file);
            string newFileName = GetNewFileName(fileName);
            string newPath = Path.Combine(folderPath, newFileName);

            // Kiểm tra xem tên mới đã tồn tại chưa
            if (!File.Exists(newPath))
            {
                File.Move(file, newPath);
                Debug.Log($"Đã đổi tên: {fileName} -> {newFileName}");
            }
        }
    }

    private string GetNewFileName(string fileName)
    {
        // Tách tên file và số
        var parts = fileName.Split('_');
        if (parts.Length == 2 && int.TryParse(parts[1].Split('.')[0], out int number))
        {
            // Lùi số
            return $"{parts[0]}_{number - 1}.png"; // Đảm bảo thêm đuôi .png
        }
        return fileName; // Trả về tên cũ nếu không khớp
    }
}
