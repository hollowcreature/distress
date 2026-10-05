using System.Collections;
using UnityEngine;

public class SecretBookButton : MonoBehaviour, IFocusInteractable
{
    [SerializeField] private CreditsSequence creditsSequence;
    [SerializeField] private CanvasGroup bookTextGroup;
    [SerializeField] private float fadeDuration = 1.5f;

    private FocusGlow glow;
    private bool pressed = false;

    void Awake() => glow = GetComponent<FocusGlow>();

    public void OnHoverEnter() => glow.Show();
    public void OnHoverExit() => glow.Hide();

    public void OnPress()
    {
        if (pressed) return;
        pressed = true;
        StartCoroutine(EndChapter());
    }

    public void OnDrag(Ray mouseRay) { }
    public void OnRelease() { }

    private IEnumerator EndChapter()
    {
        yield return FadeOut(bookTextGroup, fadeDuration);
        yield return ScreenFader.Instance.FadeToBlack();
        creditsSequence.Begin();
    }

    private IEnumerator FadeOut(CanvasGroup group, float duration)
    {
        float start = group.alpha;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            group.alpha = Mathf.Lerp(start, 0f, t);
            yield return null;
        }
        group.alpha = 0f;
    }
}
