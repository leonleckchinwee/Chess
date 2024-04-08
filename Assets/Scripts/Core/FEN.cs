using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public static class FEN
    {
        static Dictionary<char, int> m_PieceTypeMap = new Dictionary<char, int>()
        {
            ['p'] = Piece.Pawn, ['n'] = Piece.Knight, ['b'] = Piece.Bishop,
            ['r'] = Piece.Rook, ['q'] = Piece.Queen,  ['k'] = Piece.King
        };

        public const string m_StartingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

        public static PositionInfo LoadPositionFromFEN (string fen)
        {
            PositionInfo loadedPosition = new PositionInfo ();

            // Starts from top left
            int file = 0;
            int rank = 7;

            foreach (char symbol in fen)
            {
                if (symbol == '/')
                {
                    file = 0;
                    --rank;

                    continue;
                }

                if (char.IsDigit (symbol))
                {
                    file += (int)char.GetNumericValue (symbol);
                }
                else
                {
                    int color = char.IsUpper (symbol) ? Piece.White : Piece.Black;
                    int type = m_PieceTypeMap[char.ToLower (symbol)];

                    loadedPosition.m_Squares[rank * 8 + file] = type | color;
                    ++file;
                }
            }   

            return loadedPosition;
        }

        public class PositionInfo
        {
            public int[] m_Squares;

            public PositionInfo ()
            {
                m_Squares = new int[64];
            }
        }
    }
}