using System.Collections;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;

[System.Serializable]
public class CommMessage
{
    public string sender;
    public string timestamp;
    public string subject;
    public bool showHologram;
    public bool isRedownloaded;
    [TextArea] public string body;
    [HideInInspector] public bool isRead;
    [HideInInspector] public bool isUnlocked;
}

public class CommsInbox : MonoBehaviour
{
    public static CommsInbox Instance;

    [SerializeField] private AntennaTask commsTask;
    [SerializeField] private CommMessage[] messages;
    [SerializeField] private CommMessage[] redownloadedMessages;
    [SerializeField] private Transform listContainer;
    [SerializeField] private GameObject messageButtonPrefab;
    [SerializeField] private TMPro.TMP_Text detailSender;
    [SerializeField] private TMPro.TMP_Text detailTimestamp;
    [SerializeField] private TMPro.TMP_Text detailBody;
    [SerializeField] private GameObject navigationCursor;
    [SerializeField] private AudioSource notificationSound;
    [SerializeField] private AudioSource messageSelectSound;

    private int currIdx = 0;

    void Awake()
    {
        Instance = this;
    }

    public void UnlockMessage(int index)
    {
        messages[index].isUnlocked = true;
        RefreshList();
        notificationSound.Play();
        if (messages[index].showHologram)
        {
            HologramDisplay.Instance.Show("NEW MESSAGE RECEIVED...");
        }
    }

    public void UnlockNext()
    {
        if (currIdx == messages.Length)
            return;

        UnlockMessage(currIdx);
        currIdx++;
    }

    public IEnumerator UnlockSequence()
    {
        yield return new WaitForSeconds(2f);
        UnlockNext();
        yield return new WaitForSeconds(4f);
        UnlockNext();
        yield return new WaitForSeconds(8f);
        UnlockNext();
    }

    public void SelectMessage(int index)
    {
        messageSelectSound.Play();
        messages[index].isRead = true;
        detailSender.text = messages[index].sender;
        detailTimestamp.text = messages[index].timestamp;
        detailBody.text = messages[index].body;

        if (index == messages.Length - 1 && !messages[index].isRedownloaded)
            navigationCursor.SetActive(true);

        RefreshList();
    }

    private void RefreshList()
    {
        foreach (Transform child in listContainer)
            Destroy(child.gameObject);

        for (int i = 0; i < messages.Length; i++)
        {
            if (!messages[i].isUnlocked)
                continue;

            GameObject btn = Instantiate(messageButtonPrefab, listContainer);
            TMPro.TMP_Text label = btn.GetComponentInChildren<TMPro.TMP_Text>();
            string prefix = messages[i].isRead ? "" : "[NEW] ";
            label.text = prefix + messages[i].sender + " - " + messages[i].subject;

            int capturedIndex = i;
            btn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => SelectMessage(capturedIndex));
        }

        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(listContainer.GetComponent<RectTransform>());
    }

    public void Redownload()
    {
        messages = redownloadedMessages;
        currIdx = 0;
        foreach (var m in messages)
        {
            m.isUnlocked = false;
            m.isRead = false;
        }
        RefreshList();
        StartCoroutine(UnlockSequence());
    }
}
