using System.Collections;
using System.Collections.Generic;
using Chess.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Chess.UI
{
    public class ChessBoard : MonoBehaviour
    {
        public enum DebugType
        {
            None, EmptySquares, FriendlySquares, OpponentSquares, AttackingSquares,
            PawnMoves, BishopMoves, KnightMoves, RookMoves, QueenMoves, KingMoves
        }

        public enum PieceColorType
        {
            White, Black
        }

        [SerializeField]
        PiecePrefab         m_PiecePrefab;

        [SerializeField]
        BoardPrefab         m_BoardPrefab;

        [SerializeField]
        Color               m_Background;

        [SerializeField]
        Canvas              m_Canvas;

        [Range(0.0f, 1.0f)]
        [SerializeField]
        float               m_PieceScaling;

        [SerializeField]
        PieceColorType      m_PieceColorForDebug;

        MeshRenderer[,]     m_Squares;
        SpriteRenderer[,]   m_Pieces;
        Text                m_DebugIndicator;

        bool                m_WhiteIsBottom;
        int                 m_PieceColor;

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

            // Debug indicator
            {
                Vector3 position = BoardInfo.GetWorldPositionFromFileRank(4, 7, m_TextDepth, true);

                m_DebugIndicator = new GameObject().AddComponent<Text>();
                m_DebugIndicator.name = "Debug";

                m_DebugIndicator.transform.parent = m_Canvas.transform;
                m_DebugIndicator.transform.localScale = Vector3.one;
                m_DebugIndicator.transform.position = new Vector3(position.x - 0.5f, position.y + 0.75f, m_TextDepth);

                m_DebugIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(500.0f, 50.0f);

                m_DebugIndicator.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                m_DebugIndicator.fontSize = 30;
                m_DebugIndicator.color = Color.white;
                m_DebugIndicator.alignment = TextAnchor.MiddleCenter;
            }
        }

        // Create a border aroudn the board
        void CreateBorder(Shader shader)
        {
            Material material = new Material(shader)
            {
                color = m_Background
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

        // Reset all square to default color
        public void ResetAllSquareColor()
        {
            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    SetSquareColor(file, rank, m_BoardPrefab.LightSquares.Normal, m_BoardPrefab.DarkSquares.Normal);
                }
            }
        }

        public void ResetPosition(int file, int rank)
        {
            m_Pieces[file, rank].transform.position = BoardInfo.GetWorldPositionFromFileRank(file, rank, m_PieceDepth, m_WhiteIsBottom);
        }

        public void ResetPosition(FileRank position)
        {
            ResetPosition(position.File, position.Rank);
        }

        public bool IsValidSquare(Vector2 mousePos, out FileRank position)
        {
            int file = (int)(mousePos.x + 4);
            int rank = (int)(mousePos.y + 4);

            if (!m_WhiteIsBottom)
            {
                file = 7 - file;
                rank = 7 - rank;
            }

            if (file >= 0 && file < 8 && rank >= 0 && rank < 8)
            {
                position = new FileRank(file, rank);
                return true;
            }

            position = FileRank.None;
            return false;
        }
    
        public void SelectSquare(int file, int rank)
        {
            SetSquareColor(file, rank, m_BoardPrefab.LightSquares.Selected, m_BoardPrefab.DarkSquares.Selected);
        }

        public void SelectSquare(FileRank position)
        {
            SelectSquare(position.File, position.Rank);
        }

        public void DragPiece(int file, int rank, Vector2 mousePos)
        {
            m_Pieces[file, rank].transform.position = new Vector3(mousePos.x, mousePos.y, m_PieceDragDepth);
        }

        public void DragPiece(FileRank position, Vector2 mousPos)
        {
            DragPiece(position.File, position.Rank, mousPos);
        }

        /******************** Debug Drawings ********************/
        public void UpdateDebug(DebugType type, Board board)
        {
            if (type != DebugType.None)
                m_DebugIndicator.text = $"DEBUG MODE: {type}";
            else
                m_DebugIndicator.text = "";

            ResetAllSquareColor();

            m_PieceColor = (m_PieceColorForDebug == PieceColorType.White) ? Piece.White : Piece.Black;

            switch (type)
            {
                default:
                case DebugType.None:
                    return;

                case DebugType.EmptySquares:
                    DrawEmptySquares(board);
                    break;

                case DebugType.FriendlySquares:
                    DrawFriendlySquares(board);
                    break;

                case DebugType.OpponentSquares:
                    DrawOpponentSquares(board);
                    break;

                case DebugType.AttackingSquares:
                    DrawAttackingSquares(board);
                    break;

                case DebugType.PawnMoves:
                    DrawPawnMoves(board);
                    break;

                case DebugType.BishopMoves:
                    DrawBishopMoves(board);
                    break;

                case DebugType.KnightMoves:
                    DrawKnightMoves(board);
                    break;

                case DebugType.RookMoves:
                    DrawRookMoves(board);
                    break;

                case DebugType.QueenMoves:
                    DrawQueenMoves(board);
                    break;

                case DebugType.KingMoves:
                    DrawKingMoves(board);
                    break;
            }
        }

        void DrawEmptySquares(Board board) 
        {
            MoveGenerator generator = new MoveGenerator(board);

            foreach (FileRank move in generator.GenerateEmptySquares())
            {
                SetSquareColor(move.File, move.Rank, m_BoardPrefab.LightSquares.Debug, m_BoardPrefab.DarkSquares.Debug);
            }
        }

        void DrawFriendlySquares(Board board)
        {
            MoveGenerator generator = new MoveGenerator(board);

            foreach (FileRank move in generator.GenerateFriendlySquares())
            {
                SetSquareColor(move.File, move.Rank, m_BoardPrefab.LightSquares.Debug, m_BoardPrefab.DarkSquares.Debug);
            }
        }

        void DrawOpponentSquares(Board board)
        {
            MoveGenerator generator = new MoveGenerator(board);
            generator.GenerateMovesFor(m_PieceColor);

            foreach (FileRank move in generator.GenerateOpponentSquares())
            {
                SetSquareColor(move.File, move.Rank, m_BoardPrefab.LightSquares.Debug, m_BoardPrefab.DarkSquares.Debug);
            }
        }
    
        void DrawAttackingSquares(Board board)
        {
            MoveGenerator generator = new MoveGenerator(board);
            MoveList moves = generator.GenerateMovesFor(m_PieceColor);

            foreach (Move move in moves.m_AttackingSquares)
            {
                SetSquareColor(move.FromFile, move.FromRank, m_BoardPrefab.LightSquares.Selected, m_BoardPrefab.DarkSquares.Selected);
                SetSquareColor(move.ToFile, move.ToRank, m_BoardPrefab.LightSquares.Debug, m_BoardPrefab.DarkSquares.Debug);
            }
        }

        void DrawPawnMoves(Board board)
        {
            MoveGenerator generator = new MoveGenerator(board);
            MoveList moves = generator.GenerateMovesFor(m_PieceColor);

            foreach (Move move in moves.m_PawnMoves)
            {
                SetSquareColor(move.FromFile, move.FromRank, m_BoardPrefab.LightSquares.Selected, m_BoardPrefab.DarkSquares.Selected);
                SetSquareColor(move.ToFile, move.ToRank, m_BoardPrefab.LightSquares.Legal, m_BoardPrefab.DarkSquares.Legal);
            } 
        }

        void DrawBishopMoves(Board board)
        {
            MoveGenerator generator = new MoveGenerator(board);
            MoveList moves = generator.GenerateMovesFor(m_PieceColor);

            foreach (Move move in moves.m_BishopMoves)
            {
                SetSquareColor(move.FromFile, move.FromRank, m_BoardPrefab.LightSquares.Selected, m_BoardPrefab.DarkSquares.Selected);
                SetSquareColor(move.ToFile, move.ToRank, m_BoardPrefab.LightSquares.Legal, m_BoardPrefab.DarkSquares.Legal);
            } 
        }

        void DrawKnightMoves(Board board)
        {
            MoveGenerator generator = new MoveGenerator(board);
            MoveList moves = generator.GenerateMovesFor(m_PieceColor);

            foreach (Move move in moves.m_KnightMoves)
            {
                SetSquareColor(move.FromFile, move.FromRank, m_BoardPrefab.LightSquares.Selected, m_BoardPrefab.DarkSquares.Selected);
                SetSquareColor(move.ToFile, move.ToRank, m_BoardPrefab.LightSquares.Legal, m_BoardPrefab.DarkSquares.Legal);
            } 
        }

        void DrawRookMoves(Board board)
        {
            MoveGenerator generator = new MoveGenerator(board);
            MoveList moves = generator.GenerateMovesFor(m_PieceColor);

            foreach (Move move in moves.m_RookMoves)
            {
                SetSquareColor(move.FromFile, move.FromRank, m_BoardPrefab.LightSquares.Selected, m_BoardPrefab.DarkSquares.Selected);
                SetSquareColor(move.ToFile, move.ToRank, m_BoardPrefab.LightSquares.Legal, m_BoardPrefab.DarkSquares.Legal);
            } 
        }

        void DrawQueenMoves(Board board)
        {
            MoveGenerator generator = new MoveGenerator(board);
            MoveList moves = generator.GenerateMovesFor(m_PieceColor);

            foreach (Move move in moves.m_QueenMoves)
            {
                SetSquareColor(move.FromFile, move.FromRank, m_BoardPrefab.LightSquares.Selected, m_BoardPrefab.DarkSquares.Selected);
                SetSquareColor(move.ToFile, move.ToRank, m_BoardPrefab.LightSquares.Legal, m_BoardPrefab.DarkSquares.Legal);
            } 
        }

        void DrawKingMoves(Board board)
        {
            MoveGenerator generator = new MoveGenerator(board);
            MoveList moves = generator.GenerateMovesFor(m_PieceColor);

            foreach (Move move in moves.m_KingMoves)
            {
                SetSquareColor(move.FromFile, move.FromRank, m_BoardPrefab.LightSquares.Selected, m_BoardPrefab.DarkSquares.Selected);
                SetSquareColor(move.ToFile, move.ToRank, m_BoardPrefab.LightSquares.Legal, m_BoardPrefab.DarkSquares.Legal);
            } 
        }
    }
}
