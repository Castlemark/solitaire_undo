using UnityEngine;

public class CardVisuals : MonoBehaviour
{
    [SerializeField] private SpriteRenderer cardFace;
    [SerializeField] private Sprite[] cardFaceSprites;

    public void SetCardVisuals()
    {
        if (cardFaceSprites.Length > 0)
        {
            int randomIndex = Random.Range(0, cardFaceSprites.Length);
            cardFace.sprite = cardFaceSprites[randomIndex];
        }
    }
}
