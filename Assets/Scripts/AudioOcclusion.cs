using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioLowPassFilter))]
public class AudioOcclusion : MonoBehaviour
{
    [SerializeField] private LayerMask occlusionLayers;
    [SerializeField] private float openCutoff = 22000f;
    [SerializeField] private float occludedCutoff = 800f;
    [SerializeField] private float smoothSpeed = 5f;

    private AudioLowPassFilter lowPass;
    private Transform listener;

    void Awake()
    {
        lowPass = GetComponent<AudioLowPassFilter>();
        lowPass.cutoffFrequency = openCutoff;
    }

    void Start()
    {
        var listenerObj = FindFirstObjectByType<AudioListener>();
        if (listenerObj != null) listener = listenerObj.transform;
    }

    void Update()
    {
        if (listener == null) return;

        Vector3 dir = listener.position - transform.position;
        bool blocked = Physics.Raycast(transform.position, dir.normalized, dir.magnitude, occlusionLayers);
        float target = blocked ? occludedCutoff : openCutoff;
        lowPass.cutoffFrequency = Mathf.Lerp(lowPass.cutoffFrequency, target, Time.deltaTime * smoothSpeed);
    }
}
