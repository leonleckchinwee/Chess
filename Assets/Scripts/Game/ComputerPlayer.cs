using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Game
{
    public class ComputerPlayer : Player
    {
        Board m_Board;

        public ComputerPlayer(Board board)
        {
            m_Board = board;
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}
