using System.Collections;
using TMPro;
using UnityEngine;

public class ThoughtDisplay : MonoBehaviour
{
    [SerializeField] float typeSpeed;
    [SerializeField] float displayDuration;
    [SerializeField] float fadeDuration;
    [SerializeField] private TMP_Text textComponent;

    public static ThoughtDisplay Instance;

    void Awake()
    {
        Instance = this;
    }
    public void Show(string text)
    {
        StopAllCoroutines();
        StartCoroutine(TypeAndFade(text));
    }
    private IEnumerator TypeAndFade(string text)
    {
        textComponent.text = "";
        textComponent.alpha = 1;

        foreach (char c in text)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        yield return new WaitForSeconds(displayDuration);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            textComponent.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
    }
}
