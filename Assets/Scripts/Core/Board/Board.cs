using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Chess
{
    public class Board
    {
        public int m_CurrentColorTurn;

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

            m_CurrentColorTurn = loadedPosition.m_WhiteTurnToMove ? Piece.White : Piece.Black;
        }

        public void MakeMove()
        {

        }

        public void UnmakeMove()
        {

        }

        public int GetPieceAt(int file, int rank)
        {
            return m_ChessBoard[file, rank];
        }

        public int GetPieceAt(FileRank position)
        {
            return m_ChessBoard[position.File, position.Rank];
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

        public void PlacePieceAt(FileRank position, int piece)
        {
            m_ChessBoard[position.File, position.Rank] = piece;
        }

        public void PlacePieceAt(int position, int piece)
        {
            BoardInfo.PositionToFileRank(position, out int file, out int rank);
            m_ChessBoard[file, rank] = piece;
        }

        public void RemovePieceAt(int file, int rank)
        {
            m_ChessBoard[file, rank] = Piece.None;
        }

        public void RemovePieceAt(FileRank position)
        {
            m_ChessBoard[position.File, position.Rank] = Piece.None;
        }

        public void RemovePieceAt(int position)
        {
            BoardInfo.PositionToFileRank(position, out int file, out int rank);
            m_ChessBoard[file, rank] = Piece.None;
        }
    
        public bool IsEmpty(int file, int rank)
        {
            return Piece.IsEmpty(m_ChessBoard[file, rank]);
        }

        public bool IsEmpty(FileRank position)
        {
            return IsEmpty(position.File, position.Rank);
        }
    }
}
