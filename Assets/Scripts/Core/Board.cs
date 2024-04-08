using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public class Board
    {
        public int[] m_Squares;

        public Board()
        {
            m_Squares = new int[64];
        }

        public void LoadPositionFromFEN (string fen)
        {
            FEN.LoadPositionFromFEN(fen);
        }

        public void LoadDefaultStartingPosition ()
        {
            FEN.PositionInfo positions = FEN.LoadPositionFromFEN (FEN.m_StartingFEN);

            for (int index = 0; index < 64; ++index)
            {
                int piece = positions.m_Squares[index];
                m_Squares[index] = piece;
            }
        }
    }
}