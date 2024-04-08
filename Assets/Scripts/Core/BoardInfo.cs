using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public static class BoardInfo
    {
        public const string Files = "ABCDEFGH";
        public const string Ranks = "12345678";

        public static int File (int square)
        {
            return square >> 3;
        }

        public static int Rank (int square)
        {
            return square & 0b000111;
        }

        public static int CoordinatesToIndex (int rank, int file)
        {
            return rank * 8 + file;
        }

        public static int CoordinatesToIndex (Coordinates coordinates)
        {
            return coordinates.m_Rank * 8 + coordinates.m_File;
        }

        public static Vector3 GetWorldPositionFromCoordinates (int rank, int file, float depth, bool isWhiteBottom)
        {
            if (isWhiteBottom)
                return new Vector3(-3.5f + file, -3.5f + rank, depth);

            return new Vector3(-3.5f + 7f - file, 7 - rank - 3.5f, depth);
        }

        public static Vector3 GetWorldPositionFromCoordinates (Coordinates coordinates, float depth, bool isWhiteBottom)
        {
            if (isWhiteBottom)
                return new Vector3(-3.5f + coordinates.m_File, -3.5f + coordinates.m_Rank, depth);
            
            return new Vector3(-3.5f + 7f - coordinates.m_File, 7f - coordinates.m_Rank - 3.5f, depth);
        }

        public static string GetPositionNameFromCoordinates (int rank, int file)
        {
            return Files[file] + "" + (rank + 1);
        }

        public static string GetPositionNameFromCoordinates (Coordinates coordinates)
        {
            return Files[coordinates.m_File] + "" + (coordinates.m_Rank + 1);
        }
    }
}
