using System;

namespace Chess
{
    public static class PrecomputedBits
    {
        public static ulong WhiteStartRankMasks => 255UL << 8;  // Starting white pawn rank for double advancement
        public static ulong BlackStartRankMasks => 255UL << 48; // Starting black pawn rank for double advancement

        public static ulong[] FileMasks => GenerateFileMasks.Value; // File mask given file index (0 - 7)
        public static ulong[] RankMasks => GenerateRankMasks.Value; // Rank mask given rank index (0 - 7)

        public static ulong[] KnightMoves   => GenerateKnightMoves.Value;   // Knight moves
        public static ulong[] DiagonalMoves => GenerateDiagonalMoves.Value; // Diagonal moves
        public static ulong[] StraightMoves => GenerateStraightMoves.Value; // Straight moves

        public static void Initialize()
        {
            _ = FileMasks;
            _ = RankMasks;
            _ = KnightMoves;
            _ = DiagonalMoves;
            _ = StraightMoves;
        }

        // Precomputed file masks
        static Lazy<ulong[]> GenerateFileMasks => new Lazy<ulong[]>(() =>
        {
            ulong[] fileMasks = new ulong[8];

            for (int i = 0; i < 8; i++)
            {
                fileMasks[i] = 0x0101010101010101ul << i;
            }

            return fileMasks;
        });

        // Precomputed rank masks
        static Lazy<ulong[]> GenerateRankMasks => new Lazy<ulong[]>(() =>
        {
            ulong[] rankMasks = new ulong[8];

            for (int i = 0; i < 8; ++i)
            {
                rankMasks[i] = 255UL << (i * 8);
            }

            return rankMasks;
        });

        // Precomputed knight moves
        static Lazy<ulong[]> GenerateKnightMoves = new Lazy<ulong[]>(() => 
        {
            int[] m_KnightAtkOffsets = { -17, -15, -10, -6, 6, 10, 15, 17 }; // Knight attack offsets
            ulong[] knightMoves      = new ulong[64];

            for (int i = 0; i < 64; ++i)
            {
                ulong knightMove = 0UL;
                
                foreach (int offset in m_KnightAtkOffsets)
                {
                    int targetSquare = i + offset;

                    if (targetSquare >= 0 && targetSquare < 64 && Math.Abs(i % 8 - targetSquare % 8) <= 2)
                        knightMove |= 1UL << targetSquare;
                }

                knightMoves[i] = knightMove;
            }

            return knightMoves;
        });

        // Precomputed diagonal moves
        static Lazy<ulong[]> GenerateDiagonalMoves = new Lazy<ulong[]>(() =>
        {
            int[] m_DiagonalAtkOffsets  = { -9, -7, 7, 9 }; // Diagonal sliding attack offsets
            ulong[] diagonalMoves       = new ulong[64];

            for (int i = 0; i < 64; ++i)
            {
                ulong diagonalMove = 0UL;

                foreach (int offset in m_DiagonalAtkOffsets)
                {
                    for (int file = 1; file <= 7; ++file)
                    {
                        int targetSquare = i + offset * file;

                        // Out of bounds check
                        if (targetSquare >= 0 && targetSquare < 64)
                        {
                            int currentRank = i / 8;
                            int targetRank = targetSquare / 8;

                            if (Math.Abs(currentRank - targetRank) == Math.Abs(i % 8 - targetSquare % 8))
                                diagonalMove |= 1UL << targetSquare;
                        }
                    }
                }

                diagonalMoves[i] = diagonalMove;
            }

            return diagonalMoves;
        });

        // Precomputed straight moves
        static Lazy<ulong[]> GenerateStraightMoves = new Lazy<ulong[]>(() =>
        {
            ulong[] moves = new ulong[8];

            for (int i = 0; i < 8; ++i)
            {
                moves[i] = 255UL << (i * 8) | 0x0101010101010101ul << i;
            }

            return moves;
        });
    }
}
