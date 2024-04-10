using System;

namespace Chess
{
    public struct FileRank
    {
        private byte m_Data;    

        public int File => (m_Data >> 4) & 0x0F;    // Left 4 bits for file
        public int Rank => m_Data & 0x0F;           // Right 4 bits for rank

        const int MinValue = 0;
        const int MaxValue = 8;

        public static readonly FileRank None = new FileRank(255);   // Special value for error

        public FileRank(int file, int rank)
        {
            if (!IsValidFileRank(file, rank))
                throw new ArgumentOutOfRangeException("File and rank must be of values between 0 to 8!");

            m_Data = Pack(file, rank); 
        }

        private FileRank(byte value)
        {
            m_Data = value;
        }

        public void Set(int file, int rank)
        {
            if (!IsValidFileRank(file, rank))
                throw new ArgumentOutOfRangeException("File and rank must be of values between 0 to 8!");

            m_Data = Pack(file, rank); 
        }

        public static byte Pack(int file, int rank)
        {
            return (byte)((file << 4) | rank);
        }

        public static bool IsValidFileRank(int file, int rank)
        {
            return file >= MinValue && file < MaxValue && rank >= MinValue && rank < MaxValue;
        }

        public static bool IsValidFileRank(FileRank position)
        {
            return IsValidFileRank(position.File, position.Rank);
        }

        public override string ToString()
        {
            if (m_Data == 255)
                return "Invalid FileRank!";

            return $"({BoardInfo.GetPositionNameFromFileRank(File, Rank)})";
        }

        public override bool Equals(object obj)
        {
            if (obj is not FileRank)
                return false;

            FileRank position = (FileRank)obj;
            return m_Data == position.m_Data; 
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}