using System.Collections;
using UnityEngine;

public class CryoPod : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform podInteriorAnchor;
    [SerializeField] private Collider endgameTrigger;
    [SerializeField] private Transform lid;
    [SerializeField] private Vector3 lidOpenEuler = new Vector3(-80f, 0f, 0f);
    [SerializeField] private float lidOpenDuration = 0.8f;
    [SerializeField] private float lidCloseDuration = 0.6f;
    [SerializeField] private float lidCloseDelay = 3f;

    [SerializeField] private GameObject sun;
    [SerializeField] private Light sunLight;
    [SerializeField] private GameObject earth;
    [SerializeField] private Light sunLightFaceEarth;
    [SerializeField] private AudioClip cryoSoundSleep;
    [SerializeField] private AudioClip cryoSoundWakeUp;
    [SerializeField] private AudioSource cryoSound;
    [SerializeField] private GameObject solInfo;
    [SerializeField] private GameObject missionText;
    [SerializeField] private GameObject statusText;
    [SerializeField] private RectTransform hologramRTransform;

    public Collider cryoCollider;
    private Quaternion lidClosedRot;
    private Quaternion lidOpenRot;

    void Awake()
    {
        cryoCollider = GetComponent<Collider>();
        cryoCollider.enabled = false;
        if (lid != null)
        {
            lidClosedRot = lid.localRotation;
            lidOpenRot = lidClosedRot * Quaternion.Euler(lidOpenEuler);
        }
    }

    public void Interact()
    {
        cryoCollider.enabled = false;
        cryoSound.generator = cryoSoundSleep;
        cryoSound.Play();
        StartCoroutine(EnterSequence());
    }

    public void SnapInAndWake()
    {
        FocusController.Instance.EnterCutscene(podInteriorAnchor, null);
        StartCoroutine(WakeSequence());
    }

    private IEnumerator WakeSequence()
    {
        cryoSound.generator = cryoSoundWakeUp;
        cryoSound.Play();
        yield return StartCoroutine(RotateLid(lidOpenRot, lidOpenDuration));
        yield return new WaitForSeconds(1f);
        FocusController.Instance.ExitCutscene();
        yield return new WaitForSeconds(lidCloseDelay);
        yield return StartCoroutine(RotateLid(lidClosedRot, lidCloseDuration));
    }

    private IEnumerator EnterSequence()
    {
        yield return StartCoroutine(RotateLid(lidOpenRot, lidOpenDuration));
        FocusController.Instance.EnterCutscene(podInteriorAnchor, OnInsidePod);
    }

    private void OnInsidePod()
    {
        StartCoroutine(SleepSequence());
    }

    private IEnumerator SleepSequence()
    {
        HologramDisplay.Instance.Clear();
        yield return StartCoroutine(RotateLid(lidClosedRot, lidCloseDuration));
        yield return ScreenFader.Instance.FadeToBlack();
        sun.SetActive(false);
        sunLight.enabled = false;
        earth.SetActive(true);
        sunLightFaceEarth.enabled = true;
        yield return new WaitForSeconds(3f);
        FocusController.Instance.ExitCutscene();
        cryoSound.generator = cryoSoundWakeUp;
        cryoSound.Play();
        StartCoroutine(RotateLid(lidOpenRot, lidOpenDuration));
        yield return ScreenFader.Instance.FadeFromBlack();
        yield return new WaitForSeconds(lidCloseDelay);
        yield return StartCoroutine(RotateLid(lidClosedRot, lidCloseDuration));
        endgameTrigger.enabled = true;

        solInfo.SetActive(false);
        missionText.SetActive(false);
        statusText.SetActive(false);
        hologramRTransform.sizeDelta = new Vector2(hologramRTransform.sizeDelta.x, 185.8978f);
        hologramRTransform.anchoredPosition = new Vector2(-0.0096301f, -9.8348e-07f);

        yield return new WaitForSeconds(1f);
        HologramDisplay.Instance.Show("TARGET PLANET REACHED: EARTH");
        AnnouncerController.Instance.PlayVoiceline(6, false);
    }

    private IEnumerator RotateLid(Quaternion target, float duration)
    {
        Quaternion start = lid.localRotation;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            lid.localRotation = Quaternion.Slerp(start, target, Mathf.Clamp01(t));
            yield return null;
        }
        lid.localRotation = target;
    }

}
