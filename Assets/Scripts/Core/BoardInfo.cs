using UnityEngine;

namespace Chess
{
    public static class BoardInfo
    {
        public const string RankNames = "12345678";
        public const string FileNames = "ABCDEFGH";

        // Get rank index (row A - H)
        public static int RankIndex(int square)
        {
            return square >> 3;
        }

        // Get file index (column 0 - 7)
        public static int FileIndex(int square)
        {
            return square & 0b000111;
        }

        // Get index from (rank, file)
        public static int IndexFromCoordinates(int rank, int file)
        {
            return rank * 8 + file;
        }

        // Get index from coordinates
        public static int IndexFromCoordinates(Coordinates coordinates)
        {
            return IndexFromCoordinates(coordinates.RankIndex, coordinates.FileIndex);
        }

        // Get coordinates from square index
        public static Coordinates CoordinatesFromIndex(int square)
        {
            return new Coordinates(RankIndex(square), FileIndex(square));
        }

        // Get square name from (rank, file)
        public static string GetNameFromCoordinates(int rank, int file)
        {
            return FileNames[file] + "" + (rank + 1);
        }

        // Get square name from coordinates
        public static string GetNameFromCoordinates(Coordinates coordinates)
        {
            return GetNameFromCoordinates(coordinates.RankIndex, coordinates.FileIndex);
        }

        // Get square name from square index
        public static string GetNameFromIndex(int square)
        {
            return GetNameFromCoordinates(RankIndex(square), FileIndex(square));
        }
    }
}