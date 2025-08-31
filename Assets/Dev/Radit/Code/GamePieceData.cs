using UnityEngine;

[CreateAssetMenu(fileName = "NewGamePieceData", menuName = "Game/Game Piece Data")]
public class GamePieceData : ScriptableObject
{
    // The type or name of the piece (e.g., "Pikachu", "Charmander").
    public string pieceType;
    // The sprite for this piece.
    public Sprite pieceSprite;
    // The color of the piece.
    public Color pieceColor;
}
