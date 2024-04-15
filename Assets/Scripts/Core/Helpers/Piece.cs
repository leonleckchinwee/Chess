namespace Chess
{
    public struct Piece
    {
        public const int None       = 0;
        public const int Pawn       = 1;
        public const int Knight     = 2;
        public const int Bishop     = 3;
        public const int Rook       = 4;
        public const int Queen      = 5;
        public const int King       = 6;

        public const int White      = 8;
        public const int Black      = 16;

        const int PieceMask  = 7;    // Last 3 bits
        const int ColorMask  = 24;   // First 2 bits

        public static int PieceType(int piece)
        {
            return piece & PieceMask;  // Extract the last 3 bits
        }

        public static int PieceColor(int piece)
        {
            return piece & ColorMask;  // Extract the first 3 bits
        }

        public static int MakePiece(int pieceType, int pieceColor)
        {
            return pieceType | pieceColor;
        }

        // Get opponent color
        public static int OpponentColor(int piece)
        {
            return IsWhite(piece) ? Black : White;
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
            return ((int)piece & PieceMask) == Rook;
        }

        public static bool IsQueen(int piece)
        {
            return ((int)piece & PieceMask) == Queen;
        }

        public static bool IsKing(int piece)
        {
            return ((int)piece & PieceMask) == King;
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

        public static string GetColorName(int color)
        {
            int pieceColor = PieceColor(color);
            switch (pieceColor)
            {
                case White:
                    return "White";

                case Black:
                    return "Black";

                default:
                    return "None";
            }
        } 
    }
}