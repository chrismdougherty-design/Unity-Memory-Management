// ResourceTester.cs
using UnityEngine;

public class ResourceTester : MonoBehaviour
{
    public string[] texturePaths =
    {
        "Wakfu Yugo",
        "Wakfu Lokus",
        "Raptor Shop Qurtet",
        "Velkhana",
        "Monster hunter logo"
    };

    private ImageDisplayPool displayPool;
    private ImageDisplay currentDisplay;
    private int currentTextureIndex = 0;

    void Start()
    {
        Debug.Log("Starting Resource Manager Test...");
        
        // Find or create ImageDisplayPool
        displayPool = FindObjectOfType<ImageDisplayPool>();
        if (displayPool == null)
        {
            Debug.LogWarning("ImageDisplayPool not found in scene!");
        }
        
        Invoke("RunTest", 1f);
    }

    void RunTest()
    {
        // Load each texture 3 times
        for (int i = 0; i < 3; i++)
        {
            Debug.Log($"\n--- Load Pass {i + 1} ---");
            foreach (string path in texturePaths)
            {
                Texture2D tex = ResourceManager.Instance.LoadTexture(path);
            }
        }

        // Print statistics
        Debug.Log("\n");
        ResourceManager.Instance.PrintStats();
        
        // Start cycling through images using the pool
        if (displayPool != null && texturePaths.Length > 0)
        {
            currentDisplay = displayPool.StartCycling(texturePaths, 3f);
            Debug.Log("Started cycling images from pool");
        }
        // Expected result:
        // - 5 textures loaded (first pass)
        // - 10 cache hits (second and third passes)
    }

    void Update()
    {
        // Press Space to toggle auto-cycling
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (displayPool != null)
            {
                displayPool.StopCycling();
                currentDisplay = displayPool.StartCycling(texturePaths, 3f);
            }
        }

        // Press Right Arrow to display next texture
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (displayPool != null)
                displayPool.StopCycling();
                
            currentTextureIndex = (currentTextureIndex + 1) % texturePaths.Length;
            if (displayPool != null)
            {
                currentDisplay = displayPool.DisplayTexture(texturePaths[currentTextureIndex], 3f);
            }
        }

        // Press Left Arrow to display previous texture
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (displayPool != null)
                displayPool.StopCycling();
                
            currentTextureIndex--;
            if (currentTextureIndex < 0)
                currentTextureIndex = texturePaths.Length - 1;
            if (displayPool != null)
            {
                currentDisplay = displayPool.DisplayTexture(texturePaths[currentTextureIndex], 3f);
            }
        }

        // Press M to display multiple images at once
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (displayPool != null)
            {
                displayPool.ClearAll();
                displayPool.DisplayMultiple(texturePaths, 3f);
                Debug.Log("Displaying all images simultaneously");
            }
        }

        // Press U to unload a specific texture
        if (Input.GetKeyDown(KeyCode.U))
        {
            ResourceManager.Instance.UnloadTexture(texturePaths[0]);
            ResourceManager.Instance.PrintStats();
        }

        // Press C to clear all cache and displays
        if (Input.GetKeyDown(KeyCode.C))
        {
            ResourceManager.Instance.ClearCache();
            ResourceManager.Instance.PrintStats();
            if (displayPool != null)
            {
                displayPool.ClearAll();
            }
        }

        // Press H to hide current image and stop cycling
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (displayPool != null)
            {
                displayPool.StopCycling();
            }
        }

        // Press P to print pool stats
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (displayPool != null)
            {
                displayPool.PrintStats();
            }
        }
    }
}