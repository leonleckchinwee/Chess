using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess.UI;

namespace Chess.Game
{
    public class GameManager : MonoBehaviour
    {
        public bool m_WhiteIsBottom;

        [SerializeField]
        ChessBoard  m_ChessBoardUI;

        Board       m_CurrentBoard;

        void Awake()
        {
            m_CurrentBoard = new Board();
            m_CurrentBoard.InitializeDefaultStartingPosition();
        }

        void Start()
        {
            m_ChessBoardUI.UpdateBoard(m_CurrentBoard);
        }

        void Update()
        {

        }
    }
}