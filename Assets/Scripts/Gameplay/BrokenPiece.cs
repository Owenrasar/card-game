using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class BrokenPiece : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector3 moveDirection;
    private float moveSpeed;
    private float fadeDuration;

    void Awake()
    {
        // Get the SpriteRenderer attached to this prefab
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Initializes the piece with its movement and fade parameters.
    /// This is called by the Marker script right after instantiation.
    /// </summary>
    public void Initialize(Vector3 direction, float speed, float duration, Color color)
    {
        moveDirection = direction;
        moveSpeed = speed;
        fadeDuration = duration;
        spriteRenderer.color = color;

        // Start the combined move and fade coroutine
        StartCoroutine(MoveAndFade());
    }

    private IEnumerator MoveAndFade()
    {
        Color originalColor = spriteRenderer.color;
        float timer = 0f;

        // --- NEW ---
        // 1. Define the start and end points for the movement
        Vector3 startPosition = transform.position;
        
        // Calculate the total distance to travel based on speed and duration
        // This is where the piece would end up if it moved at constant speed
        Vector3 endPosition = startPosition + (moveDirection * moveSpeed * fadeDuration);
        // -----------

        while (timer < fadeDuration)
        {
            // --- 1. Movement ---

            // Increment timer and calculate 't' (a value from 0 to 1)
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            t = Mathf.Clamp01(t); // Ensure t doesn't go slightly over 1

            // --- NEW: Easing Function ---
            // 2. Create an "Ease-Out" t-value.
            // We use the start of a Sine wave (Mathf.Sin)
            // This starts fast and slows to a 0-speed stop.
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);
            // --------------------------
            
            // 3. Apply the movement using Lerp
            // Instead of Translate, we set the position directly
            // using our new 'easedT' value.
            transform.position = Vector3.Lerp(startPosition, endPosition, easedT);


            // --- 2. Fading ---
            // Fading still happens linearly based on the original 't'
            // (You could also use 'easedT' here if you want the fade to ease too)
            Color c = originalColor;
            c.a = Mathf.Lerp(originalColor.a, 0f, t);
            spriteRenderer.color = c;

            // Wait for the next frame
            yield return null;
        }

        // After the loop finishes, destroy this GameObject
        Destroy(gameObject);
    }
}