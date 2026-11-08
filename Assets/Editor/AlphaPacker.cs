using System.IO;
using UnityEditor;
using UnityEngine;

public class AlphaPacker : EditorWindow
{
    private Texture2D color;
    private Texture2D opacity;

    [MenuItem("Tools/Alpha Packer")]
    private static void Open() => GetWindow<AlphaPacker>("Alpha Packer");

    void OnGUI()
    {
        color = (Texture2D)EditorGUILayout.ObjectField("Color", color, typeof(Texture2D), false);
        opacity = (Texture2D)EditorGUILayout.ObjectField("Opacity", opacity, typeof(Texture2D), false);

        GUI.enabled = color != null && opacity != null;
        if (GUILayout.Button("Pack"))
            Pack();
        GUI.enabled = true;
    }

    private void Pack()
    {
        Texture2D col = LoadRaw(color);
        Texture2D op = LoadRaw(opacity);

        int w = col.width;
        int h = col.height;
        var pixels = new Color[w * h];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                Color c = col.GetPixel(x, y);
                c.a = op.GetPixelBilinear((x + 0.5f) / w, (y + 0.5f) / h).r;
                pixels[y * w + x] = c;
            }
        }

        var output = new Texture2D(w, h, TextureFormat.RGBA32, false);
        output.SetPixels(pixels);
        output.Apply();

        string colorPath = AssetDatabase.GetAssetPath(color);
        string outPath = (Path.GetDirectoryName(colorPath) + "/" + Path.GetFileNameWithoutExtension(colorPath) + "_Alpha.png").Replace('\\', '/');
        File.WriteAllBytes(outPath, output.EncodeToPNG());
        AssetDatabase.ImportAsset(outPath);

        var importer = (TextureImporter)AssetImporter.GetAtPath(outPath);
        importer.alphaSource = TextureImporterAlphaSource.FromInput;
        importer.alphaIsTransparency = true;
        importer.SaveAndReimport();

        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Texture2D>(outPath));
    }

    private static Texture2D LoadRaw(Texture2D source)
    {
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(File.ReadAllBytes(AssetDatabase.GetAssetPath(source)));
        return tex;
    }
}
