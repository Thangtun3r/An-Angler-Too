using UnityEngine;
using System.IO;
using System.Collections;

public class PhotoshootExporter : MonoBehaviour
{
    [Header("Capture")]
    public Camera captureCamera;
    public RenderTexture renderTexture;

    [Header("Target")]
    public Transform photoshootRoot;

    [Header("Export")]
    public string folderName = "FishIcons";
    public int textureSize = 512;

    [ContextMenu("Start Photoshoot")]
    public void StartPhotoshoot()
    {
        StartCoroutine(CaptureAll());
    }

    IEnumerator CaptureAll()
    {
        // Create folder
        string folderPath = Path.Combine(Application.dataPath, folderName);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        // Disable all children first
        foreach (Transform child in photoshootRoot)
            child.gameObject.SetActive(false);

        yield return null;

        foreach (Transform child in photoshootRoot)
        {
            child.gameObject.SetActive(true);
            yield return null; // let Unity update transforms

            Capture(child.name, folderPath);

            child.gameObject.SetActive(false);
            yield return null;
        }

        Debug.Log("📸 Photoshoot complete!");
    }

    void Capture(string fileName, string folderPath)
    {
        RenderTexture.active = renderTexture;
        captureCamera.Render();

        Texture2D tex = new Texture2D(
            textureSize,
            textureSize,
            TextureFormat.RGBA32,
            false
        );

        tex.ReadPixels(
            new Rect(0, 0, textureSize, textureSize),
            0, 0
        );
        tex.Apply();

        byte[] png = tex.EncodeToPNG();
        File.WriteAllBytes(
            Path.Combine(folderPath, fileName + ".png"),
            png
        );

        RenderTexture.active = null;
        DestroyImmediate(tex);
    }
}