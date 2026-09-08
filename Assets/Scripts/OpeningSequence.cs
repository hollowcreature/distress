using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class OpeningSequence : MonoBehaviour
{
    [SerializeField] private TMP_Text journalText;
    [SerializeField] private AudioClip cryoSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float typeSpeed = 0.04f;
    [SerializeField] private float silenceAfterCryo = 1.5f;
    public bool skippable = false;
    private string journalEntry = "Another day, nothing yet. Sector 7 came up empty like the rest.\n No atmosphere, no water, nothing worth bringing home.\n Going back to cryo. Maybe sector 8 will be different.\n    Darlene";

    void Awake()
    {
        journalText.text = "";
    }

    public IEnumerator Sequence(Action onComplete)
    {
        yield return new WaitForSeconds(2f);

        var charWait = new WaitForSeconds(typeSpeed);
        var pauseWait = new WaitForSeconds(0.5f);

        foreach (char c in journalEntry)
        {
            journalText.text += c;
            yield return charWait;
            if (c == '.' || c == '!' || c == '?' || c == ',')
                yield return pauseWait;
        }

        yield return new WaitForSeconds(1f);

        float fadeTime = 1.5f;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeTime;
            journalText.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        journalText.alpha = 0f;

        audioSource.spatialBlend = 0.0f;
        audioSource.Play();
        yield return new WaitForSeconds(cryoSound.length);

        audioSource.spatialBlend = 1.0f;
        yield return new WaitForSeconds(silenceAfterCryo);
        onComplete?.Invoke();
    }

    public void Cleanup()
    {
        journalText.text = "";
    }
}
