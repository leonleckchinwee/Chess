using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Chess;
using System.Diagnostics;

public class MoveGenerationTests
{
    [Test]
    public void GenerateAllMoveTest()
    {
        string testFEN = "rnbqkb1r/pp1p1ppp/5n2/2p1p3/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2 ";

        PrecomputedBits.Initialize();

        Board board = new Board();
        board.InitializePosition(testFEN);
        // board.InitializeDefaultStartingPosition();

        Stopwatch watch = Stopwatch.StartNew();

        MoveGenerator generator = new MoveGenerator(board);
        var b = generator.GenerateMoves(Square.F3);

        watch.Stop();
        UnityEngine.Debug.Log($"Time taken: {watch.Elapsed.TotalMilliseconds}ms");

        foreach (var a in b.KnightMoves)
        {
            UnityEngine.Debug.Log(a);
        }
    }
}
