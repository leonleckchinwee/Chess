using System;

namespace Chess
{
    public struct Move
    {
        private short m_Data;

        public int FromFile => (m_Data >> 12) & 0x0F;
        public int FromRank => (m_Data >> 8) & 0x0F;
        public int ToFile   => (m_Data >> 4) & 0x0F;
        public int ToRank   =>  m_Data & 0x0F;

        public FileRank FromFileRank => new FileRank(FromFile, FromRank);
        public FileRank ToFileRank   => new FileRank(ToFile, ToRank);

        const int MinValue = 0;
        const int MaxValue =  8;

        public Move(int fromFile, int fromRank, int toFile, int toRank)
        {
            if (!IsValidFileRank(fromFile, fromRank, toFile, toRank))
                throw new ArgumentOutOfRangeException("File and rank must be of values between 0 to 8!");

            m_Data = Pack(fromFile, fromRank, toFile, toRank);
        } 

        public Move(FileRank from, FileRank to)
        {
            if (!IsValidFileRank(from.File, from.Rank, to.File, to.Rank))
                throw new ArgumentOutOfRangeException("File and rank must be of values between 0 to 8!");

            m_Data = Pack(from, to);
        }

        public Move(FileRank from, int toFile, int toRank)
        {
            if (!IsValidFileRank(from.File, from.Rank, toFile, toRank))
                throw new ArgumentOutOfRangeException("File and rank must be of values between 0 to 8!");

            m_Data = Pack(from.File, from.Rank, toFile, toRank);
        }

        public Move(int fromFile, int fromRank, FileRank to)
        {
            if (!IsValidFileRank(fromFile, fromRank, to.File, to.Rank))
                throw new ArgumentOutOfRangeException("File and rank must be of values between 0 to 8!");

            m_Data = Pack(fromFile, fromRank, to.File, to.Rank);
        }

        public static short Pack(int fromFile, int fromRank, int toFile, int toRank)
        {
            return (short)((fromFile << 12) | (fromRank << 8) | (toFile << 4) | toRank);
        }

        public static short Pack(FileRank from, FileRank to)
        {
            return (short)((from.File << 12) | (from.Rank << 8) | (to.File << 4) | to.Rank);
        }

        public static bool IsValidFileRank(int file, int rank)
        {
            return file >= MinValue && file < MaxValue && rank >= MinValue && rank < MaxValue;
        }

        public static bool IsValidFileRank(int fromFile, int fromRank, int toFile, int toRank)
        {
            return fromFile >= MinValue && fromFile < MaxValue && fromRank >= MinValue && fromRank < MaxValue 
                && toFile   >= MinValue && toFile   < MaxValue && toRank   >= MinValue && toRank   < MaxValue;
        }

        public override string ToString()
        {
            return $"({BoardInfo.GetPositionNameFromFileRank(FromFile, FromRank)} -> {BoardInfo.GetPositionNameFromFileRank(ToFile, ToRank)})";
        }

        public override bool Equals(object obj)
        {
            if (obj is not Move)
                return false;

            Move move = (Move)obj;
            return m_Data == move.m_Data;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
