using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Chess
{
    public class ChessBoardUI : MonoBehaviour
    {
        public PiecePrefab          m_Pieces;                           // Chesspiece prefab info
        public BoardPrefab          m_Squares;                          // Chessboard prefab info
        public bool                 m_WhiteIsBottom         = false;    // White at bottom

        public float                m_PieceScaling          = 0.15f;    // Scaling for all pieces
            
        private MeshRenderer[,]      m_SquareRenderers;                  // Squares
        private SpriteRenderer[,]    m_PieceRenderers;                   // Pieces

        const float                 m_BoardDepth            = 1.0f;     // Default chessboard depth
        const float                 m_PieceDepth            = -1.0f;    // Default chesspiece depth
        const float                 m_PieceFloatingDepth    = -2.0f;    // Chesspiece dragging depth

        void Awake ()
        {
            Shader shader = Shader.Find ("Unlit/Color");

            m_SquareRenderers = new MeshRenderer[8, 8];
            m_PieceRenderers = new SpriteRenderer[8, 8];

            // Create default chessboard
            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    CreateBoard (rank, file, out Transform square, shader);
                    CreatePiece (rank, file, square);  
                }
            }

            ResetSquareColor ();
        }

        void CreateBoard (int rank, int file, out Transform square, Shader shader)
        {
            square = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;

            square.parent = transform;
            square.name = BoardInfo.GetPositionNameFromCoordinates (rank, file);
            square.position = BoardInfo.GetWorldPositionFromCoordinates (rank, file, m_BoardDepth, m_WhiteIsBottom);

            m_SquareRenderers[rank, file] = square.gameObject.GetComponent<MeshRenderer>();

            Material material = new Material(shader);
            m_SquareRenderers[rank, file].material = material; 
        }

        void CreatePiece (int rank, int file, Transform square)
        {
            SpriteRenderer piece = new GameObject("").AddComponent<SpriteRenderer>();

            piece.transform.parent = square;
            piece.transform.position = BoardInfo.GetWorldPositionFromCoordinates (rank, file, m_PieceDepth, m_WhiteIsBottom);
            piece.transform.localScale = Vector3.one * m_PieceScaling;

            m_PieceRenderers[rank, file] = piece;
        }

        void ResetSquareColor ()
        {
            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    SetSquareColor (rank, file, m_Squares.LightSquares.Normal, m_Squares.DarkSquares.Normal);
                }
            }
        }    

        void SetSquareColor (int rank, int file, Color white, Color black)
        {
            Coordinates coordinates= new Coordinates (rank, file);
            m_SquareRenderers[rank, file].material.color = coordinates.IsLightSquare() ? white : black;
        }

        public void UpdatePieces (Board board)
        {
            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    int index = BoardInfo.CoordinatesToIndex (rank, file);
                    int piece = board.m_Squares[index];

                    m_PieceRenderers[rank, file].sprite = m_Pieces.GetPieceSprite (piece);
                    m_PieceRenderers[rank, file].transform.position = BoardInfo.GetWorldPositionFromCoordinates (rank, file, m_PieceDepth, m_WhiteIsBottom);
                    m_PieceRenderers[rank, file].name = Piece.GetPieceTypeName (piece);
                }
            }
        }
    }
}
