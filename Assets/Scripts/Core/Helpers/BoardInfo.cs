using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Chess
{
    public struct BoardInfo
    {
        public const string Files = "ABCDEFGH";
        public const string Ranks = "12345678";

        // Square position to (file, rank)
        public static void PositionToFileRank(int position, out int file, out int rank)
        {
            file = position % 8;
            rank = position / 8;
        }

        // Overloaded function for FileRank
        public static void PositionToFileRank(int position, out FileRank fileRank)
        {
            fileRank = new FileRank(position % 8, position / 8);
        }

        // (file, rank) to square position
        public static int FileRankToPosition(int file, int rank)
        {
            return rank * 8 + file;
        }

        // Overloaded function for FileRank
        public static int FileRankToPosition(FileRank position)
        {
            return FileRankToPosition(position.File, position.Rank);
        }

        // Check if (file, rank) is light square
        public static bool IsLightSquare(int file, int rank)
        {
            return (file + rank) % 2 != 0;
        }

        // Overloaded function for FileRank
        public static bool IsLightSquare(FileRank pos)
        {
            return IsLightSquare(pos.File, pos.Rank);
        }

        // Check if (file, rank) is dark square
        public static bool IsDarkSquare(int file, int rank)
        {
            return (file + rank) % 2 == 0;
        }

        // Overloaded function for FileRank
        public static bool IsDarkSquare(FileRank pos)
        {
            return IsDarkSquare(pos.File, pos.Rank);
        }

        // Get world position from (file, rank)
        public static Vector3 GetWorldPositionFromFileRank(int file, int rank, float depth, bool whiteIsBottom)
        {
            if (whiteIsBottom)
                return new Vector3(-3.5f + file, -3.5f + rank, depth);

            return new Vector3(-3.5f + 7 - file, 7 - rank - 3.5f, depth);
        }

        // Overloaded function for FileRank
        public static Vector3 GetWorldPositionFromFileRank(FileRank position, float depth, bool whiteIsBottom)
        {
            return GetWorldPositionFromFileRank(position.File, position.Rank, depth, whiteIsBottom);
        }

        // Get square name from (file, rank)
        public static string GetPositionNameFromFileRank(int file, int rank)
        {
            return $"{Files[file]}{rank + 1}";
        }

        // Overloaded function for FileRank
        public static string GetPositionNameFromFileRank(FileRank pos)
        {
            return $"{Files[pos.File]}{pos.Rank + 1}";
        }
    }
}