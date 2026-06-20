using System.Collections;
using TMPro;
using UnityEngine;

public class ObjectiveDisplay : MonoBehaviour
{
    [SerializeField] float typeSpeed;
    [SerializeField] float fadeDuration;
    [SerializeField] private TMP_Text textComponent;

    public static ObjectiveDisplay Instance;

    void Awake()
    {
        Instance = this;
    }
    public void Show(string text)
    {
        StopAllCoroutines();
        StartCoroutine(Type(text));
    }
    private IEnumerator Type(string text)
    {
        textComponent.text = "";
        textComponent.alpha = 1f;

        foreach (char c in text)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        textComponent.alpha = 0.4f;
    }

    private IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            textComponent.alpha = Mathf.Lerp(0.4f, 0f, t);
            yield return null;
        }
    }

    public void Clear()
    {
        StartCoroutine(FadeOut());
    }
}
