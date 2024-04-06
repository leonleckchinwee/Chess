using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public static class FEN
    {
        static Dictionary<char, int> pieceTypes = new Dictionary<char, int>()
        {
            ['p'] = Piece.Pawn, ['n'] = Piece.Knight, ['b'] = Piece.Bishop,
            ['r'] = Piece.Rook, ['q'] = Piece.Queen, ['k'] = Piece.King
        };

        public const string startingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

        public static LoadedInfo LoadPositionFromFEN(string fen)
        {
            LoadedInfo position = new LoadedInfo();

            string[] sections = fen.Split (' ');

			int file = 0;
			int rank = 7;

			foreach (char symbol in sections[0]) {
				if (symbol == '/') {
					file = 0;
					rank--;
				} else {
					if (char.IsDigit (symbol)) {
						file += (int) char.GetNumericValue (symbol);
					} else {
						int pieceColour = (char.IsUpper (symbol)) ? Piece.White : Piece.Black;
						int pieceType = pieceTypes[char.ToLower (symbol)];
						position.Squares[rank * 8 + file] = pieceType | pieceColour;
						file++;
					}
				}
			}

            return position;
        }

        public class LoadedInfo
        {
            public int[] Squares;

            // public bool WhiteCastleKingside;
            // public bool WhiteCastleQueenside;

            // public bool BlackCastleKingside;
            // public bool BlackCastleQueenside;

            // public bool WhiteToMove;

            public LoadedInfo()
            {
                Squares = new int[64];
            }
        }
    }
}