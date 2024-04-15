using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public struct MoveList
    {
        public int FriendlyColor;

        public List<Move> Captures;
        public List<Move> PawnMoves;
        public List<Move> KnightMoves;
        public List<Move> BishopMoves;
        public List<Move> RookMoves;
        public List<Move> QueenMoves;
        public List<Move> KingMoves;

        public static MoveList None => new MoveList(Piece.None);

        public MoveList(int color)
        {
            FriendlyColor = color;

            Captures    = new List<Move>();
            PawnMoves   = new List<Move>();
            KnightMoves = new List<Move>();
            BishopMoves = new List<Move>();
            RookMoves   = new List<Move>();
            QueenMoves  = new List<Move>();
            KingMoves   = new List<Move>();
        }
    }
}