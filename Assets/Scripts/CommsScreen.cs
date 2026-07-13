using UnityEngine;
using UnityEngine.Rendering;

public class CommsScreen : RepairTask
{
    [SerializeField] private AntennaTask antennaTask;
    [SerializeField] private ModuleTask moduleTask;
    [SerializeField] private GameObject inboxGroup;
    [SerializeField] private GameObject calibrateGroup;

    protected override void Awake()
    {
        base.Awake();
        inboxGroup.SetActive(false);
        calibrateGroup.SetActive(false);

        antennaTask.OnRepaired += _ =>
        {
            inboxGroup.SetActive(true);
            calibrateGroup.SetActive(false);
        };

        moduleTask.OnRepaired += _ => calibrateGroup.SetActive(true);
    }
    protected override bool AttemptStep() => false;
}
