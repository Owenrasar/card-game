using UnityEngine;
using TMPro;

public class Notify : MonoBehaviour
{
    public TextMeshPro textMesh;
    public float moveDuration = 3.0f;
    public AnimationCurve arcCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 startPos;
    private Vector3 controlPoint;
    private Vector3 endPos;
    private float elapsedTime;

    void Awake()
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();
    }

    /// <summary>
    /// Displays a notification text at a given position, color, and arc height.
    /// </summary>
    /// <param name="message">Text to display</param>
    /// <param name="position">World position to start from</param>
    /// <param name="color">Text color</param>
    /// <param name="height">Arc height above the start position</param>
    public void DisplayNotify(string message, Vector3 position, Color color, float height)
    {
        if (message == "0") return;
        transform.position = position;
        textMesh.text = message;
        textMesh.color = color;

        startPos = position;

        // Randomize slight left/right offset
        float horizontalOffset = Random.Range(-0.5f, 0.5f);
        endPos = position + new Vector3(horizontalOffset, height, 0);

        // Control point defines the apex of the arc
        float apexX = position.x + Random.Range(-0.3f, 0.3f);
        controlPoint = new Vector3(apexX, position.y + height, position.z);

        elapsedTime = 0f;
    }

    void Update()
    {
        if (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);

            // Quadratic Bézier curve for smooth arc
            Vector3 m1 = Vector3.Lerp(startPos, controlPoint, t);
            Vector3 m2 = Vector3.Lerp(controlPoint, endPos, t);
            transform.position = Vector3.Lerp(m1, m2, t);

            // Fade out over time
            Color c = textMesh.color;
            c.a = 1 - t;
            textMesh.color = c;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
