using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Chess
{
    public class Board
    {
        int[,] m_ChessBoard;


        public Board()
        {
            m_ChessBoard = new int[8, 8];


        }

        public void InitializeDefaultStartingPosition()
        {
            InitializePosition(FEN.m_StartingFEN);
        }

        public void InitializePosition(string fen)
        {
            FEN.PositionInfo loadedPosition = FEN.LoadPositionFromFEN(fen);

            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    PlacePieceAt(file, rank, loadedPosition.m_Squares[file, rank]);
                }
            }
        }

        public int GetPieceAt(int file, int rank)
        {
            return m_ChessBoard[file, rank];
        }

        public int GetPieceAt(int position)
        {
            BoardInfo.PositionToFileRank(position, out int file, out int rank);
            return m_ChessBoard[file, rank];
        }

        public void PlacePieceAt(int file, int rank, int piece)
        {
            m_ChessBoard[file, rank] = piece;
        }

        public void PlacePieceAt(int position, int piece)
        {
            BoardInfo.PositionToFileRank(position, out int file, out int rank);
            m_ChessBoard[file, rank] = piece;
        }
    }
}
