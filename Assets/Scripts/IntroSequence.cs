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
    [SerializeField] private SlidingDoor genDoorRight;

    private bool crashed = false;
    private Coroutine countdownCoroutine;

    void Start()
    {
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
        yield return ScreenFader.Instance.FadeToBlack(0.05f);
        playerRoot.SetPositionAndRotation(wakeUpPosition.position, wakeUpPosition.rotation);
        yield return new WaitForSeconds(2f);
        HologramDisplay.Instance.Show("EMERGENCY GENERATOR INITIATED\n PLEASE RESTORE POWER");
        yield return ScreenFader.Instance.FadeFromBlack(wakeUpFadeDuration);
        yield return new WaitForSeconds(2f);
        genDoorLeft.Open();
        genDoorRight.Open();
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
