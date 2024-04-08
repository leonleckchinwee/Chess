using UnityEngine.UI;
using UnityEngine;

namespace Chess
{
    public class DebugDrawing : MonoBehaviour
    {
        public enum DebugType
        {
            None, SquareInfo, ChessPosition
        }

        public DebugType    m_DebugType = DebugType.None;
        public float        m_UiDepth = 0.0f; 

        private Text        m_DebugModeHeadsUp;
        private Text[,]     m_Texts;
        private bool        m_WhiteIsBottom;

        void Start()
        {
            m_Texts = new Text[8, 8];
            m_WhiteIsBottom = FindObjectOfType<ChessBoardUI>().m_WhiteIsBottom;

            InitializeDebugHUD ();

            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    InitializeDebugDrawing (rank, file);
                } 
            }

            if (m_DebugType != DebugType.None)
                DrawDebug ();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                m_DebugType = (m_DebugType == DebugType.None) ? DebugType.SquareInfo : DebugType.None;
                DrawDebug ();
            }

            if (Input.GetKeyDown(KeyCode.Tab) && m_DebugType != DebugType.None)
            {
                CycleDebugType ();
                DrawDebug ();
            }
        }

        void CycleDebugType ()
        {
            int length = System.Enum.GetValues (typeof (DebugType)).Length;
            int nextIndex = (int)(m_DebugType + 1) % length;

            if ((DebugType)nextIndex == DebugType.None)
                ++nextIndex;

            m_DebugType = (DebugType)nextIndex;
        }

        void InitializeDebugDrawing (int rank, int file)
        {
            GameObject textTransform = new GameObject($"({rank}, {file})");
            m_Texts[rank, file] = textTransform.AddComponent<Text>();

            m_Texts[rank, file].transform.position = BoardInfo.GetWorldPositionFromCoordinates (rank, file, m_UiDepth, m_WhiteIsBottom);
            m_Texts[rank, file].transform.SetParent (transform, true);
            m_Texts[rank, file].transform.localScale = Vector3.one;

            m_Texts[rank, file].font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            m_Texts[rank, file].color = Color.red;
        }

        void InitializeDebugHUD ()
        {
            GameObject headsUp = new GameObject("Debug Mode");
            
            m_DebugModeHeadsUp = headsUp.AddComponent<Text>();
            m_DebugModeHeadsUp.rectTransform.sizeDelta = new Vector2(500f, 100f);

            m_DebugModeHeadsUp.transform.SetParent (transform, true);
            m_DebugModeHeadsUp.transform.position = new Vector3(0f, 4.5f, m_UiDepth);
            m_DebugModeHeadsUp.transform.localScale = Vector3.one;
            
            m_DebugModeHeadsUp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            m_DebugModeHeadsUp.alignment = TextAnchor.MiddleCenter;
            m_DebugModeHeadsUp.fontSize = 50;
            m_DebugModeHeadsUp.color = Color.red;
        }

        void DrawDebug ()
        {
            DrawDebugHUD ();

            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    switch (m_DebugType)
                    {
                        case DebugType.SquareInfo:
                            DrawSquareInfo (rank, file);
                            break;
                        
                        case DebugType.ChessPosition:
                            DrawChessPosition (rank, file);
                            break;

                        default:
                        case DebugType.None:
                            ClearDebugDrawing (rank, file);
                            break;
                    }
                } 
            }
        }

        void DrawSquareInfo (int rank, int file)
        {
            m_Texts[rank, file].alignment = TextAnchor.MiddleLeft;
            m_Texts[rank, file].fontSize = 24;
            m_Texts[rank, file].text = $"Rank: {rank + 1}\nFile: {file + 1}\nIndex: {rank * 8 + file}";
        }

        void DrawChessPosition (int rank, int file)
        {
            m_Texts[rank, file].alignment = TextAnchor.MiddleCenter;
            m_Texts[rank, file].fontSize = 40;
            m_Texts[rank, file].text = BoardInfo.GetPositionNameFromCoordinates (rank, file);
        }

        void DrawDebugHUD ()
        {
            m_DebugModeHeadsUp.text = "DEBUG MODE ON";
        }

        void ClearDebugDrawing (int rank, int file)
        {
            m_DebugModeHeadsUp.text = "";
            m_Texts[rank, file].text = "";
        }
    }
}
