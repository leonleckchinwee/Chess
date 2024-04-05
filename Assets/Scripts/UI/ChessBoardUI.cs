using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public class ChessBoardUI : MonoBehaviour
    {
        public PiecePrefab Pieces;
        public BoardPrefab Squares;

        public MeshRenderer[,] SquareRenderers;
        public SpriteRenderer[,] PieceRenderers;

        public bool WhiteIsBottom = false;

        const float BoardDepth = 0.0f;
        const float PieceDepth = -1.0f;
        const float PieceFloatingDepth = -2.0f;

        void Awake()
        {
            CreateBoard();
        }

        void CreateBoard()
        {
            Shader shader = Shader.Find("Unlit/Color");

            SquareRenderers = new MeshRenderer[8, 8];
            PieceRenderers = new SpriteRenderer[8, 8];

            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    Transform square = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;

                    square.parent = transform;  // Parent-child for neatness
                    square.name = BoardInfo.GetNameFromCoordinates(rank, file);
                    square.position = GetPositionFromCoordinates(rank, file, BoardDepth);

                    SquareRenderers[file, rank] = square.gameObject.GetComponent<MeshRenderer>();

                    Material material = new Material(shader);
                    SquareRenderers[file, rank].material = material;

                    SpriteRenderer piece = new GameObject("Piece").AddComponent<SpriteRenderer>();
                    piece.transform.parent = square;
                    piece.transform.position = GetPositionFromCoordinates(rank, file, PieceDepth);
                    piece.transform.localScale = Vector3.one * 100.0f / (2000.0f / 6.0f);
                
                    PieceRenderers[file, rank] = piece;
                }
            }

            ResetSquareColor();
        }

        Vector3 GetPositionFromCoordinates(int rank, int file, float depth = 0.0f)
        {
            if (WhiteIsBottom)
                return new Vector3(-3.5f + file, -3.5f + rank, depth);

            return new Vector3(-3.5f + 7 - file, 7 - rank - 3.5f, depth);
        }

        Vector3 GetPositionFromCoordinates(Coordinates coordinates, float depth)
        {
            return GetPositionFromCoordinates(coordinates.RankIndex, coordinates.FileIndex, depth);
        }

        void ResetSquareColor(bool highlight = true)
        {
            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    SetSquareColor(rank, file, Squares.LightSquares.Normal, Squares.DarkSquares.Normal);
                }
            }

            if (highlight)
            {
                // TODO...
            }
        }

        void SetSquareColor(Coordinates coordinates, Color light, Color dark)
        {
            SquareRenderers[coordinates.FileIndex, coordinates.RankIndex].material.color = (coordinates.IsLightSquare()) ? light : dark;
        }

        void SetSquareColor(int rank, int file, Color light, Color dark)
        {
            Coordinates coordinates = new Coordinates(rank, file);
            SetSquareColor(coordinates, light, dark);
        }
    }
}
