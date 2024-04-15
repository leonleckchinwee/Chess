using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public struct BitBoard
    {
        ulong[] m_Pieces;        // White pieces: 0 - 5, Black pieces: 6 - 11
        ulong[] m_AllPieces;     // 2 bitboard for all pieces (0 for white, 1 for black)

        // Get current state of board and store piece informations
        public BitBoard(Board board)
        {
            m_Pieces = new ulong[12];
            m_AllPieces = new ulong[2];

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

                    int color = Piece.IsWhite(piece) ? 0 : 1;
                    m_AllPieces[color] |= squareBit;
                }
            }
        }

        // Get all pieces of same color (regardless of type)
        public ulong GetAllPiecesOf(int color)
        {
            int index = Piece.IsWhite(color) ? 0 : 1;
            return m_AllPieces[index];
        }

        // Get all pieces of same type and color
        public ulong GetPiecesOf(int pieceWithColor)
        {
            int index = GetPieceIndex(pieceWithColor);
            return m_Pieces[index];
        }

        public int GetPieceAt(int squareIndex)
        {
            if (squareIndex < 0 || squareIndex > 63)
                throw new ArgumentOutOfRangeException("Square index must be between 0 and 63");

            ulong position = BitManip.SetBitAt(squareIndex);

            if ((position & GetOccupiedSquares()) == 0)
                return Piece.None;

            int color = (position & m_AllPieces[0]) != 0 ? Piece.White : Piece.Black;
            for (int i = 0; i < 6; ++i)
            {
                int index = i;

                if (Piece.IsBlack(color))
                    index += 6;

                if ((position & m_Pieces[index]) != 0)
                    return Piece.MakePiece(i + 1, color);
            } 

            return Piece.None;
        }

        // Get all occupied squares (regardless of type or color)
        public ulong GetOccupiedSquares()
        {
            return m_AllPieces[0] | m_AllPieces[1];
        }

        // Get all empty squares
        public ulong GetEmptySquares()
        {
            return ~(m_AllPieces[0] | m_AllPieces[1]);
        }

        // TODO: Clear current state and update 
        public void Update(Board board)
        {
            
        }

        public void Clear()
        {
            for (int i = 0; i < m_Pieces.Length; ++i)
            {
                m_Pieces[i] = 0;
            }

            m_AllPieces[0] = 0;
            m_AllPieces[1] = 0;
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
            if (file < 0 || file > 7 || rank < 0 || rank > 7)
                throw new ArgumentOutOfRangeException("File and rank must be between 0 and 7");

            return rank * 8 + file;
        }
    }
}
