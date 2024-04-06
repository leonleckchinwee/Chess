using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Game
{
    public class GameManager : MonoBehaviour
    {
        public enum PlayerType 
        {
            Human, AI
        }

        public PlayerType m_WhitePlayerType;
        public PlayerType m_BlackPlayerType;

        public Board m_Board { get; private set; }
        public ChessBoardUI m_BoardUI;

        public AudioManager m_AudioManager;

        private Player m_WhitePlayer;
        private Player m_BlackPlayer;

        // Start is called before the first frame update
        void Start()
        {
            m_Board = new Board();

            m_Board.LoadDefaultPosition();

            CreatePlayers();
        }

        // Update is called once per frame
        void Update()
        {
            UpdatePlayers();

            m_BoardUI.UpdateBoard(m_Board);
        }

        void CreatePlayers()
        {
            if (m_WhitePlayer != null)
                m_WhitePlayer.OnMoveChosen -= OnMoveChosen;
            if (m_BlackPlayer != null)
                m_BlackPlayer.OnMoveChosen -= OnMoveChosen;

            m_WhitePlayer = m_WhitePlayerType == PlayerType.Human ? new Player(m_Board, Piece.White) : new Player(m_Board, Piece.White, true);
            m_BlackPlayer = m_BlackPlayerType == PlayerType.Human ? new Player(m_Board, Piece.Black) : new Player(m_Board, Piece.Black, true);

            m_WhitePlayer.OnMoveChosen += OnMoveChosen;
            m_BlackPlayer.OnMoveChosen += OnMoveChosen;
        }

        void UpdatePlayers()
        {
            m_WhitePlayer.Update();
            m_BlackPlayer.Update();
        }

        void OnMoveChosen(Move move)
        {
            bool moved = m_Board.MakeMove(move);

            if (!moved)
                return;

            m_Board.ChangeTurn();
            
            m_AudioManager.PlayPlacementSFX();
        }
    }
}
