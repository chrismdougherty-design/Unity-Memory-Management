// TextureLoadTest.cs - Quick test to verify textures can be loaded
using UnityEngine;

public class TextureLoadTest : MonoBehaviour
{
    public string[] texturePaths =
    {
        "Wakfu Yugo",
        "Wakfu Lokus",
        "Raptor Shop Qurtet",
        "Velkhana",
        "Monster hunter logo"
    };

    void Start()
    {
        Debug.Log("=== TEXTURE LOAD TEST ===");
        Debug.Log($"Testing {texturePaths.Length} textures...\n");

        foreach (string path in texturePaths)
        {
            TestLoadTexture(path);
        }
    }

    void TestLoadTexture(string path)
    {
        Debug.Log($"Attempting to load: '{path}'");
        
        Texture2D texture = Resources.Load<Texture2D>(path);
        
        if (texture != null)
        {
            Debug.Log($"✓ SUCCESS: {path}");
            Debug.Log($"  Size: {texture.width}x{texture.height}");
            Debug.Log($"  Format: {texture.format}");
        }
        else
        {
            Debug.LogError($"✗ FAILED: Could not load '{path}'");
            Debug.LogError($"  Check that file exists at: Assets/Resources/{path}.jpg");
        }
    }
}
