using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    List<AudioClip> m_ChessPlacements = new List<AudioClip>();

    private AudioSource m_AudioSource;

    // Start is called before the first frame update
    void Start()
    {
        m_AudioSource = gameObject.GetComponent<AudioSource>();
    }

    public void PlayPlacementSFX()
    {
        int rng = Random.Range(0, m_ChessPlacements.Count);
        m_AudioSource.clip = m_ChessPlacements[rng];
        m_AudioSource.Play();
    }
}
