using UnityEngine;

public class CommsMonitor : MonoBehaviour
{
    [SerializeField] private ModuleTask moduleTask;
    [SerializeField] private Renderer screenRenderer;
    [SerializeField] private Color screenColor = Color.green;
    [SerializeField] private float screenIntensity = 1.5f;
    [SerializeField] private TMPro.TMP_Text screenText;

    void Awake()
    {
        moduleTask.OnRepaired += _ => PowerOn();
    }

    private void PowerOn()
    {
        screenRenderer.material.EnableKeyword("_EMISSION");
        screenRenderer.material.SetColor("_EmissionColor", screenColor * screenIntensity);
    }
}
