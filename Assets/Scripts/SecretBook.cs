using System.Collections;
using UnityEngine;

public class SecretBook : RepairTask
{
    [SerializeField] private CanvasGroup bookTextGroup;
    [SerializeField] private float textFadeDuration = 1.5f;

    public override bool AlwaysInteractable => true;

    protected override bool AttemptStep() => false;

    public override void OnFocusEnter()
    {
        bookTextGroup.gameObject.SetActive(true);
        StartCoroutine(FadeCanvas(bookTextGroup, 0f, 1f, textFadeDuration));
    }

    public override void OnFocusExit()
    {
        StopAllCoroutines();
        bookTextGroup.alpha = 0f;
        bookTextGroup.gameObject.SetActive(false);
    }

    private IEnumerator FadeCanvas(CanvasGroup group, float from, float to, float duration)
    {
        group.alpha = from;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            group.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        group.alpha = to;
    }
}
