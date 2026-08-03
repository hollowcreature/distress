using System.Collections;
using UnityEngine;

public class IntroSequence : MonoBehaviour
{
    [SerializeField] private float crashDelay = 15f;
    [SerializeField] private Transform wakeUpPosition;
    [SerializeField] private Transform playerRoot;
    [SerializeField] private AudioSource alarmAudio;
    [SerializeField] private Camera cam;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.1f;
    [SerializeField] private float wakeUpFadeDuration = 2f;
    [SerializeField] private SlidingDoor genDoorLeft;

    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private Light[] sceneLights;
    [SerializeField] private Light[] emergencyLights;
    [SerializeField] private Renderer[] emissivePanels;
    [SerializeField] private Material darkPanelMat;
    [SerializeField] private Material emissivePanelMat;

    private bool crashed = false;
    private Coroutine countdownCoroutine;

    void Start()
    {
        mainCanvas.SetActive(true);
        StartCoroutine(IntroFadeIn());
    }

    private IEnumerator IntroFadeIn()
    {
        yield return ScreenFader.Instance.FadeFromBlack();
        HologramDisplay.Instance.Show("VESSEL DRIFTING OFF, READJUST COURSE IMMEDIATELY");
        // alarm
    }

    private IEnumerator CrashCountdown(float duration)
    {
        yield return new WaitForSeconds(duration);
        TriggerCrash();
    }

    public void OnExitCryoRoom()
    {
        if (crashed)
            return;
        countdownCoroutine = StartCoroutine(CrashCountdown(crashDelay));
    }
    public void OnHallwayMidPoint()
    {
        if (crashed)
            return;
        if (countdownCoroutine != null)
            StopCoroutine(countdownCoroutine);
        TriggerCrash();
    }

    private void TriggerCrash()
    {
        if (crashed)
            return;

        crashed = true;
        StartCoroutine(CrashSequence());
    }

    private IEnumerator CrashSequence()
    {
        if (alarmAudio != null)
            alarmAudio.Stop();

        yield return StartCoroutine(ShakeCamera());
        TurnOffLights();
        yield return ScreenFader.Instance.FadeToBlack(0.05f);
        playerRoot.SetPositionAndRotation(wakeUpPosition.position, wakeUpPosition.rotation);
        yield return new WaitForSeconds(2f);
        HologramDisplay.Instance.Show("EMERGENCY GENERATOR INITIATED\n PLEASE RESTORE POWER");
        yield return ScreenFader.Instance.FadeFromBlack(wakeUpFadeDuration);
        yield return new WaitForSeconds(2f);
        genDoorLeft.Open();
    }

    private void TurnOffLights()
    {
        foreach (var light in sceneLights) light.enabled = false;
        foreach (var panel in emissivePanels) panel.material = darkPanelMat;
        foreach (var light in emergencyLights) light.enabled = true;
    }

    public void RestoreLights()
    {
        foreach (var light in emergencyLights) light.enabled = false;
        foreach (var light in sceneLights) light.enabled = true;
        foreach (var panel in emissivePanels) panel.material = emissivePanelMat;
    }

    private IEnumerator ShakeCamera()
    {
        Vector3 originalPos = cam.transform.localPosition;
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            cam.transform.localPosition = originalPos + Random.insideUnitSphere * shakeMagnitude;
            elapsed += Time.deltaTime;
            yield return null;
        }
        cam.transform.localPosition = originalPos;
    }
}
