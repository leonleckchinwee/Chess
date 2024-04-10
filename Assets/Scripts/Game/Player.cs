using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Game
{
    public abstract class Player
    {
        public event Action<Move> OnMoveSelected;
        public event Action<Move> OnPiecePlace;

        public abstract void Update();

        public virtual void OnMoveSelect(Move move)
        {
            OnMoveSelected?.Invoke(move);
        }

        public virtual void OnTryPlacePiece(Move move)
        {
            OnPiecePlace?.Invoke(move);
        }
    }
}
