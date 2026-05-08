using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CardHolder : MonoBehaviour
{
    [SerializeField] private Transform holdedCardTargetPosition;
    [SerializeField] private SpriteRenderer cardArea;
    [SerializeField] private Collider2D cardHolderCollider;

    [Header("Drag Attention animation")]
    [SerializeField] private float FadeInDuration;
    [SerializeField] private float AttentionScaleFactor;
    [SerializeField] private float AttentionScaleDuration;

    private CardController holdedCard;
    private Tween attentionTween;
    private Vector3 originalScale;
    private float maxAlpha;

    void Awake()
    {
        originalScale = cardArea.transform.localScale;
        maxAlpha = cardArea.color.a;
        cardArea.color = new Color(cardArea.color.r, cardArea.color.g, cardArea.color.b, 0.0f);

        SubscribeToDragEvents();
    }

    private void SubscribeToDragEvents()
    {
        EventBus.DragStarted += OnCardDragStarted;
        EventBus.DragStopped += OnCardDragStopped;
    }

    private void UnsubsribeFromDragEvents()
    {
        EventBus.DragStarted -= OnCardDragStarted;
        EventBus.DragStopped -= OnCardDragStopped;
    }

    private void OnCardDragStarted(CardController controller)
    {

        if (attentionTween != null)
        {
            KillAttentionAnimation();
        }

        Vector3 targetScale = originalScale * AttentionScaleFactor;
        attentionTween = cardArea.transform.DOScale(targetScale, AttentionScaleDuration).SetLoops(-1, LoopType.Yoyo);

        Tween fadeInTween = cardArea.DOColor(new Color(cardArea.color.r, cardArea.color.g, cardArea.color.b, maxAlpha), 0.25f)
            .OnComplete(() => attentionTween.Play())
            .Play();
    }

    private void OnCardDragStopped(CardController controller)
    {
        KillAttentionAnimation();

        if (holdedCard != null)
        {
            holdedCard.TravelTo(holdedCardTargetPosition.position);
        }
    }

    private void KillAttentionAnimation()
    {
        attentionTween?.Kill();
        attentionTween = null;

        // Reset to original state
        cardArea.transform.localScale = originalScale;
        cardArea.color = new Color(cardArea.color.r, cardArea.color.g, cardArea.color.b, 0.0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var card = other.gameObject.GetComponent<CardController>();
        if (card != null)
        {
            holdedCard = card;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("CardHolder: OnTriggerExit2D called with " + other.gameObject.name);

        var card = other.gameObject.GetComponent<CardController>();
        if (card != null && holdedCard == card)
        {
            holdedCard = null;
        }
    }
}
