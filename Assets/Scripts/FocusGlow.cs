using UnityEngine;

public class FocusGlow : MonoBehaviour
{
    [SerializeField] private Renderer rend;
    [SerializeField] private Color glowColor = Color.white;
    [SerializeField] private float intensity = 1.5f;

    public void Show()
    {
        rend.material.EnableKeyword("_EMISSION");
        rend.material.SetColor("_EmissionColor", glowColor * intensity);
    }

    public void Hide()
    {
        rend.material.SetColor("_EmissionColor", Color.black);
        rend.material.DisableKeyword("_EMISSION");
    }
}
