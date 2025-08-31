using UnityEngine;

public class GamePiece : MonoBehaviour
{
    // A public reference to the Scriptable Object containing the piece's data.
    public GamePieceData pieceData;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Ensure the piece has data and a SpriteRenderer component.
        if (pieceData != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = pieceData.pieceSprite;
            spriteRenderer.color = pieceData.pieceColor;
        }
    }
}
