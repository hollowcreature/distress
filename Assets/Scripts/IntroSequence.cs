using System.Collections;
using UnityEngine;

public class IntroSequence : MonoBehaviour
{
    [SerializeField] private float crashDelay = 15f;
    [SerializeField] private Transform wakeUpPosition;
    [SerializeField] private Transform playerRoot;
    [SerializeField] private AudioClip alarmAudio;
    [SerializeField] private Camera cam;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.1f;
    [SerializeField] private float wakeUpFadeDuration = 2f;
    [SerializeField] private SlidingDoor genDoorLeft;

    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private Light[] sceneLights;
    [SerializeField] private Renderer[] emissivePanels;
    [SerializeField] private Material darkPanelMat;
    [SerializeField] private Material emissivePanelMat;
    [SerializeField] private Renderer[] emergencyStrips;
    [SerializeField] private Material emergencyStripOnMat;
    [SerializeField] private Material emergencyStripOffMat;
    [SerializeField] private Material alarmMat;
    [SerializeField] private float alarmPulseSpeed = 3f;
    [SerializeField] private float alarmLightMinIntensity = 0.2f;
    [SerializeField] private float alarmLightMaxIntensity = 1.5f;

    [SerializeField] private AudioSource[] speakers;

    private bool crashed = false;
    private Coroutine countdownCoroutine;
    private Coroutine alarmCoroutine;
    private Material alarmMatInstance;
    private Color[] originalLightColors;
    private float[] originalLightIntensities;

    void Start()
    {
        mainCanvas.SetActive(true);
        StartCoroutine(IntroFadeIn());
    }

    private IEnumerator IntroFadeIn()
    {
        yield return ScreenFader.Instance.FadeFromBlack();
        HologramDisplay.Instance.Show("VESSEL DRIFTING OFF, READJUST COURSE IMMEDIATELY");
        yield return new WaitForSeconds(1f);
        foreach (var speaker in speakers)
        {
            speaker.generator = alarmAudio;
            speaker.loop = true;
            speaker.Play();
        }
        originalLightColors = new Color[sceneLights.Length];
        originalLightIntensities = new float[sceneLights.Length];
        for (int i = 0; i < sceneLights.Length; i++)
        {
            originalLightColors[i] = sceneLights[i].color;
            originalLightIntensities[i] = sceneLights[i].intensity;
        }
        alarmMatInstance = Instantiate(alarmMat);
        foreach (var panel in emissivePanels) panel.material = alarmMatInstance;
        alarmCoroutine = StartCoroutine(PulseAlarm());
    }

    private IEnumerator PulseAlarm()
    {
        while (true)
        {
            float t = (Mathf.Sin(Time.time * alarmPulseSpeed) + 1f) / 2f;
            alarmMatInstance.SetColor("_EmissionColor", Color.red * Mathf.Lerp(0f, 2f, t));
            float intensity = Mathf.Lerp(alarmLightMinIntensity, alarmLightMaxIntensity, t);
            foreach (var light in sceneLights)
            {
                light.color = Color.red;
                light.intensity = intensity;
            }
            yield return null;
        }
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
        foreach (var speaker in speakers) speaker.Stop();
        SoundManager.Instance.shipHum.Stop();

        yield return StartCoroutine(ShakeCamera());
        yield return ScreenFader.Instance.FadeToBlack(0.05f);
        playerRoot.SetPositionAndRotation(wakeUpPosition.position, wakeUpPosition.rotation);
        TurnOffLights();
        yield return new WaitForSeconds(2f);
        HologramDisplay.Instance.Show("EMERGENCY GENERATOR ENGAGED\n PLEASE RESTORE POWER");
        yield return ScreenFader.Instance.FadeFromBlack(wakeUpFadeDuration);
        yield return new WaitForSeconds(2f);
        genDoorLeft.Open();
    }

    [ContextMenu("Preview: Turn Off Lights")]
    private void TurnOffLights()
    {
        if (alarmCoroutine != null) StopCoroutine(alarmCoroutine);
        foreach (var light in sceneLights) light.enabled = false;
        foreach (var panel in emissivePanels) panel.material = darkPanelMat;
        foreach (var strip in emergencyStrips) strip.material = emergencyStripOnMat;
    }

    [ContextMenu("Preview: Restore Lights")]
    public void RestoreLights()
    {
        for (int i = 0; i < sceneLights.Length; i++)
        {
            sceneLights[i].enabled = true;
            if (originalLightColors != null && i < originalLightColors.Length)
            {
                sceneLights[i].color = originalLightColors[i];
                sceneLights[i].intensity = originalLightIntensities[i];
            }
        }
        foreach (var panel in emissivePanels) panel.material = emissivePanelMat;
        foreach (var strip in emergencyStrips) strip.material = emergencyStripOffMat;
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
