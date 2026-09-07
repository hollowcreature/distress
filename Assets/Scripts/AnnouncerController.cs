using UnityEngine;

public class AnnouncerController : MonoBehaviour
{
    public static AnnouncerController Instance { get; private set; }
    [SerializeField] private AudioSource[] announcerSpeakers;
    [SerializeField] private AudioClip[] announcerVoicelines;

    void Awake()
    {
        Instance = this;
    }

    public void PlayVoiceline(int index, bool loop)
    {
        foreach (AudioSource speaker in announcerSpeakers)
        {
            speaker.generator = announcerVoicelines[index];
            speaker.loop = loop;
            speaker.Play();
        }
    }

    public void StopVoiceline()
    {
        foreach (AudioSource speaker in announcerSpeakers)
            speaker.Stop();
    }
}
