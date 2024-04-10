using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chess.UI;
using UnityEngine;

namespace Chess.Game
{
    public class HumanPlayer : Player
    {
        enum PlayerAction
        {
            Idling, Dragging
        }

        Board           m_Board;
        Camera          m_Camera;
        ChessBoard      m_UI;

        PlayerAction    m_Action;
        FileRank        m_CurrentSelectedPosition;

        int             m_PlayerColor;
        bool            m_CurrentlySelecting;
        bool            m_SelectOnlyMode;

        public HumanPlayer(Board board, int color)
        {
            m_Board     = board;
            m_Camera    = Camera.main;
            m_UI        = GameObject.FindObjectOfType<ChessBoard>();
            
            m_PlayerColor        = color;
            m_Action             = PlayerAction.Idling;
            m_CurrentlySelecting = false;
            m_SelectOnlyMode     = false;
        }

        public override void Update()
        {
            Vector2 mousePosition = m_Camera.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetKeyDown(KeyCode.LeftShift))
                m_SelectOnlyMode = true;

            switch (m_Action)
            {
                case PlayerAction.Dragging:
                    PickUpPiece(mousePosition);
                    break;

                default:
                case PlayerAction.Idling:
                    SelectPiece(mousePosition);
                    break;
            }

            if (m_CurrentlySelecting && Input.GetMouseButtonDown(1))
                CancelSelections();

            m_SelectOnlyMode = false;
        }

        void SelectPiece(Vector2 mousePos)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (m_UI.IsValidSquare(mousePos, out m_CurrentSelectedPosition))
                {
                    if (Piece.IsSameColor(m_Board.GetPieceAt(m_CurrentSelectedPosition), m_PlayerColor))
                    {
                        m_UI.SelectSquare(m_CurrentSelectedPosition);
                        
                        if (!m_SelectOnlyMode)
                        {
                            OnMoveSelect(new Move(m_CurrentSelectedPosition, m_CurrentSelectedPosition));
                            m_Action = PlayerAction.Dragging;
                        }
                    }
                    else
                    {
                        m_UI.SelectSquare(m_CurrentSelectedPosition);
                    }

                    m_CurrentlySelecting = true;
                }
            }
        }

        void PickUpPiece(Vector2 mousePos)
        {
            m_UI.DragPiece(m_CurrentSelectedPosition, mousePos);

            if (Input.GetMouseButtonDown(0))
            {
                if (m_UI.IsValidSquare(mousePos, out FileRank target))
                {
                    if (m_CurrentSelectedPosition.Equals(target))
                        return;
                        
                    OnTryPlacePiece(new Move(m_CurrentSelectedPosition, target));
                    CancelSelections();
                }
            }

            m_SelectOnlyMode = false;
        }

        void CancelSelections()
        {
            m_UI.ResetAllSquareColor();
            m_UI.ResetPosition(m_CurrentSelectedPosition);

            m_Action = PlayerAction.Idling;
            m_CurrentlySelecting = false;
        }
    }
}