using UnityEngine;

public class CommsMonitor : MonoBehaviour
{
    [SerializeField] private Renderer screenRenderer;
    [SerializeField] private Color screenColor = Color.green;
    [SerializeField] private float screenIntensity = 1.5f;

    public void PowerOn()
    {
        screenRenderer.material.EnableKeyword("_EMISSION");
        screenRenderer.material.SetColor("_EmissionColor", screenColor * screenIntensity);
    }
}
