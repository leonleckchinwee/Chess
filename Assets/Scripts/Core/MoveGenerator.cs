using System;
using System.Collections.Generic;
using System.IO;
using Chess.Game;
using UnityEngine;

namespace Chess
{
    public class MoveGenerator
    {
        Board m_CurrentBoard;

        public readonly int m_FriendlyColor;
        public readonly int m_OpponentColor;

        public readonly MoveList m_FriendlyMoves;
        public readonly MoveList m_OpponentMoves;

        public readonly BitBoard m_BitBoard;

        public MoveGenerator(Board board)
        {
            m_CurrentBoard      = board;

            m_FriendlyColor     = board.m_CurrentColorTurn;
            m_OpponentColor     = Piece.OpponentColor(m_FriendlyColor);

            m_BitBoard          = new BitBoard(board);

            m_FriendlyMoves     = GenerateMovesFor(m_FriendlyColor);
            m_OpponentMoves     = GenerateMovesFor(m_OpponentColor);
        }

        public MoveList GenerateMovesFor(int friendlyColor)
        {
            MoveList moves = new MoveList(friendlyColor);

            int opponentColor = Piece.OpponentColor(friendlyColor);

            List<List<FileRank>> allPositions = m_BitBoard.GetAllPieces(friendlyColor);

            FileRank targetPos = new FileRank(0, 0);
            int targetPiece;

            // Pawn moves
            {
                int forwardDirection = Piece.IsWhite(friendlyColor) ? 1 : -1;

                foreach (FileRank pawnPosition in allPositions[0])
                {
                    // Out of bounds check
                    int posToCheck = pawnPosition.Rank + forwardDirection;
                    if (!IsValidIndex(posToCheck))
                        continue;

                    targetPos.Set(pawnPosition.File, posToCheck);
                    targetPiece = m_CurrentBoard.GetPieceAt(targetPos);

                    // Check for forward square being blocked
                    if (Piece.IsEmpty(targetPiece))
                    {
                        moves.m_PawnMoves.Add(new Move(pawnPosition, targetPos));
                        moves.m_AttackingSquares.Add(new Move(pawnPosition, targetPos));

                        // Only check double advance if first step is valid
                        // Double advance only available if pawn is at starting rank
                        int startRank = Piece.IsWhite(friendlyColor) ? 1 : 6;
                        
                        if (pawnPosition.Rank == startRank)
                        {
                            posToCheck = pawnPosition.Rank + forwardDirection * 2;     // No need check if out of bounds

                            targetPos.Set(pawnPosition.File, posToCheck);

                            targetPiece = m_CurrentBoard.GetPieceAt(targetPos);
                            
                            // Check if double advance square is blocked
                            if (Piece.IsEmpty(targetPiece))
                                moves.m_PawnMoves.Add(new Move(pawnPosition, targetPos));
                        }
                    }

                    // Check if opponent is within range (only check diagonally left and right)
                    for (int offset = -1; offset <= 1; offset += 2)
                    {
                        posToCheck = pawnPosition.File + offset;

                        // Out of bounds check
                        if (!IsValidIndex(posToCheck)) 
                            continue;

                        targetPos.Set(posToCheck, pawnPosition.Rank + forwardDirection);

                        targetPiece = m_CurrentBoard.GetPieceAt(targetPos);

                        // Can't attack empty square and friendly pieces
                        if (Piece.IsEmpty(targetPiece) || Piece.IsSameColor(friendlyColor, friendlyColor))
                            continue;

                        moves.m_PawnMoves.Add(new Move(pawnPosition, targetPos));
                        moves.m_AttackingSquares.Add(new Move(pawnPosition, targetPos));
                    }

                    // TODO: En-Passant...
                }
            }

            int file, rank;
            bool isValid;

            // Knight moves
            {
                // NE-Upper, NE-Lower, SE-Upper, SE-Lower, SW-Lower, SW-Upper, NW-Lower, NW-Upper
                int[] fileDirection = { 1, 2,  2,  1, -1, -2, -2, -1 };
                int[] rankDirection = { 2, 1, -1, -2, -2, -1,  1,  2 };

                foreach (FileRank knightPosition in allPositions[1])
                {
                    for (int i = 0; i < fileDirection.Length; ++i)
                    {
                        file = knightPosition.File + fileDirection[i];
                        rank = knightPosition.Rank + rankDirection[i];

                        // Out of bounds check, skip to next direction
                        if (!FileRank.IsValidFileRank(file, rank))
                            continue;

                        targetPiece = m_CurrentBoard.GetPieceAt(file, rank);

                        // Check is square is blocked, skip to next direction
                        if (!Piece.IsEmpty(targetPiece))
                        {
                            // Opponent square available to attack
                            if (Piece.IsSameColor(targetPiece, opponentColor))
                            {
                                moves.m_KnightMoves.Add(new Move(knightPosition, file, rank));
                                moves.m_AttackingSquares.Add(new Move(knightPosition, file, rank));
                            }
                            continue;
                        }

                        moves.m_KnightMoves.Add(new Move(knightPosition, file, rank));
                        moves.m_AttackingSquares.Add(new Move(knightPosition, file, rank));
                    }
                }
            }

            // Bishop moves
            {
                // NW, NE, SE, SW
                int[] fileDirection = { -1, 1,  1, -1 };
                int[] rankDirection = {  1, 1, -1, -1 };       

                foreach (FileRank bishopPosition in allPositions[2])
                {
                    for (int i = 0; i < fileDirection.Length; ++i)
                    {
                        file = bishopPosition.File + fileDirection[i];
                        rank = bishopPosition.Rank + rankDirection[i];

                        isValid = FileRank.IsValidFileRank(file, rank);

                        while (isValid)
                        {
                            targetPiece = m_CurrentBoard.GetPieceAt(file, rank);

                            // Check is square is blocked, skip to next direction
                            if (!Piece.IsEmpty(targetPiece))
                            {
                                // Opponent square available to attack
                                if (Piece.IsSameColor(targetPiece, opponentColor))
                                {
                                    moves.m_BishopMoves.Add(new Move(bishopPosition, file, rank));
                                    moves.m_AttackingSquares.Add(new Move(bishopPosition, file, rank));
                                }
                                break;
                            }

                            moves.m_BishopMoves.Add(new Move(bishopPosition, file, rank));        // Valid move
                            moves.m_AttackingSquares.Add(new Move(bishopPosition, file, rank));

                            file += fileDirection[i];
                            rank += rankDirection[i];

                            isValid = FileRank.IsValidFileRank(file, rank);
                        }
                    }
                }
            }

            // Rook moves
            {
                // N, E, S, W
                int[] fileDirection = { 0, 1,  0, -1 };
                int[] rankDirection = { 1, 0, -1,  0 };

                foreach (FileRank rookPosition in allPositions[3])
                {
                    for (int i = 0; i < fileDirection.Length; ++i)
                    {
                        file = rookPosition.File + fileDirection[i];
                        rank = rookPosition.Rank + rankDirection[i];

                        isValid = FileRank.IsValidFileRank(file, rank);

                        while (isValid)
                        {
                            targetPiece = m_CurrentBoard.GetPieceAt(file, rank);

                            // Check is square is blocked, skip to next direction
                            if (!Piece.IsEmpty(targetPiece))
                            {
                                // Opponent square available to attack
                                if (Piece.IsSameColor(targetPiece, opponentColor))
                                {
                                    moves.m_RookMoves.Add(new Move(rookPosition, file, rank));
                                    moves.m_AttackingSquares.Add(new Move(rookPosition, file, rank));
                                }
                                break;
                            }

                            moves.m_RookMoves.Add(new Move(rookPosition, file, rank));
                            moves.m_AttackingSquares.Add(new Move(rookPosition, file, rank));

                            file += fileDirection[i];
                            rank += rankDirection[i];

                            isValid = FileRank.IsValidFileRank(file, rank);
                        }
                    }
                }
            }

            // Queen moves
            {
                // N, NE, E, SE, S, SW, W, NW
                int[] fileDirection = { 0, 1, 1,  1,  0, -1, -1, -1 };
                int[] rankDirection = { 1, 1, 0, -1, -1, -1,  0,  1 };

                foreach (FileRank queenPosition in allPositions[4])
                {
                    for (int i = 0; i < fileDirection.Length; ++i)
                    {
                        file = queenPosition.File + fileDirection[i];
                        rank = queenPosition.Rank + rankDirection[i];

                        isValid = FileRank.IsValidFileRank(file, rank);

                        while (isValid)
                        {
                            targetPiece = m_CurrentBoard.GetPieceAt(file, rank);

                            // Check is square is blocked, skip to next direction
                            if (!Piece.IsEmpty(targetPiece))
                            {
                                // Opponent square available to attack
                                if (Piece.IsSameColor(targetPiece, opponentColor))
                                {
                                    moves.m_QueenMoves.Add(new Move(queenPosition, file, rank));
                                    moves.m_AttackingSquares.Add(new Move(queenPosition, file, rank));
                                }
                                break;
                            }

                            moves.m_QueenMoves.Add(new Move(queenPosition, file, rank));
                            moves.m_AttackingSquares.Add(new Move(queenPosition, file, rank));

                            file += fileDirection[i];
                            rank += rankDirection[i];

                            isValid = FileRank.IsValidFileRank(file, rank);
                        }
                    }
                }
            }

            // King moves
            {

            }

            return moves;
        }

