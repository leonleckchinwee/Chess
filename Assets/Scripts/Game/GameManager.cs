using System.Collections.Generic;
using System.Linq;
using System;

using UnityEngine;
using Chess.UI;

namespace Chess.Game
{
    public class GameManager : MonoBehaviour
    {
        enum PlayerType
        {
            Human, Computer
        }

        public bool m_WhiteIsBottom;
        public bool m_AllowDebugDrawing;

        [SerializeField]
        ChessBoard  m_ChessBoardUI;

        [SerializeField]
        PlayerType  m_PlayerOne;

        [SerializeField]
        PlayerType  m_PlayerTwo;        

        [SerializeField]
        string      m_Fen;  // Starting fen string

        Board       m_CurrentBoard;
        Audio       m_Audio;

        Player      m_WhitePlayer;
        Player      m_BlackPlayer;

        int         m_ColorToMove;

        Action<ChessBoard.DebugType, Board> OnDebugDraw;

        void Awake()
        {
            //PrecomputedBits.Initialize();

            m_Audio = gameObject.GetComponentInChildren<Audio>();

            m_CurrentBoard = new Board();

            if (m_Fen == "")
                m_CurrentBoard.InitializeDefaultStartingPosition();
            else
                m_CurrentBoard.InitializePosition(m_Fen);

            m_ColorToMove = m_CurrentBoard.m_CurrentColorTurn;

            OnDebugDraw += m_ChessBoardUI.UpdateDebug;

            m_ChessBoardUI.UpdateBoard(m_CurrentBoard);
        }

        void Start()
        {
            CreatePlayers();
        }

        void Update()
        {
            if (m_AllowDebugDrawing)
                DebugControls();

            if (Piece.IsWhite(m_ColorToMove))
                m_WhitePlayer.Update();
            else
                m_BlackPlayer.Update();
        }

        void OnPieceSelected(Move move)
        {
            // MoveGenerator generator = new MoveGenerator(m_CurrentBoard);
            //m_CurrentLegalMoves = generator.GeneratePseudoLegalMovesFor(m_ColorToMove);

            // // TODO: Naive approach...
            // int piece = m_CurrentBoard.GetPieceAt(move.FromFileRank);
            // List<Move> moves;

            // switch (Piece.PieceType(piece))
            // {
            //     case Piece.Pawn:
            //         moves = m_CurrentLegalMoves.m_PawnMoves;
            //         break;

            //     case Piece.Knight:
            //         moves = m_CurrentLegalMoves.m_KnightMoves;
            //         break;

            //     case Piece.Bishop:
            //         moves = m_CurrentLegalMoves.m_BishopMoves;
            //         break;

            //     case Piece.Rook:
            //         moves = m_CurrentLegalMoves.m_RookMoves;
            //         break;

            //     case Piece.Queen:
            //         moves = m_CurrentLegalMoves.m_QueenMoves;
            //         break;

            //     case Piece.King:
            //         moves = m_CurrentLegalMoves.m_KingMoves;
            //         break;

            //     default:
            //     case Piece.None:
            //         return;
            // }

            // foreach (Move m in moves)
            // {
            //     m_ChessBoardUI.SelectSquare(m.ToFileRank);
            // }
        }

        void UpdateOnMoveSelected(Move move)
        {
            // if (m_CurrentLegalMoves.m_AttackingSquares.Contains(move))
            // {
            //     int piece = m_CurrentBoard.GetPieceAt(move.FromFileRank);

            //     m_CurrentBoard.PlacePieceAt(move.ToFileRank, piece);
            //     m_CurrentBoard.RemovePieceAt(move.FromFileRank);

            //     OnTurnSwitch();

            //     m_Audio.PlayPlacementSfx();  // TODO: Capture piece sound!
            // }

            m_ChessBoardUI.UpdateBoard(m_CurrentBoard);
        }

        void OnTurnSwitch()
        {
            m_ColorToMove = Piece.OpponentColor(m_ColorToMove);
            m_CurrentBoard.m_CurrentColorTurn = m_ColorToMove;
        }

        void CreatePlayers()
        {
            // Unsubscribe if not null
            if (m_WhitePlayer != null)
            {
                m_WhitePlayer.OnMoveSelected    -= OnPieceSelected;
                m_WhitePlayer.OnPiecePlace      -= UpdateOnMoveSelected;
            }
            if (m_BlackPlayer != null)
            {
                m_BlackPlayer.OnMoveSelected    -= OnPieceSelected;
                m_BlackPlayer.OnPiecePlace      -= UpdateOnMoveSelected;
            }

            m_WhitePlayer = (m_PlayerOne == PlayerType.Human) ? new HumanPlayer(m_CurrentBoard, Piece.White) : new ComputerPlayer(m_CurrentBoard);
            m_BlackPlayer = (m_PlayerTwo == PlayerType.Human) ? new HumanPlayer(m_CurrentBoard, Piece.Black) : new ComputerPlayer(m_CurrentBoard);

            // Subscribe to update only when needed
            m_WhitePlayer.OnMoveSelected    += OnPieceSelected;         // On move selected but not placed
            m_BlackPlayer.OnMoveSelected    += OnPieceSelected;         // ..
            m_WhitePlayer.OnPiecePlace      += UpdateOnMoveSelected;    // On piece placed (check if legal)
            m_BlackPlayer.OnPiecePlace      += UpdateOnMoveSelected;    // ..
        }

        void DebugControls()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
                UpdateDebugDrawing(ChessBoard.DebugType.None);

            if (Input.GetKeyDown(KeyCode.Alpha0))
                UpdateDebugDrawing(ChessBoard.DebugType.AttackingSquares);

            if (Input.GetKeyDown(KeyCode.Alpha1))
                UpdateDebugDrawing(ChessBoard.DebugType.EmptySquares);

            if (Input.GetKeyDown(KeyCode.Alpha2))
                UpdateDebugDrawing(ChessBoard.DebugType.FriendlySquares);

            if (Input.GetKeyDown(KeyCode.Alpha3))
                UpdateDebugDrawing(ChessBoard.DebugType.OpponentSquares);
            
            if (Input.GetKeyDown(KeyCode.Alpha4))
                UpdateDebugDrawing(ChessBoard.DebugType.PawnMoves);

            if (Input.GetKeyDown(KeyCode.Alpha5))
                UpdateDebugDrawing(ChessBoard.DebugType.BishopMoves);

            if (Input.GetKeyDown(KeyCode.Alpha6))
                UpdateDebugDrawing(ChessBoard.DebugType.KnightMoves);

            if (Input.GetKeyDown(KeyCode.Alpha7))
                UpdateDebugDrawing(ChessBoard.DebugType.RookMoves);

            if (Input.GetKeyDown(KeyCode.Alpha8))
                UpdateDebugDrawing(ChessBoard.DebugType.QueenMoves);

            if (Input.GetKeyDown(KeyCode.Alpha9))
                UpdateDebugDrawing(ChessBoard.DebugType.KingMoves);
        }

        void UpdateDebugDrawing(ChessBoard.DebugType debugType)
        {
            OnDebugDraw?.Invoke(debugType, m_CurrentBoard);
        }
    }
}