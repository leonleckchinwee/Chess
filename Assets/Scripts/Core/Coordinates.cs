using System;

namespace Chess
{
    public struct Coordinates : IComparable<Coordinates>
    {
        public readonly int RankIndex;
        public readonly int FileIndex;

        public Coordinates(int rank, int file)
        {
            RankIndex = rank;
            FileIndex = file;
        }

        public bool IsLightSquare()
        {
            return (RankIndex + FileIndex) % 2 != 0;
        }

        public bool IsDarkSquare()
        {
            return (RankIndex + FileIndex) % 2 == 0;
        }

        public int CompareTo(Coordinates other)
        {
            return (RankIndex == other.RankIndex && FileIndex == other.FileIndex) ? 0 : 1;
        }
    }
}
