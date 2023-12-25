using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource soundEffectSource;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip[] effectClips;
    [SerializeField] private AudioClip[] bgmclips;

    public void PlaySound(int num)
    {
        switch (num)
        {
            case 0:
                soundEffectSource.volume = 0.3f;
                soundEffectSource.clip = effectClips[0];
                soundEffectSource.PlayOneShot(soundEffectSource.clip);
                break;
            case 1:
                bgmSource.volume = 1f;
                bgmSource.clip = bgmclips[0];
                bgmSource.PlayOneShot(bgmSource.clip);
                break;
            default:
                break;
        }
    }
}
