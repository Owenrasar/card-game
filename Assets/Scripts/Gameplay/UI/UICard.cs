using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UICard : MonoBehaviour
{
    public AudioSource hoverSound;
    public AudioSource playSound;

    public TextMeshPro nameLabel;
    public TextMeshPro dashLabel;
    public TextMeshPro actionLabel;
    public Card linkedCard;
    public Timeline linkedTimeline;
    public DeckManager deck;
    public UITimeline linkedUITimeline;

    [Header("Positions (local space, relative to parent)")]
    public float offPosY;
    public float bottomPosY;
    public float topPosY;

    [Header("Settings")]
    public float moveSpeed = 2f;
    // New variables for scaling
    // public Vector3 lowerScale = Vector3.one; // Replaced by lowerScaleXZ
    // public Vector3 upperScale = new Vector3(1.1f, 1.1f, 1.1f); // Replaced by upperScaleXZ
    public float lowerScale = 0.4f;
    public float upperScale = 0.75f;

    private Coroutine currentMove;
    private RectTransform rectTransform;
    private Canvas cardCanvas;

    public void Init()
    {

        deck = transform.parent.parent.parent.Find("PlayerDeck").GetComponent<DeckManager>();
        
        rectTransform = GetComponent<RectTransform>();
        // Initialize scale to lowerScaleXZ, and set Y scale to 1.0f to match old Vector3.one default
        rectTransform.localScale = new Vector3(lowerScale, lowerScale, lowerScale);
        linkedCard = deck.Draw();

        cardCanvas = gameObject.transform.GetChild(0).gameObject.GetComponent<Canvas>();
    
        // Set a default order
        if (cardCanvas != null)
        {
            cardCanvas.sortingOrder = 0;
            nameLabel.sortingOrder = 0;
            dashLabel.sortingOrder = 0;
            actionLabel.sortingOrder = 0;
        }
        UpdateCard();
        Lower();
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
            -5.1f
        );
        
        // Target X/Z scale is from variable, Y scale is from current transform
        Vector3 targetScale = new Vector3(
            upperScale,
            upperScale,
            upperScale
        );

        if (cardCanvas != null)
        {
            // Set this card's order to 1 (or any number higher than the default)
            cardCanvas.sortingOrder = 1;
            nameLabel.sortingOrder = 1;
            dashLabel.sortingOrder = 1;
            actionLabel.sortingOrder = 1;
        }
        hoverSound.Play();
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
           -5 
        );
        
        // Target X/Z scale is from variable, Y scale is from current transform
        Vector3 targetScale = new Vector3(
            lowerScale,
            lowerScale,
            lowerScale
        );

        if (cardCanvas != null)
        {
            // Set it back to the default order
            cardCanvas.sortingOrder = 0;
            nameLabel.sortingOrder = 0;
            dashLabel.sortingOrder = 0;
            actionLabel.sortingOrder = 0;
        }
        currentMove = StartCoroutine(MoveAndScaleRoutine(targetPos, targetScale));
    }

    public void MaxLower()
    {
        if (currentMove != null)
            StopCoroutine(currentMove);

        // Call the updated routine with position and scale
        // Target Y pos is from variable, X/Z pos are from current transform
        Vector3 targetPos = new Vector3(
            rectTransform.localPosition.x,
            offPosY,
           -5 
        );
        
        // Target X/Z scale is from variable, Y scale is from current transform
        Vector3 targetScale = new Vector3(
            lowerScale,
            lowerScale,
            lowerScale
        );

        if (cardCanvas != null)
        {
            // Set it back to the default order
            cardCanvas.sortingOrder = 0;
            nameLabel.sortingOrder = 0;
            dashLabel.sortingOrder = 0;
            actionLabel.sortingOrder = 0;
        }
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


    public void UpdateCard()
    {
        nameLabel.text = "(" + linkedCard.cost.ToString() + ")" + linkedCard.name;
        actionLabel.text = "";
        foreach (var action in linkedCard.actions)
        {
            string type = action.giveType();
            if (type == "Block") {
                actionLabel.text += "[" + type + ", Val:" + (linkedCard.cost + action.valueMod).ToString() + ", Time:" + action.arg.ToString() + "]\n";
            } else {
                actionLabel.text += "[" + type + ", Val:" + (linkedCard.cost + action.valueMod).ToString() + ", Area:" + action.arg.ToString() + "]\n";
            }
        }
    }

    public void PlayToTimeline()
    {
        playSound.Play();
        linkedTimeline.cards.Add(linkedCard);
        linkedUITimeline.UpdateActions();
        deck = transform.parent.parent.parent.Find("PlayerDeck").GetComponent<DeckManager>();
        deck.DiscardFromHand(linkedCard);
        linkedCard = deck.Draw();
        UpdateCard();
    }
}

