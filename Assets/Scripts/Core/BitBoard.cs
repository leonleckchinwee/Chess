using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public class BitBoard
    {
        ulong[,]    m_Boards;       // 12 Bitboards (1 for each color, 1 for each piece type)
        ulong[]     m_EmptySquares; // 2 Bitboards for empty squares (1 for each color)

        public BitBoard(Board board)
        {
            m_Boards        = new ulong[2, 6];

            m_EmptySquares  = new ulong[2];

            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    int piece = board.GetPieceAt(file, rank);

                    if (Piece.IsEmpty(piece))                        
                        continue;

                    int pieceType = Piece.PieceType(piece) - 1;
                    int pieceColor = Piece.IsWhite(piece) ? 0 : 1;

                    int squareIndex = rank * 8 + file;

                    m_EmptySquares[pieceColor] |= 1UL << squareIndex;
                    m_Boards[pieceColor, pieceType] |= 1UL << squareIndex;
                }
            }
        }

        public int GetPieceAt(int file, int rank, int color)
        {
            int colorIndex = Piece.IsWhite(color) ? 0 : 1;

            for (int pieceIndex = 0; pieceIndex < 6; ++pieceIndex)
            {
                ulong mask = 1UL << (rank * 8 + file);

                if ((m_Boards[colorIndex, pieceIndex] & mask) == 0)
                    continue;

                return pieceIndex + 1;
            }

            return Piece.None;
        }

        public int GetPieceAt(int position, int color)
        {
            BoardInfo.PositionToFileRank(position, out int file, out int rank);
            return GetPieceAt(file, rank, color);
        }

        public List<FileRank> GetAllEmptySquares()
        {
            ulong combinedBoard = m_EmptySquares[0] | m_EmptySquares[1];

            List<FileRank> result = new List<FileRank>();

            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    ulong squareIndex = 1UL << (rank * 8 + file);

                    if ((combinedBoard & squareIndex) == 0)
                        result.Add(new FileRank(file, rank));
                }
            }

            return result;
        }

        public List<FileRank> GetEmptySquares(int color)
        {
            int colorIndex = Piece.IsWhite(color) ? 0 : 1;
            ulong board = m_EmptySquares[colorIndex];
            
            List<FileRank> result = new List<FileRank>();

            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    ulong squareIndex = 1UL << (rank * 8 + file);

                    if ((board & squareIndex) == 0)
                        result.Add(new FileRank(file, rank));
                }
            }

            return result;
        }

        public List<FileRank> GetAllOccupiedSquares()
        {
            ulong combinedBoard = m_EmptySquares[0] | m_EmptySquares[1];

            List<FileRank> result = new List<FileRank>();

            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    ulong squareIndex = 1UL << (rank * 8 + file);

                    if ((combinedBoard & squareIndex) != 0)
                        result.Add(new FileRank(file, rank));
                }
            }

            return result;
        }

        public List<FileRank> GetOccupiedSquares(int color)
        {
            int colorIndex = Piece.IsWhite(color) ? 0 : 1;
            ulong board = m_EmptySquares[colorIndex];
            
            List<FileRank> result = new List<FileRank>();

            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    ulong squareIndex = 1UL << (rank * 8 + file);

                    if ((board & squareIndex) != 0)
                        result.Add(new FileRank(file, rank));
                }
            }

            return result;
        }

        public List<FileRank> GetFriendlySquares(int color)
        {
            int colorIndex = Piece.IsWhite(color) ? 0 : 1;
            ulong board = 0;

            List<FileRank> result = new List<FileRank>();

            for (int pieceType = 0; pieceType < 6; ++pieceType)
            {
                board |= m_Boards[colorIndex, pieceType];
            }

            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    ulong squareIndex = 1UL << (rank * 8 + file);

                    if ((m_EmptySquares[colorIndex] & squareIndex) == 0)
                        continue;

                    if ((board & squareIndex) != 0)
                        result.Add(new FileRank(file, rank));
                }
            }

            return result;
        }

        public List<FileRank> GetPiecePositions(int pieceType, int color)
        {
            pieceType -= 1;
            int colorIndex = Piece.IsWhite(color) ? 0 : 1;
            ulong board = m_Boards[colorIndex, pieceType];

            List<FileRank> result = new List<FileRank>();

            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    ulong squareIndex = 1UL << (rank * 8 + file);

                    if ((m_EmptySquares[colorIndex] & squareIndex) == 0)
                        continue;

                    if ((board & squareIndex) != 0)
                        result.Add(new FileRank(file, rank));
                }
            }

            return result;
        }

        public List<List<FileRank>> GetAllPieces(int color)
        {
            int colorIndex = Piece.IsWhite(color) ? 0 : 1;

            List<List<FileRank>> result = new List<List<FileRank>>();

            for (int pieceType = 0; pieceType < 6; ++pieceType)
            {
                result.Add(new List<FileRank>()); // Initialize list for each piece type
            }

            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    ulong squareIndex = 1UL << (rank * 8 + file);

                    if ((m_EmptySquares[colorIndex] & squareIndex) == 0)
                        continue;

                    for (int pieceType = 0; pieceType < 6; ++pieceType)
                    {
                        if ((m_Boards[colorIndex, pieceType] & squareIndex) != 0)
                            result[pieceType].Add(new FileRank(file, rank));
                    }
                }
            }

            return result;
        }
    }
}
