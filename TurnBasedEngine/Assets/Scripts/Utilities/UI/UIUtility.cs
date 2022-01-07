using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUtility : MonoBehaviour
{
    private protected void PlayAudioSource(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            if (audioSource.clip != null)
            {
                audioSource.Play();
            }

        }
    }
}
