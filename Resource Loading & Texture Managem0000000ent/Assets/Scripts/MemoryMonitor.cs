// MemoryMonitor.cs - Attach to UI Text component
using UnityEngine;
using UnityEngine.UI;

public class MemoryMonitor : MonoBehaviour
{
    public Text displayText;
    public float updateInterval = 0.5f;
    private float timer;
    
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
        UpdateDisplay();
        timer = 0;
        }
    }

    void UpdateDisplay()
    {
        if (displayText == null)
        {
            displayText = GetComponent<Text>();
            if (displayText == null)
            {
                Debug.LogWarning("MemoryMonitor: displayText is not assigned and no Text component found.");
                return;
            }
        }

        // Get total allocated memory
        long totalMemory = System.GC.GetTotalMemory(false);
        float memoryMB = totalMemory / 1024f / 1024f;
        
        // Build display string
        string info = "=== MEMORY MONITOR ===\n";
        info += $"Total Memory: {memoryMB:F2} MB\n";
        info += $"FPS: {(Time.deltaTime > 0f ? 1f / Time.deltaTime : 0f):F0}\n\n";
        
        // Add ResourceManager stats if available
        if (ResourceManager.Instance != null)
        {
            info += "=== RESOURCE MANAGER ===\n";
            info += $"Textures Loaded: {ResourceManager.Instance.GetLoadCount()}\n";
            info += $"Cache Hits: {ResourceManager.Instance.GetCacheHitCount()}\n";
            info += $"Textures Cached: {ResourceManager.Instance.GetCachedTextureCount()}\n";
            info += $"Texture Memory: {ResourceManager.Instance.GetTotalTextureMemoryMB():F2} MB\n\n";
        }
        
        info += "Press U to unload texture\n";
        info += "Press C to clear cache";
        displayText.text = info;
    }
}