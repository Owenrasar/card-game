using UnityEngine;
using System.Collections; // Needed for IEnumerator

public class Marker : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private Color originalColor;
    private Coroutine fadeRoutine;
    private Coroutine flashRoutine;

    // --- NEW PUBLIC VARIABLES ---
    [Header("Break Effect")]
    public GameObject breakPrefab;
    public float breakSpeed = 2f;
    public float breakFadeDuration = 1f;
    // ----------------------------

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    // Call this to start the fade in/out effect
    public void PreRender(float duration = 0.75f)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeInOut(duration));
    }

    private IEnumerator FadeInOut(float duration)
    {

        // Start fully transparent
        Color transparentColor = originalColor;
        transparentColor.a = 0f;
        spriteRenderer.color = transparentColor;

        float halfDuration = duration / 2f;
        float timer = 0f;

        // --- Fade In ---
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            float t = timer / halfDuration;
            Color c = originalColor;
            c.a = Mathf.Lerp(0f, originalColor.a, t);
            spriteRenderer.color = c;
            yield return null;
        }

        // --- Fade Out ---
        timer = 0f;
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            float t = timer / halfDuration;
            Color c = originalColor;
            c.a = Mathf.Lerp(originalColor.a, 0f, t);
            spriteRenderer.color = c;
            yield return null;
        }

        // Restore transparent (optional)
        spriteRenderer.color = transparentColor;
        fadeRoutine = null;
    }

    /// <summary>
    /// Flashes the sprite from its current color to white and back.
    /// </summary>
    /// <param name="toWhiteTime">Time to reach full white.</param>
    /// <param name="toNormalTime">Time to return to original color.</param>
    public void FlashActive(float toWhiteTime = 0.25f, float toNormalTime = 0.25f)
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine(toWhiteTime, toNormalTime));
    }

    private IEnumerator FlashRoutine(float toWhiteTime, float toNormalTime)
    {
        Color originalColor = spriteRenderer.color;
        Color whiteColor = Color.white;
        whiteColor.a = originalColor.a; // keep same alpha

        float timer = 0f;

        // --- Flash to White ---
        while (timer < toWhiteTime)
        {
            timer += Time.deltaTime;
            float t = timer / toWhiteTime;
            spriteRenderer.color = Color.Lerp(originalColor, whiteColor, t);
            yield return null;
        }

        // --- Return to Original ---
        timer = 0f;
        while (timer < toNormalTime)
        {
            timer += Time.deltaTime;
            float t = timer / toNormalTime;
            spriteRenderer.color = Color.Lerp(whiteColor, originalColor, t);
            yield return null;
        }

        // Restore original
        spriteRenderer.color = originalColor;
        flashRoutine = null;
    }

    // --- NEW BREAK FUNCTION ---

    /// <summary>
    /// Disables the current sprite and spawns two breaking pieces.
    /// </summary>
    public void Break()
    {
        // Stop any current fade or flash
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            fadeRoutine = null;
        }
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        // Turn off this marker's sprite
        spriteRenderer.enabled = false;

        // Instantiate and initialize the break-down piece
        if (breakPrefab != null)
        {
            GameObject downPiece = Instantiate(breakPrefab, transform.position, Quaternion.identity);
            // Try to get the BrokenPiece script and initialize it
            BrokenPiece pieceScript = downPiece.GetComponent<BrokenPiece>();
            if (pieceScript != null)
            {
                pieceScript.Initialize(Vector3.down, breakSpeed, breakFadeDuration, originalColor);
            }
            else
            {
                Debug.LogWarning("breakPrefab is missing the BrokenPiece.cs script!");
            }
            
            GameObject upPiece = Instantiate(breakPrefab, transform.position, Quaternion.Euler(0, 0, 180f));
            BrokenPiece upPieceScript = upPiece.GetComponent<BrokenPiece>();
            
            if (upPieceScript != null)
            {
                upPieceScript.Initialize(Vector3.up, breakSpeed, breakFadeDuration, originalColor);
            }
        }
    }
}