        public List<Move> GenerateLegalPawnPositions(int color)
        {
            return Piece.IsSameColor(color, m_FriendlyColor) ? m_FriendlyMoves.m_PawnMoves : m_OpponentMoves.m_PawnMoves; 
        }

        public List<Move> GenerateLegalMovesAt(int file, int rank)
        {
            List<Move> moves = new List<Move>();

            int piece = m_CurrentBoard.GetPieceAt(file, rank);
            int pieceType = Piece.PieceType(piece);

            if (pieceType == Piece.Pawn)
            {   
                foreach (Move move in m_FriendlyMoves.m_PawnMoves)
                {
                    if (move.FromFile == file && move.FromRank == rank)
                        moves.Add(move);
                }
            }

            return moves;
        }

        public List<Move> GenerateLegalMovesAt(FileRank position)
        {
            return GenerateLegalMovesAt(position.File, position.Rank);
        }

        public List<FileRank> GenerateEmptySquares()
        {
            return m_BitBoard.GetAllEmptySquares();
        }

        public List<FileRank> GenerateFriendlySquares()
        {
            return m_BitBoard.GetFriendlySquares(m_FriendlyColor);
        }

        public List<FileRank> GenerateOpponentSquares()
        {
            return m_BitBoard.GetFriendlySquares(m_OpponentColor);
        }

        bool IsValidIndex(int index)
		{
            return index >= 0 && index < 8;
        }
    }
}
