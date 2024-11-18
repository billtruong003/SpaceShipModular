using UnityEditor;
using UnityEngine;

public class CrunchCompressionTool
{
    [MenuItem("Tools/Apply Crunch Compression to All Textures")]
    public static void ApplyCrunchCompression()
    {
        string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D");
        int totalTextures = textureGuids.Length;

        for (int i = 0; i < totalTextures; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(textureGuids[i]);
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (textureImporter != null)
            {
                if (!textureImporter.crunchedCompression)
                    textureImporter.crunchedCompression = true;
                if (textureImporter.compressionQuality != 50)
                    textureImporter.compressionQuality = 50;
                textureImporter.SaveAndReimport();
            }

            EditorUtility.DisplayProgressBar("Applying Crunch Compression", $"Processing texture {i + 1} of {totalTextures}", (float)i / (float)totalTextures);
        }

        EditorUtility.ClearProgressBar();
        Debug.Log("Crunch Compression applied to all textures.");
    }
}