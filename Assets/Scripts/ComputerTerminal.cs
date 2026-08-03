using UnityEngine;

public class ComputerTerminal : RepairTask
{
    [SerializeField] private RepairTask generatorTask;
    [SerializeField] private RepairTask sensorTask;
    [SerializeField] private RepairTask commsTask;
    [SerializeField] private TMPro.TMP_Text screenText;
    [SerializeField] private GameObject sensorDoorButton;
    [SerializeField] private GameObject logsButton;

    public override bool AlwaysInteractable => true;

    protected override void Awake()
    {
        base.Awake();

        logsButton.GetComponent<CanvasGroup>().alpha = 0f;
        Material mat = logsButton.GetComponent<Renderer>().material;
        Color c = mat.GetColor("_BaseColor");
        c.a = 0f;
        mat.SetColor("_BaseColor", c);
        logsButton.GetComponent<Collider>().enabled = false;
        logsButton.GetComponent<Renderer>().enabled = false;

        UpdateScreen();
    }

    public void UpdateScreen()
    {
        if (commsTask.IsRepaired)
        {
            screenText.text = "ALL SYSTEMS OPERATIONAL";
        }
        else if (sensorTask.IsRepaired)
        {
            screenText.text = "SENSOR ARRAY RESTORED - COMMS OFFLINE";
        }
        else if (generatorTask.IsRepaired)
        {
            screenText.text = "POWER RESTORED - SENSOR ARRAY OFFLINE";
            sensorDoorButton.SetActive(true);
        }
        else
        {
            screenText.text = "SYSTEM OFFLINE";
            sensorDoorButton.SetActive(false);
        }
    }

    protected override bool AttemptStep()
    {
        HologramDisplay.Instance.Show("COURSE SET. \n RETURN TO CRYO SLEEP");
        return true;
    }
}
