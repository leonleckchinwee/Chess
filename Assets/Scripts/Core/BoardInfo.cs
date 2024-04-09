using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Chess
{
    public static class BoardInfo
    {
        public const string Files = "ABCDEFGH";
        public const string Ranks = "12345678";

        public const int A1 = 0, A2 = 8,  A3 = 16, A4 = 24, A5 = 32, A6 = 40, A7 = 48, A8 = 56;
        public const int B1 = 1, B2 = 9,  B3 = 17, B4 = 25, B5 = 33, B6 = 41, B7 = 49, B8 = 57;
        public const int C1 = 2, C2 = 10, C3 = 18, C4 = 26, C5 = 34, C6 = 42, C7 = 50, C8 = 58;
        public const int D1 = 3, D2 = 11, D3 = 19, D4 = 27, D5 = 35, D6 = 43, D7 = 51, D8 = 59;
        public const int E1 = 4, E2 = 12, E3 = 20, E4 = 28, E5 = 36, E6 = 44, E7 = 52, E8 = 60;
        public const int F1 = 5, F2 = 13, F3 = 21, F4 = 29, F5 = 37, F6 = 45, F7 = 53, F8 = 61;
        public const int G1 = 6, G2 = 14, G3 = 22, G4 = 30, G5 = 38, G6 = 46, G7 = 54, G8 = 62;
        public const int H1 = 7, H2 = 15, H3 = 23, H4 = 31, H5 = 39, H6 = 47, H7 = 55, H8 = 63;

        public static void PositionToFileRank(int position, out int file, out int rank)
        {
            file = position % 8;
            rank = position / 8;
        }

        public static bool IsLightSquare(int file, int rank)
        {
            return (file + rank) % 2 != 0;
        }

        public static bool IsDarkSquare(int file, int rank)
        {
            return (file + rank) % 2 == 0;
        }

        public static Vector3 GetWorldPositionFromFileRank(int file, int rank, float depth, bool whiteIsBottom)
        {
            if (whiteIsBottom)
                return new Vector3(-3.5f + file, -3.5f + rank, depth);

            return new Vector3(-3.5f + 7 - file, 7 - rank - 3.5f, depth);
        }

        public static string GetPositionNameFromFileRank(int file, int rank)
        {
            return $"{Files[file]}{rank + 1}";
        }
    }
}