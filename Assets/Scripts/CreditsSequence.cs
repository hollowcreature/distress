using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsSequence : MonoBehaviour
{
    [SerializeField] private GameObject creditsCanvas;
    [SerializeField] private CanvasGroup creditsCanvasGroup;
    [SerializeField] private float endFadeDuration = 1.5f;
    [SerializeField] private RectTransform creditsContent;
    [SerializeField] private float scrollSpeed = 40f;
    [SerializeField] private float fastForwardMultiplier = 4f;
    [SerializeField] private float endYPosition = 2000f;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private TMP_Text skipPrompt;
    [SerializeField] private TMP_Text fastForwardPrompt;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float promptFadeInDuration = 1.5f;
    [SerializeField] private float songFadeInDuration = 2f;

    private bool active = false;
    private bool finished = false;
    private bool promptsVisible = false;

    [ContextMenu("Trigger Credits")]
    public void Begin()
    {
        creditsCanvas.SetActive(true);
        active = true;
        finished = false;
        promptsVisible = false;
        skipPrompt.alpha = 0f;
        fastForwardPrompt.alpha = 0f;
        StartCoroutine(FadeInPrompts());

        var creditsSong = SoundManager.Instance.creditsSong;
        float targetVolume = creditsSong.volume;
        creditsSong.volume = 0f;
        creditsSong.Play();
        StartCoroutine(FadeAudio(creditsSong, targetVolume, songFadeInDuration));
    }

    private IEnumerator FadeAudio(AudioSource source, float targetVolume, float duration)
    {
        float startVolume = source.volume;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            source.volume = Mathf.Lerp(startVolume, targetVolume, t);
            yield return null;
        }
        source.volume = targetVolume;
    }

    private IEnumerator FadeInPrompts()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / promptFadeInDuration;
            skipPrompt.alpha = Mathf.Lerp(0f, 1f, t);
            fastForwardPrompt.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
        promptsVisible = true;
    }

    void Update()
    {
        if (!active || finished) return;

        float speed = scrollSpeed;
        if (Input.GetKey(KeyCode.DownArrow))
            speed *= fastForwardMultiplier;

        creditsContent.anchoredPosition += Vector2.up * speed * Time.deltaTime;

        if (promptsVisible)
        {
            PulsePrompt(skipPrompt);
            PulsePrompt(fastForwardPrompt);
        }

        if (Input.GetKeyDown(KeyCode.Escape) || creditsContent.anchoredPosition.y >= endYPosition)
            EndCredits();
    }

    private void PulsePrompt(TMP_Text prompt)
    {
        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
        prompt.alpha = Mathf.Lerp(0.3f, 1f, t);
    }

    private void EndCredits()
    {
        finished = true;
        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        var creditsSong = SoundManager.Instance.creditsSong;
        StartCoroutine(FadeAudio(creditsSong, 0f, endFadeDuration));

        float startAlpha = creditsCanvasGroup.alpha;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / endFadeDuration;
            creditsCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }
        creditsCanvasGroup.alpha = 0f;

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
