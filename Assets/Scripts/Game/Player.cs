using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Game
{
    public class Player : PlayerHelper
    {
        public enum PlayerState
        {
            None, IsDragging
        }

        PlayerState     m_CurrentState = PlayerState.None;

        bool            m_IsAiPlaying;
        bool            m_IsCurrentlySelecting;

        Coordinates     m_CurrentSelectedSquare;
        Board           m_Board;

        Camera          m_Camera;
        ChessBoardUI    m_ChessBoardUI;

        int             m_PlayerColor;
        bool            m_IsPlayerTurn = false;

        public Player(Board board, int color, bool isAI = false)
        {
            m_Board = board;
            m_PlayerColor = color;
            m_IsAiPlaying = isAI;

            m_Camera = Camera.main;
            m_ChessBoardUI = GameObject.FindObjectOfType<ChessBoardUI>();

            m_CurrentState = PlayerState.None;

            if (Piece.IsWhite(color))
                m_IsPlayerTurn = true;
        }

        public override void Update()
        {
            if (!m_IsPlayerTurn)
                return;

            if (m_IsAiPlaying)
            {
                // TODO...
            }

            Vector2 mousePosition = m_Camera.ScreenToWorldPoint(Input.mousePosition);
            switch (m_CurrentState)
            {
                case PlayerState.IsDragging:
                    DragPiece(mousePosition);
                    break;

                default:
                case PlayerState.None:
                    SelectPiece(mousePosition);
                    break;
            }

            // Cancel current selection if right mouse clicked
            if (m_IsCurrentlySelecting && Input.GetMouseButtonDown(1))
                CancelCurrentSelection(mousePosition);
        }

        void DragPiece(Vector2 position)
        {
            m_ChessBoardUI.DragPiece(m_CurrentSelectedSquare, position);

            if (Input.GetMouseButtonDown(0))
                PlacePiece(position);
        }

        void PlacePiece(Vector2 position)
        {
            if (m_ChessBoardUI.IsSquareUnderMouse(position, out Coordinates targetSquare))
            {
                if (targetSquare.Equals(m_CurrentSelectedSquare))
                {
                    CancelCurrentSelection(position);
                    return;
                }

                int fromIndex = BoardInfo.IndexFromCoordinates(m_CurrentSelectedSquare);
                int targetIndex = BoardInfo.IndexFromCoordinates(targetSquare);

                Move move = new Move()
                {
                    m_FromSquare = fromIndex, m_ToSquare = targetIndex, m_IsLegalMove = true    // TODO...
                };

                MoveChosen(move);

                m_ChessBoardUI.CancelSelection(m_CurrentSelectedSquare);

                m_CurrentState = PlayerState.None;
                m_IsCurrentlySelecting = false;
            }
        }

        void SelectPiece(Vector2 position)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (m_ChessBoardUI.IsSquareUnderMouse(position, out m_CurrentSelectedSquare))
                {
                    int index = BoardInfo.IndexFromCoordinates(m_CurrentSelectedSquare);

                    if (Piece.IsColor(m_Board.Squares[index], m_Board.ColorToMove))
                    {
                        m_ChessBoardUI.SelectSquare(m_CurrentSelectedSquare);

                        m_CurrentState = PlayerState.IsDragging;
                        m_IsCurrentlySelecting = true;
                    }
                }
            }
        }

        void CancelCurrentSelection(Vector2 position)
        {
            // Only able to cancel if current is clicked or dragging
            if (m_CurrentState == PlayerState.None)
                return;

            // Deselect and reset
            m_ChessBoardUI.ResetPiecePosition(m_CurrentSelectedSquare);
            m_ChessBoardUI.CancelSelection(m_CurrentSelectedSquare);

            m_CurrentState = PlayerState.None;
            m_IsCurrentlySelecting = false;
        }

        public override void NotifyPlayerTurn()
        {
            if (Piece.IsColor(m_PlayerColor, m_Board.ColorToMove))
                m_IsPlayerTurn = true;
        }
    }
}
