using UnityEngine;

namespace Chess.UI
{
    [CreateAssetMenu(menuName = "Chessboard")]
    public class BoardPrefab : ScriptableObject
    {
        public SquareColor LightSquares;
        public SquareColor DarkSquares;

        [System.Serializable]
        public struct SquareColor
        {
            public Color Normal;
            public Color Legal;
            public Color Selected;
            public Color MoveFromHighlight;
            public Color MoveToHighlight;
            public Color Debug;
        }
    }
}
