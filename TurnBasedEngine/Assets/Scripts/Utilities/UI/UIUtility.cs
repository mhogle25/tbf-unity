using UnityEngine;

public abstract class UIUtility : MonoBehaviour
{
    public virtual bool Interactable {
        get 
        {
            return this.interactable;
        }

        set
        {
            this.interactable = value;
        }
    }
    private protected bool interactable = true;

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
