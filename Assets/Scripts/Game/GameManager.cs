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

        MoveGenerator   m_Generator;
        Board           m_CurrentBoard;
        Audio           m_Audio;

        Player          m_WhitePlayer;
        Player          m_BlackPlayer;

        int             m_ColorToMove;
        List<Move>      m_CurrentMoves;

        Action<ChessBoard.DebugType, Board> OnDebugDraw;

        void Awake()
        {
            PrecomputedBits.Initialize();

            m_Audio = gameObject.GetComponentInChildren<Audio>();

            m_CurrentBoard = new Board();

            // Initialize starting board
            if (m_Fen == "")
                m_CurrentBoard.InitializeDefaultStartingPosition();
            else
                m_CurrentBoard.InitializePosition(m_Fen);

            m_ColorToMove = m_CurrentBoard.m_CurrentColorTurn;  // Starting color

            m_Generator = new MoveGenerator(m_CurrentBoard);    // Move generator

            if (m_AllowDebugDrawing)    // Debug drawing event
                OnDebugDraw += m_ChessBoardUI.UpdateDebug;
        }

        void Start()
        {
            CreatePlayers();
            
            m_ChessBoardUI.UpdateBoard(m_CurrentBoard);
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
            MoveList selectedMoves = m_Generator.GeneratePseudolegalMoves(BoardInfo.FileRankToPosition(move.FromFileRank));
            int selectedPiece      = m_CurrentBoard.GetPieceAt(move.FromFileRank);

            switch (Piece.PieceType(selectedPiece))
            {
                case Piece.Pawn:
                    m_CurrentMoves = selectedMoves.PawnMoves;
                    break;

                case Piece.Knight:
                    m_CurrentMoves = selectedMoves.KnightMoves;
                    break;

                case Piece.Bishop:
                    m_CurrentMoves = selectedMoves.BishopMoves;
                    break;

                case Piece.Rook:
                    m_CurrentMoves = selectedMoves.RookMoves;
                    break;

                case Piece.Queen:
                    m_CurrentMoves = selectedMoves.QueenMoves;
                    break;

                case Piece.King:
                    m_CurrentMoves = selectedMoves.KingMoves;
                    break;

                default:
                    m_CurrentMoves = new List<Move>();
                    break;
            }

            foreach (Move square in m_CurrentMoves)
            {
                m_ChessBoardUI.HighlightLegalMoves(square.ToFileRank);
            }
        }

        void UpdateOnMoveSelected(Move move)
        {
            if (m_CurrentMoves.Contains(move))
            {
                m_CurrentBoard.MakeMove(move);
                
                m_Generator.Update(m_CurrentBoard);
                
                m_Audio.PlayPlacementSfx();

                OnTurnSwitch();
            }

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