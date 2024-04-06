using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public class ChessBoardUI : MonoBehaviour
    {
        public PiecePrefab          m_Pieces;                           // Chesspiece prefab info
        public BoardPrefab          m_Squares;                          // Chessboard prefab info
            
        public MeshRenderer[,]      m_SquareRenderers;                  // Squares
        public SpriteRenderer[,]    m_PieceRenderers;                   // Pieces

        public bool                 m_WhiteIsBottom         = false;    // White at bottom

        public float                m_PieceScaling          = 0.15f;    // Scaling for all pieces

        const float                 m_BoardDepth            = 0.0f;     // Default chessboard depth
        const float                 m_PieceDepth            = -1.0f;    // Default chesspiece depth
        const float                 m_PieceFloatingDepth    = -2.0f;    // Chesspiece dragging depth

        void Awake()
        {
            CreateBoard();
        }

        void CreateBoard()
        {
            Shader shader = Shader.Find("Unlit/Color");

            m_SquareRenderers = new MeshRenderer[8, 8];
            m_PieceRenderers = new SpriteRenderer[8, 8];

            // Create blank 8x8 chessboard
            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    Transform square = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;

                    square.parent = transform;
                    square.name = BoardInfo.GetNameFromCoordinates(rank, file);
                    square.position = GetPositionFromCoordinates(rank, file, m_BoardDepth);

                    m_SquareRenderers[file, rank] = square.gameObject.GetComponent<MeshRenderer>();

                    Material material = new Material(shader);
                    m_SquareRenderers[file, rank].material = material;

                    SpriteRenderer piece = new GameObject("New Piece").AddComponent<SpriteRenderer>();
                    piece.transform.parent = square;
                    piece.transform.position = GetPositionFromCoordinates(rank, file, m_PieceDepth);
                    piece.transform.localScale = Vector3.one * m_PieceScaling;

                    m_PieceRenderers[file, rank] = piece;
                }
            }

            // Set the checkered pattern
            ResetSquareColor();
        }

        void ResetSquareColor(bool highlight = true)
        {
            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    SetSquareColor(rank, file, m_Squares.LightSquares.Normal, m_Squares.DarkSquares.Normal);
                }
            }
        }

        void SetSquareColor(Coordinates coordinates, Color light, Color dark)
        {
            m_SquareRenderers[coordinates.FileIndex, coordinates.RankIndex].material.color = (coordinates.IsLightSquare()) ? light : dark;
        }

        void SetSquareColor(int rank, int file, Color light, Color dark)
        {
            Coordinates coordinates = new Coordinates(rank, file);
            SetSquareColor(coordinates, light, dark);
        }

        Vector3 GetPositionFromCoordinates(int rank, int file, float depth = 0.0f)
        {
            if (m_WhiteIsBottom)
                return new Vector3(-3.5f + file, -3.5f + rank, depth);

            return new Vector3(-3.5f + 7 - file, 7 - rank - 3.5f, depth);
        }

        Vector3 GetPositionFromCoordinates(Coordinates coordinates, float depth)
        {
            return GetPositionFromCoordinates(coordinates.RankIndex, coordinates.FileIndex, depth);
        }

        public void UpdateBoard(Board board)
        {
            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    int piece = board.Squares[BoardInfo.IndexFromCoordinates(rank, file)];

                    m_PieceRenderers[file, rank].name = Piece.PieceTypeName(piece);
                    m_PieceRenderers[file, rank].sprite = m_Pieces.GetPieceSprite(piece);
                    m_PieceRenderers[file, rank].transform.position = GetPositionFromCoordinates(rank, file, m_PieceDepth);
                    m_PieceRenderers[file, rank].transform.localScale = Vector3.one * m_PieceScaling;
                }
            }
        }

        public void SelectSquare(Coordinates coordinates)
        {
            SetSquareColor(coordinates, m_Squares.LightSquares.Selected, m_Squares.DarkSquares.Selected);
        }

        public void CancelSelection(Coordinates coordinates)
        {
            ResetSquareColor();
        }

        public void DragPiece(Coordinates coordinates, Vector2 mousePosition)
        {
            m_PieceRenderers[coordinates.FileIndex, coordinates.RankIndex].transform.position 
                    = new Vector3(mousePosition.x, mousePosition.y, m_PieceFloatingDepth);
        }

        public bool IsSquareUnderMouse(Vector2 mousePosition, out Coordinates coordinates)
        {
            int file = (int)(mousePosition.x + 4);
            int rank = (int)(mousePosition.y + 4);

            if (!m_WhiteIsBottom)
            {
                file = 7 - file;
                rank = 7 - rank;
            }

            coordinates = new Coordinates(rank, file);
            return file >= 0 && file < 8 && rank >= 0 && rank < 8;
        }

        

        public void ResetPiecePosition(Coordinates coordinates)
        {
            Vector3 position = GetPositionFromCoordinates(coordinates, m_PieceDepth);
            m_PieceRenderers[coordinates.FileIndex, coordinates.RankIndex].transform.position = position;
        }        
    }
}
