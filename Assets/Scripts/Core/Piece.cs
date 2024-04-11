namespace Chess
{
    public static class Piece
    {
        public const int None       = 0b00000;  // 0

        public const int Pawn       = 0b00001;  // 1
        public const int Knight     = 0b00010;  // 2
        public const int Bishop     = 0b00011;  // 3
        public const int Rook       = 0b00100;  // 4
        public const int Queen      = 0b00101;  // 5
        public const int King       = 0b00110;  // 6

        public const int White      = 0b01000;  // 8
        public const int Black      = 0b10000;  // 16

        public const int PieceMask  = 0b00111;  // Last 3 bits
        public const int ColorMask  = 0b11000;  // First 2 bits

        public static int PieceType(int piece)
        {
            return piece & PieceMask;  // Extract the last 3 bits
        }

        public static int PieceColor(int piece)
        {
            return piece & ColorMask;  // Extract the first 3 bits
        }

        // Get opponent color
        public static int OpponentColor(int piece)
        {
            return Piece.IsWhite(piece & ColorMask) ? Piece.Black : Piece.White;
        }

        // Color to check must be of type color
        public static bool IsSameColor(int piece, int color)
        {
            return (piece & ColorMask) == color;
        }   

        public static bool IsWhite(int piece)
        {
            return (piece & ColorMask) == White;
        }

        public static bool IsBlack(int piece)
        {
            return (piece & ColorMask) == Black;
        }

        public static bool IsEmpty(int piece)
        {
            return (piece & PieceMask) == None;
        }

        public static bool IsPawn(int piece)
        {
            return (piece & PieceMask) == Pawn;
        }

        public static bool IsKnight(int piece)
        {
            return (piece & PieceMask) == Knight;
        }

        public static bool IsBishop(int piece)
        {
            return (piece & PieceMask) == Bishop;
        }

        public static bool IsRook(int piece)
        {
            return (piece & PieceMask) == Rook;
        }

        public static bool IsQueen(int piece)
        {
            return (piece & PieceMask) == Queen;
        }

        public static bool IsKing(int piece)
        {
            return (piece & PieceMask) == King;
        }

        public static bool IsSlidingPiece(int piece)
        {
            return IsBishop(piece) || IsRook(piece) || IsQueen(piece);
        }

        public static bool IsBishopOrQueen(int piece)
        {
            return IsBishop(piece) || IsQueen(piece);
        }

        public static bool IsRookOrQueen(int piece)
        {
            return IsRook(piece) || IsQueen(piece);
        }

        public static string GetTypeName(int piece)
        {
            int pieceType = PieceType(piece);
            switch (pieceType)
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

        public static string GetColorName(int piece)
        {
            return IsWhite(piece) ? "White" : "Black";
        } 
    }
}