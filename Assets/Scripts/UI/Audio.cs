using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public class Audio : MonoBehaviour
    {
        [SerializeField]
        List<AudioClip>     m_PlacementSounds;  // Audio for piece placements  

        [SerializeField]
        List<AudioClip>     m_CapturedSounds;   // Audio for capturing

        [SerializeField]
        List<AudioClip>     m_Bgm;              // Background music

        AudioSource         m_Audio;
    
        void Start()
        {
            m_Audio = GetComponent<AudioSource>();
        }

        public void PlayPlacementSfx()
        {
            int rng = Random.Range(0, m_PlacementSounds.Count);

            m_Audio.clip = m_PlacementSounds[rng];
            m_Audio.Play();
        }
        
        public void PlayCapturedSfx()
        {

        }
    }
}
