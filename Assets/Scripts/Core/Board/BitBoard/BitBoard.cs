using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Chess
{
    public struct BitBoard
    {
        public ulong[] m_Pieces;        // White pieces: 0 - 5
        public ulong m_EmptySquares;    // Black pieces: 6 - 11

        // Get current state of board and store piece informations
        public BitBoard(Board board)
        {
            m_Pieces = new ulong[12];
            m_EmptySquares = 0;

            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    int piece = board.GetPieceAt(file, rank);
                    if (piece == Piece.None)
                        continue;

                    int index       = GetPieceIndex(piece);
                    int squareIndex = GetSquareIndex(file, rank);
                    ulong squareBit = 1UL << squareIndex;

                    m_Pieces[index] |= squareBit;
                }
            }

            m_EmptySquares = ~CombineAll();
        }

        // Clear current state and update
        public void Update(Board board)
        {
            Clear();

            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    int piece = board.GetPieceAt(file, rank);
                    if (piece == Piece.None)
                        continue;

                    int index       = GetPieceIndex(piece);
                    int squareIndex = GetSquareIndex(file, rank);
                    ulong squareBit = 1UL << squareIndex;

                    m_Pieces[index] |= squareBit;
                }
            }

            m_EmptySquares = ~CombineAll();
        }

        // Clear all
        public void Clear()
        {
            for (int i = 0; i < m_Pieces.Length; ++i)
            {
                m_Pieces[i] = 0;
            }

            m_EmptySquares = 0;
        }

        // Get any type at (file, rank)
        public int GetAnyPieceOn(int file, int rank)
        {
            if (IsEmpty(file, rank))
                return Piece.None;

            int squareIndex = GetSquareIndex(file, rank);
            ulong squareBit = 1UL << squareIndex;

            for (int i = 0; i < m_Pieces.Length; ++i)
            {
                if ((m_Pieces[i] & squareBit) != 0)
                {
                    int color = i < 6 ? Piece.White : Piece.Black;
                    int pieceType = i < 6 ? i + 1 : i - 5;
                    return pieceType | color;
                }
            }
  
            return Piece.None;
        }

        // Overloaded function for FileRank
        public int GetAnyPieceOn(FileRank position)
        {
            return GetAnyPieceOn(position.File, position.Rank);
        }

        // Overloaded function for square position
        public int GetAnyPieceOn(int position)
        {
            BoardInfo.PositionToFileRank(position, out int file, out int rank);
            return GetAnyPieceOn(file, rank);
        }

        // Get any piece of color on (file, rank)
        public int GetPieceOfColorOn(int file, int rank, int color)
        {
            if (IsEmpty(file, rank))
                return Piece.None;

            int index = Piece.IsWhite(color) ? 0 : 6;
            int squareIndex = GetSquareIndex(file, rank);
            ulong squareBit = 1UL << squareIndex;

            for (int max = index; index < max + 6; ++index)
            {
                if ((m_Pieces[index] & squareBit) != 0)
                {
                    int pieceType = Piece.IsWhite(color) ? index + 1 : index - 5;
                    return pieceType | color;
                }
            }

            return Piece.None;
        }

        // Overloaded function for FileRank
        public int GetPieceOfColorOn(FileRank position, int color)
        {
            return GetPieceOfColorOn(position.File, position.Rank, color);
        }

        // Overloaded function for square position
        public int GetPieceOfColorOn(int position, int color)
        {
            BoardInfo.PositionToFileRank(position, out int file, out int rank);
            return GetPieceOfColorOn(file, rank, color);
        }

        // Get all empty squares
        public List<FileRank> GetEmptySquares()
        {
            return BitManip.FindOccupiedSquares(~CombineAll());
        }
        
        // Get occupied spaces
        public List<FileRank> GetOccupiedSquares()
        {
            return BitManip.FindOccupiedSquares(CombineAll());
        }
        
        // Get all pieces of color
        public List<FileRank> GetAllPiecesOf(int pieceWithColor)
        {
            int index = GetPieceIndex(pieceWithColor);

            return BitManip.FindOccupiedSquares(m_Pieces[index]);
        }

        public List<FileRank> GetAllPiecesOf(int[] pieceWithColor)
        {
            List<FileRank> positions = new List<FileRank>();

            foreach (int piece in pieceWithColor)
            {
                int index = GetPieceIndex(piece);
                positions.AddRange(BitManip.FindOccupiedSquares(m_Pieces[index]));
            }

            return positions;
        }

        // Set piece on (file, rank)
        public void SetPieceOn(int file, int rank, int pieceWithColor)
        {
            if (!FileRank.IsValidFileRank(file, rank))
                throw new ArgumentException("Invalid file or rank!");

            int index       = GetPieceIndex(pieceWithColor);
            int squareIndex = GetSquareIndex(file, rank);
            ulong squareBit = 1UL << squareIndex;

            m_Pieces[index] |= squareBit;
        }

        // Overloaded function for FileRank
        public void SetPieceOn(FileRank position, int pieceWithColor)
        {
            SetPieceOn(position.File, position.Rank, pieceWithColor);
        }

        // Overloaded function for square position
        public void SetPieceOn(int position, int pieceWithColor)
        {
            BoardInfo.PositionToFileRank(position, out int file, out int rank);
            SetPieceOn(file, rank, pieceWithColor);
        }

        // Remove any piece on (file, rank)
        public void RemoveAnyPieceOn(int file, int rank)
        {
            if (!FileRank.IsValidFileRank(file, rank))
                throw new ArgumentException("Invalid file or rank!");

            int squareIndex = GetSquareIndex(file, rank);
            ulong squareBit = ~(1UL << squareIndex);

            for (int i = 0; i < m_Pieces.Length; ++i)
            {
                m_Pieces[i] &= squareBit;
            }
        }

        // Overloaded function for FileRank
        public void RemoveAnyPieceOn(FileRank position)
        {
            RemoveAnyPieceOn(position.File, position.Rank);
        }

        // Overloaded function for square position
        public void RemoveAnyPieceOn(int position)
        {
            BoardInfo.PositionToFileRank(position, out int file, out int rank);
            RemoveAnyPieceOn(file, rank);
        }

        // Check if square is empty at (file, rank)
        public bool IsEmpty(int file, int rank)
        {
            if (!FileRank.IsValidFileRank(file, rank))
                throw new ArgumentException("Invalid file or rank!");

            int squareIndex = GetSquareIndex(file, rank);
            ulong squareBit = 1UL << squareIndex;

            return (m_EmptySquares & squareBit) != 0;
        }

        // Overloaded function for FileRank
        public bool IsEmpty(FileRank position)
        {
            return IsEmpty(position.File, position.Rank);
        }

        // Overloaded function for square position
        public bool IsEmpty(int position)
        {
            BoardInfo.PositionToFileRank(position, out int file, out int rank);
            return IsEmpty(file, rank);
        }

        // Check if piece is on square at (file, rank)
        public bool IsPieceOn(int file, int rank, int pieceWithColor)
        {
            if (!FileRank.IsValidFileRank(file, rank))
                throw new ArgumentException("Invalid file or rank!");

            int index       = GetPieceIndex(pieceWithColor);
            int squareIndex = GetSquareIndex(file, rank);
            ulong squareBit = 1UL << squareIndex;

            return (m_Pieces[index] & squareBit) != 0;
        }

        // Overloaded function for FileRank
        public bool IsPieceOn(FileRank position, int pieceWithColor)
        {
            return IsPieceOn(position.File, position.Rank, pieceWithColor);
        }

        // Overloaded function for square position
        public bool IsPieceOn(int position, int PieceWithColor)
        {
            BoardInfo.PositionToFileRank(position, out int file, out int rank);
            return IsPieceOn(file, rank, PieceWithColor);
        }

        // Get Piece index
        int GetPieceIndex(int pieceWithColor)
        {
            int pieceType = Piece.PieceType(pieceWithColor);
            if (pieceType == Piece.None)
                return -1;

            int index = Piece.IsWhite(pieceWithColor) ? pieceType - 1 : pieceType + 5;

            return index;
        }

        // Get square index
        int GetSquareIndex(int file, int rank)
        {
            return rank * 8 + file;
        }
    
        // Combine all boards into one
        ulong CombineAll()
        {
            ulong combined = 0;
            for (int i = 0; i < m_Pieces.Length; ++i)
            {
                combined |= m_Pieces[i];
            }

            return combined;
        }

        // Combine white boards into one
        ulong CombineWhites()
        {
            ulong combined = 0;
            for (int i = 0; i < 6; ++i)
            {
                combined |= m_Pieces[i];
            }

            return combined;
        }

        // Combine black boards into one
        ulong CombineBlacks()
        {
            ulong combined = 0;
            for (int i = 6; i < 12; ++i)
            {
                combined |= m_Pieces[i];
            }

            return combined;
        }
    }
}
