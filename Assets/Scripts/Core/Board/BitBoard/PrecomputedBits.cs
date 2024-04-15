using System;

namespace Chess
{
    public static class PrecomputedBits
    {
        public static ulong WhiteStartRankMasks => 255UL << 8;  // Starting white pawn rank for double advancement
        public static ulong BlackStartRankMasks => 255UL << 48; // Starting black pawn rank for double advancement

        public static ulong[] FileMasks => GenerateFileMasks.Value; // File mask given file index (0 - 7)
        public static ulong[] RankMasks => GenerateRankMasks.Value; // Rank mask given rank index (0 - 7)

        public static ulong[] KnightMoves   => GenerateKnightMoves.Value;
        public static ulong[] DiagonalMoves => GenerateDiagonalMoves.Value;
        public static ulong[] RookMoves     => GenerateStraightMoves.Value;

        public static void Initialize()
        {
            _ = FileMasks;
            _ = RankMasks;
            _ = KnightMoves;
            _ = DiagonalMoves;
            _ = RookMoves;
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

        static Lazy<ulong[]> GenerateStraightMoves = new Lazy<ulong[]>(() =>
        {
            ulong[] moves = new ulong[8];

            for (int i = 0; i < 8; ++i)
            {
                moves[i] = 255UL << (i * 8) | 0x0101010101010101ul << i;
            }

            return moves;
        });

/*
        public static ulong[] KnightMoves      = GenerateKnightMoves.Value;    // Knight moves
        public static ulong[] DiagonalMoves    = GenerateDiagonalMoves.Value;  // Diagonal moves
        public static ulong[] RookMoves        = GenerateRookMoves.Value;      // Rook moves   

        public static ulong[] PawnStartingMoves = GeneratePawnStartMoveMasks.Value;   // 0 for white pawns, 1 for black pawns

        

        
        

        

        

        // Precomputed start ranks for pawns (double advancements)
        static Lazy<ulong[]> GeneratePawnStartMoveMasks => new Lazy<ulong[]>(() =>
        {
            ulong[] masks = new ulong[2];

            masks[0] = (255UL << 16) | (255UL << 8);
            masks[1] = (255UL << 48) | (255UL << 40);

            return masks;
        });
        */

        /*
        public static ulong[] m_AllKnightMoves      => m_KnightMoves.Value;            // All available knight moves
        public static ulong[] m_AllStraightMoves    => m_SlidingStraightMoves.Value;   // All straight sliding moves
        public static ulong[] m_AllDiagonalMoves    => m_SlidingDiagonalMoves.Value;   // All diagonal sliding moves
        public static ulong[] m_AllKingMoves        => m_KingMoves.Value;              // All king moves

        static readonly int[] m_KnightAtkOffsets    = { -17, -15, -10, -6, 6, 10, 15, 17 }; // Knight attack offsets
        static readonly int[] m_StraightAtkOffsets  = { -8, -1, 1, 8 };                     // Straight sliding attack offsets
        static readonly int[] m_DiagonalAtkOffsets  = { -9, -7, 7, 9 };                     // Diagonal sliding attack offsets
        static readonly int[] m_KingAtkOffsets      = { -9, -8, -7, -1, 1, 7, 8, 9};        // King attack offsets

        // Initializer for precomputating bits
        public static void PrecomputeBits()
        {
            // Discarding all for faster retrieval later...
            _ = m_AllKnightMoves;
            _ = m_AllStraightMoves;
            _ = m_AllDiagonalMoves;
            _ = m_AllKingMoves;
        }

        // Lazy precomputed knight attack moves for all squares
        

        // Lazy precomputed straight attack moves for all squares
        static readonly Lazy<ulong[]> m_SlidingStraightMoves = new Lazy<ulong[]>(() =>
        {
            ulong[] straightMoves = new ulong[64];

            for (int i = 0; i < 64; ++i)
            {
                ulong straightMove = 0UL;

                foreach (int offset in m_StraightAtkOffsets)
                {
                    for (int file = 1; file <= 7; ++file)
                    {
                        int targetSquare = i + offset * file;

                        // Out of bounds check
                        if (targetSquare >= 0 && targetSquare < 64)
                        {
                            int currentRank = i / 8;
                            int targetRank = targetSquare / 8;

                            if (currentRank == targetRank || i % 8 == targetSquare % 8)
                            {
                                straightMove |= 1UL << targetSquare;
                            } 
                            else
                                break;
                        } 
                    }
                }

                straightMoves[i] = straightMove;
            } 

            return straightMoves;
        });

        // Lazy precomputed diagonal attack moves for all squares

    
        static readonly Lazy<ulong[]> m_KingMoves = new Lazy<ulong[]>(() => 
        {
            ulong[] kingMoves = new ulong[64];

            for (int i = 0; i < 64; ++i)
            {
                ulong kingMove = 0UL;

                foreach (int offset in m_KingAtkOffsets)
                {
                    int targetSquare = i + offset;

                    // Out of bounds check
                    if (targetSquare >= 0 && targetSquare < 64)
                    {
                        int currentRank = i / 8;
                        int targetRank = targetSquare / 8;

                        // Target square must be in range of one square
                        if (Math.Abs(currentRank - targetRank) <= 1 && Math.Abs(i % 8 - targetSquare % 8) <= 1)
                            kingMove |= 1UL << targetSquare;
                    }
                }

                kingMoves[i] = kingMove;
            }

            return kingMoves;
        });
        */
    }
}
