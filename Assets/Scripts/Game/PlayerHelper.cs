using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;

namespace Chess.Game
{
    public abstract class PlayerHelper
    {
        public event Action<Move> OnMoveChosen;

        public abstract void Update();
        public abstract void NotifyPlayerTurn();

        public virtual void MoveChosen(Move move)
        {
            OnMoveChosen?.Invoke(move);
        }
    }

    //public class Player
    //{
    //     public enum PlayerState
    //     {
    //         None, IsDragging
    //     }

    //     public event EventHandler OnMoveChosen;

    //     PlayerState     m_CurrentState;

    //     bool            m_IsAI;
    //     bool            m_IsCurrentlySelecting;

    //     Coordinates     m_CurrentSelectedSquare;
    //     Board           m_Board;

    //     Camera          m_Camera;
    //     ChessBoardUI    m_ChessBoardUI;

    //     public Player(Board board, bool isAI = false)
    //     {
    //         m_Board = board;
    //         m_IsAI = isAI;

    //         m_Camera = Camera.main;
    //         m_ChessBoardUI = GameObject.FindObjectOfType<ChessBoardUI>();

    //         m_CurrentState = PlayerState.None;
    //     }

    //     public void Update()
    //     {
    //         if (m_IsAI)
    //         {
    //             // TODO...
    //         }

    //         Vector2 mousePosition = m_Camera.ScreenToWorldPoint(Input.mousePosition);
    //         switch (m_CurrentState)
    //         {
    //             case PlayerState.IsDragging:
    //                 DragPiece(mousePosition);
    //                 break;

    //             default:
    //             case PlayerState.None:
    //                 SelectPiece(mousePosition);
    //                 break;
    //         }

    //         // Cancel current selection if right mouse clicked
    //         if (m_IsCurrentlySelecting && Input.GetMouseButtonDown(1))
    //             CancelCurrentSelection(mousePosition);
    //     }

    //     void DragPiece(Vector2 position)
    //     {
    //         m_ChessBoardUI.DragPiece(m_CurrentSelectedSquare, position);

    //         if (Input.GetMouseButtonDown(0))
    //             PlacePiece(position);
    //     }

    //     void PlacePiece(Vector2 position)
    //     {
    //         if (m_ChessBoardUI.IsSquareUnderMouse(position, out Coordinates targetSquare))
    //         {
    //             if (targetSquare.Equals(m_CurrentSelectedSquare))
    //             {
    //                 CancelCurrentSelection(position);
    //                 return;
    //             }

    //             int pieceIndex = BoardInfo.IndexFromCoordinates(m_CurrentSelectedSquare);
    //             int piece = Piece.PieceType(pieceIndex);

    //             int targetIndex = BoardInfo.IndexFromCoordinates(targetSquare);

    //             Move move = new Move(pieceIndex, targetIndex);
    //             move.m_PieceType = piece;

    //             m_Board.MakeMove(move);

    //             m_CurrentState = PlayerState.None;
    //             m_IsCurrentlySelecting = false;
    //         }
    //     }

    //     void SelectPiece(Vector2 position)
    //     {
    //         if (Input.GetMouseButtonDown(0))
    //         {
    //             if (m_ChessBoardUI.IsSquareUnderMouse(position, out m_CurrentSelectedSquare))
    //             {
    //                 int index = BoardInfo.IndexFromCoordinates(m_CurrentSelectedSquare);

    //                 if (Piece.IsColor(m_Board.Squares[index], m_Board.ColorToMove))
    //                 {
    //                     m_ChessBoardUI.SelectSquare(m_CurrentSelectedSquare);

    //                     m_CurrentState = PlayerState.IsDragging;
    //                     m_IsCurrentlySelecting = true;
    //                 }
    //             }
    //         }
    //     }

    //     void CancelCurrentSelection(Vector2 position)
    //     {
    //         // Only able to cancel if current is clicked or dragging
    //         if (m_CurrentState == PlayerState.None)
    //             return;

    //         // Deselect and reset
    //         m_ChessBoardUI.ResetPiecePosition(m_CurrentSelectedSquare);
    //         m_ChessBoardUI.CancelSelection(m_CurrentSelectedSquare);

    //         m_CurrentState = PlayerState.None;
    //         m_IsCurrentlySelecting = false;
    //     }
    // }

    
        
    //     void HandlePieceDragging(Vector2 position)
    //     {
    //         boardUI.DragPiece(selectedCoordinates, position);

    //         if (Input.GetMouseButtonDown(0))
    //             HandlePiecePlacement(position);
    //     }
}
