using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private const string VolumePrefKey = "MasterVolume";

    [SerializeField] private string gameplaySceneName = "SampleScene";
    [SerializeField] private CanvasGroup fadeOverlay;
    [SerializeField] private CanvasGroup controlsPanel;
    [SerializeField] private CanvasGroup menuContent;
    [SerializeField] private CanvasGroup volumeContent;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioSource menuAmbience;
    [SerializeField] private float ambienceFadeDuration = 2f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float controlsHoldDuration = 3f;
    [SerializeField] private Transform cameraToRotate;
    [SerializeField] private Vector3 cameraRotationSpeed = new Vector3(0f, 1.5f, 0f);

    private bool transitioning = false;
    private float ambienceMaxVolume;

    void Update()
    {
        if (cameraToRotate != null)
            cameraToRotate.Rotate(cameraRotationSpeed * Time.deltaTime, Space.World);
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        float savedVolume = PlayerPrefs.GetFloat(VolumePrefKey, 1f);
        AudioListener.volume = savedVolume;
        volumeSlider.value = savedVolume;

        fadeOverlay.gameObject.SetActive(true);
        fadeOverlay.alpha = 1f;

        ambienceMaxVolume = menuAmbience.volume;
        menuAmbience.volume = 0f;
        menuAmbience.Play();
        StartCoroutine(FadeAudio(menuAmbience, ambienceMaxVolume, ambienceFadeDuration));

        volumeContent.alpha = 0f;
        StartCoroutine(FadeInMenu());
    }

    private IEnumerator FadeInMenu()
    {
        yield return new WaitForSeconds(1f);
        yield return Fade(fadeOverlay, 1f, 0f, fadeDuration);
        yield return new WaitForSeconds(4f);
        yield return Fade(volumeContent, 0f, 1f, fadeDuration);
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

    public void PlayGame()
    {
        if (transitioning) return;
        transitioning = true;
        menuContent.interactable = false;
        menuContent.blocksRaycasts = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(PlaySequence());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat(VolumePrefKey, volume);
    }

    private IEnumerator PlaySequence()
    {
        StartCoroutine(FadeAudio(menuAmbience, 0f, fadeDuration));
        yield return FadeOutMenuToBlack();
        yield return Fade(controlsPanel, 0f, 1f, fadeDuration);
        yield return WaitOrSkip(controlsHoldDuration);
        yield return Fade(controlsPanel, 1f, 0f, fadeDuration);

        SceneManager.LoadScene(gameplaySceneName);
    }

    private IEnumerator WaitOrSkip(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            if (Input.anyKeyDown)
                yield break;
            t += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOutMenuToBlack()
    {
        fadeOverlay.gameObject.SetActive(true);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            fadeOverlay.alpha = Mathf.Lerp(0f, 1f, t);
            menuContent.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        fadeOverlay.alpha = 1f;
        menuContent.alpha = 0f;
    }

    private IEnumerator Fade(CanvasGroup group, float from, float to, float duration)
    {
        group.gameObject.SetActive(true);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            group.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        group.alpha = to;
        if (to == 0f)
            group.gameObject.SetActive(false);
    }
}
