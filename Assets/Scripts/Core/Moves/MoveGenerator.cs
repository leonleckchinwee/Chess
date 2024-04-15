using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public class MoveGenerator
    {
        public int      m_FriendlyColor { get; private set; }
        public int      m_OpponentColor { get; private set; }

        public BitBoard m_BitBoard      { get; private set; }

        public MoveGenerator(Board board)
        {
            m_FriendlyColor = board.m_CurrentColorTurn;
            m_OpponentColor = Piece.OpponentColor(m_FriendlyColor);

            m_BitBoard      = new BitBoard(board);
        }

        public void Update(Board board)
        {
            m_FriendlyColor = board.m_CurrentColorTurn;
            m_OpponentColor = Piece.OpponentColor(m_FriendlyColor);

            m_BitBoard      = new BitBoard(board);
        }

        public List<Move> GeneratePseudolegalMoves(int piece, int square)
        {
            return new List<Move>();
        }

        // Generate all legal moves given square position
        public MoveList GenerateMoves(int position)
        {
            if (position < 0 || position > 63)
                throw new ArgumentOutOfRangeException("position", $"Square index {position} is out of range!");

            int pieceType = m_BitBoard.GetPieceAt(position);

            if (pieceType == Piece.None)
            {
                Debug.Log($"No valid move at {position}");
                return MoveList.None;
            }

            MoveList result = new MoveList(Piece.PieceColor(pieceType));
            
            ulong currentPosition   = BitManip.SetBitAt(position);
            ulong opponentSquares   = m_BitBoard.GetAllPiecesOf(Piece.OpponentColor(pieceType));
            ulong emptySquares      = m_BitBoard.GetEmptySquares();

            ulong legalMoves        = 0;
            ulong captures          = 0;

            if (Piece.IsPawn(pieceType))
            {
                bool isWhite            = Piece.IsWhite(pieceType);
                int forwardDirection    = 8;

                if (isWhite)
                {
                    ulong whiteStartRank = PrecomputedBits.WhiteStartRankMasks; 

                    legalMoves |= (currentPosition << forwardDirection) & emptySquares;
                    
                    if ((currentPosition & whiteStartRank) != 0)
                        legalMoves |= (currentPosition << (forwardDirection * 2)) & (emptySquares << forwardDirection) & emptySquares;

                    captures |= (currentPosition << (forwardDirection + 1)) & opponentSquares | (currentPosition << (forwardDirection - 1)) & opponentSquares;

                    // No legal moves
                    if (legalMoves == 0)
                        return result;

                    result.PawnMoves.AddRange(BitManip.BitsToMoves(currentPosition, legalMoves));
                    result.PawnMoves.AddRange(BitManip.BitsToMoves(currentPosition, captures));

                    result.Captures.AddRange(BitManip.BitsToMoves(currentPosition, captures));
                }
                else
                {
                    ulong blackStartRank = PrecomputedBits.BlackStartRankMasks;

                    legalMoves |= (currentPosition >> forwardDirection) & emptySquares;

                    if ((currentPosition & blackStartRank) != 0)
                        legalMoves |= (currentPosition >> (forwardDirection * 2)) & (emptySquares >> forwardDirection) & emptySquares;

                    captures |= (currentPosition >> (forwardDirection + 1)) & opponentSquares | (currentPosition >> (forwardDirection - 1)) & opponentSquares;

                    // No legal moves
                    if (legalMoves == 0)
                        return result;

                    result.PawnMoves.AddRange(BitManip.BitsToMoves(currentPosition, legalMoves));
                    result.PawnMoves.AddRange(BitManip.BitsToMoves(currentPosition, captures));

                    result.Captures.AddRange(BitManip.BitsToMoves(currentPosition, captures));
                }

                // TODO: Pawn promotions
                // TODO: En-passant
            }
            else if (Piece.IsKnight(pieceType))
            {
                ulong targets   = PrecomputedBits.KnightMoves[position];

                int numTargets  = BitManip.PopCount(targets);
                int targetIndex = BitManip.LeastSigSetBit(targets);

                ulong targetPosition = BitManip.SetBitAt(targetIndex);

                for (int i = 0; i < numTargets; ++i)
                {
                    legalMoves |= targetPosition & emptySquares;
                    captures   |= targetPosition & opponentSquares;

                    if (legalMoves != 0)
                        result.KnightMoves.AddRange(BitManip.BitsToMoves(currentPosition, legalMoves));
                        
                    if (captures != 0)
                    {
                        result.KnightMoves.AddRange(BitManip.BitsToMoves(currentPosition, captures));
                        result.Captures.AddRange(BitManip.BitsToMoves(currentPosition, captures));
                    }

                    captures = 0;
                    legalMoves = 0;

                    targetIndex = BitManip.NextLeastSigSetBit(targets, targetIndex);    

                    if (targetIndex == -1)
                        continue;
                        
                    targetPosition = BitManip.SetBitAt(targetIndex); 
                } 
            }

            // TODO: other pieces

            return result;
        }

        // Generate all legal given color pawn moves
        public List<Move> GenerateAllPawnMoves(int color)
        {
            ulong originalPosition = m_BitBoard.GetPiecesOf(Piece.MakePiece(Piece.Pawn, color));

            if (originalPosition == 0)  // No pawns on board
                return new List<Move>();

            List<Move> result       = new List<Move>();

            ulong opponentSquares   = m_BitBoard.GetAllPiecesOf(Piece.OpponentColor(color));
            ulong emptySquares      = m_BitBoard.GetEmptySquares();

            int[] splitPositions    = BitManip.FindAllSetBitIndex(originalPosition);    // Split all positions

            bool isWhite            = Piece.IsWhite(color);  
            int forwardDirection    = 8; 
            ulong legalMoves        = 0;         

            if (isWhite)
            {
                ulong whiteStartRank = PrecomputedBits.WhiteStartRankMasks; // White pawns starting rank (A2 - H2)

                foreach (int index in splitPositions)
                {
                    ulong position = BitManip.SetBitAt(index);  // Current pawn position

                    legalMoves |= (position << forwardDirection) & emptySquares;    // One square forward

                    if ((position & whiteStartRank) != 0)   // Double advancement
                        legalMoves |= (position << (forwardDirection * 2)) & (emptySquares << forwardDirection) & emptySquares;

                    // Captures
                    legalMoves |= (position << (forwardDirection + 1)) & opponentSquares | (position << (forwardDirection - 1)) & opponentSquares;

                    // No legal moves
                    if (legalMoves == 0)
                        continue;

                    result.AddRange(BitManip.BitsToMoves(position, legalMoves));
                    legalMoves = 0;
                }
            }
            else
            {
                ulong blackStartRank = PrecomputedBits.BlackStartRankMasks; // Black pawns starting rank (A7 - H7)

                foreach (int index in splitPositions)
                {
                    ulong position = BitManip.SetBitAt(index);  // Current pawn position

                    legalMoves |= (position >> forwardDirection) & emptySquares;    // One square forward

                    if ((position & blackStartRank) != 0)   // Double advancement
                        legalMoves |= (position >> (forwardDirection * 2)) & (emptySquares >> forwardDirection) & emptySquares;

                    // Captures
                    legalMoves |= (position >> (forwardDirection + 1)) & opponentSquares | (position >> (forwardDirection - 1)) & opponentSquares;
     
                    if (legalMoves == 0)    // No legal moves
                        continue;

                    result.AddRange(BitManip.BitsToMoves(position, legalMoves));
                    legalMoves    = 0;
                }
            }

            // TODO: Pawn promotions
            // TODO: En-Passant

            return result;
        }
    
        public List<Move> GenerateAllKnightMoves(int color)
        {
            ulong originalPosition = m_BitBoard.GetPiecesOf(Piece.MakePiece(Piece.Knight, color));

            if (originalPosition == 0)  // No knights on board
                return new List<Move>();

            List<Move> result = new List<Move>();

            ulong opponentSquares   = m_BitBoard.GetAllPiecesOf(Piece.OpponentColor(color));
            ulong emptySquares      = m_BitBoard.GetEmptySquares();

            int[] splitPositions    = BitManip.FindAllSetBitIndex(originalPosition);    // Split all positions
            ulong legalMoves        = 0;

            foreach (int index in splitPositions)
            {
                ulong position  = BitManip.SetBitAt(index);             // Current knight position
                ulong targets   = PrecomputedBits.KnightMoves[index];   // Target positions

                int numTargets  = BitManip.PopCount(targets);           // Number of targets

                int targetIndex      = BitManip.LeastSigSetBit(targets);
                ulong targetPosition = BitManip.SetBitAt(targetIndex);

                // Max number of positions = 8
                for (int i = 0; i < numTargets; ++i)
                {
                    legalMoves |= targetPosition & emptySquares;    // Empty squares 
                    legalMoves |= targetPosition & opponentSquares; // Captures

                    if (legalMoves != 0)    // No legal moves
                        result.AddRange(BitManip.BitsToMoves(position, legalMoves));    

                    legalMoves  = 0;

                    // Find next available target
                    targetIndex = BitManip.NextLeastSigSetBit(targets, targetIndex);    

                    if (targetIndex == -1)
                        continue;
                        
                    targetPosition = BitManip.SetBitAt(targetIndex);    
                }
            }

            return result;
        }
    }
}
