using System.Collections;
using UnityEngine;

public class CryoPod : MonoBehaviour, IInteractable
{
    [SerializeField] ComputerTerminal terminal;
    [SerializeField] private Transform podInteriorAnchor;

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
    }

}
