// ImageDisplay.cs - Displays loaded textures on screen
using UnityEngine;
using UnityEngine.UI;

public class ImageDisplay : MonoBehaviour
{
    public RawImage displayImage;
    public Text imageInfoText;
    public float displayDuration = 3f;
    
    private float displayTimer;
    private bool isDisplaying = false;
    private string currentImageName;
    
    // Auto-cycling
    private bool isAutoCycling = false;
    private string[] cyclePaths;
    private int cycleIndex = 0;

    void Start()
    {
        // Auto-find components if not assigned
        if (displayImage == null)
        {
            displayImage = GetComponent<RawImage>();
            if (displayImage == null)
                displayImage = GetComponentInChildren<RawImage>();
        }
        
        if (imageInfoText == null)
        {
            imageInfoText = GetComponent<Text>();
            if (imageInfoText == null)
                imageInfoText = GetComponentInChildren<Text>();
        }
        
        if (displayImage == null)
        {
            Debug.LogError($"ImageDisplay on {gameObject.name}: RawImage component not found! Add RawImage to this GameObject or a child.", gameObject);
        }
        else
        {
            Debug.Log($"ImageDisplay on {gameObject.name}: RawImage found.");
            // Ensure RawImage is visible
            displayImage.enabled = true;
            displayImage.color = Color.white; // Make sure it's not transparent
            Debug.Log($"RawImage color set to: {displayImage.color}");
        }
    }

    void Update()
    {
        // Handle auto-cycling
        if (isAutoCycling && cyclePaths != null && cyclePaths.Length > 0)
        {
            displayTimer -= Time.deltaTime;
            if (displayTimer <= 0)
            {
                // Move to next image
                cycleIndex = (cycleIndex + 1) % cyclePaths.Length;
                DisplayTextureInternal(cyclePaths[cycleIndex]);
                displayTimer = displayDuration;
                Debug.Log($"Cycling: {cycleIndex + 1}/{cyclePaths.Length}");
            }
        }
        // Handle single display timer
        else if (isDisplaying)
        {
            displayTimer -= Time.deltaTime;
            if (displayTimer <= 0)
            {
                HideImage();
            }
        }
    }

    /// <summary>
    /// Display a texture by name
    /// </summary>
    public void DisplayTexture(string texturePath, float duration = 0)
    {
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("ResourceManager not found!");
            return;
        }

        Texture2D texture = ResourceManager.Instance.LoadTexture(texturePath);
        
        if (texture == null)
        {
            Debug.LogError($"Failed to load texture: {texturePath}");
            return;
        }

        // Stop cycling when manually displaying a texture
        isAutoCycling = false;
        cyclePaths = null;
        
        // Set display duration
        if (duration > 0)
            displayDuration = duration;
        
        DisplayTextureInternal(texturePath, texture);
    }

    /// <summary>
    /// Internal method to display a texture (without loading)
    /// </summary>
    private void DisplayTextureInternal(string texturePath, Texture2D texture = null)
    {
        if (displayImage == null)
        {
            // Try to find it again
            displayImage = GetComponent<RawImage>();
            if (displayImage == null)
                displayImage = GetComponentInChildren<RawImage>();
                
            if (displayImage == null)
            {
                Debug.LogError($"ImageDisplay: RawImage component not found on {gameObject.name}!", gameObject);
                return;
            }
        }

        if (texture == null)
        {
            Debug.Log($"Loading texture: {texturePath}");
            texture = ResourceManager.Instance.LoadTexture(texturePath);
            if (texture == null)
            {
                Debug.LogError($"Failed to load texture: {texturePath}");
                return;
            }
            Debug.Log($"✓ Loaded: {texturePath}");
        }

        displayImage.texture = texture;
        displayImage.enabled = true;
        
        // Ensure it's visible
        CanvasGroup canvasGroup = GetComponentInParent<CanvasGroup>();
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
        
        currentImageName = texturePath;
        
        // Only set isDisplaying if NOT cycling
        if (!isAutoCycling)
        {
            isDisplaying = true;
            displayTimer = displayDuration;
        }

        // Update info text
        if (imageInfoText != null)
        {
            imageInfoText.text = $"Now showing: {texturePath}\n({texture.width}x{texture.height})";
        }

        Debug.Log($"✓ Displaying: {texturePath}");
    }

    /// <summary>
    /// Hide the current image
    /// </summary>
    public void HideImage()
    {
        if (displayImage != null)
        {
            displayImage.texture = null;
        }
        isDisplaying = false;

        if (imageInfoText != null)
        {
            imageInfoText.text = "";
        }

        Debug.Log("Image hidden");
    }

    /// <summary>
    /// Check if currently displaying an image
    /// </summary>
    public bool IsDisplaying() => isDisplaying;

    /// <summary>
    /// Check if currently auto-cycling
    /// </summary>
    public bool IsAutoCycling() => isAutoCycling;

    /// <summary>
    /// Reset display for reuse in pool
    /// </summary>
    public void ResetForPool()
    {
        StopCycling();
        HideImage();
        isDisplaying = false;
        isAutoCycling = false;
        displayTimer = 0;
        currentImageName = null;
        cyclePaths = null;
        cycleIndex = 0;
    }

    /// <summary>
    /// Start cycling through images automatically
    /// </summary>
    public void StartCycling(string[] texturePaths, float delayBetween = 3f)
    {
        if (texturePaths == null || texturePaths.Length == 0)
        {
            Debug.LogError("No texture paths provided!");
            return;
        }

        cyclePaths = texturePaths;
        cycleIndex = 0;
        displayDuration = delayBetween;
        isAutoCycling = true;
        isDisplaying = false; // Don't use single-display timer while cycling
        
        // Display first image
        if (cyclePaths.Length > 0)
        {
            DisplayTextureInternal(cyclePaths[0]);
            displayTimer = displayDuration;
        }
        
        Debug.Log($"Started cycling through {cyclePaths.Length} images ({displayDuration}s per image)");
    }

    /// <summary>
    /// Stop cycling through images
    /// </summary>
    public void StopCycling()
    {
        isAutoCycling = false;
        isDisplaying = false;
        Debug.Log("Stopped cycling");
    }

    /// <summary>
    /// Display images sequentially (one-time sequence)
    /// </summary>
    public void DisplaySequence(string[] texturePaths, float delayBetween = 2f)
    {
        StartCoroutine(DisplaySequenceCoroutine(texturePaths, delayBetween));
    }

    private System.Collections.IEnumerator DisplaySequenceCoroutine(string[] texturePaths, float delay)
    {
        foreach (string path in texturePaths)
        {
            DisplayTexture(path, delay);
            yield return new WaitForSeconds(delay);
        }
    }
}
