using System.Collections;
using TMPro;
using UnityEngine;

public class KeypadTask : RepairTask
{
    [SerializeField] private Canvas keypadCanvas;
    [SerializeField] private TMP_Text text;
    [SerializeField] private GameObject numbergroup;
    [SerializeField] private TMP_Text deniedText;
    [SerializeField] private TMP_Text grantedText;
    [SerializeField] private string correctCode;
    [SerializeField] private SlidingDoor door;
    [SerializeField] private string grantedThought;
    [SerializeField] private RepairTask keyTask;

    [SerializeField] private AudioClip deniedSound;
    [SerializeField] private AudioClip grantedSound;
    [SerializeField] private AudioSource accessSound;

    public override void Interact()
    {
        keypadCanvas.gameObject.SetActive(true);
        text.text = "";
        base.Interact();
    }

    public override void OnFocusEnter() { }
    public override void OnFocusExit() => keypadCanvas.gameObject.SetActive(false);

    public void AppendChar(char c)
    {
        if (text.text.Length == 4) return;

        text.text += c;
        if (text.text.Length < 4) return;

        if (text.text == correctCode && (keyTask == null || keyTask.IsRepaired))
            StartCoroutine(AccessGranted());
        else
            StartCoroutine(AccessDenied());
    }

    private IEnumerator AccessGranted()
    {
        yield return new WaitForSeconds(1f);
        numbergroup.SetActive(false);

        accessSound.generator = grantedSound;
        accessSound.Play();
        grantedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        grantedText.gameObject.SetActive(false);
        door.OpenBroken();
        ThoughtDisplay.Instance.Show(grantedThought);
        TryRepair();
    }

    private IEnumerator AccessDenied()
    {
        yield return new WaitForSeconds(1f);
        numbergroup.SetActive(false);
        accessSound.generator = deniedSound;

        for (int i = 0; i < 3; i++)
        {
            accessSound.Play();
            deniedText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.4f);
            deniedText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.4f);
        }

        text.text = "";
        numbergroup.SetActive(true);
    }

    protected override bool AttemptStep() => true;
}
