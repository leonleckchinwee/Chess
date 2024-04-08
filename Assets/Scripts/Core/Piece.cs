using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public static class Piece
    {
        public const int Pawn      = 0b00001;
        public const int Knight    = 0b00010;
        public const int Bishop    = 0b00011;
        public const int Rook      = 0b00100;
        public const int Queen     = 0b00101;
        public const int King      = 0b00110;
 
        public const int White     = 0b01000;
        public const int Black     = 0b10000;

        // Extract piece type from index
        public static int PieceType (int piece)
        {
            return piece & 0b00111;
        }

        // Extract piece color from index
        public static int PieceColor (int piece)
        {
            return piece & 0b11000;
        }

        public static bool IsPawn (int piece)
        {
            return (piece & Pawn) == Pawn;
        }

        public static bool IsKnight (int piece)
        {
            return (piece & Knight) == Knight;
        }

        public static bool IsBishop (int piece)
        {
            return (piece & Bishop) == Bishop;
        }

        public static bool IsRook (int piece)
        {
            return (piece & Rook) == Rook;
        }

        public static bool IsQueen (int piece)
        {
            return (piece & Queen) == Queen;
        }

        public static bool IsKing (int piece)
        {
            return (piece & King) == King;
        }

        public static bool IsWhite (int piece)
        {
            return (piece & White) == White;
        }

        public static bool IsBlack (int piece)
        {
            return (piece & Black) == Black;
        }

        public static bool IsSlidingPiece (int piece)
        {
            return IsBishop (piece) || IsRook (piece) || IsQueen (piece);
        }

        public static bool IsSameColor (int piece, int color)
        {
            return (piece & 0b11000) == color;
        }

        public static string GetPieceTypeName (int piece)
        {
            int type = PieceType (piece);

            switch (type)
            {
                case Pawn:
                    return "Pawn";
                case Knight:
                    return "Knight";
                case Bishop:
                    return "Bishop";
                case Rook:
                    return "Rook";
                case Queen:
                    return "Queen";
                case King:
                    return "King";
                default:
                    return "None";
            }
        }

        public static string GetPieceColorName (int piece)
        {
            return IsWhite (piece) ? "White" : "Black";
        }
    }
}