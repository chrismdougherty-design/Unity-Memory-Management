// ImageDisplayPool.cs - Object pooling system for image displays
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageDisplayPool : MonoBehaviour
{
    // Prefab with RawImage component to pool
    public GameObject displayPrefab;
    
    // How many display objects to create at start
    public int poolSize = 5;
    public float displayDuration = 3f;
    
    // Pool storage
    private List<ImageDisplay> availableDisplays = new List<ImageDisplay>();
    private List<ImageDisplay> activeDisplays = new List<ImageDisplay>();
    
    // Cycling state
    private ImageDisplay currentCyclingDisplay;
    private string[] cyclingTextures;
    private int cycleDisplayIndex = 0;
    
    // Singleton
    public static ImageDisplayPool Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializePool();
    }

    void Update()
    {
        // If cycling with pool, manage display transitions
        if (currentCyclingDisplay != null && cyclingTextures != null && cyclingTextures.Length > 0)
        {
            // Check if current display is done
            if (!currentCyclingDisplay.IsDisplaying() && !currentCyclingDisplay.IsAutoCycling())
            {
                // Move to next display and image
                cycleDisplayIndex = (cycleDisplayIndex + 1) % cyclingTextures.Length;
                
                // Return current display to pool
                ReturnDisplay(currentCyclingDisplay);
                
                // Get next display and show next image
                currentCyclingDisplay = GetDisplay();
                currentCyclingDisplay.DisplayTexture(cyclingTextures[cycleDisplayIndex], displayDuration);
                
                Debug.Log($"Pool cycling: Display changed to show {cyclingTextures[cycleDisplayIndex]} ({cycleDisplayIndex + 1}/{cyclingTextures.Length})");
            }
        }
    }

    /// <summary>
    /// Create pooled display objects
    /// </summary>
    private void InitializePool()
    {
        if (displayPrefab == null)
        {
            Debug.LogError("ImageDisplayPool: displayPrefab not assigned! Please assign a prefab with RawImage component in Inspector.", gameObject);
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject displayObj = Instantiate(displayPrefab, transform);
            displayObj.name = $"ImageDisplay_{i}";
            
            // Ensure it has RawImage component
            RawImage rawImage = displayObj.GetComponent<RawImage>();
            if (rawImage == null)
                rawImage = displayObj.GetComponentInChildren<RawImage>();
            
            if (rawImage == null)
            {
                Debug.LogError($"Display prefab {displayPrefab.name} does not have a RawImage component!", displayPrefab);
                Destroy(displayObj);
                continue;
            }
            
            ImageDisplay display = displayObj.GetComponent<ImageDisplay>();
            if (display == null)
                display = displayObj.AddComponent<ImageDisplay>();
            
            displayObj.SetActive(false);
            availableDisplays.Add(display);
        }
        
        Debug.Log($"ImageDisplayPool initialized with {availableDisplays.Count} displays.");
    }

    /// <summary>
    /// Get a display from the pool
    /// </summary>
    public ImageDisplay GetDisplay()
    {
        ImageDisplay display = null;

        // Reuse available display
        if (availableDisplays.Count > 0)
        {
            display = availableDisplays[0];
            availableDisplays.RemoveAt(0);
        }
        else
        {
            // Expand pool if needed
            GameObject newDisplayObj = Instantiate(displayPrefab, transform);
            display = newDisplayObj.GetComponent<ImageDisplay>();
            if (display == null)
            {
                display = newDisplayObj.AddComponent<ImageDisplay>();
            }
            Debug.Log("ImageDisplayPool expanded.");
        }

        display.gameObject.SetActive(true);
        activeDisplays.Add(display);
        return display;
    }

    /// <summary>
    /// Return display to the pool (stop cycling and hide)
    /// </summary>
    public void ReturnDisplay(ImageDisplay display)
    {
        if (display == null) return;

        display.ResetForPool();
        display.gameObject.SetActive(false);

        if (activeDisplays.Contains(display))
            activeDisplays.Remove(display);

        availableDisplays.Add(display);
    }

    /// <summary>
    /// Stop cycling images
    /// </summary>
    public void StopCycling()
    {
        if (currentCyclingDisplay != null)
        {
            ReturnDisplay(currentCyclingDisplay);
            currentCyclingDisplay = null;
        }
        cyclingTextures = null;
        cycleDisplayIndex = 0;
        Debug.Log("Pool cycling stopped");
    }

    /// <summary>
    /// Display a single texture
    /// </summary>
    public ImageDisplay DisplayTexture(string texturePath, float duration = 0)
    {
        ImageDisplay display = GetDisplay();
        display.DisplayTexture(texturePath, duration > 0 ? duration : displayDuration);
        return display;
    }

    /// <summary>
    /// Display multiple textures simultaneously
    /// </summary>
    public ImageDisplay[] DisplayMultiple(string[] texturePaths, float duration = 0)
    {
        ImageDisplay[] displays = new ImageDisplay[texturePaths.Length];
        for (int i = 0; i < texturePaths.Length; i++)
        {
            displays[i] = GetDisplay();
            displays[i].DisplayTexture(texturePaths[i], duration > 0 ? duration : displayDuration);
        }
        return displays;
    }

    /// <summary>
    /// Start cycling through images using the pool (each image on a different display)
    /// </summary>
    public ImageDisplay StartCycling(string[] texturePaths, float delayBetween = 0)
    {
        if (texturePaths == null || texturePaths.Length == 0)
        {
            Debug.LogError("No texture paths provided for cycling!");
            return null;
        }

        cyclingTextures = texturePaths;
        cycleDisplayIndex = 0;
        float delay = delayBetween > 0 ? delayBetween : displayDuration;

        // Get first display and show first image
        currentCyclingDisplay = GetDisplay();
        currentCyclingDisplay.DisplayTexture(texturePaths[0], delay);
        
        Debug.Log($"Pool started cycling through {texturePaths.Length} images using pool displays");
        return currentCyclingDisplay;
    }

    /// <summary>
    /// Display images in sequence (one at a time)
    /// </summary>
    public ImageDisplay DisplaySequence(string[] texturePaths, float delayBetween = 0)
    {
        ImageDisplay display = GetDisplay();
        display.DisplaySequence(texturePaths, delayBetween > 0 ? delayBetween : displayDuration);
        return display;
    }

    /// <summary>
    /// Return all active displays to the pool
    /// </summary>
    public void ClearAll()
    {
        currentCyclingDisplay = null;
        cyclingTextures = null;
        
        foreach (ImageDisplay display in new List<ImageDisplay>(activeDisplays))
        {
            ReturnDisplay(display);
        }
        Debug.Log("All displays cleared and returned to pool.");
    }

    /// <summary>
    /// Get pool statistics
    /// </summary>
    public void PrintStats()
    {
        Debug.Log($"=== ImageDisplayPool Stats ===");
        Debug.Log($"Available displays: {availableDisplays.Count}");
        Debug.Log($"Active displays: {activeDisplays.Count}");
        Debug.Log($"Total displays: {availableDisplays.Count + activeDisplays.Count}");
    }
}
