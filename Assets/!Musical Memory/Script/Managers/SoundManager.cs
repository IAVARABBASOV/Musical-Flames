using System.Collections;
using System.Collections.Generic;
using IA.Utils;
using UnityEngine;

namespace MusicalMemory.SoundSystem
{
    public class SoundManager : Singleton<SoundManager>
    {
        [SerializeField] private AudioSource source;

        public void PlaySoundClip(AudioClip _clip)
        {
            source.PlayOneShot(_clip);
        }
    }
}
