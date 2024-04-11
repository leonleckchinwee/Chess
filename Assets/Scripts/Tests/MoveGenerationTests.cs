using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Chess;
using System.Diagnostics;

public class MoveGenerationTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void GeneratePawnMoveTest()
    {
        Board board = new Board();
        board.InitializeDefaultStartingPosition();

        Stopwatch watch = new Stopwatch();
        watch.Start();

        MoveGenerator generator = new MoveGenerator(board);
        generator.GeneratePawnMoves(Piece.White);

        watch.Stop();

        UnityEngine.Debug.Log($"Generate Pawn Move Test: {watch.Elapsed.TotalMilliseconds}ms");
    }

    [Test]
    public void GenerateAllMoveTest()
    {
        Board board = new Board();
        board.InitializeDefaultStartingPosition();

        Stopwatch watch = new Stopwatch();
        watch.Start();

        MoveGenerator generator = new MoveGenerator(board);
        var a = generator.GenerateAllMovesFor(Piece.White);

        watch.Stop();

        UnityEngine.Debug.Log($"Generate Pawn Move Test: {watch.Elapsed.TotalMilliseconds}ms");

        foreach (var b in a.m_KnightMoves)
        {
            UnityEngine.Debug.Log(b);
        }
    }
}
