using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public static class BitManip
    {
        // Find all the bits set to 1 and convert into FileRank
        public static List<FileRank> FindOccupiedSquares(ulong board)
        {
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
    }
}
