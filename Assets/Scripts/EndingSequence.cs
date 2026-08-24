using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EndingSequence : MonoBehaviour
{
    [SerializeField] private TMP_Text journalText;
    [SerializeField] private float startDelay = 4f;
    [SerializeField] private float typeSpeed = 0.04f;
    [SerializeField][TextArea] private string badEndingText;
    [SerializeField][TextArea] private string goodEndingText;
    [SerializeField] private AudioSource shipHum;

    void Awake()
    {
        journalText.text = "";
        journalText.alpha = 0f;
        journalText.gameObject.SetActive(false);
    }

    public void Begin()
    {
        StartCoroutine(Sequence());
    }

    private IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            source.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }
        source.volume = 0f;
        source.Stop();
    }

    private IEnumerator Sequence()
    {
        journalText.gameObject.SetActive(true);
        float fadeDuration = Mathf.Max(0f, startDelay - 5f);
        if (shipHum != null)
            StartCoroutine(FadeOut(shipHum, fadeDuration));
        yield return new WaitForSeconds(startDelay);
        HologramDisplay.Instance.Show("MISSION STATUS: FAILED");
        yield return new WaitForSeconds(3f);
        yield return ScreenFader.Instance.FadeToBlack();
        yield return new WaitForSeconds(1f);


        string text = NecklacePickup.Found ? goodEndingText : badEndingText;
        journalText.text = "";
        journalText.alpha = 1f;

        var charWait = new WaitForSeconds(typeSpeed);
        var pauseWait = new WaitForSeconds(1.0f);

        foreach (char c in text)
        {
            journalText.text += c;
            yield return charWait;
            if (c == '.' || c == '!' || c == '?' || c == '\n')
                yield return pauseWait;
        }
    }
}
