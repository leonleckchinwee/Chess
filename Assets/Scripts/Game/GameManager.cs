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
        PlayerType m_PlayerOne;

        [SerializeField]
        PlayerType m_PlayerTwo;

        Board       m_CurrentBoard;

        Player      m_WhitePlayer;
        Player      m_BlackPlayer;

        int         m_ColorToMove;
        List<Move>  m_CurrentLegalMoves;

        Action<ChessBoard.DebugType, Board> OnDebugDraw;

        void Awake()
        {
            string testFEN = "2q1rr1k/3bbnnp/p2p1pp1/2pPp3/PpP1P1P1/1P2BNNP/2BQ1PRK/7R w - - bm f5";

            m_CurrentBoard = new Board();
            m_CurrentBoard.InitializeDefaultStartingPosition();
            //m_CurrentBoard.InitializePosition(testFEN);

            m_ChessBoardUI.UpdateBoard(m_CurrentBoard);

            m_ColorToMove = m_CurrentBoard.m_CurrentColorTurn;

            OnDebugDraw += m_ChessBoardUI.UpdateDebug;
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
            MoveGenerator generator = new MoveGenerator(m_CurrentBoard);
            m_CurrentLegalMoves = generator.GenerateLegalMovesAt(move.FromFileRank);

            foreach (Move m in m_CurrentLegalMoves)
            {   
                m_ChessBoardUI.SelectSquare(m.ToFileRank);
            } 
        }

        void UpdateOnMoveSelected(Move move)
        {
            if (m_CurrentLegalMoves.Contains(move))
            {
                int piece = m_CurrentBoard.GetPieceAt(move.FromFileRank);

                m_CurrentBoard.PlacePieceAt(move.ToFileRank, piece);
                m_CurrentBoard.RemovePieceAt(move.FromFileRank);
            }

            m_ChessBoardUI.UpdateBoard(m_CurrentBoard);
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
            m_WhitePlayer.OnMoveSelected    += OnPieceSelected;
            m_WhitePlayer.OnPiecePlace      += UpdateOnMoveSelected;
            m_BlackPlayer.OnMoveSelected    += OnPieceSelected;
            m_BlackPlayer.OnPiecePlace      += UpdateOnMoveSelected;
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