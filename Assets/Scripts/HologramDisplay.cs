using System.Collections;
using UnityEngine;

public class HologramDisplay : MonoBehaviour
{
    public static HologramDisplay Instance;

    [SerializeField] private TMPro.TMP_Text textComponent;
    [SerializeField] private float scrambleDuration = 0.05f;
    [SerializeField] private float fadeDuration = 1f;
    private const string charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789#@!%";

    void Awake()
    {
        Instance = this;
    }

    public Coroutine Show(string text)
    {
        StopAllCoroutines();
        return StartCoroutine(Scramble(text));
    }
    public void Clear() => StartCoroutine(FadeOut());
    private IEnumerator Scramble(string text)
    {
        textComponent.alpha = 1f;
        textComponent.text = "";

        for (int i = 0; i < text.Length; i++)
        {
            float elapsed = 0f;
            while (elapsed < scrambleDuration)
            {
                textComponent.text = text.Substring(0, i) + charset[Random.Range(0, charset.Length)];
                elapsed += Time.deltaTime;
                yield return null;
            }
            textComponent.text = text.Substring(0, i + 1);
        }
    }

    private IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            textComponent.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        textComponent.text = "";
    }
}
