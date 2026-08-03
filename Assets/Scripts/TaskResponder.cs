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

    void Awake()
    {
        genTask.OnRepaired += _ =>
        {
            HologramDisplay.Instance.Clear();
            computerTerminal.UpdateScreen();
            introSequence.RestoreLights();
            HologramDisplay.Instance.Show("EMERGENCY GENERATOR — POWER RESTORED \n RUNNING DIAGNOSTICS... \n SHIP: ISV DISTRESS \n DATE: 2401.03.14 \n UPTIME: 47 YEARS 3 MONTHS 12 DAYS \n STATUS: CRITICAL");
        };

        senTask.OnRepaired += _ =>
        {
            computerTerminal.UpdateScreen();
            HologramDisplay.Instance.Show("SENSOR ARRAY ONLINE \n SCANNING ENVIRONMENT... \n PROXIMITY: CLEAR \n DEBRIS FIELD: NONE DETECTED \n NEAREST BODY: SOL SYSTEM — 0.3 LY \n ESTIMATED ARRIVAL: 847 DAYS");
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
        };

        keySlotTask.OnRepaired += _ =>
        {
            screenButton.escalatedPrivilege = true;
            HologramDisplay.Instance.Show("EMERGENCY PRIVILEGE ESCALATION INITIATED");
            airlockDoor.Unlock();
        };
    }
}
