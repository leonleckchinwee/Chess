using UnityEngine;

namespace Chess
{
    [CreateAssetMenu(menuName = "Chess Piece")]
    public class PiecePrefab : ScriptableObject
    {
        public ChessSprites Whites;
        public ChessSprites Blacks;

        public Sprite GetPieceSprite(int piece)
        {
            ChessSprites sprite = Piece.IsWhite(piece) ? Whites : Blacks;
            int pieceType = Piece.PieceType(piece);

            switch (pieceType)
            {
                case Piece.Pawn:
                    return sprite.Pawn;
                
                case Piece.Knight:
                    return sprite.Knight;
                
                case Piece.Bishop:
                    return sprite.Bishop;

                case Piece.Rook:
                    return sprite.Rook;

                case Piece.Queen:
                    return sprite.Queen;

                case Piece.King:
                    return sprite.King;

                default:
                    //Debug.Log(piece);
                    return null;
            }
        }

        [System.Serializable]
        public class ChessSprites
        {
            public Sprite Pawn, Knight, Bishop, Rook, Queen, King;

            public Sprite this[int i] => new Sprite[]
            {
                Pawn, Knight, Bishop, Rook, Queen, King
            }[i];
        }
    }
}