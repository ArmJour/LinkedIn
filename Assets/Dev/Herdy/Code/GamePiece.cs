using UnityEngine;

public class GamePiece : MonoBehaviour
{
    public GamePieceData pieceData;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Kalau sudah ada data bawaan di inspector
        if (pieceData != null)
        {
            ApplyData(pieceData);
        }
    }

    // Fungsi ini dipanggil setiap kali kita assign piece baru
    public void SetData(GamePieceData data)
    {
        pieceData = data;
        ApplyData(pieceData);
    }

    private void ApplyData(GamePieceData data)
    {
        if (data != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = data.pieceSprite;
            spriteRenderer.color = data.pieceColor;
        }
    }
}
