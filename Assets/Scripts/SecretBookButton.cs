using System.Collections;
using UnityEngine;

public class SecretBookButton : MonoBehaviour
{
    [SerializeField] private CreditsSequence creditsSequence;
    [SerializeField] private CanvasGroup bookTextGroup;
    [SerializeField] private float fadeDuration = 1.5f;

    private bool pressed = false;

    public void OnEndChapterClicked()
    {
        if (pressed) return;
        pressed = true;
        StartCoroutine(EndChapter());
    }

    private IEnumerator EndChapter()
    {
        yield return ScreenFader.Instance.FadeToBlack();
        creditsSequence.Begin();
    }
}
