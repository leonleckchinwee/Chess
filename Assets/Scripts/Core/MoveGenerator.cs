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

        public int      m_FriendlyColor { get; private set; }
        public int      m_OpponentColor { get; private set; }

        public MoveList m_FriendlyMoves { get; private set; }
        public MoveList m_OpponentMoves { get; private set; }

        public BitBoard m_BitBoard      { get; private set; }

        public MoveGenerator(Board board)
        {
            m_CurrentBoard      = board;

            m_FriendlyColor     = board.m_CurrentColorTurn;
            m_OpponentColor     = Piece.OpponentColor(m_FriendlyColor);

            m_BitBoard          = new BitBoard(board);

            m_FriendlyMoves     = new MoveList(m_FriendlyColor);
            m_OpponentMoves     = new MoveList(m_OpponentColor);
        }

        public void UpdateGenerator(Board board)
        {
            m_CurrentBoard  = board;

            m_FriendlyColor = board.m_CurrentColorTurn;
            m_OpponentColor = Piece.OpponentColor(m_FriendlyColor);

            m_BitBoard      = new BitBoard(board);  // TODO: clear and reset

            m_FriendlyMoves.Clear();
            m_OpponentMoves.Clear();
        }

        public MoveList GenerateAllMovesFor(int friendlyColor)
        {
            MoveList moves      = new MoveList(friendlyColor);
            FileRank targetPos  = new FileRank();

            int pieceType;
            int targetPiece;

            // Pawn moves
            {
                pieceType = Piece.Pawn | friendlyColor;

                int forwardDirection = Piece.IsWhite(friendlyColor) ? 1 : -1;

                List<FileRank> pawns = m_BitBoard.GetAllPiecesOf(pieceType);
                
                foreach (FileRank pawn in pawns)
                {
                    // Out of bounds check
                    int posToCheck = pawn.Rank + forwardDirection;
                    if (!IsValidIndex(posToCheck))
                        continue;

                    targetPos.Set(pawn.File, posToCheck);
                    targetPiece = m_CurrentBoard.GetPieceAt(targetPos);

                    // Check if forward square is blocked
                    if (Piece.IsEmpty(targetPiece))
                    {
                        moves.m_PawnMoves.Add(new Move(pawn, targetPos));
                        moves.m_AttackingSquares.Add(new Move(pawn, targetPos));

                        // Double advance check (only available if pawn at starting rank)
                        if (pawn.Rank == (Piece.IsWhite(friendlyColor) ? 1 : 6))
                        {
                            posToCheck = pawn.Rank + forwardDirection * 2;

                            targetPos.Set(pawn.File, posToCheck);
                            targetPiece = m_CurrentBoard.GetPieceAt(targetPos);

                            if (Piece.IsEmpty(targetPiece))
                                moves.m_PawnMoves.Add(new Move(pawn, targetPos));
                        }
                    }

                    // Check if opponent within attack range (only check diagonally left and right)
                    for (int offset = -1; offset <= 1; offset += 2)
                    {
                        posToCheck = pawn.File + offset;

                        // Out of bounds check
                        if (!IsValidIndex(posToCheck))
                            continue;

                        targetPos.Set(posToCheck, pawn.Rank + forwardDirection);
                        targetPiece = m_CurrentBoard.GetPieceAt(targetPos);

                        // Can't attack empty squares or friendly pieces
                        if (Piece.IsEmpty(targetPiece) || Piece.IsSameColor(targetPiece, friendlyColor))
                            continue;

                        moves.m_PawnMoves.Add(new Move(pawn, targetPos));
                        moves.m_AttackingSquares.Add(new Move(pawn, targetPos));
                    }
                }
            }

            int file, rank;
            bool isValid;

            // Knight moves
            {
                pieceType = Piece.Knight | m_FriendlyColor;

                // NE-Upper, NE-Lower, SE-Upper, SE-Lower, SW-Lower, SW-Upper, NW-Lower, NW-Upper
                int[] fileDirection = { 1, 2,  2,  1, -1, -2, -2, -1 };
                int[] rankDirection = { 2, 1, -1, -2, -2, -1,  1,  2 };

                List<FileRank> knights = m_BitBoard.GetAllPiecesOf(pieceType);

                foreach (FileRank knight in knights)
                {
                    for (int i = 0; i < fileDirection.Length; ++i)
                    {
                        file = knight.File + fileDirection[i];
                        rank = knight.Rank + rankDirection[i];

                        // Out of bounds check
                        if (!FileRank.IsValidFileRank(file, rank))
                            continue;

                        targetPos.Set(file, rank);
                        targetPiece = m_CurrentBoard.GetPieceAt(file, rank);

                        // Check if square is occupied
                        if (!Piece.IsEmpty(targetPiece))
                        {
                            // Valid opponent square to attack
                            if (!Piece.IsSameColor(targetPiece, friendlyColor))
                            {
                                moves.m_KnightMoves.Add(new Move(knight, targetPos));
                                moves.m_AttackingSquares.Add(new Move(knight, targetPos));
                            }

                            continue;
                        }

                        // Empty squares valid to move
                        moves.m_KnightMoves.Add(new Move(knight, targetPos));
                        moves.m_AttackingSquares.Add(new Move(knight, targetPos));
                    }
                }
            }

            // Sliding Pieces
            {
                int[] pieceTypes = 
                { 
                    Piece.Bishop | m_FriendlyColor, 
                    Piece.Rook | m_FriendlyColor,
                    Piece.Queen | m_FriendlyColor
                };

                // N, NE, E, SE, S, SW, W, NW
                int[] fileDirection = { 0, 1, 1,  1,  0, -1, -1, -1 };
                int[] rankDirection = { 1, 1, 0, -1, -1, -1,  0,  1 };

                List<FileRank> positions = m_BitBoard.GetAllPiecesOf(pieceTypes);

                foreach (FileRank position in positions)
                {
                    pieceType = m_CurrentBoard.GetPieceAt(position);

                    Debug.Log(position);

                    for (int i = 0; i < fileDirection.Length; ++i)
                    {
                        file = position.File + fileDirection[i];
                        rank = position.Rank + rankDirection[i];

                        isValid = FileRank.IsValidFileRank(file, rank);

                        while (isValid)
                        {
                            targetPos.Set(file, rank);
                            targetPiece = m_CurrentBoard.GetPieceAt(targetPos);

                            bool isDiagonal = (i % 2 != 0);

                            if (!Piece.IsEmpty(targetPiece))
                            {
                                if (!Piece.IsSameColor(targetPiece, friendlyColor))
                                {
                                    if (isDiagonal)
                                    {
                                        if (Piece.IsBishop(pieceType))
                                            moves.m_BishopMoves.Add(new Move(position, targetPos));

                                        if (Piece.IsQueen(pieceType))
                                            moves.m_QueenMoves.Add(new Move(position, targetPos));
                                    }
                                    else
                                    {
                                        if (Piece.IsRook(pieceType))
                                            moves.m_RookMoves.Add(new Move(position, targetPos));

                                        if (Piece.IsQueen(pieceType))
                                            moves.m_QueenMoves.Add(new Move(position, targetPos));
                                    }

                                    moves.m_AttackingSquares.Add(new Move(position, targetPos));
                                }

                                break;
                            }

                            if (isDiagonal)
                            {
                                if (Piece.IsBishop(pieceType))
                                    moves.m_BishopMoves.Add(new Move(position, targetPos));

                                if (Piece.IsQueen(pieceType))
                                    moves.m_QueenMoves.Add(new Move(position, targetPos));
                            }
                            else
                            {
                                if (Piece.IsRook(pieceType))
                                    moves.m_RookMoves.Add(new Move(position, targetPos));

                                if (Piece.IsQueen(pieceType))
                                    moves.m_QueenMoves.Add(new Move(position, targetPos));
                            }

                            file += fileDirection[i];
                            rank += rankDirection[i];

                            isValid = FileRank.IsValidFileRank(file, rank);
                        }                   
                    }
                }
            }

            return moves;
        }

        public List<Move> GeneratePawnMoves(int friendlyColor)
        {
            int pieceType        = Piece.Pawn | friendlyColor;
            int forwardDirection = Piece.IsWhite(friendlyColor) ? 1 : -1;
            int targetPiece;

            List<Move> moves     = new List<Move>();
            FileRank targetPos   = new FileRank();
            List<FileRank> pawns = m_BitBoard.GetAllPiecesOf(pieceType);
            
            foreach (FileRank pawn in pawns)
            {
                // Out of bounds check
                int posToCheck = pawn.Rank + forwardDirection;
                if (!IsValidIndex(posToCheck))
                    continue;

                targetPos.Set(pawn.File, posToCheck);
                targetPiece = m_CurrentBoard.GetPieceAt(targetPos);

                // Check if forward square is blocked
                if (Piece.IsEmpty(targetPiece))
                {
                    moves.Add(new Move(pawn, targetPos));

                    // Double advance check (only available if pawn at starting rank)
                    if (pawn.Rank == (Piece.IsWhite(friendlyColor) ? 1 : 6))
                    {
                        posToCheck = pawn.Rank + forwardDirection * 2;

                        targetPos.Set(pawn.File, posToCheck);
                        targetPiece = m_CurrentBoard.GetPieceAt(targetPos);

                        if (Piece.IsEmpty(targetPiece))
                            moves.Add(new Move(pawn, targetPos));
                    }
                }

                // Check if opponent within attack range (only check diagonally left and right)
                for (int offset = -1; offset <= 1; offset += 2)
                {
                    posToCheck = pawn.File + offset;

                    // Out of bounds check
                    if (!IsValidIndex(posToCheck))
                        continue;

                    targetPos.Set(posToCheck, pawn.Rank + forwardDirection);
                    targetPiece = m_CurrentBoard.GetPieceAt(targetPos);

                    // Can't attack empty squares or friendly pieces
                    if (Piece.IsEmpty(targetPiece) || Piece.IsSameColor(targetPiece, friendlyColor))
                        continue;

                    moves.Add(new Move(pawn, targetPos));
                }
            }

            return moves;
        }

        public List<FileRank> GenerateEmptySquares()
        {
            return m_BitBoard.GetEmptySquares();
        }

        public List<FileRank> GenerateFriendlySquares()
        {
            List<FileRank> squares = new List<FileRank>();

            foreach (FileRank position in m_BitBoard.GetOccupiedSquares())
            {
                int piece = m_CurrentBoard.GetPieceAt(position);

                if (Piece.IsSameColor(piece, m_FriendlyColor))
                    squares.Add(position);
            }

            return squares;
        }

        public List<FileRank> GenerateOpponentSquares()
        {
            List<FileRank> squares = new List<FileRank>();

            foreach (FileRank position in m_BitBoard.GetOccupiedSquares())
            {
                int piece = m_CurrentBoard.GetPieceAt(position);

                if (!Piece.IsSameColor(piece, m_FriendlyColor))
                    squares.Add(position);
            }

            return squares;
        }

        bool IsValidIndex(int index)
		{
            return index >= 0 && index < 8;
        }
    }
}
