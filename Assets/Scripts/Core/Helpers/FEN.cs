using System.Collections.Generic;

namespace Chess
{
    public static class FEN
    {
        static Dictionary<char, int> m_PieceTypeMap = new Dictionary<char, int>()
        {
            ['p'] = Piece.Pawn, ['n'] = Piece.Knight, ['b'] = Piece.Bishop,
            ['r'] = Piece.Rook, ['q'] = Piece.Queen,  ['k'] = Piece.King
        };

        public const string m_StartingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public static PositionInfo LoadPositionFromFEN(string fen)
        {
            PositionInfo loadedPosition = new PositionInfo();
            string[] splitString = fen.Split(' ');
            int fields = splitString.Length;

            loadedPosition.m_WhiteTurnToMove = true;    // Default white to move if not specified

            // Starts from top left
            int file = 0;
            int rank = 7;

            // Piece positions
            foreach (char symbol in splitString[0])
            {
                if (symbol == '/')
                {
                    file = 0;
                    --rank;

                    continue;
                }

                if (char.IsDigit(symbol))
                {
                    file += (int)char.GetNumericValue(symbol);
                }
                else
                {
                    int color = char.IsUpper(symbol) ? Piece.White : Piece.Black;
                    int type = m_PieceTypeMap[char.ToLower(symbol)];

                    loadedPosition.m_Squares[file, rank] = Piece.MakePiece(type, color);
                    ++file;
                }
            }   

            // Player turn ot move
            if (fields > 1)
            {
                loadedPosition.m_WhiteTurnToMove = splitString[1] == "w";
            }

            // Castling rights
            if (fields > 2)
            {
                // White side
                if (splitString[2].Contains('K') && splitString[2].Contains('Q'))
                    loadedPosition.m_WhiteCastleSide = PositionInfo.CastleSide.Both;
                else if (splitString[2].Contains('K'))
                    loadedPosition.m_WhiteCastleSide = PositionInfo.CastleSide.King;
                else if (splitString[2].Contains('Q'))
                    loadedPosition.m_WhiteCastleSide = PositionInfo.CastleSide.Queen;
                else
                    loadedPosition.m_WhiteCastleSide = PositionInfo.CastleSide.None;

                // Black side
                if (splitString[2].Contains('K') && splitString[2].Contains('Q'))
                    loadedPosition.m_BlackCastleSide = PositionInfo.CastleSide.Both;
                else if (splitString[2].Contains('K'))
                    loadedPosition.m_BlackCastleSide = PositionInfo.CastleSide.King;
                else if (splitString[2].Contains('Q'))
                    loadedPosition.m_BlackCastleSide = PositionInfo.CastleSide.Queen;
                else
                    loadedPosition.m_BlackCastleSide = PositionInfo.CastleSide.None;
            }

            // En-Passant
            if (fields > 3)
            {
                string enPassantFileName = splitString[3][0].ToString();

				if (BoardInfo.Files.Contains(enPassantFileName)) 
                {
					loadedPosition.m_EnPassantTarget = BoardInfo.Files.IndexOf(enPassantFileName) + 1;
				}
            }

            // Half-moves
            if (fields > 4)
            {
                int.TryParse(splitString[4], out loadedPosition.m_HalfMoves);
            }

            return loadedPosition;
        }

        public class PositionInfo
        {
            public enum CastleSide
            {
                None, King, Queen, Both
            }

            public int[,]       m_Squares;
            public bool         m_WhiteTurnToMove;

            public CastleSide   m_WhiteCastleSide;
            public CastleSide   m_BlackCastleSide;

            public int          m_EnPassantTarget;
            public int          m_HalfMoves;

            public PositionInfo()
            {
                m_Squares = new int[8, 8];
            }
        }
    }
}