using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public static class BitManip
    {
        static readonly Lazy<byte[]> PopCountTable = new Lazy<byte[]>(InitPopCountTable);   // Population count lookup table

        // Initialize Population count lookup table
        static byte[] InitPopCountTable()
        {
            byte[] table = new byte[256];

            // Brian Kernighan's algorithm
            for (int i = 0; i < 256; ++i)
            {
                int count = i;

                count = (count & 0x55) + ((count >> 1) & 0x55); // Count set bits in lower and upper 4 bits
                count = (count & 0x33) + ((count >> 2) & 0x33); // Count set bits in groups of 4
                count = (count & 0x0F) + ((count >> 4) & 0x0F); // Count set bits in groups of 8

                table[i] = (byte)count;
            }

            return table;
        }

        // Count number of set bits (bits set to 1)
        public static int PopCount(ulong bitBoard)
        {   
            if (bitBoard == 0)
                return 0;

            byte[] lookUpTable = PopCountTable.Value;

            int count = 0;
            for (int i = 0; i < 8; ++i)
            {
                byte value = (byte)(bitBoard >> (8 * i) & 0xFF);
                count += lookUpTable[value];
            }

            return count;
        }

        // Count number of zeroes from most significant bit to least (left to right)
        public static int CountLeadingZeroes(ulong bitBoard)
        {
            if (bitBoard == 0)
            {
                Debug.Log("Counting leading zeroes in an empty board!");
                return -1;
            }

            // TODO: Find a way to make this count more efficient!
            int count = 0;

            while ((bitBoard & 0x8000000000000000UL) == 0)
            {
                ++count;
                bitBoard <<= 1;
            }

            return count;
        }

        // Finds index of least significant bit (right-most 1)
        public static int LeastSigSetBit(ulong bitBoard)
        {
            if (bitBoard == 0) 
                throw new Exception("Bitboard is empty!");

            int position = CountLeadingZeroes(bitBoard ^ (bitBoard - 1));
            
            return 63 - position;
        }

        // Finds index of next least significant bit from current index
        public static int NextLeastSigSetBit(ulong bitBoard, int index)
        {
            if (index < 0 || index > 63)
                throw new ArgumentOutOfRangeException("index", $"{index} - Index must be between 0 and 63!");

            for (int i = index + 1; i < 64; ++i)
            {
                if ((bitBoard & (1UL << i)) != 0)
                    return i;
            }

            return -1;
        }

        // Set bit at index (all other bits as 0)
        public static ulong SetBitAt(int index)
        {
            if (index < 0 || index > 63)
                throw new ArgumentOutOfRangeException("index", $"{index} - Index must be between 0 and 63!");

            return 1UL << index;
        }

        // Unset bit at index (all other bits untouched)
        public static ulong UnsetBitAt(ulong bitBoard, int index)
        {
            if (index < 0 || index > 63)
                throw new ArgumentOutOfRangeException("index", "Index must be between 0 and 63!");

            return bitBoard & ~(1UL << index);
        }

        // Finds all set bit and add to array (each element should only contain one position)
        public static ulong[] FindAllSetBits(ulong bitBoard)
        {
            int len = PopCount(bitBoard);
            
            if (PopCount(bitBoard) == 0)
                throw new ArgumentOutOfRangeException("origin", "Origin board must only have one position");

            ulong[] result  = new ulong[len];
            int index       = LeastSigSetBit(bitBoard);

            for (int i = 0; i < len; ++i)
            {
                result[i] = SetBitAt(index);
                index = NextLeastSigSetBit(bitBoard, index);
            }

            return result;
        }

        // Finds all set bit index
        public static int[] FindAllSetBitIndex(ulong bitBoard)
        {
            int len = PopCount(bitBoard);
            
            if (len == 0)
                throw new ArgumentOutOfRangeException("bitBoard", "Finding set bits in an empty board!");

            int[] result = new int[len];
            int index    = LeastSigSetBit(bitBoard);

            for (int i = 0; i < len; ++i)
            {
                result[i] = index;
                index = NextLeastSigSetBit(bitBoard, index);
            }

            return result;
        }

        // Returns index of set bit (there must only be one set bit)
        public static int FindSetBitIndex(ulong bitBoard)
        {
            int len = PopCount(bitBoard);

            if (len == 0 || len > 1)
                throw new ArgumentOutOfRangeException("bitBoard", "Board must only have one set bit!");

            return LeastSigSetBit(bitBoard);
        }

        // Bitboard to moves
        public static List<Move> BitsToMoves(ulong origin, ulong target)
        {
            if (PopCount(origin) != 1)
                throw new ArgumentOutOfRangeException("origin", "Origin board is invalid!");

            if (PopCount(target) == 0)
                throw new ArgumentOutOfRangeException("target", "Target board is empty!");

            int index = LeastSigSetBit(origin);
            FileRank start = new FileRank(index % 8, index / 8);

            List<Move> result = new List<Move>();
            int numMoves = PopCount(target);

            index = LeastSigSetBit(target);
            for (int i = 0; i < numMoves; ++i)
            {
                result.Add(new Move(start, index % 8, index / 8));
                index = NextLeastSigSetBit(target, index);
            }

            return result;
        }

        // TODO...
        // Find all the bits set to 1 and convert into FileRank
        public static List<FileRank> FindOccupiedSquares(ulong board)
        {
            int numSquares = PopCount(board);
            
            List<FileRank> result = new List<FileRank>();

            for (int i = 0; i < 64; ++i)
            {
                if (((board >> i) & 1UL) == 1UL)
                {
                    int file = i % 8;
                    int rank = i / 8;

                    result.Add(new FileRank(file, rank));
                }
            }

            return result;
        }

        public static List<FileRank> BitBoardToFileRank(ulong board)
        {
            List<FileRank> result = new List<FileRank>();

            for (int i = 0; i < 64; ++i)
            {
                if ((board >> i & 1) != 0)
                {
                    int file = i % 8;
                    int rank = i / 8;

                    result.Add(new FileRank(file, rank));
                }
            }

            return result;
        }  

        /*

        // Finds index of most significant bit (left-most 1)
        public static int MostSigSetBit(ulong bitBoard)
        {
            return 63 - CountLeadingZeroes(bitBoard);
        }  

        public static int NextMostSigSetBit(ulong bitBoard, int index)
        {
            for (int i = index; i >= 0; --i)
            {
                if ((bitBoard & (1UL << i)) != 0)
                    return i;
            }

            return -1;
        }
        */
    }
}
