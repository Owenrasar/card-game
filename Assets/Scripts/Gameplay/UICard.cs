using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UICard : MonoBehaviour
{
    public TextMeshPro nameLabel;
    public TextMeshPro actionLabel;
    public Card linkedCard;
    public Timeline linkedTimeline;
    public DeckManager deck;

    [Header("Positions (local space, relative to parent)")]
    // public Vector3 bottomPos; // Replaced by bottomPosY
    // public Vector3 topPos; // Replaced by topPosY
    public float bottomPosY = 0f;
    public float topPosY = 50f; // Added a default value

    [Header("Settings")]
    public float moveSpeed = 2f;
    // New variables for scaling
    // public Vector3 lowerScale = Vector3.one; // Replaced by lowerScaleXZ
    // public Vector3 upperScale = new Vector3(1.1f, 1.1f, 1.1f); // Replaced by upperScaleXZ
    public float lowerScaleXZ = 1.0f;
    public float upperScaleXZ = 1.1f; // Default to 110%

    private Coroutine currentMove;
    private RectTransform rectTransform;

    private void Awake()
    {
        deck = transform.parent.parent.parent.Find("Deck").GetComponent<DeckManager>();
        
        rectTransform = GetComponent<RectTransform>();
        // Initialize scale to lowerScaleXZ, and set Y scale to 1.0f to match old Vector3.one default
        rectTransform.localScale = new Vector3(lowerScaleXZ, 1.0f, lowerScaleXZ);
        linkedCard = deck.Draw();
        Init();
    }

    public void Raise()
    {
        if (currentMove != null)
            StopCoroutine(currentMove);

        // Call the updated routine with position and scale
        // Target Y pos is from variable, X/Z pos are from current transform
        Vector3 targetPos = new Vector3(
            rectTransform.localPosition.x,
            topPosY,
            rectTransform.localPosition.z
        );
        
        // Target X/Z scale is from variable, Y scale is from current transform
        Vector3 targetScale = new Vector3(
            upperScaleXZ,
            rectTransform.localScale.y,
            upperScaleXZ
        );

        currentMove = StartCoroutine(MoveAndScaleRoutine(targetPos, targetScale));
    }

    /// <summary>
    /// Starts moving downward (top → bottom) and scaling down.
    /// Cancels any ongoing Raise() coroutine.
    /// </summary>
    public void Lower()
    {
        if (currentMove != null)
            StopCoroutine(currentMove);

        // Call the updated routine with position and scale
        // Target Y pos is from variable, X/Z pos are from current transform
        Vector3 targetPos = new Vector3(
            rectTransform.localPosition.x,
            bottomPosY,
            rectTransform.localPosition.z
        );
        
        // Target X/Z scale is from variable, Y scale is from current transform
        Vector3 targetScale = new Vector3(
            lowerScaleXZ,
            rectTransform.localScale.y,
            lowerScaleXZ
        );

        currentMove = StartCoroutine(MoveAndScaleRoutine(targetPos, targetScale));
    }

    /// <summary>
    /// Coroutine to move and scale the card simultaneously.
    /// </summary>
    /// <param name="targetPos">The target localPosition.</param>
    /// <param name="targetScale">The target localScale.</param>
    private IEnumerator MoveAndScaleRoutine(Vector3 targetPos, Vector3 targetScale)
    {
        Vector3 startPos = rectTransform.localPosition;
        Vector3 startScale = rectTransform.localScale;

        float distance = Vector3.Distance(startPos, targetPos);
        
        // Check if we are already (practically) at the target
        if (distance < 0.001f && Vector3.Distance(startScale, targetScale) < 0.001f)
            yield break;

        float t = 0f;

        while (t < 1f)
        {
            // Use distance of position move to determine speed
            if (distance > 0.001f)
            {
                t += Time.deltaTime * (moveSpeed / distance);
            }
            else
            {
                // If only scaling, just use moveSpeed as a duration factor
                t += Time.deltaTime * moveSpeed; 
            }

            // Ease-out quadratic: starts fast, slows near target
            float easedT = 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 2f);

            // Interpolate both position and scale
            rectTransform.localPosition = Vector3.Lerp(startPos, targetPos, easedT);
            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, easedT);
            
            yield return null;
        }

        // Snap to final position and scale
        rectTransform.localPosition = targetPos;
        rectTransform.localScale = targetScale;
        
        currentMove = null;
    }


    public void Init()
    {
        nameLabel.text = "(" + linkedCard.cost.ToString() + ")" + linkedCard.name;
        actionLabel.text = "";
        foreach (var action in linkedCard.actions)
        {
            string type = action.giveType();
            actionLabel.text += "[" + type + ", Val:" + (linkedCard.cost + action.valueMod).ToString() + ", Arg:" + action.arg.ToString() + "]\n";
        }
    }

    public void PlayToTimeline()
    {
        linkedTimeline.cards.Add(linkedCard);
        deck = transform.parent.parent.parent.Find("Deck").GetComponent<DeckManager>();
        linkedCard = deck.Draw();
        Init();
    }
}

