namespace Chess
{
    public static class Piece
    {
        public const int None               = 0b00000;  // 0
        public const int Pawn               = 0b00001;  // 1
        public const int Knight             = 0b00010;  // 2
        public const int Bishop             = 0b00011;  // 3
        public const int Rook               = 0b00100;  // 4
        public const int Queen              = 0b00101;  // 5
        public const int King               = 0b00110;  // 6

        public const int White              = 0b01000;  // 8
        public const int Black              = 0b10000;  // 16

        private const int WhitePieceMask    = 0b01000;
        private const int BlackPieceMask    = 0b10000;

        private const int PieceTypeMask     = 0b00111;
        private const int PieceColorMask    = WhitePieceMask | BlackPieceMask;

        // Get piece color
        public static int Color (int piece)
        {
            return piece & PieceColorMask;
        }

        // Get piece type
        public static int PieceType (int piece)
        {
            return piece & PieceTypeMask;
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

        public static bool IsBishopOrRook(int piece)
        {
            return IsBishop(piece) || IsRook(piece);
        }

        public static bool IsBishopOrQueen(int piece)
        {
            return IsBishop(piece) || IsQueen(piece);
        }

        public static bool IsRookOrQueen(int piece)
        {
            return IsRook(piece) || IsQueen(piece);
        }

        public static bool IsSlidingPiece(int piece)
        {
            return IsBishop(piece) || IsRook(piece) || IsQueen(piece);
        }

        public static bool IsWhite (int piece)
        {
            return (piece & WhitePieceMask) == WhitePieceMask;
        }

        public static bool IsBlack (int piece)
        {
            return (piece & BlackPieceMask) == BlackPieceMask;
        }

        public static bool IsColor (int piece, int color)
        {
            return (piece & PieceColorMask) == color;
        }

        public static string ColorName(int piece)
        {
            return IsWhite(piece) ? "White" : "Black";
        }

        public static string PieceTypeName(int piece)
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
    }
}