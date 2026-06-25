using System.Collections;
using UnityEngine;

public class EndgameTrigger : MonoBehaviour
{
    [SerializeField] private CommsInbox comms;
    [SerializeField] private GameObject logsButton;
    private bool triggered = false;

    void Awake()
    {
        GetComponent<Collider>().enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || triggered)
            return;

        triggered = true;
        StartCoroutine(EndgameEvent());
    }

    private IEnumerator EndgameEvent()
    {
        yield return new WaitForSeconds(1f);
        HologramDisplay.Instance.Show("REDOWNLOADING MESSAGES...");
        yield return new WaitForSeconds(2f);
        comms.Redownload();
        yield return new WaitForSeconds(15f);
        HologramDisplay.Instance.Show("CRITICAL ERROR DETECTED - CONSULT VESSEL LOGS");
        yield return new WaitForSeconds(0.5f);
        logsButton.GetComponent<Collider>().enabled = true;
        logsButton.GetComponent<Renderer>().enabled = true;
        StartCoroutine(logsButton.GetComponent<ScreenButton>().FadeIn());
    }
}
