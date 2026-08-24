using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField] public AudioSource shipHum;
    [SerializeField] public AudioSource genHum;

    void Awake() => Instance = this;
}
