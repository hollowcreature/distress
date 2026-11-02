using System.IO;
using UnityEditor;
using UnityEngine;

public class SmoothnessPacker : EditorWindow
{
    private Texture2D roughness;
    private Texture2D metallic;
    private float metallicValue = 0f;

    [MenuItem("Tools/Smoothness Packer")]
    private static void Open() => GetWindow<SmoothnessPacker>("Smoothness Packer");

    void OnGUI()
    {
        roughness = (Texture2D)EditorGUILayout.ObjectField("Roughness", roughness, typeof(Texture2D), false);
        metallic = (Texture2D)EditorGUILayout.ObjectField("Metallic (optional)", metallic, typeof(Texture2D), false);
        if (metallic == null)
            metallicValue = EditorGUILayout.Slider("Metallic Value", metallicValue, 0f, 1f);

        GUI.enabled = roughness != null;
        if (GUILayout.Button("Pack"))
            Pack();
        GUI.enabled = true;
    }

    private void Pack()
    {
        Texture2D rough = LoadRaw(roughness);
        Texture2D metal = metallic != null ? LoadRaw(metallic) : null;

        int w = rough.width;
        int h = rough.height;
        var pixels = new Color[w * h];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float m = metal != null ? metal.GetPixelBilinear((x + 0.5f) / w, (y + 0.5f) / h).r : metallicValue;
                float s = 1f - rough.GetPixel(x, y).r;
                pixels[y * w + x] = new Color(m, m, m, s);
            }
        }

        var output = new Texture2D(w, h, TextureFormat.RGBA32, false, true);
        output.SetPixels(pixels);
        output.Apply();

        string roughPath = AssetDatabase.GetAssetPath(roughness);
        string outPath = (Path.GetDirectoryName(roughPath) + "/" + Path.GetFileNameWithoutExtension(roughPath) + "_MetallicSmoothness.png").Replace('\\', '/');
        File.WriteAllBytes(outPath, output.EncodeToPNG());
        AssetDatabase.ImportAsset(outPath);

        var importer = (TextureImporter)AssetImporter.GetAtPath(outPath);
        importer.sRGBTexture = false;
        importer.SaveAndReimport();

        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Texture2D>(outPath));
    }

    private static Texture2D LoadRaw(Texture2D source)
    {
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false, true);
        tex.LoadImage(File.ReadAllBytes(AssetDatabase.GetAssetPath(source)));
        return tex;
    }
}
