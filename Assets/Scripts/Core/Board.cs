using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public class Board
    {
        public const int White = 0;
        public const int Black = 1;

        public int[] Squares;

        public int ColorToMove;

        void Initialize()
        {
            Squares = new int[64];
        }

        public void LoadDefaultPosition()
        {
            Initialize();

            var loadedPosition = FEN.LoadPositionFromFEN(FEN.startingFEN);

            for (int index = 0; index < 64; ++index)
            {
                int piece = loadedPosition.Squares[index];
                Squares[index] = piece;
            }

            ColorToMove = Piece.White;
        }

        public void LoadPosition(string position)
        {
            Initialize();

            var loadedPosition = FEN.LoadPositionFromFEN(position);

            for (int index = 0; index < 64; ++index)
            {
                int piece = loadedPosition.Squares[index];
                Squares[index] = piece;
            }

            ColorToMove = Piece.White;
        }

        public bool TryMakeMove(Move move)
        {
            int targetPiece = Squares[move.m_ToSquare];

            if (Piece.IsColor(targetPiece, ColorToMove))
                return false;

            (Squares[move.m_ToSquare], Squares[move.m_FromSquare]) = (Squares[move.m_FromSquare], 0);
            return true;
        }

        public bool MakeMove(Move move)
        {
            return TryMakeMove(move);
        }

        public void ChangeTurn()
        {
            if (Piece.IsWhite(ColorToMove))
                ColorToMove = Piece.Black;
            else
                ColorToMove = Piece.White;
        }
    }
}