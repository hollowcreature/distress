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
        yield return new WaitForSeconds(1f);
        HologramDisplay.Instance.Show("TARGET PLANET REACHED: EARTH");
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
