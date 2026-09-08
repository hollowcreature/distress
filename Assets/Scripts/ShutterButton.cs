using System.Collections;
using UnityEngine;

public class ShutterButton : MonoBehaviour, IFocusInteractable
{
    [SerializeField] private ShutterController shutterController;
    [SerializeField] private AudioSource pressSound;
    [SerializeField] private float fadeDuration = 0.5f;

    private FocusGlow glow;
    private CanvasGroup canvasGroup;
    private Renderer buttonRenderer;
    private bool pressed = false;

    void Awake()
    {
        glow = GetComponent<FocusGlow>();
        canvasGroup = GetComponent<CanvasGroup>();
        buttonRenderer = GetComponent<Renderer>();
    }

    public void OnHoverEnter() => glow.Show();
    public void OnHoverExit() => glow.Hide();

    public void OnPress()
    {
        if (pressed) return;
        pressed = true;

        if (pressSound != null) pressSound.Play();
        shutterController.Open();
        StartCoroutine(FadeOut());
    }

    public void OnDrag(Ray mouseRay) { }
    public void OnRelease() { }

    private IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            if (canvasGroup != null) canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            if (buttonRenderer != null)
            {
                Color c = buttonRenderer.material.GetColor("_BaseColor");
                c.a = Mathf.Lerp(1f, 0f, t);
                buttonRenderer.material.SetColor("_BaseColor", c);
            }
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
