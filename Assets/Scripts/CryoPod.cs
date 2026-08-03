using System.Collections;
using UnityEngine;

public class CryoPod : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform podInteriorAnchor;
    [SerializeField] private Collider endgameTrigger;

    public Collider cryoCollider;
    void Awake()
    {
        cryoCollider = GetComponent<Collider>();
        cryoCollider.enabled = false;
    }

    public void Interact()
    {
        FocusController.Instance.EnterCutscene(podInteriorAnchor, OnInsidePod);
    }

    private void OnInsidePod()
    {
        StartCoroutine(SleepSequence());
    }

    private IEnumerator SleepSequence()
    {
        GetComponent<Collider>().enabled = false;
        yield return ScreenFader.Instance.FadeToBlack();
        yield return new WaitForSeconds(3f);
        FocusController.Instance.ExitCutscene();
        yield return ScreenFader.Instance.FadeFromBlack();
        endgameTrigger.enabled = true;
        yield return new WaitForSeconds(2f);
        HologramDisplay.Instance.Show("TARGET PLANET REACHED");
    }

}
