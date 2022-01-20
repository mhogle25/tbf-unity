
using UnityEngine;

public class UIUtility : MonoBehaviour
{
    public virtual bool Interactable {
        get 
        {
            return _interactable;
        }

        set
        {
            _interactable = value;
        }
    }
    private protected bool _interactable = true;

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
