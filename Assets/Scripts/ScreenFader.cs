using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    [SerializeField] private Image panel;
    [SerializeField] private float defaultDuration = 1f;

    void Awake()
    {
        Instance = this;
        panel.color = new Color(0f, 0f, 0f, 0f);
    }

    public Coroutine FadeToBlack(float duration = -1f) =>
        StartCoroutine(Fade(0f, 1f, duration < 0 ? defaultDuration : duration));

    public Coroutine FadeFromBlack(float duration = -1f) =>
        StartCoroutine(Fade(1f, 0f, duration < 0 ? defaultDuration : duration));

    private IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            panel.color = new Color(0f, 0f, 0f, Mathf.Lerp(from, to, t));
            yield return null;
        }
        panel.color = new Color(0f, 0f, 0f, to);
    }
}
