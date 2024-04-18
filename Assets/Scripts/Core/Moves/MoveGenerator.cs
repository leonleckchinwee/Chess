using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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

        // Generate all pesudo-legal moves given square position
        public MoveList GeneratePseudolegalMoves(int position)
        {
            if (position < 0 || position > 63)
                throw new ArgumentOutOfRangeException("position", $"Square index {position} is out of range!");

            int pieceType = m_BitBoard.GetPieceAt(position);

            if (pieceType == Piece.None)
            {
                Debug.Log($"No valid move at {position}!");
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
                    if (legalMoves != 0)
                    {
                        result.PawnMoves.AddRange(BitManip.BitsToMoves(currentPosition, legalMoves));
                    }

                    // No captures
                    if (captures != 0)
                    {
                        result.PawnMoves.AddRange(BitManip.BitsToMoves(currentPosition, captures));
                        result.Captures.AddRange(BitManip.BitsToMoves(currentPosition, captures));
                    }
                }
                else
                {
                    ulong blackStartRank = PrecomputedBits.BlackStartRankMasks;

                    legalMoves |= (currentPosition >> forwardDirection) & emptySquares;

                    if ((currentPosition & blackStartRank) != 0)
                        legalMoves |= (currentPosition >> (forwardDirection * 2)) & (emptySquares >> forwardDirection) & emptySquares;

                    captures |= (currentPosition >> (forwardDirection + 1)) & opponentSquares | (currentPosition >> (forwardDirection - 1)) & opponentSquares;

                    // No legal moves
                    if (legalMoves != 0)
                    {
                        result.PawnMoves.AddRange(BitManip.BitsToMoves(currentPosition, legalMoves));
                    }

                    // No captures
                    if (captures != 0)
                    {
                        result.PawnMoves.AddRange(BitManip.BitsToMoves(currentPosition, captures));
                        result.Captures.AddRange(BitManip.BitsToMoves(currentPosition, captures));
                    }
                }

                // TODO: Pawn promotions
                // TODO: En-passant
            }
            else if (Piece.IsKnight(pieceType))
            {
                ulong targets = PrecomputedBits.KnightMoves[position];
                ulong[] targetPositions = BitManip.FindAllSetBits(targets);

                foreach (ulong target in targetPositions)
                {
                    legalMoves |= target & emptySquares;
                    captures   |= target & opponentSquares;

                    if (legalMoves != 0)
                    {
                        result.KnightMoves.AddRange(BitManip.BitsToMoves(currentPosition, legalMoves));
                    }

                    if (captures != 0)
                    {
                        result.KnightMoves.AddRange(BitManip.BitsToMoves(currentPosition, captures));
                        result.Captures.AddRange(BitManip.BitsToMoves(currentPosition, captures));
                    }

                    legalMoves = 0;
                    captures   = 0;
                }
            }
            else if (Piece.IsBishop(pieceType))
            {
                Direction[] directions = { Direction.BottomLeft, Direction.BottomRight, Direction.TopLeft, Direction.TopRight };

                foreach (Direction direction in directions)
                {
                    ulong[] rays = RayMoves(currentPosition, direction, emptySquares, opponentSquares);

                    if (rays == null)
                        continue;

                    if (rays[0] != 0)
                    {
                        result.BishopMoves.AddRange(BitManip.BitsToMoves(currentPosition, rays[0]));
                    }

                    if (rays[1] != 0)
                    {
                        result.BishopMoves.AddRange(BitManip.BitsToMoves(currentPosition, rays[1]));
                        result.Captures.AddRange(BitManip.BitsToMoves(currentPosition, rays[1]));
                    }
                }
            }
            else if (Piece.IsRook(pieceType))
            {
                Direction[] directions = { Direction.Bottom, Direction.Left, Direction.Right, Direction.Top };

                foreach (Direction direction in directions)
                {
                    ulong[] rays = RayMoves(currentPosition, direction, emptySquares, opponentSquares);

                    if (rays == null)   // No valid move in direction
                        continue;

                    if (rays[0] != 0)   // Valid unblocked moves
                    {
                        result.RookMoves.AddRange(BitManip.BitsToMoves(currentPosition, rays[0]));
                    }

                    if (rays[1] != 0)   // Valid captures
                    {
                        result.RookMoves.AddRange(BitManip.BitsToMoves(currentPosition, rays[1]));
                        result.Captures.AddRange(BitManip.BitsToMoves(currentPosition, rays[1]));
                    }
                }
            }
            else if (Piece.IsQueen(pieceType))
            {
                Direction[] directions = { Direction.BottomLeft, Direction.Bottom, Direction.BottomRight,
                                           Direction.Left, Direction.Right, 
                                           Direction.TopLeft, Direction.Top, Direction.TopRight };

                foreach (Direction direction in directions)
                {
                    ulong[] rays = RayMoves(currentPosition, direction, emptySquares, opponentSquares);

                    if (rays == null)   // No valid move in direction
                        continue;

                    if (rays[0] != 0)   // Valid unblocked moves
                    {
                        result.QueenMoves.AddRange(BitManip.BitsToMoves(currentPosition, rays[0]));
                    }

                    if (rays[1] != 0)   // Valid captures
                    {
                        result.QueenMoves.AddRange(BitManip.BitsToMoves(currentPosition, rays[1]));
                        result.Captures.AddRange(BitManip.BitsToMoves(currentPosition, rays[1]));
                    }
                }
            }
            else if (Piece.IsKing(pieceType))
            {

            }

            return result;
        }

        public MoveList GenerateAllPseudolegalMoves()
        {
            throw new NotImplementedException();
        }

        // Generate all legal pawn moves for given color
        public List<Move> GenerateAllPawnMoves(int color)
        {
            ulong originalPosition = m_BitBoard.GetPiecesOf(Piece.MakePiece(Piece.Pawn, color));

            if (originalPosition == 0)  // No pawns on board
                return new List<Move>();

            List<Move> result = new List<Move>();

            ulong opponentSquares   = m_BitBoard.GetAllPiecesOf(Piece.OpponentColor(color));
            ulong emptySquares      = m_BitBoard.GetEmptySquares();

            int forwardDirection    = 8; 
            ulong legalMoves        = 0;  
            ulong captures          = 0;

            ulong[] splitPositions  = BitManip.FindAllSetBits(originalPosition);    // Split all positions

            if (Piece.IsWhite(color))
            {
                ulong whiteStartRank = PrecomputedBits.WhiteStartRankMasks; // White pawns starting rank (A2 - H2)

                foreach (ulong position in splitPositions)
                {
                    legalMoves |= (position << forwardDirection) & emptySquares;    // One square forward

                    if ((position & whiteStartRank) != 0)   // Double advancement
                        legalMoves |= (position << (forwardDirection * 2)) & (emptySquares << 8) & emptySquares;    

                    // Captures
                    captures |= (position << (forwardDirection + 1)) & opponentSquares | (position << (forwardDirection - 1)) & opponentSquares;

                    if (legalMoves != 0)    // Legal moves
                    {
                        result.AddRange(BitManip.BitsToMoves(position, legalMoves));
                    }

                    if (captures != 0)  // Valid captures
                    {
                        result.AddRange(BitManip.BitsToMoves(position, captures));
                    }

                    legalMoves = 0;
                    captures   = 0;
                }
            }
            else
            {
                ulong blackStartRank = PrecomputedBits.BlackStartRankMasks; //Black pawns starting rank (A7 - H7)

                foreach (ulong position in splitPositions)
                {
                    legalMoves |= (position >> forwardDirection) & emptySquares;    // One square forward

                    if ((position & blackStartRank) != 0)   // Double advancement
                        legalMoves |= (position >> (forwardDirection * 2)) & (emptySquares >> forwardDirection) & emptySquares;

                    // Captures
                    captures |= (position >> (forwardDirection + 1)) & opponentSquares | (position >> (forwardDirection - 1)) & opponentSquares;

                    if (legalMoves != 0)    // Legal moves
                    {
                        result.AddRange(BitManip.BitsToMoves(position, legalMoves));
                    }

                    if (captures != 0)  // Valid captures
                    {
                        result.AddRange(BitManip.BitsToMoves(position, captures));
                    }

                    legalMoves = 0;
                    captures   = 0;
                }
            }

            // TODO: Pawn promotions
            // TODO: En-Passant

            return result;
        }

        // Generate all legal knight moves for given color
        public List<Move> GenerateAllKnightMoves(int color)
        {
            ulong originalPosition = m_BitBoard.GetPiecesOf(Piece.MakePiece(Piece.Knight, color));

            if (originalPosition == 0)  // No knights on board
                return new List<Move>();

            List<Move> result = new List<Move>();

            ulong opponentSquares   = m_BitBoard.GetAllPiecesOf(Piece.OpponentColor(color));
            ulong emptySquares      = m_BitBoard.GetEmptySquares();

            ulong legalMoves        = 0;
            ulong captures          = 0;

            ulong[] splitPositions  = BitManip.FindAllSetBits(originalPosition);    // Split all positions

            foreach (ulong position in splitPositions)
            {
                int index       = BitManip.LeastSigSetBit(position);    // Current square index
                ulong targets   = PrecomputedBits.KnightMoves[index];   // Target positions

                ulong[] targetPositions = BitManip.FindAllSetBits(targets); // All valid target positions

                foreach (ulong target in targetPositions)
                {
                    legalMoves |= target & emptySquares;    // Empty square
                    captures   |= target & opponentSquares; // Captures
                    
                    if (legalMoves != 0)    // Legal moves
                    {
                        result.AddRange(BitManip.BitsToMoves(position, legalMoves));
                    }

                    if (captures != 0)  // Valid captures
                    {
                        result.AddRange(BitManip.BitsToMoves(position, captures));
                    }

                    legalMoves = 0;
                    captures   = 0;
                }
            }

            return result;
        }
    
        // Generate all legal bishop moves for given color
        public List<Move> GenerateAllBishopMoves(int color)
        {
            ulong originalPosition = m_BitBoard.GetPiecesOf(Piece.MakePiece(Piece.Bishop, color));

            if (originalPosition == 0)  // No bishops on board
                return new List<Move>();

            List<Move> result = new List<Move>();

            ulong opponentSquares   = m_BitBoard.GetAllPiecesOf(Piece.OpponentColor(color));
            ulong emptySquares      = m_BitBoard.GetEmptySquares();

            ulong[] splitPositions  = BitManip.FindAllSetBits(originalPosition);    // Split all positions

            Direction[] directions  = { Direction.BottomLeft, Direction.BottomRight, Direction.TopLeft, Direction.TopRight };

            foreach (ulong position in splitPositions)
            {
                foreach (Direction direction in directions)
                {
                    ulong[] rays = RayMoves(position, direction, emptySquares, opponentSquares);

                    if (rays == null)   // No valid move in direction
                        continue;

                    if (rays[0] != 0)   // Valid unblocked moves
                    {
                        result.AddRange(BitManip.BitsToMoves(position, rays[0]));
                    }

                    if (rays[1] != 0)   // Valid captures
                    {
                        result.AddRange(BitManip.BitsToMoves(position, rays[1]));
                    }
                }
            }

            return result;
        }

        // Generate all legal rook moves for given color
        public List<Move> GenerateAllRookMoves(int color)
        {
            ulong originalPosition = m_BitBoard.GetPiecesOf(Piece.MakePiece(Piece.Rook, color));

            if (originalPosition == 0)  // No rook on board
                return new List<Move>();

            List<Move> result = new List<Move>();

            ulong opponentSquares   = m_BitBoard.GetAllPiecesOf(Piece.OpponentColor(color));
            ulong emptySquares      = m_BitBoard.GetEmptySquares();

            ulong[] splitPositions  = BitManip.FindAllSetBits(originalPosition);    // Split all rooks into individual bitboards

            Direction[] directions  = { Direction.Bottom, Direction.Left, Direction.Right, Direction.Top };

            foreach (ulong position in splitPositions)
            {
                foreach (Direction direction in directions)
                {
                    ulong[] rays = RayMoves(position, direction, emptySquares, opponentSquares);

                    if (rays == null)   // No valid move in direction
                        continue;

                    if (rays[0] != 0)   // Valid unblocked moves
                    {
                        result.AddRange(BitManip.BitsToMoves(position, rays[0]));
                    }

                    if (rays[1] != 0)   // Valid captures
                    {
                        result.AddRange(BitManip.BitsToMoves(position, rays[1]));
                    }
                }
            }

            return result;
        }

        // Generate all legal queen moves for given color
        public List<Move> GenerateAllQueenMoves(int color)
        {
            ulong originalPosition = m_BitBoard.GetPiecesOf(Piece.MakePiece(Piece.Queen, color));

            if (originalPosition == 0)  // No queen on board
                return new List<Move>();

            List<Move> result = new List<Move>();

            ulong opponentSquares   = m_BitBoard.GetAllPiecesOf(Piece.OpponentColor(color));
            ulong emptySquares      = m_BitBoard.GetEmptySquares();
            ulong[] splitPositions  = BitManip.FindAllSetBits(originalPosition);

            Direction[] directions = { Direction.BottomLeft, Direction.Bottom, Direction.BottomRight,
                                       Direction.Left, Direction.Right, 
                                       Direction.TopLeft, Direction.Top, Direction.TopRight };

            foreach (ulong position in splitPositions)
            {
                foreach (Direction direction in directions)
                {
                    ulong[] rays = RayMoves(position, direction, emptySquares, opponentSquares);

                    if (rays == null)   // No valid move in direction
                        continue;

                    if (rays[0] != 0)   // Valid unblocked moves
                    {
                        result.AddRange(BitManip.BitsToMoves(position, rays[0]));
                    }

                    if (rays[1] != 0)   // Valid captures
                    {
                        result.AddRange(BitManip.BitsToMoves(position, rays[1]));
                    }
                }
            }

            return result;
        }

        // Generate all pseudo-legal king moves for given color
        public List<Move> GenerateAllKingMoves(int color)
        {
            ulong originalPosition = m_BitBoard.GetPiecesOf(Piece.MakePiece(Piece.King, color));    // There must only be one king on board

            if (originalPosition == 0 || BitManip.PopCount(originalPosition) != 1)  // No king on board
                throw new Exception("King must be on board!");

            List<Move> result = new List<Move>();

            int currentIndex        = BitManip.LeastSigSetBit(originalPosition);
            ulong opponentSquares   = m_BitBoard.GetAllPiecesOf(Piece.OpponentColor(color));
            ulong emptySquares      = m_BitBoard.GetEmptySquares();

            Direction[] directions = { Direction.BottomLeft, Direction.Bottom, Direction.BottomRight,
                                       Direction.Left, Direction.Right, 
                                       Direction.TopLeft, Direction.Top, Direction.TopRight };

            foreach (Direction direction in directions)
            {
                int index = currentIndex + (int)direction;

                if (!IsValid(index))
                    continue;

                ulong currentSquare = BitManip.SetBitAt(index);
                ulong validSquare = currentSquare & emptySquares;

                if (validSquare == 0)
                {
                    validSquare = currentSquare & opponentSquares;

                    if (validSquare != 0)
                    {
                        result.AddRange(BitManip.BitsToMoves(originalPosition, validSquare));
                    }

                    continue;                
                }

                result.AddRange(BitManip.BitsToMoves(originalPosition, validSquare));
            }

            return result;
        }

        // Cast a ray in given direction and returns (legal moves, captures)
        ulong[] RayMoves(ulong currentPosition, Direction currentDirection, ulong emptySquares, ulong opponentSquares)
        {
            if (BitManip.PopCount(currentPosition) != 1)
                throw new ArgumentException(nameof(currentPosition), "Position is invalid is do ray move!");

            int originalIndex = BitManip.LeastSigSetBit(currentPosition);
            int currentIndex = originalIndex + (int)currentDirection;
            if (!IsValid(currentIndex))
                return null;

            // Check if targets need to be sorted in reverse
            bool reverse = false;
            if (currentDirection == Direction.Left || currentDirection == Direction.Bottom || 
                currentDirection == Direction.BottomLeft || currentDirection == Direction.BottomRight)
                reverse = true;

            ulong ray           = 0;
            ulong currentSquare = BitManip.SetBitAt(currentIndex);
            ulong direction     = GetDirection(currentDirection, originalIndex);

            // Out of bounds check
            while ((currentSquare & direction) != 0)
            {
                ray |= currentSquare;   // Get all valid squares in given direction (in ascending index order)

                currentIndex += (int)currentDirection;

                if (!IsValid(currentIndex))
                    break;

                currentSquare = BitManip.SetBitAt(currentIndex);
            }

            if (ray == 0)
                return null;

            ulong[] targetPositions = BitManip.FindAllSetBits(ray); // Split into individual positions

            if (reverse)
                BitManip.SortReverse(ref targetPositions);

            ulong legalMoves = 0;
            ulong captures   = 0;

            foreach (ulong target in targetPositions)
            {
                ulong validSquare = target & emptySquares;

                if (validSquare == 0)
                {
                    validSquare = target & opponentSquares;

                    if (validSquare != 0)   // Valid capture target
                        captures |= validSquare;

                    break;
                }

                legalMoves |= validSquare;  // Valid empty square
            }

            return new ulong[2] { legalMoves, captures };
        }

        // Check if square index is within range (0 - 63)
        bool IsValid(int index)
        {
            return index >= 0 && index < 64;
        }

        // Get precomputed bits given direction and square index
        ulong GetDirection(Direction direction, int index)
        {
            if (!IsValid(index))
                throw new ArgumentOutOfRangeException(nameof(index), $"{index} : Square index out of range!");
            
            BoardInfo.PositionToFileRank(index, out int file, out int rank);

            return direction switch
            {
                Direction.Top or Direction.Bottom => PrecomputedBits.FileMasks[file],
                Direction.Left or Direction.Right => PrecomputedBits.RankMasks[rank],
                Direction.TopLeft or Direction.TopRight or Direction.BottomRight or Direction.BottomLeft => PrecomputedBits.DiagonalMoves[index],
                _ => 0,
            };
        }
    }
}
