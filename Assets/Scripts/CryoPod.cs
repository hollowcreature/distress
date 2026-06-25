using System.Collections;
using UnityEngine;

public class CryoPod : MonoBehaviour, IInteractable
{
    [SerializeField] ComputerTerminal terminal;
    [SerializeField] private Transform podInteriorAnchor;
    [SerializeField] private Collider endgameTrigger;

    void Awake()
    {
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;
        terminal.OnRepaired += _ => collider.enabled = true;
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
        yield return ScreenFader.Instance.FadeToBlack();
        yield return new WaitForSeconds(3f);
        FocusController.Instance.ExitCutscene();
        yield return ScreenFader.Instance.FadeFromBlack();
        endgameTrigger.enabled = true;
        yield return new WaitForSeconds(2f);
        HologramDisplay.Instance.Show("TARGET PLANET REACHED");
    }

}
