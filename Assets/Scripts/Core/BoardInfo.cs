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

        public static void PositionToFileRank(int position, out int file, out int rank)
        {
            file = position % 8;
            rank = position / 8;
        }

        public static bool IsLightSquare(int file, int rank)
        {
            return (file + rank) % 2 != 0;
        }

        public static bool IsLightSquare(FileRank pos)
        {
            return IsLightSquare(pos.File, pos.Rank);
        }

        public static bool IsDarkSquare(int file, int rank)
        {
            return (file + rank) % 2 == 0;
        }

        public static bool IsDarkSquare(FileRank pos)
        {
            return IsDarkSquare(pos.File, pos.Rank);
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

        public static string GetPositionNameFromFileRank(FileRank pos)
        {
            return $"{Files[pos.File]}{pos.Rank + 1}";
        }
    }
}