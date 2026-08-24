using UnityEngine;

public class EmergencyLightPulse : MonoBehaviour
{
    [SerializeField] private float minIntensity = 0.1f;
    [SerializeField] private float maxIntensity = 0.4f;
    [SerializeField] private float speed = 1.2f;
    [SerializeField] private Renderer emissiveStrip;
    [SerializeField] private Color emissiveColor = Color.red;

    private Light lt;

    void Awake() => lt = GetComponent<Light>();

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
        if (lt != null)
            lt.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        if (emissiveStrip != null)
            emissiveStrip.material.SetColor("_EmissionColor", emissiveColor * Mathf.Lerp(minIntensity, maxIntensity, t));
    }
}
