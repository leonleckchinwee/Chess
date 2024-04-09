using System.Collections;
using System.Collections.Generic;
using Chess.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Chess.UI
{
    public class ChessBoard : MonoBehaviour
    {
        [SerializeField]
        PiecePrefab         m_PiecePrefab;

        [SerializeField]
        BoardPrefab         m_BoardPrefab;

        [SerializeField]
        Canvas              m_Canvas;

        [Range(0.0f, 1.0f)]
        [SerializeField]
        float               m_PieceScaling;

        MeshRenderer[,]     m_Squares;
        SpriteRenderer[,]   m_Pieces;

        bool                m_WhiteIsBottom;

        const float m_BoardDepth        = 5.0f;
        const float m_PieceDepth        = 1.0f;
        const float m_PieceDragDepth    = 0.0f;
        const float m_TextDepth         = -5.0f;

        void Awake()
        {
            m_WhiteIsBottom = FindObjectOfType<GameManager>().m_WhiteIsBottom;

            Shader shader = Shader.Find("Unlit/Color");

            m_Squares = new MeshRenderer[8, 8];
            m_Pieces  = new SpriteRenderer[8, 8];

            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    CreateBoard(file, rank, shader);
                    SetSquareColor(file, rank, m_BoardPrefab.LightSquares.Normal, m_BoardPrefab.DarkSquares.Normal);
                }
            }   

            CreateBorder(shader);
            CreateIndicators();
        }

        // Creates a blank chessboard
        void CreateBoard(int file, int rank, Shader shader)
        {
            Material material = new Material(shader);

            Transform square = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;

            square.parent = transform;
            square.name = BoardInfo.GetPositionNameFromFileRank(file, rank);
            square.position = BoardInfo.GetWorldPositionFromFileRank(file, rank, m_BoardDepth, m_WhiteIsBottom);

            m_Squares[file, rank] = square.GetComponent<MeshRenderer>();
            m_Squares[file, rank].material = material;

            CreatePieces(file, rank, square);
        }

        // Create empty chess pieces to be filled later
        void CreatePieces(int file, int rank, Transform parent)
        {
            SpriteRenderer piece = new GameObject().AddComponent<SpriteRenderer>();

            piece.transform.parent = parent;
            piece.transform.position = BoardInfo.GetWorldPositionFromFileRank(file, rank, m_PieceDepth, m_WhiteIsBottom);
            piece.transform.localScale = Vector3.one * m_PieceScaling;

            m_Pieces[file, rank] = piece;
        }

        // Create file, rank indicators
        void CreateIndicators()
        {
            for (int file = 0; file < 8; ++file)
            {
                Vector3 position = BoardInfo.GetWorldPositionFromFileRank(file, 0, m_TextDepth, true);
                string name = $"{BoardInfo.Files[file]}";

                Text indicator = new GameObject().AddComponent<Text>();
                indicator.name = name;

                indicator.transform.parent = m_Canvas.transform;
                indicator.transform.localScale = Vector3.one;
                indicator.transform.position = new Vector3(position.x, position.y - 1.15f, position.z);

                indicator.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                indicator.fontSize = 30;
                indicator.color = Color.white;
                indicator.alignment = TextAnchor.UpperCenter;
                indicator.text = name;
            }

            for (int rank = 0; rank < 8; ++rank)
            {
                Vector3 position = BoardInfo.GetWorldPositionFromFileRank(0, rank, m_TextDepth, true);
                string name = $"{rank + 1}";

                Text indicator = new GameObject().AddComponent<Text>();
                indicator.name = name;

                indicator.transform.parent = m_Canvas.transform;
                indicator.transform.localScale = Vector3.one;
                indicator.transform.position = new Vector3(position.x - 1.25f, position.y, position.z);

                indicator.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                indicator.fontSize = 30;
                indicator.color = Color.white;
                indicator.alignment = TextAnchor.MiddleRight;
                indicator.text = name;
            }
        }

        // Create a border aroudn the board
        void CreateBorder(Shader shader)
        {
            Material material = new Material(shader)
            {
                color = m_BoardPrefab.DebugSquares.Normal
            };

            Transform square = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;

            square.parent = transform;
            square.name = "Border";
            square.position = new Vector3(0.0f, 0.0f, m_BoardDepth + 1.0f);
            square.localScale = new Vector3(9.0f, 9.0f, 0.0f);

            square.GetComponent<MeshRenderer>().material = material;
        }

        // Set a square color
        void SetSquareColor(int file, int rank, Color light, Color dark)
        {
            m_Squares[file, rank].material.color = BoardInfo.IsLightSquare(file, rank) ? light : dark;
        }

        // Reset all square to default color
        void ResetAllSquareColor()
        {
            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    SetSquareColor(file, rank, m_BoardPrefab.LightSquares.Normal, m_BoardPrefab.DarkSquares.Normal);
                }
            }
        }

        // Update board with new positions
        public void UpdateBoard(Board board)
        {
            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    int pieceType = board.GetPieceAt(file, rank);

                    m_Pieces[file, rank].sprite = m_PiecePrefab.GetPieceSprite(pieceType);
                    m_Pieces[file, rank].name = BoardInfo.GetPositionNameFromFileRank(file, rank);
                    m_Pieces[file, rank].transform.position = BoardInfo.GetWorldPositionFromFileRank(file, rank, m_PieceDepth, m_WhiteIsBottom);
                }
            }
        }
    }
}
