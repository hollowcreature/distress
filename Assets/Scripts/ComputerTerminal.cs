using UnityEngine;

public class ComputerTerminal : RepairTask
{
    [SerializeField] private RepairTask generatorTask;
    [SerializeField] private RepairTask sensorTask;
    [SerializeField] private RepairTask commsTask;
    [SerializeField] private TMPro.TMP_Text screenText;
    [SerializeField] private GameObject sensorDoorButton;
    [SerializeField] private GameObject commsDoorButton;
    [SerializeField] private GameObject navigationButton;

    protected override void Awake()
    {
        base.Awake();
        foreach (RepairTask task in new RepairTask[] { generatorTask, sensorTask, commsTask })
        {
            task.OnRepaired += _ => UpdateScreen();
        }

        UpdateScreen();
    }

    void UpdateScreen()
    {
        if (commsTask.IsRepaired)
        {
            screenText.text = "NAVIGATION ONLINE - SET COURSE";
            sensorDoorButton.SetActive(false);
            commsDoorButton.SetActive(false);
            navigationButton.SetActive(true);
        }
        else if (sensorTask.IsRepaired)
        {
            screenText.text = "SENSOR ARRAY RESTORED - COMMS OFFLINE";
            sensorDoorButton.SetActive(false);
            commsDoorButton.SetActive(true);
            navigationButton.SetActive(false);
        }
        else if (generatorTask.IsRepaired)
        {
            screenText.text = "POWER RESTORED - SENSOR ARRAY OFFLINE";
            sensorDoorButton.SetActive(true);
            commsDoorButton.SetActive(false);
            navigationButton.SetActive(false);
        }
        else
        {
            screenText.text = "SYSTEM OFFLINE";
            sensorDoorButton.SetActive(false);
            commsDoorButton.SetActive(false);
            navigationButton.SetActive(false);
        }
    }

    protected override bool AttemptStep()
    {
        HologramDisplay.Instance.Show("COURSE SET. \n RETURN TO CRYO SLEEP");
        return true;
    }
}
