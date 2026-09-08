using UnityEngine;

public class TaskResponder : MonoBehaviour
{
    [SerializeField] private IntroSequence introSequence;
    [SerializeField] private PanelGenerator genTask;
    [SerializeField] private SensorTask senTask;
    [SerializeField] private AntennaTask commsTask;
    [SerializeField] private CommsInbox commsInbox;
    [SerializeField] private CommsMonitor commsMonitor;
    [SerializeField] private CommsScreen commsScreen;
    [SerializeField] private ModuleTask moduleTask;
    [SerializeField] private ModulePanel modulePanel;
    [SerializeField] private ComputerTerminal computerTerminal;
    [SerializeField] private KeySlotTask keySlotTask;
    [SerializeField] private CryoPod cryoPod;
    [SerializeField] private NavigationCursor navCursor;
    [SerializeField] private ScreenButton screenButton;
    [SerializeField] private SlidingDoor airlockDoor;
    [SerializeField] private GameObject shutterButton;
    [SerializeField] private GameObject solInfo;
    [SerializeField] private ShutterController shutterController;
    [SerializeField] private GameObject missionText;
    [SerializeField] private GameObject statusText;

    void Awake()
    {
        genTask.OnRepaired += _ =>
        {
            HologramDisplay.Instance.Clear();
            computerTerminal.UpdateScreen();
            introSequence.RestoreLights();
            SoundManager.Instance.shipHum.Play();
            SoundManager.Instance.genHum.Play();
            missionText.SetActive(true);
            statusText.SetActive(true);
            HologramDisplay.Instance.Show("POWER RESTORED \n RUNNING DIAGNOSTICS... \n SHIP: ISV EXPLORER \n DATE: 2574.06.01 \n CRYO DURATION: 3 YEARS 2 MONTHS 14 DAYS");
            AnnouncerController.Instance.PlayVoiceline(1, false);
        };

        senTask.OnRepaired += _ =>
        {
            shutterButton.SetActive(true);
            solInfo.SetActive(true);
            computerTerminal.UpdateScreen();
            HologramDisplay.Instance.Show("SENSOR ARRAY ONLINE \n SCANNING ENVIRONMENT... \n PROXIMITY: CLEAR \n DEBRIS FIELD: NONE DETECTED");
            AnnouncerController.Instance.PlayVoiceline(3, false);
        };

        commsTask.OnRepaired += _ =>
        {
            StartCoroutine(commsInbox.UnlockSequence());
            commsScreen.inboxGroup.SetActive(true);
            commsScreen.calibrateGroup.SetActive(false);
            computerTerminal.UpdateScreen();
        };

        moduleTask.OnRepaired += _ =>
        {
            commsMonitor.PowerOn();
            commsScreen.calibrateGroup.SetActive(true);
            StartCoroutine(modulePanel.SpringBack(modulePanel.closeDuration));
        };

        computerTerminal.OnRepaired += _ =>
        {
            cryoPod.cryoCollider.enabled = true;
            StartCoroutine(navCursor.FadeOut());
            shutterController.Close();
            HologramDisplay.Instance.Show("COURSE SET. \n RETURN TO CRYO SLEEP");
            AnnouncerController.Instance.PlayVoiceline(5, false);
        };

        keySlotTask.OnRepaired += _ =>
        {
            screenButton.escalatedPrivilege = true;
            HologramDisplay.Instance.Show("EMERGENCY PRIVILEGE ESCALATION INITIATED");
            AnnouncerController.Instance.PlayVoiceline(10, false);
            airlockDoor.Unlock();
        };
    }
}
