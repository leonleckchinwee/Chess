using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public struct Coordinates : IComparable<Coordinates>
    {
        public int m_File;  // Vertical   / Row    / X
        public int m_Rank;  // Horizontal / Column / Y

        public Coordinates (int rank, int file)
        {
            m_Rank = rank;
            m_File = file;
        }

        public bool IsLightSquare ()
        {
            return (m_File + m_Rank) % 2 != 0;
        }

        public bool IsDarkSquare ()
        {
            return (m_File + m_Rank) % 2 == 0;
        }

        public int CompareTo (Coordinates other)
        {
            return (Mathf.Approximately (m_File, other.m_File) && Mathf.Approximately (m_Rank, other.m_Rank)) ? 0 : 1;
        }

        public override bool Equals (object obj)
        {
            if (obj is not Coordinates)
                return false;

            Coordinates other = (Coordinates)obj;
            return Mathf.Approximately (m_File, other.m_File) && Mathf.Approximately (m_Rank, other.m_Rank);
        }

        public override int GetHashCode ()
        {
            return m_File.GetHashCode () ^ m_Rank.GetHashCode ();
        }
    }
}
