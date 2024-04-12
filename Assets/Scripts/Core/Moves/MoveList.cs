using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public struct MoveList
    {
        public readonly int         m_FriendlyColor;    // Friendly color
        public readonly int         m_OpponentColor;    // Opponent color

        public readonly List<Move>  m_AttackingSquares; // All squares currently under attack by friendly pieces
        public readonly List<Move>  m_PawnMoves;        // All friendly pawn moves
        public readonly List<Move>  m_BishopMoves;      // All friendly bishop moves
        public readonly List<Move>  m_KnightMoves;      // All friendly knight moves
        public readonly List<Move>  m_RookMoves;        // All friendly rook moves
        public readonly List<Move>  m_QueenMoves;       // All friendly queen moves
        public readonly List<Move>  m_KingMoves;        // All friendly king moves

        public MoveList(int friendlyColor)
        {
            m_FriendlyColor = friendlyColor;
            m_OpponentColor = Piece.OpponentColor(friendlyColor);

            m_AttackingSquares  = new List<Move>();
            m_PawnMoves         = new List<Move>();
            m_BishopMoves       = new List<Move>();
            m_KnightMoves       = new List<Move>();
            m_RookMoves         = new List<Move>();
            m_QueenMoves        = new List<Move>();
            m_KingMoves         = new List<Move>();
        }

        public void Clear()
        {
            m_AttackingSquares.Clear();
            m_PawnMoves.Clear();
            m_BishopMoves.Clear();
            m_RookMoves.Clear();
            m_QueenMoves.Clear();
            m_KingMoves.Clear();
        }
    }
}