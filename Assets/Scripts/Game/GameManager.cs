using System.Collections;
using System.Collections.Generic;
using Chess;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Board m_Board;
    public ChessBoardUI m_BoardUI;

    // Start is called before the first frame update
    void Start()
    {
        m_Board = new Board ();
        m_Board.LoadDefaultStartingPosition ();
    }

    // Update is called once per frame
    void Update()
    {
        m_BoardUI.UpdatePieces (m_Board);
    }
}
