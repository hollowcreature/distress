using UnityEngine;

public class EvilDoorSound : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioSource evilSound;

    private void playEvilSound()
    {
        if (evilSound.isPlaying)
            return;

        evilSound.Play();
    }
    public void Interact() => playEvilSound();
}
